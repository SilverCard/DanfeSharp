/*
  Copyright 2010-2011 Stefano Chizzolini. http://www.pdfclown.org

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

using System;
using System.Drawing;

namespace org.pdfclown.documents.contents
{
  /**
    <summary>Rotation (clockwise) [PDF:1.6:3.6.2].</summary>
  */
  public enum RotationEnum
  {
    /**
      Downward (0° clockwise).
    */
    Downward = 0,
    /**
      Leftward (90° clockwise).
    */
    Leftward = 90,
    /**
      Upward (180° clockwise).
    */
    Upward = 180,
    /**
      Rightward (270° clockwise).
    */
    Rightward = 270
  }

  internal static class RotationEnumExtension
  {
    /**
      <summary>Gets the direction corresponding to the given value.</summary>
    */
    public static RotationEnum Get(
      PdfInteger value
      )
    {
      if(value == null)
        return RotationEnum.Downward;

      int normalizedValue = (int)(Math.Round(value.RawValue / 90d) % 4) * 90;
      if(normalizedValue < 0)
      {normalizedValue += 360 * (int)Math.Ceiling(-normalizedValue / 360d);}
      return (RotationEnum)normalizedValue;
    }

    public static SizeF Transform(
      this RotationEnum rotation,
      SizeF size
      )
    {
      if((int)rotation % 180 == 0)
        return new SizeF(size.Width, size.Height);
      else
        return new SizeF(size.Height, size.Width);
    }
  }
}