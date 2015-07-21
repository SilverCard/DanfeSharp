/*
  Copyright 2012 Stefano Chizzolini. http://www.pdfclown.org

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

using org.pdfclown.util;
using System;

namespace org.pdfclown.tokens
{
  /**
    <summary>Encoding for text strings in a PDF document outside the document's content streams
    [PDF:1.7:D].</summary>
  */
  public sealed class PdfDocEncoding
    : LatinEncoding
  {
    #region types
    private class Chars
      : BiDictionary<int,char>
    {
      internal Chars(
        )
      {
        this[0x80] = '\u2022';
        this[0x81] = '\u2020';
        this[0x82] = '\u2021';
        this[0x84] = '\u2014';
        this[0x85] = '\u2013';
        this[0x86] = '\u0192';
        this[0x87] = '\u2044';
        this[0x88] = '\u2039';
        this[0x89] = '\u203A';
        this[0x8A] = '\u2212';
        this[0x8B] = '\u2030';
        this[0x8C] = '\u201E';
        this[0x8D] = '\u201C';
        this[0x8E] = '\u201D';
        this[0x8F] = '\u2018';
        this[0x90] = '\u2019';
        this[0x91] = '\u201A';
        this[0x92] = '\u2122';
        this[0x93] = '\uFB01';
        this[0x94] = '\uFB02';
        this[0x95] = '\u0141';
        this[0x96] = '\u0152';
        this[0x97] = '\u0160';
        this[0x98] = '\u0178';
        this[0x99] = '\u017D';
        this[0x9A] = '\u0131';
        this[0x9B] = '\u0142';
        this[0x9C] = '\u0153';
        this[0x9D] = '\u0161';
        this[0x9E] = '\u017E';
        this[0x9F] = '\u009F';
        this[0xA0] = '\u20AC';
      }

      private bool IsIdentity(
        int code
        )
      {return code < 128 || (code > 160 && code < 256);}

      public override int Count
      {
        get
        {return 256;}
      }

      public override int GetKey(
        char value
        )
      {
        return IsIdentity(value)
          ? (int)value
          : base.GetKey(value);
      }

      public override char this[
        int key
        ]
      {
        get
        {
          return IsIdentity(key)
            ? (char)key
            : base[key];
        }
      }
    }
    #endregion

    #region static
    #region fields
    private static readonly PdfDocEncoding instance = new PdfDocEncoding();
    #endregion

    #region interface
    public static PdfDocEncoding Get(
      )
    {return instance;}
    #endregion
    #endregion

    #region dynamic
    #region constructors
    private PdfDocEncoding(
      )
    {chars = new Chars();}
    #endregion
    #endregion
  }
}