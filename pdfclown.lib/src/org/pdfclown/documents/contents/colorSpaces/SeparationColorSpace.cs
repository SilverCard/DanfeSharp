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
using org.pdfclown.objects;
using org.pdfclown.util;

using System;
using System.Collections.Generic;
using drawing = System.Drawing;

namespace org.pdfclown.documents.contents.colorSpaces
{
  /**
    <summary>Special color space that provides a means for specifying the use of additional colorants
    or for isolating the control of individual color components of a device color space for
    a subtractive device [PDF:1.6:4.5.5].</summary>
    <remarks>When such a space is the current color space, the current color is a single-component value,
    called a tint, that controls the application of the given colorant or color components only.</remarks>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class SeparationColorSpace
    : SpecialDeviceColorSpace
  {
    #region static
    #region fields
    /**
      <summary>Special colorant name referring collectively to all components available on an output
      device, including those for the standard process components.</summary>
      <remarks>When a separation space with this component name is the current color space, painting
      operators apply tint values to all available components at once.</remarks>
    */
    public static readonly string AllComponentName = (string)PdfName.All.Value;
    #endregion
    #endregion

    #region dynamic
    #region constructors
    //TODO:IMPL new element constructor!

    internal SeparationColorSpace(
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
      {return 1;}
    }

    /**
      <summary>Gets the name of the colorant that this separation color space is intended
      to represent.</summary>
      <remarks>Special names:
        <list type="bullet">
          <item><see cref="AllComponentName">All</see></item>
          <item><see cref="NoneComponentName">None</see></item>
        </list>
      </remarks>
    */
    public override IList<string> ComponentNames
    {
      get
      {return new List<string>(new string[]{(string)((PdfName)((PdfArray)BaseDataObject)[1]).Value});}
    }

    public override Color DefaultColor
    {
      get
      {return SeparationColor.Default;}
    }

    public override Color GetColor(
      IList<PdfDirectObject> components,
      IContentContext context
      )
    {return new SeparationColor(components);}
    #endregion
    #endregion
    #endregion
  }
}