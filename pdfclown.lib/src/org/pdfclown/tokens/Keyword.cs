/*
  Copyright 2010-2011 Stefano Chizzolini. http://www.pdfclown.org

  Contributors:
    * Stefano Chizzolini (original code developer, http://www.stefanochizzolini.it)

  This file should be part of the source code distribution of "PDF Clown library"
  (the Program): see the accompanying README files for more info.

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

namespace org.pdfclown.tokens
{
  /**
    <summary>PDF keywords.</summary>
  */
  public static class Keyword
  {
    /**
      <summary>PDF array opening delimiter.</summary>
    */
    public static readonly string BeginArray = Symbol.OpenSquareBracket.ToString();
    /**
      <summary>PDF comment opening delimiter.</summary>
    */
    public static readonly string BeginComment = Symbol.Percent.ToString();
    /**
      <summary>PDF dictionary opening delimiter.</summary>
    */
    public static readonly string BeginDictionary = Symbol.OpenAngleBracket.ToString() + Symbol.OpenAngleBracket.ToString();
    /**
      <summary>PDF indirect object begin.</summary>
    */
    public const string BeginIndirectObject = "obj";
    /**
      <summary>PDF literal string opening delimiter.</summary>
    */
    public static readonly string BeginLiteralString = Symbol.OpenRoundBracket.ToString();
    /**
      <summary>PDF stream data begin.</summary>
    */
    public const string BeginStream = "stream";
    /**
      <summary>PDF file begin.</summary>
    */
    public const string BOF = "%PDF-";
    /**
      <summary>PDF date marker.</summary>
    */
    public const string DatePrefix = "D:";
    /**
      <summary>PDF array closing delimiter.</summary>
    */
    public static readonly string EndArray = Symbol.CloseSquareBracket.ToString();
    /**
      <summary>PDF dictionary closing delimiter.</summary>
    */
    public static readonly string EndDictionary = Symbol.CloseAngleBracket.ToString() + Symbol.CloseAngleBracket.ToString();
    /**
      <summary>PDF indirect object end.</summary>
    */
    public const string EndIndirectObject = "endobj";
    /**
      <summary>PDF literal string closing delimiter.</summary>
    */
    public static readonly string EndLiteralString = Symbol.CloseRoundBracket.ToString();
    /**
      <summary>PDF stream data end.</summary>
    */
    public const string EndStream = "endstream";
    /**
      <summary>PDF file end.</summary>
    */
    public const string EOF = "%%EOF";
    /**
      <summary>PDF boolean false.</summary>
    */
    public const string False = "false";
    /**
      <summary>PDF free xref entry marker.</summary>
    */
    public const string FreeXrefEntry = "f";
    /**
      <summary>PDF in-use xref entry marker.</summary>
    */
    public const string InUseXrefEntry = "n";
    /**
      <summary>PDF name marker.</summary>
    */
    public static readonly string NamePrefix = Symbol.Slash.ToString();
    /**
      <summary>PDF null object.</summary>
    */
    public const string Null = "null";
    /**
      <summary>PDF indirect reference marker.</summary>
    */
    public static readonly string Reference = Symbol.CapitalR.ToString();
    /**
      <summary>PDF xref start offset.</summary>
    */
    public const string StartXRef = "startxref";
    /**
      <summary>PDF trailer begin.</summary>
    */
    public const string Trailer = "trailer";
    /**
      <summary>PDF boolean true.</summary>
    */
    public const string True = "true";
    /**
      <summary>PDF xref begin.</summary>
    */
    public const string XRef = "xref";
  }
}

