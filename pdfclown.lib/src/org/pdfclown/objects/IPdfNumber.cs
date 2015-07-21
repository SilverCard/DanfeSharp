/*
  Copyright 2006-2011 Stefano Chizzolini. http://www.pdfclown.org

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

namespace org.pdfclown.objects
{
  /**
    <summary>PDF number interface.</summary>
  */
  public interface IPdfNumber
    : IPdfSimpleObject<double>
  {
    /**
      <summary>Gets the double-precision floating-point representation of the value.</summary>
    */
    double DoubleValue
    {
      get;
    }

    /**
      <summary>Gets the floating-point representation of the value.</summary>
    */
    float FloatValue
    {
      get;
    }

    /**
      <summary>Gets the integer representation of the value.</summary>
    */
    int IntValue
    {
      get;
    }
  }

  internal class PdfNumber
  {
    public static int Compare(
      object obj1,
      object obj2
      )
    {
      if(!(obj1 is IPdfNumber))
        throw new ArgumentException("obj1 MUST implement IPdfNumber");
      if(!(obj2 is IPdfNumber))
        throw new ArgumentException("obj2 MUST implement IPdfNumber");

      return ((IPdfNumber)obj1).RawValue.CompareTo(((IPdfNumber)obj2).RawValue);
    }

    public static bool Equal(
      object obj1,
      object obj2
      )
    {
      if(!(obj1 is IPdfNumber))
        throw new ArgumentException("obj1 MUST implement IPdfNumber");
      if(!(obj2 is IPdfNumber))
        throw new ArgumentException("obj2 MUST implement IPdfNumber");

      return ((IPdfNumber)obj1).RawValue.Equals(((IPdfNumber)obj2).RawValue);
    }

    public static int GetHashCode(
      object obj
      )
    {
      if(!(obj is IPdfNumber))
        throw new ArgumentException("obj MUST implement IPdfNumber");

      double value = ((IPdfNumber)obj).RawValue;
      int intValue = (int)value;

      return value == intValue ? intValue.GetHashCode() : value.GetHashCode();
    }
  }
}