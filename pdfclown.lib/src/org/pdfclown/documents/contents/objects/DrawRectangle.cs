/*
  Copyright 2007-2012 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.objects;

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace org.pdfclown.documents.contents.objects
{
  /**
    <summary>'Append a rectangle to the current path as a complete subpath' operation
    [PDF:1.6:4.4.1].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class DrawRectangle
    : Operation
  {
    #region static
    #region fields
    public static readonly string OperatorKeyword = "re";
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public DrawRectangle(
      double x,
      double y,
      double width,
      double height
      ) : base(
        OperatorKeyword,
        new List<PdfDirectObject>(
          new PdfDirectObject[]
          {
            PdfReal.Get(x),
            PdfReal.Get(y),
            PdfReal.Get(width),
            PdfReal.Get(height)
          }
          )
        )
    {}

    public DrawRectangle(
      IList<PdfDirectObject> operands
      ) : base(OperatorKeyword,operands)
    {}
    #endregion

    #region interface
    #region public
    public double Height
    {
      get
      {return ((IPdfNumber)operands[3]).RawValue;}
      set
      {operands[3] = PdfReal.Get(value);}
    }

    public override void Scan(
      ContentScanner.GraphicsState state
      )
    {
      GraphicsPath pathObject = state.Scanner.RenderObject;
      if(pathObject != null)
      {
        double x = X,
          y = Y,
          width = Width,
          height = Height;
        pathObject.AddRectangle(
          new RectangleF((float)x, (float)y, (float)width, (float)height)
          );
        pathObject.CloseFigure();
      }
    }

    public double Width
    {
      get
      {return ((IPdfNumber)operands[2]).RawValue;}
      set
      {operands[2] = PdfReal.Get(value);}
    }

    public double X
    {
      get
      {return ((IPdfNumber)operands[0]).RawValue;}
      set
      {operands[0] = PdfReal.Get(value);}
    }

    public double Y
    {
      get
      {return ((IPdfNumber)operands[1]).RawValue;}
      set
      {operands[1] = PdfReal.Get(value);}
    }
    #endregion
    #endregion
    #endregion
  }
}