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
using org.pdfclown.util;
using org.pdfclown.util.io;

using System;
using System.IO;
using System.Collections.Generic;
using drawing = System.Drawing;
using System.Text;

namespace org.pdfclown.documents.contents.fonts
{
  /**
    <summary>Composite font, also called Type 0 font [PDF:1.6:5.6].</summary>
    <remarks>Do not confuse it with <see cref="Type0Font">Type 0 CIDFont</see>: the latter is
    a composite font descendant describing glyphs based on Adobe Type 1 font format.</remarks>
  */
  [PDF(VersionEnum.PDF12)]
  public abstract class CompositeFont
    : Font
  {
    #region static
    #region interface
    #region public
    new public static CompositeFont Get(
      Document context,
      bytes::IInputStream fontData
      )
    {
      OpenFontParser parser = new OpenFontParser(fontData);
      switch(parser.OutlineFormat)
      {
        case OpenFontParser.OutlineFormatEnum.CFF:
          return new Type0Font(context,parser);
        case OpenFontParser.OutlineFormatEnum.TrueType:
          return new Type2Font(context,parser);
      }
      throw new NotSupportedException("Unknown composite font format.");
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region fields
    #endregion

    #region constructors
    internal CompositeFont(
      Document context,
      OpenFontParser parser
      ) : base(context)
    {Load(parser);}

    protected CompositeFont(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region protected
    /**
      <summary>Gets the CIDFont dictionary that is the descendant of this composite font.</summary>
    */
    protected PdfDictionary CIDFontDictionary
    {get{return (PdfDictionary)((PdfArray)BaseDataObject.Resolve(PdfName.DescendantFonts)).Resolve(0);}}

    protected override PdfDictionary Descriptor
    {get{return (PdfDictionary)CIDFontDictionary.Resolve(PdfName.FontDescriptor);}}

    protected void LoadEncoding(
      )
    {
      PdfDataObject encodingObject = BaseDataObject.Resolve(PdfName.Encoding);

      // CMap [PDF:1.6:5.6.4].
      IDictionary<ByteArray,int> cmap = CMap.Get(encodingObject);

      // 1. Unicode.
      if(codes == null)
      {
        codes = new BiDictionary<ByteArray,int>();
        if(encodingObject is PdfName
          && !(encodingObject.Equals(PdfName.IdentityH)
            || encodingObject.Equals(PdfName.IdentityV)))
        {
          /*
            NOTE: According to [PDF:1.6:5.9.1], the fallback method to retrieve
            the character-code-to-Unicode mapping implies getting the UCS2 CMap
            (Unicode value to CID) corresponding to the font's one (character code to CID);
            CIDs are the bridge from character codes to Unicode values.
          */
          BiDictionary<ByteArray,int> ucs2CMap;
          {
            PdfDictionary cidSystemInfo = (PdfDictionary)CIDFontDictionary.Resolve(PdfName.CIDSystemInfo);
            String registry = (String)((PdfTextString)cidSystemInfo[PdfName.Registry]).Value;
            String ordering = (String)((PdfTextString)cidSystemInfo[PdfName.Ordering]).Value;
            String ucs2CMapName = registry + "-" + ordering + "-" + "UCS2";
            ucs2CMap = new BiDictionary<ByteArray,int>(CMap.Get(ucs2CMapName));
          }
          if(ucs2CMap.Count > 0)
          {
            foreach(KeyValuePair<ByteArray,int> cmapEntry in cmap)
            {codes[cmapEntry.Key] = ConvertUtils.ByteArrayToInt(ucs2CMap.GetKey(cmapEntry.Value).Data);}
          }
        }
        if(codes.Count == 0)
        {
          /*
            NOTE: In case no clue is available to determine the Unicode resolution map,
            the font is considered symbolic and an identity map is synthesized instead.
          */
          symbolic = true;
          foreach(KeyValuePair<ByteArray,int> cmapEntry in cmap)
          {codes[cmapEntry.Key] = ConvertUtils.ByteArrayToInt(cmapEntry.Key.Data);}
        }
      }

      // 2. Glyph indexes.
      /*
      TODO: gids map for glyph indexes as glyphIndexes is used to map cids!!!
      */
      // Character-code-to-CID mapping [PDF:1.6:5.6.4,5].
      glyphIndexes = new Dictionary<int,int>();
      foreach(KeyValuePair<ByteArray,int> cmapEntry in cmap)
      {
        if(!codes.ContainsKey(cmapEntry.Key))
          continue;

        glyphIndexes[codes[cmapEntry.Key]] = cmapEntry.Value;
      }
    }

    protected override void OnLoad(
      )
    {
      LoadEncoding();

      // Glyph widths.
      {
        glyphWidths = new Dictionary<int,int>();
        PdfArray glyphWidthObjects = (PdfArray)CIDFontDictionary.Resolve(PdfName.W);
        if(glyphWidthObjects != null)
        {
          for(IEnumerator<PdfDirectObject> iterator = glyphWidthObjects.GetEnumerator(); iterator.MoveNext();)
          {
            //TODO: this algorithm is valid only in case cid-to-gid mapping is identity (see cidtogid map)!!
            /*
              NOTE: Font widths are grouped in one of the following formats [PDF:1.6:5.6.3]:
                1. startCID [glyphWidth1 glyphWidth2 ... glyphWidthn]
                2. startCID endCID glyphWidth
            */
            int startCID = ((PdfInteger)iterator.Current).RawValue;
            iterator.MoveNext();
            PdfDirectObject glyphWidthObject2 = iterator.Current;
            if(glyphWidthObject2 is PdfArray) // Format 1: startCID [glyphWidth1 glyphWidth2 ... glyphWidthn].
            {
              int cID = startCID;
              foreach(PdfDirectObject glyphWidthObject in (PdfArray)glyphWidthObject2)
              {glyphWidths[cID++] = ((PdfInteger)glyphWidthObject).RawValue;}
            }
            else // Format 2: startCID endCID glyphWidth.
            {
              int endCID = ((PdfInteger)glyphWidthObject2).RawValue;
              iterator.MoveNext();
              int glyphWidth = ((PdfInteger)iterator.Current).RawValue;
              for(int cID = startCID; cID <= endCID; cID++)
              {glyphWidths[cID] = glyphWidth;}
            }
          }
        }
      }
      // Default glyph width.
      {
        PdfInteger defaultGlyphWidthObject = (PdfInteger)BaseDataObject[PdfName.W];
        defaultGlyphWidth = (defaultGlyphWidthObject == null ? 0 : defaultGlyphWidthObject.RawValue);
      }
    }
    #endregion

    #region private
    /**
      <summary>Loads the font data.</summary>
    */
    private void Load(
      OpenFontParser parser
      )
    {
      glyphIndexes = parser.GlyphIndexes;
      glyphKernings = parser.GlyphKernings;
      glyphWidths = parser.GlyphWidths;

      PdfDictionary baseDataObject = BaseDataObject;

      // BaseFont.
      baseDataObject[PdfName.BaseFont] = new PdfName(parser.FontName);

      // Subtype.
      baseDataObject[PdfName.Subtype] = PdfName.Type0;

      // Encoding.
      baseDataObject[PdfName.Encoding] = PdfName.IdentityH; //TODO: this is a simplification (to refine later).

      // Descendant font.
      PdfDictionary cidFontDictionary = new PdfDictionary(
        new PdfName[]{PdfName.Type},
        new PdfDirectObject[]{PdfName.Font}
        ); // CIDFont dictionary [PDF:1.6:5.6.3].
      {
        // Subtype.
        PdfName subType;
        switch(parser.OutlineFormat)
        {
          case OpenFontParser.OutlineFormatEnum.TrueType: subType = PdfName.CIDFontType2; break;
          case OpenFontParser.OutlineFormatEnum.CFF: subType = PdfName.CIDFontType0; break;
          default: throw new NotImplementedException();
        }
        cidFontDictionary[PdfName.Subtype] = subType;

        // BaseFont.
        cidFontDictionary[PdfName.BaseFont] = new PdfName(parser.FontName);

        // CIDSystemInfo.
        cidFontDictionary[PdfName.CIDSystemInfo] = new PdfDictionary(
          new PdfName[]
          {
            PdfName.Registry,
            PdfName.Ordering,
            PdfName.Supplement
          },
          new PdfDirectObject[]
          {
            new PdfTextString("Adobe"),
            new PdfTextString("Identity"),
            PdfInteger.Get(0)
          }
          ); // Generic predefined CMap (Identity-H/V (Adobe-Identity-0)) [PDF:1.6:5.6.4].

        // FontDescriptor.
        cidFontDictionary[PdfName.FontDescriptor] = Load_CreateFontDescriptor(parser);

        // Encoding.
        Load_CreateEncoding(baseDataObject,cidFontDictionary);
      }
      baseDataObject[PdfName.DescendantFonts] = new PdfArray(new PdfDirectObject[]{File.Register(cidFontDictionary)});

      Load();
    }

    /**
      <summary>Creates the character code mapping for composite fonts.</summary>
    */
    private void Load_CreateEncoding(
      PdfDictionary font,
      PdfDictionary cidFont
      )
    {
      // CMap [PDF:1.6:5.6.4].
      bytes::Buffer cmapBuffer = new bytes::Buffer();
      cmapBuffer.Append(
        "%!PS-Adobe-3.0 Resource-CMap\n"
          + "%%DocumentNeededResources: ProcSet (CIDInit)\n"
          + "%%IncludeResource: ProcSet (CIDInit)\n"
          + "%%BeginResource: CMap (Adobe-Identity-UCS)\n"
          + "%%Title: (Adobe-Identity-UCS Adobe Identity 0)\n"
          + "%%Version: 1\n"
          + "%%EndComments\n"
          + "/CIDInit /ProcSet findresource begin\n"
          + "12 dict begin\n"
          + "begincmap\n"
          + "/CIDSystemInfo\n"
          + "3 dict dup begin\n"
          + "/Registry (Adobe) def\n"
          + "/Ordering (Identity) def\n"
          + "/Supplement 0 def\n"
          + "end def\n"
          + "/CMapName /Adobe-Identity-UCS def\n"
          + "/CMapVersion 1 def\n"
          + "/CMapType 0 def\n"
          + "/WMode 0 def\n"
          + "2 begincodespacerange\n"
          + "<20> <20>\n"
          + "<0000> <19FF>\n"
          + "endcodespacerange\n"
          + glyphIndexes.Count + " begincidchar\n"
        );
      // ToUnicode [PDF:1.6:5.9.2].
      bytes::Buffer toUnicodeBuffer = new bytes::Buffer();
      toUnicodeBuffer.Append(
        "/CIDInit /ProcSet findresource begin\n"
          + "12 dict begin\n"
          + "begincmap\n"
          + "/CIDSystemInfo\n"
          + "<< /Registry (Adobe)\n"
          + "/Ordering (UCS)\n"
          + "/Supplement 0\n"
          + ">> def\n"
          + "/CMapName /Adobe-Identity-UCS def\n"
          + "/CMapVersion 10.001 def\n"
          + "/CMapType 2 def\n"
          + "2 begincodespacerange\n"
          + "<20> <20>\n"
          + "<0000> <19FF>\n"
          + "endcodespacerange\n"
          + glyphIndexes.Count + " beginbfchar\n"
        );
      // CIDToGIDMap [PDF:1.6:5.6.3].
      bytes::Buffer gIdBuffer = new bytes::Buffer();
      gIdBuffer.Append((byte)0);
      gIdBuffer.Append((byte)0);
      int code = 0;
      codes = new BiDictionary<ByteArray,int>(glyphIndexes.Count);
      PdfArray widthsObject = new PdfArray(glyphWidths.Count);
      foreach(KeyValuePair<int,int> glyphIndexEntry in glyphIndexes)
      {
        // Character code (unicode to codepoint) entry.
        code++;
        byte[] charCode = (glyphIndexEntry.Key == 32
          ? new byte[]{32}
          : new byte[]
            {
              (byte)((code >> 8) & 0xFF),
              (byte)(code & 0xFF)
            });
        codes[new ByteArray(charCode)] = glyphIndexEntry.Key;

        // CMap entry.
        cmapBuffer.Append("<");
        toUnicodeBuffer.Append("<");
        for(int charCodeBytesIndex = 0,
            charCodeBytesLength = charCode.Length;
          charCodeBytesIndex < charCodeBytesLength;
          charCodeBytesIndex++
          )
        {
          string hex = ((int)charCode[charCodeBytesIndex]).ToString("X2");
          cmapBuffer.Append(hex);
          toUnicodeBuffer.Append(hex);
        }
        cmapBuffer.Append("> " + code + "\n");
        toUnicodeBuffer.Append("> <" + glyphIndexEntry.Key.ToString("X4") + ">\n");

        // CID-to-GID entry.
        int glyphIndex = glyphIndexEntry.Value;
        gIdBuffer.Append((byte)((glyphIndex >> 8) & 0xFF));
        gIdBuffer.Append((byte)(glyphIndex & 0xFF));

        // Width.
        int width;
        if(!glyphWidths.TryGetValue(glyphIndex, out width))
        {width = 0;}
        else if(width > 1000)
        {width = 1000;}
        widthsObject.Add(PdfInteger.Get(width));
      }
      cmapBuffer.Append(
        "endcidchar\n"
          + "endcmap\n"
          + "CMapName currentdict /CMap defineresource pop\n"
          + "end\n"
          + "end\n"
          + "%%EndResource\n"
          + "%%EOF"
        );
      PdfStream cmapStream = new PdfStream(cmapBuffer);
      PdfDictionary cmapHead = cmapStream.Header;
      cmapHead[PdfName.Type] = PdfName.CMap;
      cmapHead[PdfName.CMapName] = new PdfName("Adobe-Identity-UCS");
      cmapHead[PdfName.CIDSystemInfo] = new PdfDictionary(
        new PdfName[]
        {
          PdfName.Registry,
          PdfName.Ordering,
          PdfName.Supplement
        },
        new PdfDirectObject[]
        {
          new PdfTextString("Adobe"),
          new PdfTextString("Identity"),
          PdfInteger.Get(0)
        }
        ); // Generic predefined CMap (Identity-H/V (Adobe-Identity-0)) [PDF:1.6:5.6.4].
      font[PdfName.Encoding] = File.Register(cmapStream);

      PdfStream gIdStream = new PdfStream(gIdBuffer);
      cidFont[PdfName.CIDToGIDMap] = File.Register(gIdStream);

      cidFont[PdfName.W] = new PdfArray(new PdfDirectObject[]{PdfInteger.Get(1),widthsObject});

      toUnicodeBuffer.Append(
        "endbfchar\n"
          + "endcmap\n"
          + "CMapName currentdict /CMap defineresource pop\n"
          + "end\n"
          + "end\n"
        );
      PdfStream toUnicodeStream = new PdfStream(toUnicodeBuffer);
      font[PdfName.ToUnicode] = File.Register(toUnicodeStream);
    }

    /**
      <summary>Creates the font descriptor.</summary>
    */
    private PdfReference Load_CreateFontDescriptor(
      OpenFontParser parser
      )
    {
      PdfDictionary fontDescriptor = new PdfDictionary();
      {
        OpenFontParser.FontMetrics metrics = parser.Metrics;

        // Type.
        fontDescriptor[PdfName.Type] = PdfName.FontDescriptor;

        // FontName.
        fontDescriptor[PdfName.FontName] = BaseDataObject[PdfName.BaseFont];

        // Flags [PDF:1.6:5.7.1].
        FlagsEnum flags = 0;
        if(metrics.IsFixedPitch)
        {flags |= FlagsEnum.FixedPitch;}
        if(metrics.IsCustomEncoding)
        {flags |= FlagsEnum.Symbolic;}
        else
        {flags |= FlagsEnum.Nonsymbolic;}
        fontDescriptor[PdfName.Flags] = PdfInteger.Get(Convert.ToInt32(flags));

        // FontBBox.
        fontDescriptor[PdfName.FontBBox] = new Rectangle(
          new drawing::PointF(metrics.XMin * metrics.UnitNorm, metrics.YMin * metrics.UnitNorm),
          new drawing::PointF(metrics.XMax * metrics.UnitNorm, metrics.YMax * metrics.UnitNorm)
          ).BaseDataObject;

        // ItalicAngle.
        fontDescriptor[PdfName.ItalicAngle] = PdfReal.Get(metrics.ItalicAngle);

        // Ascent.
        fontDescriptor[PdfName.Ascent] = PdfReal.Get(
          metrics.Ascender == 0
            ? metrics.STypoAscender * metrics.UnitNorm
            : metrics.Ascender * metrics.UnitNorm
          );

        // Descent.
        fontDescriptor[PdfName.Descent] = PdfReal.Get(
          metrics.Descender == 0
            ? metrics.STypoDescender * metrics.UnitNorm
            : metrics.Descender * metrics.UnitNorm
          );

        // Leading.
        fontDescriptor[PdfName.Leading] = PdfReal.Get(metrics.STypoLineGap * metrics.UnitNorm);

        // CapHeight.
        fontDescriptor[PdfName.CapHeight] = PdfReal.Get(metrics.SCapHeight * metrics.UnitNorm);

        // StemV.
        /*
          NOTE: '100' is just a rule-of-thumb value, 'cause I've still to solve the
          'cvt' table puzzle (such a harsh headache!) for TrueType fonts...
          TODO:IMPL TrueType and CFF stemv real value to extract!!!
        */
        fontDescriptor[PdfName.StemV] = PdfInteger.Get(100);

        // FontFile.
        fontDescriptor[PdfName.FontFile2] = File.Register(
          new PdfStream(new bytes::Buffer(parser.FontData.ToByteArray()))
          );
      }
      return File.Register(fontDescriptor);
    }
    #endregion
    #endregion
    #endregion
  }
}