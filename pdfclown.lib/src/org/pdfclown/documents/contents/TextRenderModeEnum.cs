/*
  Copyright 2006-2010 Stefano Chizzolini. http://www.pdfclown.org

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

namespace org.pdfclown.documents.contents
{
  /**
    <summary>Glyph-outlines appearance on text showing [PDF:1.6:5.2.5].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public enum TextRenderModeEnum
  {
    /**
      <summary>Fill text glyphs.</summary>
    */
    Fill = 0,
    /**
      <summary>Stroke text glyphs.</summary>
    */
    Stroke = 1,
    /**
      <summary>Fill, then stroke text glyphs.</summary>
    */
    FillStroke = 2,
    /**
      <summary>Do nothing (invisible text glyphs).</summary>
    */
    Invisible = 3,
    /**
      <summary>Fill text glyphs, then apply to path for clipping.</summary>
    */
    FillClip = 4,
    /**
      <summary>Stroke text glyphs, then apply to path for clipping.</summary>
    */
    StrokeClip = 5,
    /**
      <summary>Fill, then stroke text glyphs, then apply to path for clipping.</summary>
    */
    FillStrokeClip = 6,
    /**
      <summary>Apply text glyphs to path for clipping.</summary>
    */
    Clip = 7
  };
}