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
using org.pdfclown.documents.contents;
using org.pdfclown.util;

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace org.pdfclown.documents.contents.objects
{
  /**
    <summary>Path-painting operation [PDF:1.6:4.4.2].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class PaintPath
    : Operation
  {
    #region static
    #region fields
    public const string CloseFillStrokeEvenOddOperatorKeyword = "b*";
    public const string CloseFillStrokeOperatorKeyword = "b";
    public const string CloseStrokeOperatorKeyword = "s";
    public const string EndPathNoOpOperatorKeyword = "n";
    public const string FillEvenOddOperatorKeyword = "f*";
    public const string FillObsoleteOperatorKeyword = "F";
    public const string FillOperatorKeyword = "f";
    public const string FillStrokeEvenOddOperatorKeyword = "B*";
    public const string FillStrokeOperatorKeyword = "B";
    public const string StrokeOperatorKeyword = "S";
  
    /**
      <summary>'Close, fill, and then stroke the path, using the nonzero winding number rule to determine
      the region to fill' operation.</summary>
    */
    public static readonly PaintPath CloseFillStroke = new PaintPath(CloseFillStrokeOperatorKeyword, true, true, true, WindModeEnum.NonZero);
    /**
      <summary>'Close, fill, and then stroke the path, using the even-odd rule to determine the region
      to fill' operation.</summary>
    */
    public static readonly PaintPath CloseFillStrokeEvenOdd = new PaintPath(CloseFillStrokeEvenOddOperatorKeyword, true, true, true, WindModeEnum.EvenOdd);
    /**
      <summary>'Close and stroke the path' operation.</summary>
    */
    public static readonly PaintPath CloseStroke = new PaintPath(CloseStrokeOperatorKeyword, true, true, false, null);
    /**
      <summary>'End the path object without filling or stroking it' operation.</summary>
    */
    public static readonly PaintPath EndPathNoOp = new PaintPath(EndPathNoOpOperatorKeyword, false, false, false, null);
    /**
      <summary>'Fill the path, using the nonzero winding number rule to determine the region to fill' operation.</summary>
    */
    public static readonly PaintPath Fill = new PaintPath(FillOperatorKeyword, false, false, true, WindModeEnum.NonZero);
    /**
      <summary>'Fill the path, using the even-odd rule to determine the region to fill' operation.</summary>
    */
    public static readonly PaintPath FillEvenOdd = new PaintPath(FillEvenOddOperatorKeyword, false, false, true, WindModeEnum.EvenOdd);
    /**
      <summary>'Fill and then stroke the path, using the nonzero winding number rule to determine the region to
      fill' operation.</summary>
    */
    public static readonly PaintPath FillStroke = new PaintPath(FillStrokeOperatorKeyword, false, true, true, WindModeEnum.NonZero);
    /**
      <summary>'Fill and then stroke the path, using the even-odd rule to determine the region to fill' operation.</summary>
    */
    public static readonly PaintPath FillStrokeEvenOdd = new PaintPath(FillStrokeEvenOddOperatorKeyword, false, true, true, WindModeEnum.EvenOdd);
    /**
      <summary>'Stroke the path' operation.</summary>
    */
    public static readonly PaintPath Stroke = new PaintPath(StrokeOperatorKeyword, false, true, false, null);
    #endregion

    #region interface
    #region private
    private static Pen GetStroke(
      ContentScanner.GraphicsState state
      )
    {
      Pen stroke = new Pen(
        state.StrokeColorSpace.GetPaint(state.StrokeColor),
        (float)state.LineWidth
        );
      {
        LineCap lineCap = state.LineCap.ToGdi();
        stroke.SetLineCap(lineCap, lineCap, lineCap.ToDashCap());
        stroke.LineJoin = state.LineJoin.ToGdi();
        stroke.MiterLimit = (float)state.MiterLimit;

        LineDash lineDash = state.LineDash;
        double[] dashArray = lineDash.DashArray;
        if(dashArray != null && dashArray.Length > 0)
        {
          stroke.DashPattern = ConvertUtils.ToFloatArray(dashArray);
          stroke.DashOffset = (float)lineDash.DashPhase;
        }
      }
      return stroke;
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region fields
    private readonly bool closed;
    private readonly bool filled;
    private readonly WindModeEnum fillMode;
    private readonly bool stroked;
    #endregion

    #region constructors
    private PaintPath(
      string operator_,
      bool closed,
      bool stroked,
      bool filled,
      WindModeEnum? fillMode
      ) : base(operator_)
    {
      this.closed = closed;
      this.stroked = stroked;
      this.filled = filled;
      this.fillMode = (fillMode.HasValue ? fillMode.Value : WindModeEnum.EvenOdd);
    }
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the filling rule.</summary>
    */
    public WindModeEnum FillMode
    {
      get
      {return fillMode;}
    }

    /**
      <summary>Gets whether the current path has to be closed.</summary>
    */
    public bool Closed
    {
      get
      {return closed;}
    }

    /**
      <summary>Gets whether the current path has to be filled.</summary>
    */
    public bool Filled
    {
      get
      {return filled;}
    }

    public override void Scan(
      ContentScanner.GraphicsState state
      )
    {
      ContentScanner scanner = state.Scanner;
      GraphicsPath pathObject = scanner.RenderObject;
      if(pathObject != null)
      {
        Graphics context = scanner.RenderContext;

        if(closed)
        {
          pathObject.CloseFigure();
        }
        if(filled)
        {
          pathObject.FillMode = fillMode.ToGdi();
          context.FillPath(state.FillColorSpace.GetPaint(state.FillColor), pathObject);
        }
        if(stroked)
        {
          context.DrawPath(GetStroke(state), pathObject);
        }
      }
    }

    /**
      <summary>Gets whether the current path has to be stroked.</summary>
    */
    public bool Stroked
    {
      get
      {return stroked;}
    }
    #endregion
    #endregion
    #endregion
  }
}