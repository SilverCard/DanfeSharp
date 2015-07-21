/*
  Copyright 2010 Stefano Chizzolini. http://www.pdfclown.org

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
    <summary>Winding rule for determining which points lie inside a path [PDF:1.6:4.4.2].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public enum WindModeEnum
  {
    /**
      <summary>Even-odd winding rule.</summary>
    */
    EvenOdd,
    /**
      <summary>Non-zero winding rule.</summary>
    */
    NonZero
  };

  internal static class WindModeEnumExtension
  {
    /**
      <summary>Converts this constant into its equivalent GDI+ code.</summary>
    */
    public static FillMode ToGdi(
      this WindModeEnum windMode
      )
    {
      switch(windMode)
      {
        case WindModeEnum.EvenOdd:
          return FillMode.Alternate;
        case WindModeEnum.NonZero:
          return FillMode.Winding;
        default:
          throw new NotSupportedException(windMode + " convertion not supported.");
      }
    }
  }
}