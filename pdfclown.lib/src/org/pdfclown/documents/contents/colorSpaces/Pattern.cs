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

using org.pdfclown.files;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;

namespace org.pdfclown.documents.contents.colorSpaces
{
  /**
    <summary>Paint that consists of a repeating graphical figure or a smoothly varying color gradient
    instead of a simple color [PDF:1.6:4.6].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public abstract class Pattern
    : Color
  {
    #region static
    #region fields
    //TODO:verify!
    public static readonly Pattern Default = new TilingPattern(null);

    private const int PatternType1 = 1;
    private const int PatternType2 = 2;
    #endregion

    #region interface
    #region public
    /**
      <summary>Wraps the specified base object into a pattern object.</summary>
      <param name="baseObject">Base object of a pattern object.</param>
      <returns>Pattern object corresponding to the base object.</returns>
    */
    public static Pattern Wrap(
      PdfDirectObject baseObject
      )
    {
      if(baseObject == null)
        return null;

      PdfDataObject dataObject = baseObject.Resolve();
      PdfDictionary dictionary = GetDictionary(dataObject);
      int patternType = ((PdfInteger)dictionary[PdfName.PatternType]).RawValue;
      switch(patternType)
      {
        case PatternType1:
          return new TilingPattern(baseObject);
        case PatternType2:
          return new ShadingPattern(baseObject);
        default:
          throw new NotSupportedException("Pattern type " + patternType + " unknown.");
      }
    }
    #endregion

    #region private
    /**
      <summary>Gets a pattern's dictionary.</summary>
      <param name="patternDataObject">Pattern data object.</param>
    */
    private static PdfDictionary GetDictionary(
      PdfDataObject patternDataObject
      )
    {
      if(patternDataObject is PdfDictionary)
        return (PdfDictionary)patternDataObject;
      else // MUST be PdfStream.
        return ((PdfStream)patternDataObject).Header;
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    //TODO:verify (colorspace is available or may be implicit?)
    protected Pattern(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}

    //TODO:verify (colorspace is available or may be implicit?)
    protected Pattern(
      PatternColorSpace colorSpace,
      PdfDirectObject baseObject
      ) : base(colorSpace, baseObject)
    {}
    #endregion

    #region interface
    #region public
    public override object Clone(
      Document context
      )
    {throw new NotImplementedException();}

    public override IList<PdfDirectObject> Components
    {
      get
      {return new List<PdfDirectObject>();}//TODO:verify (see SetFillColor/SetStrokeColor -- name!)!
    }

    /**
      <summary>Gets the pattern matrix, a transformation matrix that maps the pattern's
      internal coordinate system to the default coordinate system of the pattern's
      parent content stream (the content stream in which the pattern is defined as a resource).</summary>
      <remarks>The concatenation of the pattern matrix with that of the parent content stream establishes
      the pattern coordinate space, within which all graphics objects in the pattern are interpreted.</remarks>
    */
    public double[] Matrix
    {
      get
      {
        /*
          NOTE: Pattern-space-to-user-space matrix is identity [1 0 0 1 0 0] by default.
        */
        PdfArray matrix = (PdfArray)Dictionary[PdfName.Matrix];
        if(matrix == null)
          return new double[]
            {
              1, // a.
              0, // b.
              0, // c.
              1, // d.
              0, // e.
              0 // f.
            };
        else
          return new double[]
            {
              ((IPdfNumber)matrix[0]).RawValue, // a.
              ((IPdfNumber)matrix[1]).RawValue, // b.
              ((IPdfNumber)matrix[2]).RawValue, // c.
              ((IPdfNumber)matrix[3]).RawValue, // d.
              ((IPdfNumber)matrix[4]).RawValue, // e.
              ((IPdfNumber)matrix[5]).RawValue // f.
            };
      }
    }
    #endregion

    #region protected
    /**
      <summary>Gets this pattern's dictionary.</summary>
    */
    protected PdfDictionary Dictionary
    {
      get
      {return GetDictionary(BaseDataObject);}
    }
    #endregion
    #endregion
    #endregion
  }
}