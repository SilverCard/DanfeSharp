/*
  Copyright 2009-2010 Stefano Chizzolini. http://www.pdfclown.org

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

using org.pdfclown.util.io;

using System;
using System.Text;

namespace org.pdfclown.util
{
  /**
    <summary>Data convertion utility.</summary>
    <remarks>This class is a specialized adaptation from the original <a href="http://commons.apache.org/codec/">
    Apache Commons Codec</a> project, licensed under the <a href="http://www.apache.org/licenses/LICENSE-2.0">
    Apache License, Version 2.0</a>.</remarks>
  */
  public static class ConvertUtils
  {
    #region static
    #region fields
    private static readonly char[] HexDigits = {'0','1','2','3','4','5','6','7','8','9','a','b','c','d','e','f'};
    #endregion

    #region interface
    #region public
    public static string ByteArrayToHex(
      byte[] data
      )
    {
      int dataLength = data.Length;
      char[] result = new char[dataLength * 2];
      for(
        int dataIndex = 0,
          resultIndex = 0;
        dataIndex < dataLength;
        dataIndex++
        )
      {
        result[resultIndex++] = HexDigits[(0xF0 & data[dataIndex]) >> 4];
        result[resultIndex++] = HexDigits[0x0F & data[dataIndex]];
      }
      return new string(result);
    }

    public static int ByteArrayToInt(
      byte[] data
      )
    {return ByteArrayToInt(data,0,ByteOrderEnum.BigEndian);}

    public static int ByteArrayToInt(
      byte[] data,
      int index,
      ByteOrderEnum byteOrder
      )
    {return ByteArrayToNumber(data,index,4,byteOrder);}

    public static int ByteArrayToNumber(
      byte[] data,
      int index,
      int length,
      ByteOrderEnum byteOrder
      )
    {
      int value = 0;
      length = (int)Math.Min(length,data.Length-index);
      for(
        int i = index,
          endIndex = index+length;
        i < endIndex;
        i++
        )
      {value |= (data[i] & 0xff) << 8 * (byteOrder == ByteOrderEnum.LittleEndian ? i-index : endIndex-i-1);}
      return value;
    }

    public static byte[] HexToByteArray(
      string data
      )
    {
      byte[] result;
      {
        int dataLength = data.Length;
        if((dataLength % 2) != 0)
          throw new Exception("Odd number of characters.");

        result = new byte[dataLength / 2];
        for(
          int resultIndex = 0,
            dataIndex = 0;
          dataIndex < dataLength;
          resultIndex++
          )
        {
          result[resultIndex] = byte.Parse(
            data[dataIndex++].ToString() + data[dataIndex++].ToString(),
            System.Globalization.NumberStyles.HexNumber
            );
        }
      }
      return result;
    }

    public static byte[] IntToByteArray(
      int data
      )
    {return new byte[]{(byte)(data >> 24), (byte)(data >> 16), (byte)(data >> 8), (byte)data};}

    public static byte[] NumberToByteArray(
      int data,
      int length,
      ByteOrderEnum byteOrder
      )
    {
      byte[] result = new byte[length];
      for(
        int index = 0;
        index < length;
        index++
        )
      {result[index] = (byte)(data >> 8 * (byteOrder == ByteOrderEnum.LittleEndian ? index : length-index-1));}
      return result;
    }

    public static float[] ToFloatArray(
      double[] array
      )
    {
      float[] result = new float[array.Length];
      for(int index = 0, length = array.Length; index < length; index++)
      {result[index] = (float)array[index];}
      return result;
    }
    #endregion
    #endregion
    #endregion
  }
}