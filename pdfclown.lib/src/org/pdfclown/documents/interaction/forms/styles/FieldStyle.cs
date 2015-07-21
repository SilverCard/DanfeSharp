/*
  Copyright 2008-2011 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.documents.contents.colorSpaces;
using org.pdfclown.objects;

namespace org.pdfclown.documents.interaction.forms.styles
{
  /**
    <summary>Abstract field appearance style.</summary>
    <remarks>It automates the definition of field appearance, applying a common look.</remarks>
  */
  public abstract class FieldStyle
  {
    #region dynamic
    #region fields
    private Color backColor = DeviceRGBColor.White;
    private char checkSymbol = (char)52;
    private double fontSize = 10;
    private Color foreColor = DeviceRGBColor.Black;
    private bool graphicsVisibile = false;
    private char radioSymbol = (char)108;
    #endregion

    #region constructors
    protected FieldStyle(
      )
    {}
    #endregion

    #region interface
    #region public
    public abstract void Apply(
      Field field
      );

    public Color BackColor
    {
      get
      {return backColor;}
      set
      {backColor = value;}
    }

    public char CheckSymbol
    {
      get
      {return checkSymbol;}
      set
      {checkSymbol = value;}
    }

    public double FontSize
    {
      get
      {return fontSize;}
      set
      {fontSize = value;}
    }

    public Color ForeColor
    {
      get
      {return foreColor;}
      set
      {foreColor = value;}
    }

    public bool GraphicsVisibile
    {
      get
      {return graphicsVisibile;}
      set
      {graphicsVisibile = value;}
    }

    public char RadioSymbol
    {
      get
      {return radioSymbol;}
      set
      {radioSymbol = value;}
    }
    #endregion
    #endregion
    #endregion
  }
}