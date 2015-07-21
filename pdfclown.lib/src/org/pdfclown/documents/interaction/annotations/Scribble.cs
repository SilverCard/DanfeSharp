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
    <summary>Freehand "scribble" composed of one or more disjoint paths [PDF:1.6:8.4.5].</summary>
  */
  [PDF(VersionEnum.PDF13)]
  public sealed class Scribble
    : Annotation
  {
    #region dynamic
    #region constructors
    public Scribble(
      Page page,
      RectangleF box,
      string text,
      IList<IList<PointF>> paths
      ) : base(page, PdfName.Ink, box, text)
    {Paths = paths;}

    internal Scribble(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the coordinates of each path.</summary>
    */
    public IList<IList<PointF>> Paths
    {
      get
      {
        PdfArray pathsObject = (PdfArray)BaseDataObject[PdfName.InkList];
        IList<IList<PointF>> paths = new List<IList<PointF>>();
        double pageHeight = Page.Box.Height;
        for(
          int pathIndex = 0,
            pathLength = pathsObject.Count;
          pathIndex < pathLength;
          pathIndex++
          )
        {
          PdfArray pathObject = (PdfArray)pathsObject[pathIndex];
          IList<PointF> path = new List<PointF>();
          for(
            int pointIndex = 0,
              pointLength = pathObject.Count;
            pointIndex < pointLength;
            pointIndex += 2
            )
          {
            path.Add(
              new PointF(
                (float)((IPdfNumber)pathObject[pointIndex]).RawValue,
                (float)(pageHeight - ((IPdfNumber)pathObject[pointIndex+1]).RawValue)
                )
              );
          }
          paths.Add(path);
        }

        return paths;
      }
      set
      {
        PdfArray pathsObject = new PdfArray();
        double pageHeight = Page.Box.Height;
        foreach(IList<PointF> path in value)
        {
          PdfArray pathObject = new PdfArray();
          foreach(PointF point in path)
          {
            pathObject.Add(PdfReal.Get(point.X)); // x.
            pathObject.Add(PdfReal.Get(pageHeight-point.Y)); // y.
          }
          pathsObject.Add(pathObject);
        }

        BaseDataObject[PdfName.InkList] = pathsObject;
      }
    }
    #endregion
    #endregion
    #endregion
  }
}