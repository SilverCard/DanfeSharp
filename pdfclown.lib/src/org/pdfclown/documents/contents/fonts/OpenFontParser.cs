/*
  Copyright 2009-2012 Stefano Chizzolini. http://www.pdfclown.org

  Contributors:
    * Stefano Chizzolini (original code developer, http://www.stefanochizzolini.it)

  This file should be part of the source code distribution of "PDF Clown library" (the
  Program): see the accompanying README files for more info.

  This Program is free software; you can redistribute it and/or modify it under the terms
  of the GNU Lesser General Public License as published by the Free Software Foundation;
  either version 3 of the License, or (at your option) any later version.

  This Program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY,
  either expressed or implied; without even the implied warranty of MERCHANTABILITY or
  FITNESS FOR A PARTICULAR PURPOSE. See the License for more details.

  You should have received a copy of the GNU Lesser General Public License along with this
  Program (see README files); if not, go to the GNU website (http://www.gnu.org/licenses/).

  Redistribution and use, with or without modification, are permitted provided that such
  redistributions retain the above copyright notice, license and disclaimer, along with
  this list of conditions.
*/

using bytes = org.pdfclown.bytes;
using org.pdfclown.documents;
using org.pdfclown.files;
using org.pdfclown.objects;
using org.pdfclown.tokens;
using org.pdfclown.util.io;
using org.pdfclown.util.parsers;

using System;
using System.IO;
using System.Collections.Generic;
using text = System.Text;

namespace org.pdfclown.documents.contents.fonts
{
  /**
    <summary>Open Font Format parser [OFF:2009].</summary>
  */
  internal sealed class OpenFontParser
  {
    #region types
    /**
      <summary>Font metrics.</summary>
    */
    public sealed class FontMetrics
    {
      /**
        <summary>Whether the encoding is custom (symbolic font).</summary>
      */
      public bool IsCustomEncoding;//TODO:verify whether it can be replaced by the 'symbolic' variable!!!
      /**
        <summary>Unit normalization coefficient.</summary>
      */
      public float UnitNorm;
      /*
        Font Header ('head' table).
      */
      public int Flags; // USHORT.
      public int UnitsPerEm; // USHORT.
      public short XMin;
      public short YMin;
      public short XMax;
      public short YMax;
      public int MacStyle; // USHORT.
      /*
        Horizontal Header ('hhea' table).
      */
      public short Ascender;
      public short Descender;
      public short LineGap;
      public int AdvanceWidthMax; // UFWORD.
      public short MinLeftSideBearing;
      public short MinRightSideBearing;
      public short XMaxExtent;
      public short CaretSlopeRise;
      public short CaretSlopeRun;
      public int NumberOfHMetrics; // USHORT.
      /*
        OS/2 table ('OS/2' table).
      */
      public short STypoAscender;
      public short STypoDescender;
      public short STypoLineGap;
      public short SxHeight;
      public short SCapHeight;
      /*
        PostScript table ('post' table).
      */
      public float ItalicAngle;
      public short UnderlinePosition;
      public short UnderlineThickness;
      public bool IsFixedPitch;
    }

    /**
      <summary>Outline format.</summary>
    */
    public enum OutlineFormatEnum
    {
      /**
        <summary>TrueType format outlines.</summary>
      */
      TrueType,
      /**
        <summary>Compact Font Format outlines.</summary>
      */
      CFF
    }
    #endregion

    #region static
    #region fields
    private const int MicrosoftLanguage_UsEnglish = 0x409;
    private const int NameID_FontPostscriptName = 6;

    private const int PlatformID_Unicode = 0;
    private const int PlatformID_Macintosh = 1;
    private const int PlatformID_Microsoft = 3;
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets whether the given data represents a valid Open Font.</summary>
    */
    public static bool IsOpenFont(
      bytes::IInputStream fontData
      )
    {
      long position = fontData.Position;
      fontData.Position = 0;
      try
      {GetOutlineFormat(fontData.ReadInt());}
      catch(NotSupportedException)
      {return false;}
      finally
      {fontData.Position = position;}
      return true;
    }
    #endregion

