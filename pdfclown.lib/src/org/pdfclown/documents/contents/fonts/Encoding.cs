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
    <summary>Predefined encodings [PDF:1.6:5.5.5,D].</summary>
  */
  // TODO: This hierarchy is going to be superseded by org.pdfclown.tokens.Encoding.
  internal class Encoding
  {
    #region static
    #region fields
    private static readonly Dictionary<PdfName,Encoding> Encodings = new Dictionary<PdfName,Encoding>();
    #endregion

    #region constructors
    static Encoding(
      )
    {
    //TODO:this collection MUST be automatically populated looking for Encoding subclasses!
      Encodings[PdfName.StandardEncoding] = new StandardEncoding();
      Encodings[PdfName.MacRomanEncoding] = new MacRomanEncoding();
      Encodings[PdfName.WinAnsiEncoding] = new WinAnsiEncoding();
    }
    #endregion

    #region interface
    public static Encoding Get(
      PdfName name
      )
    {return Encodings[name];}
    #endregion
    #endregion

    #region dynamic
    #region fields
    private readonly Dictionary<ByteArray,int> codes = new Dictionary<ByteArray,int>();
    #endregion

    #region interface
    #region public
    public Dictionary<ByteArray,int> GetCodes(
      )
    {return new Dictionary<ByteArray,int>(codes);}
    #endregion

    #region protected
    protected void Put(
      int charCode,
      string charName
      )
    {codes[new ByteArray(new byte[]{(byte)charCode})] = GlyphMapping.NameToCode(charName).Value;}
    #endregion
    #endregion
    #endregion
  }
}