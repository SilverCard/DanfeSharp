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

using System;

namespace org.pdfclown.util.math
{
  /**
    <summary>Specialized math operations.</summary>
  */
  public sealed class OperationUtils
  {
    #region static
    #region fields
    /**
      <summary>Double-precision floating-point exponent bias.</summary>
    */
    private const int DoubleExponentBias = 1023;
    /**
      <summary>Double-precision floating-point exponent field bit mask.</summary>
    */
    private const long DoubleExponentBitMask = 0x7FF0000000000000L;
    /**
      <summary>Double-precision floating-point significand bit count, excluding the implicit one.
      </summary>
    */
    private const int DoubleSignificandBitCount = 52;
    /**
      <summary>Default relative floating-point precision error tolerance.</summary>
    */
    private const double Epsilon = 0.000001;
    #endregion

    #region interface
    /**
      <summary>Compares double-precision floating-point numbers applying the default error tolerance.
      </summary>
      <param name="value1">First argument to compare.</param>
      <param name="value2">Second argument to compare.</param>
      <returns>How the first argument compares to the second:
        <list type="bullet">
          <item>-1, smaller;</item>
          <item>0, equal;</item>
          <item>1, greater.</item>
        </list>
      </returns>
    */
    public static int Compare(
      double value1,
      double value2
      )
    {return Compare(value1, value2, Epsilon);}

    /**
      <summary>Compares double-precision floating-point numbers applying the specified error tolerance.
      </summary>
      <param name="value1">First argument to compare.</param>
      <param name="value2">Second argument to compare.</param>
      <param name="epsilon">Relative error tolerance.</param>
      <returns>How the first argument compares to the second:
        <list type="bullet">
          <item>-1, smaller;</item>
          <item>0, equal;</item>
          <item>1, greater.</item>
        </list>
      </returns>
    */
    public static int Compare(
      double value1,
      double value2,
      double epsilon
      )
    {
      int exponent = GetExponent(Math.Max(value1, value2));
      double delta = epsilon * Math.Pow(2, exponent);
      double difference = value1 - value2;
      if (difference > delta)
        return 1;
      else if (difference < -delta)
        return -1;
      else
        return 0;
    }

    /**
      <summary>Compares big-endian byte arrays.</summary>
      <param name="data1">First argument to compare.</param>
      <param name="data2">Second argument to compare.</param>
      <returns>How the first argument compares to the second:
        <list type="bullet">
          <item>-1, smaller;</item>
          <item>0, equal;</item>
          <item>1, greater.</item>
        </list>
      </returns>
    */
    public static int Compare(
      byte[] data1,
      byte[] data2
      )
    {
      for(
        int index = 0,
          length = data1.Length;
        index < length;
        index++
        )
      {
        switch((int)Math.Sign((data1[index] & 0xff)-(data2[index] & 0xff)))
        {
          case -1:
            return -1;
          case 1:
            return 1;
        }
      }
      return 0;
    }

    /**
      <summary>Increments a big-endian byte array.</summary>
    */
    public static void Increment(
      byte[] data
      )
    {Increment(data, data.Length-1);}

    /**
      <summary>Increments a big-endian byte array at the specified position.</summary>
    */
    public static void Increment(
      byte[] data,
      int position
      )
    {
      if((data[position] & 0xff) == 255)
      {
        data[position] = 0;
        Increment(data, position-1);
      }
      else
      {data[position]++;}
    }

    /**
      <summary>Gets the unbiased exponent of the specified argument.</summary>
    */
    private static int GetExponent(
      double value
      )
    {return (int)(((BitConverter.DoubleToInt64Bits(value) & DoubleExponentBitMask) >> (DoubleSignificandBitCount)) - DoubleExponentBias);}
    #endregion
    #endregion
  }
}