    #region private
    /**
      <summary>Gets the outline format corresponding to the specified version code.</summary>
      <param name="versionCode">OFF version.</param>
      <exception cref="NotSupportedException">If versionCode is unknown.</exception>
    */
    private static OutlineFormatEnum GetOutlineFormat(
      int versionCode
      )
    {
      // Which font file format ('sfnt') version?
      switch(versionCode)
      {
        case(0x00010000): // TrueType (standard/Windows).
        case(0x74727565): // TrueType (legacy/Apple).
          return OutlineFormatEnum.TrueType;
        case(0x4F54544F): // CFF (Type 1).
          return OutlineFormatEnum.CFF;
        default:
          throw new NotSupportedException("Unknown OpenFont format version.");
      }
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region fields
    public FontMetrics Metrics;

    public string FontName;
    public OutlineFormatEnum OutlineFormat;
    /**
      <summary>Whether glyphs are indexed by custom (non-Unicode) encoding.</summary>
    */
    public bool Symbolic;

    public Dictionary<int,int> GlyphIndexes;
    public Dictionary<int,int> GlyphKernings;
    public Dictionary<int,int> GlyphWidths;

    public bytes::IInputStream FontData;

    private Dictionary<string,int> tableOffsets;
    #endregion

    #region constructors
    internal OpenFontParser(
      bytes::IInputStream fontData
      )
    {
      FontData = fontData;
      FontData.ByteOrder = ByteOrderEnum.BigEndian; // Ensures that proper endianness is applied.

      Load();
    }
    #endregion

    #region interface
    #region private
    /**
      <summary>Loads the font data.</summary>
    */
    private void Load(
      )
    {
      LoadTableInfo();

      FontName = GetName(NameID_FontPostscriptName);

      Metrics = new FontMetrics();
      LoadTables();
      LoadCMap();
      LoadGlyphWidths();
      LoadGlyphKerning();
    }

    /**
      <summary>Loads the character to glyph index mapping table.</summary>
    */
    private void LoadCMap(
      )
    {
      /*
        NOTE: A 'cmap' table may contain one or more subtables that represent multiple encodings
        intended for use on different platforms (such as Mac OS and Windows).
        Each subtable is identified by the two numbers, such as (3,1), that represent a combination
        of a platform ID and a platform-specific encoding ID, respectively.
        A symbolic font (used to display glyphs that do not use standard encodings, i.e. neither
        MacRomanEncoding nor WinAnsiEncoding) program's "cmap" table should contain a (1,0) subtable.
        It may also contain a (3,0) subtable; if present, this subtable should map from character
        codes in the range 0xF000 to 0xF0FF by prepending the single-byte codes in the (1,0) subtable
        with 0xF0 and mapping to the corresponding glyph descriptions.
      */
      // Character To Glyph Index Mapping Table ('cmap' table).
      // Retrieve the location info!
      int tableOffset;
      if(!tableOffsets.TryGetValue("cmap", out tableOffset))
        throw new ParseException("'cmap' table does NOT exist.");

      int cmap10Offset = 0;
      int cmap31Offset = 0;
      // Header.
      // Go to the number of tables!
      FontData.Seek(tableOffset + 2);
      int tableCount = FontData.ReadUnsignedShort();

      // Encoding records.
      for(
        int tableIndex = 0;
        tableIndex < tableCount;
        tableIndex++
        )
      {
        // Platform ID.
        int platformID = FontData.ReadUnsignedShort();
        // Encoding ID.
        int encodingID = FontData.ReadUnsignedShort();
        // Subtable offset.
        int offset = FontData.ReadInt();
        switch(platformID)
        {
          case PlatformID_Macintosh:
            switch(encodingID)
            {
              case 0: // Symbolic font.
                cmap10Offset = offset;
                break;
            }
            break;
          case PlatformID_Microsoft:
            switch(encodingID)
            {
              case 0: // Symbolic font.
                break;
              case 1: // Nonsymbolic font.
                cmap31Offset = offset;
                break;
            }
            break;
        }
      }

      /*
        NOTE: Symbolic fonts use specific (non-standard, i.e. neither Unicode nor
        platform-standard) font encodings.
      */
      if(cmap31Offset > 0) // Nonsymbolic.
      {
        Metrics.IsCustomEncoding = false;
        // Go to the beginning of the subtable!
        FontData.Seek(tableOffset + cmap31Offset);
      }
      else if(cmap10Offset > 0) // Symbolic.
      {
        Metrics.IsCustomEncoding = true;
        // Go to the beginning of the subtable!
        FontData.Seek(tableOffset + cmap10Offset);
      }
      else
        throw new ParseException("CMAP table unavailable.");

      int format;
      format = FontData.ReadUnsignedShort();
      // Which cmap table format?
      switch(format)
      {
        case 0: // Byte encoding table.
          LoadCMapFormat0();
          break;
        case 4: // Segment mapping to delta values.
          LoadCMapFormat4();
          break;
        case 6: // Trimmed table mapping.
          LoadCMapFormat6();
          break;
        default:
          throw new ParseException("Cmap table format " + format + " NOT supported.");
      }
    }

    /**
      <summary>Loads format-0 cmap subtable (Byte encoding table, i.e. Apple standard
      character-to-glyph index mapping table).</summary>
    */
    private void LoadCMapFormat0(
      )
    {
      /*
        NOTE: This is a simple 1-to-1 mapping of character codes to glyph indices.
        The glyph collection is limited to 256 entries.
      */
      Symbolic = true;
      GlyphIndexes = new Dictionary<int,int>(256);

      // Skip to the mapping array!
      FontData.Skip(4);
      // Glyph index array.
      // Iterating through the glyph indexes...
      for(
        int code = 0;
        code < 256;
        code++
        )
      {
        GlyphIndexes[
          code // Character code.
          ] = FontData.ReadByte() // Glyph index.
          ;
      }
    }

    /**
      <summary>Loads format-4 cmap subtable (Segment mapping to delta values, i.e. Microsoft standard
      character to glyph index mapping table for fonts that support Unicode ranges other than the
      range [U+D800 - U+DFFF] (defined as Surrogates Area, in Unicode v 3.0)).</summary>
    */
    private void LoadCMapFormat4(
      )
    {
      /*
        NOTE: This format is used when the character codes for the characters represented by a font
        fall into several contiguous ranges, possibly with holes in some or all of the ranges (i.e.
        some of the codes in a range may not have a representation in the font).
        The format-dependent data is divided into three parts, which must occur in the following
        order:
          1. A header gives parameters for an optimized search of the segment list;
          2. Four parallel arrays (end characters, start characters, deltas and range offsets)
          describe the segments (one segment for each contiguous range of codes);
          3. A variable-length array of glyph IDs.
      */
      Symbolic = false;

      // 1. Header.
      // Get the table length!
      int tableLength = FontData.ReadUnsignedShort(); // USHORT.

      // Skip to the segment count!
      FontData.Skip(2);
      // Get the segment count!
      int segmentCount = FontData.ReadUnsignedShort() / 2;

      // 2. Arrays describing the segments.
      // Skip to the array of end character code for each segment!
      FontData.Skip(6);
      // End character code for each segment.
      int[] endCodes = new int[segmentCount]; // USHORT.
      for(
        int index = 0;
        index < segmentCount;
        index++
        )
      {endCodes[index] = FontData.ReadUnsignedShort();}

      // Skip to the array of start character code for each segment!
      FontData.Skip(2);
      // Start character code for each segment.
      int[] startCodes = new int[segmentCount]; // USHORT.
      for(
        int index = 0;
        index < segmentCount;
        index++
        )
      {startCodes[index] = FontData.ReadUnsignedShort();}

      // Delta for all character codes in segment.
      short[] deltas = new short[segmentCount];
      for(
        int index = 0;
        index < segmentCount;
        index++
        )
      {deltas[index] = FontData.ReadShort();}

      // Offsets into glyph index array.
      int[] rangeOffsets = new int[segmentCount]; // USHORT.
      for(
        int index = 0;
        index < segmentCount;
        index++
        )
      {rangeOffsets[index] = FontData.ReadUnsignedShort();}

      // 3. Glyph ID array.
      /*
        NOTE: There's no explicit field defining the array length;
        it must be inferred from the space left by the known fields.
      */
      int glyphIndexCount = tableLength / 2 // Number of 16-bit words inside the table.
        - 8 // Number of single-word header fields (8 fields: format, length, language, segCountX2, searchRange, entrySelector, rangeShift, reservedPad).
        - segmentCount * 4; // Number of single-word items in the arrays describing the segments (4 arrays of segmentCount items).
      int[] glyphIds = new int[glyphIndexCount]; // USHORT.
      for(
        int index = 0;
        index < glyphIds.Length;
        index++
        )
      {glyphIds[index] = FontData.ReadUnsignedShort();}

      GlyphIndexes = new Dictionary<int,int>(glyphIndexCount);
      // Iterating through the segments...
      for(
        int segmentIndex = 0;
        segmentIndex < segmentCount;
        segmentIndex++
        )
      {
        int endCode = endCodes[segmentIndex];
        // Is it NOT the last end character code?
        /*
          NOTE: The final segment's endCode MUST be 0xFFFF. This segment need not (but MAY)
          contain any valid mappings (it can just map the single character code 0xFFFF to
          missing glyph). However, the segment MUST be present.
        */
        if(endCode < 0xFFFF)
        {endCode++;}
        // Iterating inside the current segment...
        for(
          int code = startCodes[segmentIndex];
          code < endCode;
          code++
          )
        {
          int glyphIndex;
          // Doesn't the mapping of character codes rely on glyph ID?
          if(rangeOffsets[segmentIndex] == 0) // No glyph-ID reliance.
          {
            /*
              NOTE: If the range offset is 0, the delta value is added directly to the character
              code to get the corresponding glyph index. The delta arithmetic is modulo 65536.
            */
            glyphIndex = (code + deltas[segmentIndex]) & 0xFFFF;
          }
          else // Glyph-ID reliance.
          {
            /*
              NOTE: If the range offset is NOT 0, the mapping of character codes relies on glyph ID.
              The character code offset from start code is added to the range offset. This sum is
              used as an offset from the current location within range offset itself to index out
              the correct glyph ID. This obscure indexing trick (sic!) works because glyph ID
              immediately follows range offset in the font file. The C expression that yields the
              address to the glyph ID is:
                *(rangeOffsets[segmentIndex]/2
                + (code - startCodes[segmentIndex])
                + &idRangeOffset[segmentIndex])
              As safe C# semantics don't deal directly with pointers, we have to further
              exploit such a trick reasoning with 16-bit displacements in order to yield an index
              instead of an address (sooo-good!).
            */
            // Retrieve the glyph index!
            int glyphIdIndex = rangeOffsets[segmentIndex] / 2 // 16-bit word range offset.
              + (code - startCodes[segmentIndex]) // Character code offset from start code.
              - (segmentCount - segmentIndex); // Physical offset between the offsets into glyph index array and the glyph index array.

            /*
              NOTE: The delta value is added to the glyph ID to get the corresponding glyph index.
              The delta arithmetic is modulo 65536.
            */
            glyphIndex = (glyphIds[glyphIdIndex] + deltas[segmentIndex]) & 0xFFFF;
          }

          GlyphIndexes[
            code // Character code.
            ] = glyphIndex; // Glyph index.
        }
      }
    }

    /**
      <summary>Loads format-6 cmap subtable (Trimmed table mapping).</summary>
    */
    private void LoadCMapFormat6(
      )
    {
      // Skip to the first character code!
      FontData.Skip(4);
      int firstCode = FontData.ReadUnsignedShort();
      int codeCount = FontData.ReadUnsignedShort();
      GlyphIndexes = new Dictionary<int,int>(codeCount);
      for(
        int code = firstCode,
          lastCode = firstCode + codeCount;
        code < lastCode;
        code++
        )
      {
        GlyphIndexes[
          code // Character code.
          ] = FontData.ReadUnsignedShort(); // Glyph index.
      }
    }

    /**
      <summary>Gets a name.</summary>
      <param name="id">Name identifier.</param>
    */
    private string GetName(
      int id
      )
    {
      // Naming Table ('name' table).
      // Retrieve the location info!
      int tableOffset;
      if(!tableOffsets.TryGetValue("name", out tableOffset))
        throw new ParseException("'name' table does NOT exist.");

      // Go to the number of name records!
      FontData.Seek(tableOffset + 2);

      int recordCount = FontData.ReadUnsignedShort(); // USHORT.
      int storageOffset = FontData.ReadUnsignedShort(); // USHORT.
      // Iterating through the name records...
      for(
        int recordIndex = 0;
        recordIndex < recordCount;
        recordIndex++
        )
      {
        int platformID = FontData.ReadUnsignedShort(); // USHORT.
        // Is it the default platform?
        if(platformID == PlatformID_Microsoft)
        {
          FontData.Skip(2);
          int languageID = FontData.ReadUnsignedShort(); // USHORT.
          // Is it the default language?
          if(languageID == MicrosoftLanguage_UsEnglish)
          {
            int nameID = FontData.ReadUnsignedShort(); // USHORT.
            // Does the name ID equal the searched one?
            if(nameID == id)
            {
              int length = FontData.ReadUnsignedShort(); // USHORT.
              int offset = FontData.ReadUnsignedShort(); // USHORT.

              // Go to the name string!
              FontData.Seek(tableOffset + storageOffset + offset);

              return ReadString(length,platformID);
            }
            else
            {FontData.Skip(4);}
          }
          else
          {FontData.Skip(6);}
        }
        else
        {FontData.Skip(10);}
      }
      return null; // Not found.
    }

    /**
      <summary>Loads the glyph kerning.</summary>
    */
    private void LoadGlyphKerning(
      )
    {
      // Kerning ('kern' table).
      // Retrieve the location info!
      int tableOffset;
      if(!tableOffsets.TryGetValue("kern", out tableOffset))
        return; // NOTE: Kerning table is not mandatory.

      // Go to the table count!
      FontData.Seek(tableOffset + 2);
      int subtableCount = FontData.ReadUnsignedShort(); // USHORT.

      GlyphKernings = new Dictionary<int,int>();
      int subtableOffset = (int)FontData.Position;
      // Iterating through the subtables...
      for(
        int subtableIndex = 0;
        subtableIndex < subtableCount;
        subtableIndex++
        )
      {
        // Go to the subtable length!
        FontData.Seek(subtableOffset + 2);
        // Get the subtable length!
        int length = FontData.ReadUnsignedShort(); // USHORT.

        // Get the type of information contained in the subtable!
        int coverage = FontData.ReadUnsignedShort(); // USHORT.
        // Is it a format-0 subtable?
        /*
          NOTE: coverage bits 8-15 (format of the subtable) MUST be all zeros
          (representing format 0).
        */
        //
        if((coverage & 0xff00) == 0x0000)
        {
          int pairCount = FontData.ReadUnsignedShort(); // USHORT.

          // Skip to the beginning of the list!
          FontData.Skip(6);
          // List of kerning pairs and values.
          for(
            int pairIndex = 0;
            pairIndex < pairCount;
            pairIndex++
            )
          {
            // Get the glyph index pair (left-hand and right-hand)!
            int pair = FontData.ReadInt(); // USHORT USHORT.
            // Get the normalized kerning value!
            int value = (int)(FontData.ReadShort() * Metrics.UnitNorm);

            GlyphKernings[pair] = value;
          }
        }

        subtableOffset += length;
      }
    }

    /**
      <summary>Loads the glyph widths.</summary>
    */
    private void LoadGlyphWidths(
      )
    {
      // Horizontal Metrics ('hmtx' table).
      // Retrieve the location info!
      int tableOffset;
      if(!tableOffsets.TryGetValue("hmtx", out tableOffset))
        throw new ParseException("'hmtx' table does NOT exist.");

      // Go to the glyph horizontal-metrics entries!
      FontData.Seek(tableOffset);
      GlyphWidths = new Dictionary<int,int>(Metrics.NumberOfHMetrics);
      for(
        int index = 0;
        index < Metrics.NumberOfHMetrics;
        index++
        )
      {
        // Get the glyph advance width!
        GlyphWidths[index] = (int)(FontData.ReadUnsignedShort() * Metrics.UnitNorm);
        // Skip the left side bearing!
        FontData.Skip(2);
      }
    }

    private void LoadTableInfo(
      )
    {
      // 1. Offset Table.
      FontData.Seek(0);
      // Get the outline format!
      this.OutlineFormat = GetOutlineFormat(FontData.ReadInt());
      // Get the number of tables!
      int tableCount = FontData.ReadUnsignedShort();

      // 2. Table Directory.
      // Skip to the beginning of the table directory!
      FontData.Skip(6);
      // Collecting the table offsets...
      tableOffsets = new Dictionary<string,int>(tableCount);
      for(
        int index = 0;
        index < tableCount;
        index++
        )
      {
        // Get the table tag!
        String tag = ReadAsciiString(4);
        // Skip to the table offset!
        FontData.Skip(4);
        // Get the table offset!
        int offset = FontData.ReadInt();
        // Collect the table offset!
        tableOffsets[tag] = offset;

        // Skip to the next entry!
        FontData.Skip(4);
      }
    }

    /**
      <summary>Loads general tables.</summary>
    */
    private void LoadTables(
      )
    {
      // Font Header ('head' table).
      int tableOffset;
      if(!tableOffsets.TryGetValue("head", out tableOffset))
        throw new ParseException("'head' table does NOT exist.");

      // Go to the font flags!
      FontData.Seek(tableOffset + 16);
      Metrics.Flags = FontData.ReadUnsignedShort();
      Metrics.UnitsPerEm = FontData.ReadUnsignedShort();
      Metrics.UnitNorm = 1000f / Metrics.UnitsPerEm;
      // Go to the bounding box limits!
      FontData.Skip(16);
      Metrics.XMin = FontData.ReadShort();
      Metrics.YMin = FontData.ReadShort();
      Metrics.XMax = FontData.ReadShort();
      Metrics.YMax = FontData.ReadShort();
      Metrics.MacStyle = FontData.ReadUnsignedShort();

      // Font Header ('OS/2' table).
      if(tableOffsets.TryGetValue("OS/2", out tableOffset))
      {
        FontData.Seek(tableOffset);
        int version = FontData.ReadUnsignedShort();
        // Go to the ascender!
        FontData.Skip(66);
        Metrics.STypoAscender = FontData.ReadShort();
        Metrics.STypoDescender = FontData.ReadShort();
        Metrics.STypoLineGap = FontData.ReadShort();
        if(version >= 2)
        {
          FontData.Skip(12);
          Metrics.SxHeight = FontData.ReadShort();
          Metrics.SCapHeight = FontData.ReadShort();
        }
        else
        {
          /*
            NOTE: These are just rule-of-thumb values,
            in case the xHeight and CapHeight fields aren't available.
          */
          Metrics.SxHeight = (short)(.5 * Metrics.UnitsPerEm);
          Metrics.SCapHeight = (short)(.7 * Metrics.UnitsPerEm);
        }
      }

      // Horizontal Header ('hhea' table).
      if(!tableOffsets.TryGetValue("hhea", out tableOffset))
        throw new ParseException("'hhea' table does NOT exist.");

      // Go to the ascender!
      FontData.Seek(tableOffset + 4);
      Metrics.Ascender = FontData.ReadShort();
      Metrics.Descender = FontData.ReadShort();
      Metrics.LineGap = FontData.ReadShort();
      Metrics.AdvanceWidthMax = FontData.ReadUnsignedShort();
      Metrics.MinLeftSideBearing = FontData.ReadShort();
      Metrics.MinRightSideBearing = FontData.ReadShort();
      Metrics.XMaxExtent = FontData.ReadShort();
      Metrics.CaretSlopeRise = FontData.ReadShort();
      Metrics.CaretSlopeRun = FontData.ReadShort();
      // Go to the horizontal metrics count!
      FontData.Skip(12);
      Metrics.NumberOfHMetrics = FontData.ReadUnsignedShort();

      // PostScript ('post' table).
      if(!tableOffsets.TryGetValue("post", out tableOffset))
        throw new ParseException("'post' table does NOT exist.");

      // Go to the italic angle!
      FontData.Seek(tableOffset + 4);
      Metrics.ItalicAngle =
        FontData.ReadShort() // Fixed-point mantissa (16 bits).
        + FontData.ReadUnsignedShort() / 16384f; // Fixed-point fraction (16 bits).
      Metrics.UnderlinePosition = FontData.ReadShort();
      Metrics.UnderlineThickness = FontData.ReadShort();
      Metrics.IsFixedPitch = (FontData.ReadInt() != 0);
    }

    /**
      <summary>Reads a string from the font file using the extended ASCII encoding.</summary>
    */
    private string ReadAsciiString(
      int length
      )
    {return ReadString(length, Charset.ISO88591);}

    /**
      <summary>Reads a string.</summary>
    */
    private string ReadString(
      int length,
      int platformID
      )
    {
      // Which platform?
      switch(platformID)
      {
        case PlatformID_Unicode:
        case PlatformID_Microsoft:
          return ReadUnicodeString(length);
        default:
          return ReadAsciiString(length);
      }
    }

    /**
      <summary>Reads a string from the font file using the specified encoding.</summary>
    */
    private string ReadString(
      int length,
      text::Encoding encoding
      )
    {
      byte[] data = new byte[length];
      FontData.Read(data,0,length);
      return encoding.GetString(data);
    }

    /**
      <summary>Reads a string from the font file using the Unicode encoding.</summary>
    */
    private string ReadUnicodeString(
      int length
      )
    {return ReadString(length, Charset.UTF16LE);}
    #endregion
    #endregion
    #endregion
  }
}