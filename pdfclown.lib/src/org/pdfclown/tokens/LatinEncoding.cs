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
    <summary>Adobe standard Latin character set [PDF:1.7:D].</summary>
  */
  public class LatinEncoding
    : Encoding
  {
    #region dynamic
    #region fields
    /**
      <summary>Code-to-Unicode map.</summary>
    */
    protected BiDictionary<int,char> chars;
    #endregion

    #region interface
    public override string Decode(
      byte[] value
      )
    {return Decode(value, 0, value.Length);}

    public override string Decode(
      byte[] value,
      int index,
      int length
      )
    {
      char[] stringChars = new char[length];
      for(int decodeIndex = index, decodeLength = length + index; decodeIndex < decodeLength; decodeIndex++)
      {stringChars[decodeIndex - index] = chars[value[decodeIndex] & 0xff];}
      return new String(stringChars);
    }

    public override byte[] Encode(
      string value
      )
    {
      char[] stringChars = value.ToCharArray();
      byte[] stringBytes = new byte[stringChars.Length];
      for(int index = 0, length = stringChars.Length; index < length; index++)
      {
        int code = chars.GetKey(stringChars[index]);
        if(code == 0) //TODO: verify whether 0 collides with valid code values.
          return null;

        stringBytes[index] = (byte)code;
      }
      return stringBytes;
    }
    #endregion
    #endregion
  }
}