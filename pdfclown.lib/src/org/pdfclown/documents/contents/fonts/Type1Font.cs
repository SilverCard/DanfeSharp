/*
  Copyright 2007-2012 Stefano Chizzolini. http://www.pdfclown.org

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

using org.pdfclown.bytes;
using org.pdfclown.documents;
using org.pdfclown.objects;
using org.pdfclown.util;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using text = System.Text;
using System.Text.RegularExpressions;

namespace org.pdfclown.documents.contents.fonts
{
  /**
    <summary>Type 1 font [PDF:1.6:5.5.1;AFM:4.1].</summary>
  */
  /*
    NOTE: Type 1 fonts encompass several formats:
    * AFM+PFB;
    * CFF;
    * OpenFont/CFF (in case "CFF" table's Top DICT has no CIDFont operators).
  */
  [PDF(VersionEnum.PDF10)]
  public class Type1Font
    : SimpleFont
  {
    #region dynamic
    #region fields
    protected AfmParser.FontMetrics metrics;
    #endregion

    #region constructors
    internal Type1Font(
      Document context
      ) : base(context)
    {}

    internal Type1Font(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region protected
    protected virtual IDictionary<ByteArray,int> GetNativeEncoding(
      )
    {
      PdfDictionary descriptor = Descriptor;
      if(descriptor.ContainsKey(PdfName.FontFile)) // Embedded noncompact Type 1 font.
      {
        PdfStream fontFileStream = (PdfStream)descriptor.Resolve(PdfName.FontFile);
        PfbParser parser = new PfbParser(fontFileStream.Body);
        return parser.Parse();
      }
      else if(descriptor.ContainsKey(PdfName.FontFile3)) // Embedded compact Type 1 font.
      {
        PdfStream fontFileStream = (PdfStream)descriptor.Resolve(PdfName.FontFile3);
        PdfName fontFileSubtype = (PdfName)fontFileStream.Header[PdfName.Subtype];
        if(fontFileSubtype.Equals(PdfName.Type1C)) // CFF.
        {
          CffParser parser = new CffParser(fontFileStream.Body);
          IDictionary<ByteArray,int> codes = new Dictionary<ByteArray,int>();
          foreach(KeyValuePair<int,int> glyphIndexEntry in parser.glyphIndexes)
          {
            /*
              FIXME: Custom (non-unicode) encodings require name handling to match encoding
              differences; this method (getNativeEncoding) should therefore return a glyphindex-to-
              character-name map instead.
              Constraining native codes into target byte-arrayed encodings is wrong -- that should
              be only the final stage.
             */
            codes[new ByteArray(new byte[]{ConvertUtils.IntToByteArray(glyphIndexEntry.Value)[3]})] = glyphIndexEntry.Key;
          }
          return codes;
        }
        else if(fontFileSubtype.Equals(PdfName.OpenType)) // OpenFont/CFF.
        {throw new NotImplementedException("Embedded OpenFont/CFF font file.");}
        else
        {throw new NotSupportedException("Unsupported embedded font file format: " + fontFileSubtype);}
      }
      else // Non-embedded font.
      {return Encoding.Get(PdfName.StandardEncoding).GetCodes();}
    }

    protected override void LoadEncoding(
      )
    {//TODO: set symbolic = true/false; depending on the actual encoding!!!
      // Encoding.
      if(this.codes == null)
      {
        IDictionary<ByteArray,int> codes;
        PdfDataObject encodingObject = BaseDataObject.Resolve(PdfName.Encoding);
        if(encodingObject == null) // Native encoding.
        {codes = GetNativeEncoding();}
        else if(encodingObject is PdfName) // Predefined encoding.
        {codes = Encoding.Get((PdfName)encodingObject).GetCodes();}
        else // Custom encoding.
        {
          PdfDictionary encodingDictionary = (PdfDictionary)encodingObject;

          // 1. Base encoding.
          PdfName baseEncodingName = (PdfName)encodingDictionary[PdfName.BaseEncoding];
          if(baseEncodingName == null) // Native base encoding.
          {codes = GetNativeEncoding();}
          else // Predefined base encoding.
          {codes = Encoding.Get(baseEncodingName).GetCodes();}

          // 2. Differences.
          LoadEncodingDifferences(encodingDictionary, codes);
        }
        this.codes = new BiDictionary<ByteArray,int>(codes);
      }

      // Glyph indexes.
      if(glyphIndexes == null)
      {
        glyphIndexes = new Dictionary<int,int>();
        foreach(KeyValuePair<ByteArray,int> codeEntry in codes)
        {glyphIndexes[codeEntry.Value] = ConvertUtils.ByteArrayToInt(codeEntry.Key.Data);}
      }
    }
    #endregion
    #endregion
    #endregion
  }
}