/*
  Copyright 2008-2012 Stefano Chizzolini. http://www.pdfclown.org

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

using org.pdfclown.bytes;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.colorSpaces;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;
using System.Drawing;

namespace org.pdfclown.documents.interaction.annotations
{
  /**
    <summary>Line annotation [PDF:1.6:8.4.5].</summary>
    <remarks>It displays displays a single straight line on the page.
    When opened, it displays a pop-up window containing the text of the associated note.</remarks>
  */
  [PDF(VersionEnum.PDF13)]
  public sealed class Line
    : Annotation
  {
    #region types
    /**
      <summary>Line ending style [PDF:1.6:8.4.5].</summary>
    */
    public enum LineEndStyleEnum
    {
      /**
        Square.
      */
      Square,
      /**
        Circle.
      */
      Circle,
      /**
        Diamond.
      */
      Diamond,
      /**
        Open arrow.
      */
      OpenArrow,
      /**
        Closed arrow.
      */
      ClosedArrow,
      /**
        None.
      */
      None,
      /**
        Butt.
      */
      Butt,
      /**
        Reverse open arrow.
      */
      ReverseOpenArrow,
      /**
        Reverse closed arrow.
      */
      ReverseClosedArrow,
      /**
        Slash.
      */
      Slash
    };
    #endregion

    #region static
    #region fields
    private static readonly double DefaultLeaderLineExtensionLength = 0;
    private static readonly double DefaultLeaderLineLength = 0;
    private static readonly LineEndStyleEnum DefaultLineEndStyle = LineEndStyleEnum.None;

    private static readonly Dictionary<LineEndStyleEnum,PdfName> LineEndStyleEnumCodes;
    #endregion

    #region constructors
    static Line()
    {
      LineEndStyleEnumCodes = new Dictionary<LineEndStyleEnum,PdfName>();
      LineEndStyleEnumCodes[LineEndStyleEnum.Square] = PdfName.Square;
      LineEndStyleEnumCodes[LineEndStyleEnum.Circle] = PdfName.Circle;
      LineEndStyleEnumCodes[LineEndStyleEnum.Diamond] = PdfName.Diamond;
      LineEndStyleEnumCodes[LineEndStyleEnum.OpenArrow] = PdfName.OpenArrow;
      LineEndStyleEnumCodes[LineEndStyleEnum.ClosedArrow] = PdfName.ClosedArrow;
      LineEndStyleEnumCodes[LineEndStyleEnum.None] = PdfName.None;
      LineEndStyleEnumCodes[LineEndStyleEnum.Butt] = PdfName.Butt;
      LineEndStyleEnumCodes[LineEndStyleEnum.ReverseOpenArrow] = PdfName.ROpenArrow;
      LineEndStyleEnumCodes[LineEndStyleEnum.ReverseClosedArrow] = PdfName.RClosedArrow;
      LineEndStyleEnumCodes[LineEndStyleEnum.Slash] = PdfName.Slash;
    }
    #endregion

    #region interface
    #region private
    /**
      <summary>Gets the code corresponding to the given value.</summary>
    */
    private static PdfName ToCode(
      LineEndStyleEnum value
      )
    {return LineEndStyleEnumCodes[value];}

    /**
      <summary>Gets the line ending style corresponding to the given value.</summary>
    */
    private static LineEndStyleEnum ToLineEndStyleEnum(
      PdfName value
      )
    {
      foreach(KeyValuePair<LineEndStyleEnum,PdfName> style in LineEndStyleEnumCodes)
      {
        if(style.Value.Equals(value))
          return style.Key;
      }
      return DefaultLineEndStyle;
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public Line(
      Page page,
      PointF startPoint,
      PointF endPoint,
      string text
      ) : base(
        page,
        PdfName.Line,
        new RectangleF(
          startPoint.X,
          startPoint.Y,
          endPoint.X-startPoint.X,
          endPoint.Y-startPoint.Y
          ),
        text
        )
    {
      BaseDataObject[PdfName.L] = new PdfArray(new PdfDirectObject[]{PdfReal.Get(0),PdfReal.Get(0),PdfReal.Get(0),PdfReal.Get(0)});
      StartPoint = startPoint;
      EndPoint = endPoint;
    }

    internal Line(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets whether the contents should be shown as a caption.</summary>
    */
    public bool CaptionVisible
    {
      get
      {
        PdfBoolean captionVisibleObject = (PdfBoolean)BaseDataObject[PdfName.Cap];
        return captionVisibleObject != null
          ? captionVisibleObject.BooleanValue
          : false;
      }
      set
      {BaseDataObject[PdfName.Cap] = PdfBoolean.Get(value);}
    }

    /**
      <summary>Gets/Sets the ending coordinates.</summary>
    */
    public PointF EndPoint
    {
      get
      {
        PdfArray coordinatesObject = (PdfArray)BaseDataObject[PdfName.L];
        return new PointF(
          (float)((IPdfNumber)coordinatesObject[2]).RawValue,
          (float)((IPdfNumber)coordinatesObject[3]).RawValue
          );
      }
      set
      {
        PdfArray coordinatesObject = (PdfArray)BaseDataObject[PdfName.L];
        coordinatesObject[2] = PdfReal.Get(value.X);
        coordinatesObject[3] = PdfReal.Get(Page.Box.Height-value.Y);
      }
    }

    /**
      <summary>Gets/Sets the style of the ending line ending.</summary>
    */
    public LineEndStyleEnum EndStyle
    {
      get
      {
        PdfArray endstylesObject = (PdfArray)BaseDataObject[PdfName.LE];
        return endstylesObject != null
          ? ToLineEndStyleEnum((PdfName)endstylesObject[1])
          : DefaultLineEndStyle;
      }
      set
      {EnsureLineEndStylesObject()[1] = ToCode(value);}
    }

    /**
      <summary>Gets/Sets the color with which to fill the interior of the annotation's line endings.</summary>
    */
    public DeviceRGBColor FillColor
    {
      get
      {
        PdfArray fillColorObject = (PdfArray)BaseDataObject[PdfName.IC];
        if(fillColorObject == null)
          return null;
//TODO:use baseObject constructor!!!
        return new DeviceRGBColor(
          ((IPdfNumber)fillColorObject[0]).RawValue,
          ((IPdfNumber)fillColorObject[1]).RawValue,
          ((IPdfNumber)fillColorObject[2]).RawValue
          );
      }
      set
      {BaseDataObject[PdfName.IC] = (PdfDirectObject)value.BaseDataObject;}
    }

    /**
      <summary>Gets/Sets the length of leader line extensions that extend
      in the opposite direction from the leader lines.</summary>
    */
    public double LeaderLineExtensionLength
    {
      get
      {
        IPdfNumber leaderLineExtensionLengthObject = (IPdfNumber)BaseDataObject[PdfName.LLE];
        return leaderLineExtensionLengthObject != null
          ? leaderLineExtensionLengthObject.RawValue
          : DefaultLeaderLineExtensionLength;
      }
      set
      {
        BaseDataObject[PdfName.LLE] = PdfReal.Get(value);
        /*
          NOTE: If leader line extension entry is present, leader line MUST be too.
        */
        if(!BaseDataObject.ContainsKey(PdfName.LL))
        {LeaderLineLength = DefaultLeaderLineLength;}
      }
    }

    /**
      <summary>Gets/Sets the length of leader lines that extend from each endpoint
      of the line perpendicular to the line itself.</summary>
      <remarks>A positive value means that the leader lines appear in the direction
      that is clockwise when traversing the line from its starting point
      to its ending point; a negative value indicates the opposite direction.</remarks>
    */
    public double LeaderLineLength
    {
      get
      {
        IPdfNumber leaderLineLengthObject = (IPdfNumber)BaseDataObject[PdfName.LL];
        return leaderLineLengthObject != null
          ? -leaderLineLengthObject.RawValue
          : DefaultLeaderLineLength;
      }
      set
      {BaseDataObject[PdfName.LL] = PdfReal.Get(-value);}
    }

    /**
      <summary>Gets/Sets the starting coordinates.</summary>
    */
    public PointF StartPoint
    {
      get
      {
        PdfArray coordinatesObject = (PdfArray)BaseDataObject[PdfName.L];
        return new PointF(
          (float)((IPdfNumber)coordinatesObject[0]).RawValue,
          (float)((IPdfNumber)coordinatesObject[1]).RawValue
          );
      }
      set
      {
        PdfArray coordinatesObject = (PdfArray)BaseDataObject[PdfName.L];
        coordinatesObject[0] = PdfReal.Get(value.X);
        coordinatesObject[1] = PdfReal.Get(Page.Box.Height-value.Y);
      }
    }

    /**
      <summary>Gets/Sets the style of the starting line ending.</summary>
    */
    public LineEndStyleEnum StartStyle
    {
      get
      {
        PdfArray endstylesObject = (PdfArray)BaseDataObject[PdfName.LE];
        return endstylesObject != null
          ? ToLineEndStyleEnum((PdfName)endstylesObject[0])
          : DefaultLineEndStyle;
      }
      set
      {EnsureLineEndStylesObject()[0] = ToCode(value);}
    }
    #endregion

    #region private
    private PdfArray EnsureLineEndStylesObject(
      )
    {
      PdfArray endStylesObject = (PdfArray)BaseDataObject[PdfName.LE];
      if(endStylesObject == null)
      {
        BaseDataObject[PdfName.LE] = endStylesObject = new PdfArray(
          new PdfDirectObject[]
          {
            ToCode(DefaultLineEndStyle),
            ToCode(DefaultLineEndStyle)
          }
          );
      }
      return endStylesObject;
    }
    #endregion
    #endregion
    #endregion
  }
}