/*
  Copyright 2009-2011 Stefano Chizzolini. http://www.pdfclown.org

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

using org.pdfclown.objects;
using org.pdfclown.util;

using System;
using System.Collections.Generic;

namespace org.pdfclown.documents.contents.fonts
{
  /**
    <summary>Simple font [PDF:1.6:5.5].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public abstract class SimpleFont
    : Font
  {
    #region constructors
    protected SimpleFont(
      Document context
      ) : base(context)
    {}

    protected SimpleFont(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region protected
    protected override PdfDictionary Descriptor
    {get{return (PdfDictionary)BaseDataObject.Resolve(PdfName.FontDescriptor);}}

    protected abstract void LoadEncoding(
      );

    /**
      <summary>Loads the encoding differences into the given collection.</summary>
      <param name="encodingDictionary">Encoding dictionary.</param>
      <param name="codes">Encoding to alter applying differences.</param>
    */
    protected void LoadEncodingDifferences(
      PdfDictionary encodingDictionary,
      IDictionary<ByteArray,int> codes
      )
    {
      PdfArray differenceObjects = (PdfArray)encodingDictionary.Resolve(PdfName.Differences);
      if(differenceObjects == null)
        return;

      /*
        NOTE: Each code is the first index in a sequence of character codes to be changed.
        The first character name after the code becomes the name corresponding to that code.
        Subsequent names replace consecutive code indices until the next code appears
        in the array or the array ends.
      */
      byte[] charCodeData = new byte[1];
      foreach(PdfDirectObject differenceObject in differenceObjects)
      {
        if(differenceObject is PdfInteger)
        {charCodeData[0] = (byte)(((int)((PdfInteger)differenceObject).Value) & 0xFF);}
        else // NOTE: MUST be PdfName.
        {
          ByteArray charCode = new ByteArray(charCodeData);
          string charName = (string)((PdfName)differenceObject).Value;
          if(charName.Equals(".notdef"))
          {codes.Remove(charCode);}
          else
          {
            int? code = GlyphMapping.NameToCode(charName);
            codes[charCode] = (code ?? charCodeData[0]);
          }
          charCodeData[0]++;
        }
      }
    }

    protected override void OnLoad(
      )
    {
      LoadEncoding();

      // Glyph widths.
      if(glyphWidths == null)
      {
        glyphWidths = new Dictionary<int,int>();
        PdfArray glyphWidthObjects = (PdfArray)BaseDataObject.Resolve(PdfName.Widths);
        if(glyphWidthObjects != null)
        {
          ByteArray charCode = new ByteArray(
            new byte[]
            {(byte)((PdfInteger)BaseDataObject[PdfName.FirstChar]).IntValue}
            );
          foreach(PdfDirectObject glyphWidthObject in glyphWidthObjects)
          {
            int glyphWidth = ((IPdfNumber)glyphWidthObject).IntValue;
            if(glyphWidth > 0)
            {
              int code;
              if(codes.TryGetValue(charCode,out code))
              {glyphWidths[glyphIndexes[code]] = glyphWidth;}
            }
            charCode.Data[0]++;
          }
        }
      }
      // Default glyph width.
      {
        PdfDictionary descriptor = Descriptor;
        if(descriptor != null)
        {
          IPdfNumber defaultGlyphWidthObject = (IPdfNumber)descriptor[PdfName.MissingWidth];
          defaultGlyphWidth = (defaultGlyphWidthObject != null ? defaultGlyphWidthObject.IntValue : 0);
        }
      }
    }
    #endregion
    #endregion
  }
}