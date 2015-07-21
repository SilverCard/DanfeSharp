/*
  Copyright 2010-2012 Stefano Chizzolini. http://www.pdfclown.org

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
using System.Collections.Generic;

namespace org.pdfclown.documents.contents.colorSpaces
{
  /**
    <summary>CIE-based L*a*b* color value [PDF:1.6:4.5.4].</summary>
  */
  [PDF(VersionEnum.PDF11)]
  public sealed class LabColor
    : LeveledColor
  {
    #region dynamic
    #region constructors
    /*
      TODO:colors MUST be instantiated only indirectly by the ColorSpace.getColor method!
      This method MUST be made internal and its color space MUST be passed as argument!
    */
    public LabColor(
      double l,
      double a,
      double b
      ) : this(
        new List<PdfDirectObject>(
          new PdfDirectObject[]
          {
            PdfReal.Get(NormalizeComponent(l)),//TODO:normalize using the actual color space ranges!!!
            PdfReal.Get(NormalizeComponent(a)),
            PdfReal.Get(NormalizeComponent(b))
          }
        )
      )
    {}

    internal LabColor(
      IList<PdfDirectObject> components
      ) : base(
        null, //TODO:colorspace?
        new PdfArray(components)
        )
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the second component (a*).</summary>
    */
    public double A
    {
      get
      {return GetComponentValue(1);}
      set
      {SetComponentValue(1, value);}
    }

    /**
      <summary>Gets/Sets the third component (b*).</summary>
    */
    public double B
    {
      get
      {return GetComponentValue(2);}
      set
      {SetComponentValue(2, value);}
    }

    public override object Clone(
      Document context
      )
    {throw new NotImplementedException();}

    /**
      <summary>Gets/Sets the first component (L*).</summary>
    */
    public double L
    {
      get
      {return GetComponentValue(0);}
      set
      {SetComponentValue(0, value);}
    }
    #endregion
    #endregion
    #endregion
  }
}