/*
  Copyright 2006-2012 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.files;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;
using System.Drawing;

namespace org.pdfclown.documents.contents.colorSpaces
{
  /**
    <summary>Color space [PDF:1.6:4.5].</summary>
  */
  public abstract class ColorSpace
    : PdfObjectWrapper<PdfDirectObject>
  {
    #region static
    #region interface
    #region public
    /**
      <summary>Wraps the specified color space base object into a color space object.</summary>
      <param name="baseObject">Base object of a color space object.</param>
      <returns>Color space object corresponding to the base object.</returns>
    */
    public static ColorSpace Wrap(
      PdfDirectObject baseObject
      )
    {
      if(baseObject == null)
        return null;

      // Get the data object corresponding to the color space!
      PdfDataObject baseDataObject = baseObject.Resolve();
      /*
        NOTE: A color space is defined by an array object whose first element
        is a name object identifying the color space family [PDF:1.6:4.5.2].
        For families that do not require parameters, the color space CAN be
        specified simply by the family name itself instead of an array.
      */
      PdfName name = (PdfName)(baseDataObject is PdfArray
        ? ((PdfArray)baseDataObject)[0]
        : baseDataObject);
      if(name.Equals(PdfName.DeviceRGB))
        return new DeviceRGBColorSpace(baseObject);
      else if(name.Equals(PdfName.DeviceCMYK))
        return new DeviceCMYKColorSpace(baseObject);
      else if(name.Equals(PdfName.DeviceGray))
        return new DeviceGrayColorSpace(baseObject);
      else if(name.Equals(PdfName.CalRGB))
        return new CalRGBColorSpace(baseObject);
      else if(name.Equals(PdfName.CalGray))
        return new CalGrayColorSpace(baseObject);
      else if(name.Equals(PdfName.ICCBased))
        return new ICCBasedColorSpace(baseObject);
      else if(name.Equals(PdfName.Lab))
        return new LabColorSpace(baseObject);
      else if(name.Equals(PdfName.DeviceN))
        return new DeviceNColorSpace(baseObject);
      else if(name.Equals(PdfName.Indexed))
        return new IndexedColorSpace(baseObject);
      else if(name.Equals(PdfName.Pattern))
        return new PatternColorSpace(baseObject);
      else if(name.Equals(PdfName.Separation))
        return new SeparationColorSpace(baseObject);
      else
        throw new NotSupportedException("Color space " + name + " unknown.");
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    protected ColorSpace(
      Document context,
      PdfDirectObject baseDataObject
      ) : base(context, baseDataObject)
    {}

    protected ColorSpace(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the number of components used to represent a color value.</summary>
    */
    public abstract int ComponentCount
    {
      get;
    }

    /**
      <summary>Gets the initial color value within this color space.</summary>
    */
    public abstract Color DefaultColor
    {
      get;
    }

    /**
      <summary>Gets the rendering representation of the specified color value.</summary>
      <param name="color">Color value to convert into an equivalent rendering representation.</param>
    */
    public abstract Brush GetPaint(
      Color color
      );

    /**
      <summary>Gets the color value corresponding to the specified components
      interpreted according to this color space [PDF:1.6:4.5.1].</summary>
      <param name="components">Color components.</param>
      <param name="context">Content context.</param>
    */
    public abstract Color GetColor(
      IList<PdfDirectObject> components,
      IContentContext context
      );
    #endregion
    #endregion
    #endregion
  }
}