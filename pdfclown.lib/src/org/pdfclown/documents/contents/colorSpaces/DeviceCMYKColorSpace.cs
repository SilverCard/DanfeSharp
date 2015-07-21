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
    <summary>Device Cyan-Magenta-Yellow-Key color space [PDF:1.6:4.5.3].</summary>
  */
  [PDF(VersionEnum.PDF11)]
  public sealed class DeviceCMYKColorSpace
    : DeviceColorSpace
  {
    #region static
    #region fields
    /*
      NOTE: It may be specified directly (i.e. without being defined in the ColorSpace subdictionary
      of the contextual resource dictionary) [PDF:1.6:4.5.7].
    */
    public static readonly DeviceCMYKColorSpace Default = new DeviceCMYKColorSpace(PdfName.DeviceCMYK);
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public DeviceCMYKColorSpace(
      Document context
      ) : base(context, PdfName.DeviceCMYK)
    {}

    internal DeviceCMYKColorSpace(
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
      {return 4;}
    }

    public override Color DefaultColor
    {
      get
      {return DeviceCMYKColor.Default;}
    }

    public override Color GetColor(
      IList<PdfDirectObject> components,
      IContentContext context
      )
    {return new DeviceCMYKColor(components);}

    public override drawing::Brush GetPaint(
      Color color
      )
    {
      DeviceCMYKColor spaceColor = (DeviceCMYKColor)color;
      /*
        NOTE: This convertion algorithm was from Apache FOP.
      */
      //FIXME: verify whether this algorithm is effective (limit checking seems quite ugly to me!).
      float keyCorrection = (float)spaceColor.K / 2.5f;
      int r = (int)Math.Round((1 - spaceColor.C + keyCorrection) * 255); if(r > 255){r=255;} else if(r < 0){r=0;}
      int g = (int)Math.Round((1 - spaceColor.M + keyCorrection) * 255); if(g > 255){g=255;} else if(g < 0){g=0;}
      int b = (int)Math.Round((1 - spaceColor.Y + keyCorrection) * 255); if(b > 255){b=255;} else if(b < 0){b=0;}
      return new drawing::SolidBrush(
        drawing::Color.FromArgb(r, g, b)
        );
    }
    #endregion
    #endregion
    #endregion
  }
}