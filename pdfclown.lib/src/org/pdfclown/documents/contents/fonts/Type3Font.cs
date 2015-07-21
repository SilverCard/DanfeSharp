/*
  Copyright 2010-2012 Stefano Chizzolini. http://www.pdfclown.org

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
    <summary>Type 3 font [PDF:1.6:5.5.4].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class Type3Font
    : SimpleFont
  {
    #region dynamic
    #region constructors
     internal Type3Font(
       Document context
       ) : base(context)
     {}

    internal Type3Font(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    public override double Ascent
    {
      get
      {return 0;}
    }

    public override double Descent
    {
      get
      {return 0;}
    }
    #endregion

    #region protected
    protected override void LoadEncoding(
      )
    {
      //FIXME: consolidate with Type1Font and TrueTypeFont!
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

    #region private
    private IDictionary<ByteArray,int> GetNativeEncoding(
      )
    {
      //FIXME: consolidate with Type1Font and TrueTypeFont!
      return Encoding.Get(PdfName.StandardEncoding).GetCodes();
    }
    #endregion
    #endregion
    #endregion
  }
}