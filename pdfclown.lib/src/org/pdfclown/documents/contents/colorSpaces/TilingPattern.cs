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
using System.Collections.Generic;

namespace org.pdfclown.documents.contents.colorSpaces
{
  /**
    <summary>Pattern consisting of a small graphical figure called <i>pattern cell</i> [PDF:1.6:4.6.2].</summary>
    <remarks>Painting with the pattern replicates the cell at fixed horizontal and vertical intervals
    to fill an area.</remarks>
  */
//TODO: define as IContentContext?
  [PDF(VersionEnum.PDF12)]
  public class TilingPattern
    : Pattern
  {
    #region types
    /**
      <summary>Uncolored tiling pattern ("stencil") associated to a color.</summary>
    */
    public sealed class Colorized
      : TilingPattern
    {
      private Color color;

      internal Colorized(
        TilingPattern uncoloredPattern,
        Color color
        ) : base(
          (PatternColorSpace)uncoloredPattern.ColorSpace,
          uncoloredPattern.BaseObject
          )
      {this.color = color;}

      /**
        <summary>Gets the color applied to the stencil.</summary>
      */
      public Color Color
      {
        get
        {return color;}
      }
    }

    /**
      <summary>Pattern cell color mode.</summary>
    */
    public enum PaintTypeEnum
    {
      /**
        <summary>The pattern's content stream specifies the colors used to paint the pattern cell.</summary>
        <remarks>When the content stream begins execution, the current color is the one
        that was initially in effect in the pattern's parent content stream.</remarks>
      */
      Colored = 1,
      /**
        <summary>The pattern's content stream does NOT specify any color information.</summary>
        <remarks>
          <para>Instead, the entire pattern cell is painted with a separately specified color
          each time the pattern is used; essentially, the content stream describes a stencil
          through which the current color is to be poured.</para>
          <para>The content stream must not invoke operators that specify colors
          or other color-related parameters in the graphics state.</para>
        </remarks>
      */
      Uncolored = 2
    }

    /**
      Spacing adjustment of tiles relative to the device pixel grid.
    */
    public enum TilingTypeEnum
    {
      /**
        <summary>Pattern cells are spaced consistently, that is by a multiple of a device pixel.</summary>
      */
      ConstantSpacing = 1,
      /**
        <summary>The pattern cell is not distorted, but the spacing between pattern cells
        may vary by as much as 1 device pixel, both horizontally and vertically,
        when the pattern is painted.</summary>
      */
      VariableSpacing = 2,
      /**
        <summary>Pattern cells are spaced consistently as in tiling type 1
        but with additional distortion permitted to enable a more efficient implementation.</summary>
      */
      FasterConstantSpacing = 3
    }
    #endregion

    #region dynamic
    #region constructors
    internal TilingPattern(
      PatternColorSpace colorSpace,
      PdfDirectObject baseObject
      ) : base(colorSpace, baseObject)
    {}

    internal TilingPattern(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the colorized representation of this pattern.</summary>
      <param name="color">Color to be applied to the pattern.</param>
    */
    public Colorized Colorize(
      Color color
      )
    {
      if(PaintType != PaintTypeEnum.Uncolored)
        throw new NotSupportedException("Only uncolored tiling patterns can be colorized.");

      return new Colorized(this, color);
    }

    /**
      <summary>Gets the pattern cell's bounding box (expressed in the pattern coordinate system)
      used to clip the pattern cell.</summary>
    */
    public RectangleF Box
    {
      get
      {
        /*
          NOTE: 'BBox' entry MUST be defined.
        */
        org.pdfclown.objects.Rectangle box = org.pdfclown.objects.Rectangle.Wrap(BaseHeader[PdfName.BBox]);
        return new RectangleF((float)box.X, (float)box.Y, (float)box.Width, (float)box.Height);
      }
    }

    /**
      <summary>Gets how the color of the pattern cell is to be specified.</summary>
    */
    public PaintTypeEnum PaintType
    {
      get
      {return (PaintTypeEnum)((PdfInteger)BaseHeader[PdfName.PaintType]).RawValue;}
    }

    /**
      <summary>Gets the named resources required by the pattern's content stream.</summary>
    */
    public Resources Resources
    {
      get
      {return Resources.Wrap(BaseHeader[PdfName.Resources]);}
    }

    /**
      <summary>Gets how to adjust the spacing of tiles relative to the device pixel grid.</summary>
    */
    public TilingTypeEnum TilingType
    {
      get
      {return (TilingTypeEnum)((PdfInteger)BaseHeader[PdfName.TilingType]).RawValue;}
    }

    /**
      <summary>Gets the horizontal spacing between pattern cells (expressed in the pattern coordinate system).</summary>
    */
    public double XStep
    {
      get
      {return ((IPdfNumber)BaseHeader[PdfName.XStep]).RawValue;}
    }

    /**
      <summary>Gets the vertical spacing between pattern cells (expressed in the pattern coordinate system).</summary>
    */
    public double YStep
    {
      get
      {return ((IPdfNumber)BaseHeader[PdfName.YStep]).RawValue;}
    }
    #endregion

    #region private
    private PdfDictionary BaseHeader
    {
      get
      {return ((PdfStream)BaseDataObject).Header;}
    }
    #endregion
    #endregion
    #endregion
  }
}