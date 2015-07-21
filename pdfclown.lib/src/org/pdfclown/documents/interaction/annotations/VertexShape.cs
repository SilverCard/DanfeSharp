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
using org.pdfclown.objects;

using System;
using System.Collections.Generic;
using System.Drawing;

namespace org.pdfclown.documents.interaction.annotations
{
  /**
    <summary>Abstract vertexed shape annotation.</summary>
  */
  [PDF(VersionEnum.PDF15)]
  public abstract class VertexShape
    : Shape
  {
    #region dynamic
    #region constructors
    protected VertexShape(
      Page page,
      RectangleF box,
      string text,
      PdfName subtype
      ) : base(page, box, text, subtype)
    {}

    protected VertexShape(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the coordinates of each vertex.</summary>
    */
    public IList<PointF> Vertices
    {
      get
      {
        PdfArray verticesObject = (PdfArray)BaseDataObject[PdfName.Vertices];
        IList<PointF> vertices = new List<PointF>();
        float pageHeight = Page.Box.Height;
        for(
          int index = 0,
            length = verticesObject.Count;
          index < length;
          index += 2
          )
        {
          vertices.Add(
            new PointF(
              ((IPdfNumber)verticesObject[index]).FloatValue,
              pageHeight - ((IPdfNumber)verticesObject[index+1]).FloatValue
              )
            );
        }

        return vertices;
      }
      set
      {
        PdfArray verticesObject = new PdfArray();
        float pageHeight = Page.Box.Height;
        foreach(PointF vertex in value)
        {
          verticesObject.Add(PdfReal.Get(vertex.X)); // x.
          verticesObject.Add(PdfReal.Get(pageHeight-vertex.Y)); // y.
        }

        BaseDataObject[PdfName.Vertices] = verticesObject;
      }
    }
    #endregion
    #endregion
    #endregion
  }
}