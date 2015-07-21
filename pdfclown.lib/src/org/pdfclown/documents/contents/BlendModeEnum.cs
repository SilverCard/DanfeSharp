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

using org.pdfclown.objects;
using org.pdfclown.util;

using System;

namespace org.pdfclown.documents.contents
{
  /**
    <summary>Blend mode to be used in the transparent imaging model [PDF:1.7:7.2.4].</summary>
  */
  public enum BlendModeEnum
  {
    /**
      <summary>Select the source color, ignoring the backdrop.</summary>
    */
    Normal,
    /**
      <summary>Multiply the backdrop and source color values.</summary>
    */
    Multiply,
    /**
      <summary>Multiply the complements of the backdrop and source color values, then complement the
      result.</summary>
    */
    Screen,
    /**
      <summary>Multiply or screen the colors, depending on the backdrop color value.</summary>
    */
    Overlay,
    /**
      <summary>Select the darker of the backdrop and source colors.</summary>
    */
    Darken,
    /**
      <summary>Select the lighter of the backdrop and source colors.</summary>
    */
    Lighten,
    /**
      <summary>Brighten the backdrop color to reflect the source color.</summary>
    */
    ColorDodge,
    /**
      <summary>Darken the backdrop color to reflect the source color.</summary>
    */
    ColorBurn,
    /**
      <summary>Multiply or screen the colors, depending on the source color value.</summary>
    */
    HardLight,
    /**
      <summary>Darken or lighten the colors, depending on the source color value.</summary>
    */
    SoftLight,
    /**
      <summary>Subtract the darker of the two constituent colors from the lighter color.</summary>
    */
    Difference,
    /**
      <summary>Produce an effect similar to that of the Difference mode but lower in contrast.</summary>
    */
    Exclusion,
    /**
      <summary>Create a color with the hue of the source color and the saturation and luminosity of
      the backdrop color.</summary>
    */
    Hue,
    /**
      <summary>Create a color with the saturation of the source color and the hue and luminosity of
      the backdrop color.</summary>
    */
    Saturation,
    /**
      <summary>Create a color with the hue and saturation of the source color and the luminosity of
      the backdrop color.</summary>
    */
    Color,
    /**
      <summary>Create a color with the luminosity of the source color and the hue and saturation of
      the backdrop color.</summary>
    */
    Luminosity
  }

  internal static class BlendModeEnumExtension
  {
    private static readonly BiDictionary<BlendModeEnum?,PdfName> codes;

    static BlendModeEnumExtension()
    {
      codes = new BiDictionary<BlendModeEnum?,PdfName>();
      codes[BlendModeEnum.Normal] = PdfName.Normal;
      codes[BlendModeEnum.Multiply] = PdfName.Multiply;
      codes[BlendModeEnum.Screen] = PdfName.Screen;
      codes[BlendModeEnum.Overlay] = PdfName.Overlay;
      codes[BlendModeEnum.Darken] = PdfName.Darken;
      codes[BlendModeEnum.Lighten] = PdfName.Lighten;
      codes[BlendModeEnum.ColorDodge] = PdfName.ColorDodge;
      codes[BlendModeEnum.ColorBurn] = PdfName.ColorBurn;
      codes[BlendModeEnum.HardLight] = PdfName.HardLight;
      codes[BlendModeEnum.SoftLight] = PdfName.SoftLight;
      codes[BlendModeEnum.Difference] = PdfName.Difference;
      codes[BlendModeEnum.Exclusion] = PdfName.Exclusion;
      codes[BlendModeEnum.Hue] = PdfName.Hue;
      codes[BlendModeEnum.Saturation] = PdfName.Saturation;
      codes[BlendModeEnum.Color] = PdfName.Color;
      codes[BlendModeEnum.Luminosity] = PdfName.Luminosity;
    }

    public static BlendModeEnum? Get(
      PdfName name
      )
    {return codes.GetKey(name);}

    public static PdfName GetName(
      this BlendModeEnum blendMode
      )
    {return codes[blendMode];}
  }
}