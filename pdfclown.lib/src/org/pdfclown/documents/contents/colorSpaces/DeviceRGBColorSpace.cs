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

using org.pdfclown.documents;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;
using drawing = System.Drawing;

namespace org.pdfclown.documents.contents.colorSpaces
{
  /**
    <summary>Device Red-Green-Blue color space [PDF:1.6:4.5.3].</summary>
  */
  [PDF(VersionEnum.PDF11)]
  public sealed class DeviceRGBColorSpace
    : DeviceColorSpace
  {
    #region static
    #region fields
    /*
      NOTE: It may be specified directly (i.e. without being defined in the ColorSpace subdictionary
      of the contextual resource dictionary) [PDF:1.6:4.5.7].
    */
    public static readonly DeviceRGBColorSpace Default = new DeviceRGBColorSpace(PdfName.DeviceRGB);
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public DeviceRGBColorSpace(
      Document context
      ) : base(context, PdfName.DeviceRGB)
    {}

    internal DeviceRGBColorSpace(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    public override object Clone(
      Document context
      )
    {throw new NotImplementedException();}

    public override int ComponentCount
    {
      get
      {return 3;}
    }

    public override Color DefaultColor
    {
      get
      {return DeviceRGBColor.Default;}
    }

    public override Color GetColor(
      IList<PdfDirectObject> components,
      IContentContext context
      )
    {return new DeviceRGBColor(components);}

    public override drawing::Brush GetPaint(
      Color color
      )
    {
      DeviceRGBColor spaceColor = (DeviceRGBColor)color;
      return new drawing::SolidBrush(
        drawing::Color.FromArgb(
          (int)Math.Round(spaceColor.R * 255),
          (int)Math.Round(spaceColor.G * 255),
          (int)Math.Round(spaceColor.B * 255))
        );
    }
    #endregion
    #endregion
    #endregion
  }
}