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

using org.pdfclown.documents;
using org.pdfclown.documents.functions;
using org.pdfclown.objects;
using org.pdfclown.util;

using System;
using System.Collections.Generic;
using drawing = System.Drawing;

namespace org.pdfclown.documents.contents.colorSpaces
{
  /**
    <summary>Special device color space [PDF:1.6:4.5.5].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public abstract class SpecialDeviceColorSpace
    : SpecialColorSpace
  {
    #region static
    #region fields
    /**
      <summary>Special colorant name never producing any visible output.</summary>
      <remarks>When a color space with this component name is the current color space, painting
      operators have no effect.</remarks>
    */
    public static readonly string NoneComponentName = (string)PdfName.None.Value;
    #endregion
    #endregion

    #region dynamic
    #region constructors
    //TODO:IMPL new element constructor!

    internal SpecialDeviceColorSpace(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the alternate color space used in case any of the <see cref="ComponentNames">component names</see>
      in the color space do not correspond to a component available on the device.</summary>
    */
    public ColorSpace AlternateSpace
    {
      get
      {return ColorSpace.Wrap(((PdfArray)BaseDataObject)[2]);}
    }

    /**
      <summary>Gets the names of the color components.</summary>
    */
    public abstract IList<string> ComponentNames
    {
      get;
    }

    public override drawing::Brush GetPaint(
      Color color
      )
    {
      //TODO:enable!!!
  //    IList<PdfDirectObject> alternateColorComponents = TintFunction.Calculate(color.Components);
  //    ColorSpace alternateSpace = AlternateSpace;
  //    return alternateSpace.GetPaint(
  //      alternateSpace.GetColor(
  //        alternateColorComponents,
  //        null
  //        )
  //      );

      //TODO: remove (temporary hack)!
      return new drawing::SolidBrush(drawing::Color.Black);
    }

    /**
      <summary>Gets the function to transform a tint value into color component values
      in the <see cref="AlternateSpace">alternate color space</see>.</summary>
    */
    public Function TintFunction
    {
      get
      {return Function.Wrap(((PdfArray)BaseDataObject)[3]);}
    }
    #endregion
    #endregion
    #endregion
  }
}