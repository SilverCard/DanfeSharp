/*
  Copyright 2008-2010 Stefano Chizzolini. http://www.pdfclown.org

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
using System.Drawing.Drawing2D;

namespace org.pdfclown.documents.contents.objects
{
  /**
    <summary>Path object [PDF:1.6:4.4].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class Path
    : GraphicsObject
  {
    #region static
    #region fields
    public static readonly string[] BeginOperatorKeywords = new string[]
      {
        BeginSubpath.OperatorKeyword,
        DrawRectangle.OperatorKeyword
      };
    public static readonly string[] EndOperatorKeywords = new string[]
      {
        PaintPath.CloseFillStrokeEvenOddOperatorKeyword,
        PaintPath.CloseFillStrokeOperatorKeyword,
        PaintPath.CloseStrokeOperatorKeyword,
        PaintPath.EndPathNoOpOperatorKeyword,
        PaintPath.FillEvenOddOperatorKeyword,
        PaintPath.FillObsoleteOperatorKeyword,
        PaintPath.FillOperatorKeyword,
        PaintPath.FillStrokeEvenOddOperatorKeyword,
        PaintPath.FillStrokeOperatorKeyword,
        PaintPath.StrokeOperatorKeyword
      };
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public Path(
      )
    {}

    public Path(
      IList<ContentObject> operations
      ) : base(operations)
    {}
    #endregion

    #region interface
    #region protected
    protected override GraphicsPath CreateRenderObject(
      )
    {return new GraphicsPath();}
    #endregion
    #endregion
    #endregion
  }
}