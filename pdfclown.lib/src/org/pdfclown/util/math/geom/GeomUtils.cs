/*
  Copyright 2012 Stefano Chizzolini. http://www.pdfclown.org

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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace org.pdfclown.util.math.geom
{
  /**
    <summary>Geometric utilities.</summary>
  */
  public class GeomUtils
  {
    /**
      <summary>Gets the size scaled to the specified limit.</summary>
      <remarks>In particular, the limit matches the largest dimension and proportionally scales the
      other one; for example, a limit 300 applied to size Dimension2D(100, 200) returns
      Dimension2D(150, 300).</remarks>
      <param name="size">Size to scale.</param>
      <param name="limit">Scale limit.</param>
      <returns>Scaled size.</returns>
    */
    public static SizeF Scale(
      SizeF size,
      float limit
      )
    {
      if(limit == 0)
        return new SizeF(size);
      else
      {
        float sizeRatio = size.Width / size.Height;
        return sizeRatio > 1
          ? new SizeF(limit, limit / sizeRatio)
          : new SizeF(limit * sizeRatio, limit);
      }
    }

    /**
      <summary>Gets the size scaled to the specified limit.</summary>
      <remarks>In particular, implicit (zero-valued) limit dimensions correspond to proportional
      dimensions; for example, a limit Dimension2D(0, 300) means 300 high and proportionally wide.
      </remarks>
      <param name="size">Size to scale.</param>
      <param name="limit">Scale limit.</param>
      <returns>Scaled size.</returns>
    */
    public static SizeF Scale(
      SizeF size,
      SizeF limit
      )
    {
      if(limit.Width == 0)
      {
        if(limit.Height == 0)
          return new SizeF(size);
        else
          return new SizeF(limit.Height * size.Width / size.Height, limit.Height);
      }
      else if(limit.Height == 0)
        return new SizeF(limit.Width, limit.Width * size.Height / size.Width);
      else
        return new SizeF(limit);
    }
  }
}