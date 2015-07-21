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
    <summary>Pattern color space [PDF:1.6:4.5.5].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class PatternColorSpace
    : SpecialColorSpace
  {
    #region static
    #region fields
    /*
      NOTE: In case of no parameters, it may be specified directly (i.e. without being defined
      in the ColorSpace subdictionary of the contextual resource dictionary) [PDF:1.6:4.5.7].
    */
    //TODO:verify parameters!!!
    public static readonly PatternColorSpace Default = new PatternColorSpace(null);
    #endregion
    #endregion

    #region dynamic
    #region constructors
    //TODO:IMPL new element constructor!

    internal PatternColorSpace(
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
      {return 0;}
    }

    public override Color DefaultColor
    {
      get
      {return Pattern.Default;}
    }

    public override Color GetColor(
      IList<PdfDirectObject> components,
      IContentContext context
      )
    {
//TODO
/*
      Pattern pattern = context.Resources.Patterns[components[components.Count-1]];
      if(pattern is TilingPattern)
      {
        TilingPattern tilingPattern = (TilingPattern)pattern;
        if(tilingPattern.PaintType == PaintTypeEnum.Uncolored)
        {
          ColorSpace underlyingColorSpace = UnderlyingColorSpace;
          if(underlyingColorSpace == null)
            throw new IllegalArgumentException("Uncolored tiling patterns not supported by this color space because no underlying color space has been defined.");

          // Get the color to be used for colorizing the uncolored tiling pattern!
          Color color = underlyingColorSpace.GetColor(components, context);
          // Colorize the uncolored tiling pattern!
          pattern = tilingPattern.Colorize(color);
        }
      }
      return pattern;
*/
return null;
    }

    public override drawing::Brush GetPaint(
      Color color
      )
    {
      // FIXME: Auto-generated method stub
      return null;
    }

    /**
      <summary>Gets the color space in which the actual color of the <see cref="Pattern">pattern</see> is to be specified.</summary>
      <remarks>This feature is applicable to <see cref="TilingPattern">uncolored tiling patterns</see> only.</remarks>
    */
    public ColorSpace UnderlyingColorSpace
    {
      get
      {
        PdfDirectObject baseDataObject = BaseDataObject;
        if(baseDataObject is PdfArray)
        {
          PdfArray baseArrayObject = (PdfArray)baseDataObject;
          if(baseArrayObject.Count > 1)
            return ColorSpace.Wrap(baseArrayObject[1]);
        }
        return null;
      }
    }
    #endregion
    #endregion
    #endregion
  }
}