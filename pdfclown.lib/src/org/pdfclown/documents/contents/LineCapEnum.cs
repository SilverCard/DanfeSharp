/*
  Copyright 2006-2010 Stefano Chizzolini. http://www.pdfclown.org

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
using System.Drawing.Drawing2D;

namespace org.pdfclown.documents.contents
{
  /**
    <summary>Shape to be used at the ends of stroked open subpaths
    [PDF:1.6:4.3.2].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public enum LineCapEnum
  {
    /**
      <summary>Truncated line cap.</summary>
    */
    Butt = 0,
    /**
      <summary>Rounded line cap.</summary>
    */
    Round = 1,
    /**
      <summary>Squared-off line cap.</summary>
    */
    Square = 2
  };

  internal static class LineCapEnumExtension
  {
    public static LineCap ToGdi(
      this LineCapEnum lineCap
      )
    {
      switch(lineCap)
      {
        case LineCapEnum.Butt:
         return LineCap.Flat;
        case LineCapEnum.Round:
        return LineCap.Round;
        case LineCapEnum.Square:
         return LineCap.Square;
        default:
         throw new NotSupportedException(lineCap + " convertion not supported.");
      }
    }

    public static DashCap ToDashCap(
      this LineCap lineCap
      )
    {
      switch(lineCap)
      {
        case LineCap.Round:
        case LineCap.RoundAnchor:
          return DashCap.Round;
        case LineCap.Triangle:
          return DashCap.Triangle;
        default:
          return DashCap.Flat;
      }
    }
  }
}