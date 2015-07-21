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

using org.pdfclown.documents;
using org.pdfclown.files;
using org.pdfclown.objects;
using org.pdfclown.util;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace org.pdfclown.documents.contents.fonts
{
  /**
    <summary>TrueType font [PDF:1.6:5;OFF:2009].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class TrueTypeFont
    : SimpleFont
  {
    #region dynamic
    #region constructors
    internal TrueTypeFont(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region protected
    protected override void LoadEncoding(
      )
    {
      OpenFontParser parser;
      {
        PdfDictionary descriptor = Descriptor;
        if(descriptor.ContainsKey(PdfName.FontFile2)) // Embedded TrueType font file (without 'glyf' table).
        {
          PdfStream fontFileStream = (PdfStream)descriptor.Resolve(PdfName.FontFile2);
          parser = new OpenFontParser(fontFileStream.Body);
        }
        else if(descriptor.ContainsKey(PdfName.FontFile3))
        {
          PdfStream fontFileStream = (PdfStream)descriptor.Resolve(PdfName.FontFile3);
          PdfName fontFileSubtype = (PdfName)fontFileStream.Header[PdfName.Subtype];
          if(fontFileSubtype.Equals(PdfName.OpenType)) // Embedded OpenFont/TrueType font file (with 'glyf' table).
          {parser = new OpenFontParser(fontFileStream.Body);}
          else // Unknown.
            throw new NotSupportedException("Unknown embedded font file format: " + fontFileSubtype);
        }
        else
        {parser = null;}
      }
      if(parser != null) // Embedded font file.
      {
        // Glyph indexes.
        glyphIndexes = parser.GlyphIndexes;
        if(codes != null
          && parser.Metrics.IsCustomEncoding)
        {
          /*
            NOTE: In case of symbolic font,
            glyph indices are natively mapped to character codes,
            so they must be remapped to Unicode whenever possible
            (i.e. when ToUnicode stream is available).
          */
          Dictionary<int,int> unicodeGlyphIndexes = new Dictionary<int,int>();
          foreach(KeyValuePair<int,int> glyphIndexEntry in glyphIndexes)
          {
            int code;
            if(!codes.TryGetValue(new ByteArray(new byte[]{(byte)(int)glyphIndexEntry.Key}),out code))
              continue;

            unicodeGlyphIndexes[code] = glyphIndexEntry.Value;
          }
          glyphIndexes = unicodeGlyphIndexes;
        }
      }

      PdfDataObject encodingObject = BaseDataObject.Resolve(PdfName.Encoding);
      FlagsEnum flags = Flags;
      if((flags & FlagsEnum.Symbolic) != 0
        || ((flags & FlagsEnum.Nonsymbolic) == 0 && encodingObject == null)) // Symbolic.
      {
        symbolic = true;

        if(glyphIndexes == null)
        {
          /*
            NOTE: In case no font file is available, we have to synthesize its metrics
            from existing entries.
          */
          glyphIndexes = new Dictionary<int,int>();
          PdfArray glyphWidthObjects = (PdfArray)BaseDataObject.Resolve(PdfName.Widths);
          if(glyphWidthObjects != null)
          {
            int code = ((PdfInteger)BaseDataObject[PdfName.FirstChar]).RawValue;
            foreach(PdfDirectObject glyphWidthObject in glyphWidthObjects)
            {
              if(((PdfInteger)glyphWidthObject).RawValue > 0)
              {glyphIndexes[code] = code;}

              code++;
            }
          }
        }

        if(this.codes == null)
        {
          Dictionary<ByteArray,int> codes = new Dictionary<ByteArray,int>();
          foreach(KeyValuePair<int,int> glyphIndexEntry in glyphIndexes)
          {
            if(glyphIndexEntry.Value > 0)
            {
              int glyphCharCode = glyphIndexEntry.Key;
              byte[] charCode = new byte[]{(byte)glyphCharCode};
              codes[new ByteArray(charCode)] = glyphCharCode;
            }
          }
          this.codes = new BiDictionary<ByteArray,int>(codes);
        }
      }
      else // Nonsymbolic.
      {
        symbolic = false;

        if(this.codes == null)
        {
          Dictionary<ByteArray,int> codes;
          if(encodingObject == null) // Default encoding.
          {codes = Encoding.Get(PdfName.StandardEncoding).GetCodes();}
          else if(encodingObject is PdfName) // Predefined encoding.
          {codes = Encoding.Get((PdfName)encodingObject).GetCodes();}
          else // Custom encoding.
          {
            PdfDictionary encodingDictionary = (PdfDictionary)encodingObject;

            // 1. Base encoding.
            PdfName baseEncodingName = (PdfName)encodingDictionary[PdfName.BaseEncoding];
            if(baseEncodingName == null) // Default base encoding.
            {codes = Encoding.Get(PdfName.StandardEncoding).GetCodes();}
            else // Predefined base encoding.
            {codes = Encoding.Get(baseEncodingName).GetCodes();}

            // 2. Differences.
            LoadEncodingDifferences(encodingDictionary, codes);
          }
          this.codes = new BiDictionary<ByteArray,int>(codes);
        }

        if(glyphIndexes == null)
        {
          /*
            NOTE: In case no font file is available, we have to synthesize its metrics
            from existing entries.
          */
          glyphIndexes = new Dictionary<int,int>();
          PdfArray glyphWidthObjects = (PdfArray)BaseDataObject.Resolve(PdfName.Widths);
          if(glyphWidthObjects != null)
          {
            ByteArray charCode = new ByteArray(
              new byte[]
              {(byte)(int)((PdfInteger)BaseDataObject[PdfName.FirstChar]).RawValue}
              );
            foreach(PdfDirectObject glyphWidthObject in glyphWidthObjects)
            {
              if(((PdfInteger)glyphWidthObject).RawValue > 0)
              {
                int code;
                if(codes.TryGetValue(charCode,out code))
                {glyphIndexes[code] = (int)charCode.Data[0];}
              }
              charCode.Data[0]++;
            }
          }
        }
      }
    }
    #endregion
    #endregion
    #endregion
  }
}