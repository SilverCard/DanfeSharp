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

using org.pdfclown.documents.contents.colorSpaces;
using org.pdfclown.documents.contents.fonts;
using org.pdfclown.objects;

namespace org.pdfclown.documents.contents
{
  /**
    <summary>Text style.</summary>
  */
  public sealed class TextStyle
  {
    #region dynamic
    #region fields
    private readonly Color fillColor;
    private readonly ColorSpace fillColorSpace;
    private readonly Font font;
    private readonly double fontSize;
    private readonly TextRenderModeEnum renderMode;
    private readonly Color strokeColor;
    private readonly ColorSpace strokeColorSpace;
    #endregion

    #region constructors
    public TextStyle(
      Font font,
      double fontSize,
      TextRenderModeEnum renderMode,
      Color strokeColor,
      ColorSpace strokeColorSpace,
      Color fillColor,
      ColorSpace fillColorSpace
      )
    {
      this.font = font;
      this.fontSize = fontSize;
      this.renderMode = renderMode;
      this.strokeColor = strokeColor;
      this.strokeColorSpace = strokeColorSpace;
      this.fillColor = fillColor;
      this.fillColorSpace = fillColorSpace;
    }
    #endregion

    #region interface
    #region public
    public Color FillColor
    {get{return fillColor;}}

    public ColorSpace FillColorSpace
    {get{return fillColorSpace;}}

    public Font Font
    {get{return font;}}

    public double FontSize
    {get{return fontSize;}}

    public TextRenderModeEnum RenderMode
    {get{return renderMode;}}

    public Color StrokeColor
    {get{return strokeColor;}}

    public ColorSpace StrokeColorSpace
    {get{return strokeColorSpace;}}
    #endregion
    #endregion
    #endregion
  }
}