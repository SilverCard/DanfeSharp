/*
  Copyright 2006-2011 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.util.math;

using System;
using System.Collections.Generic;
using drawing = System.Drawing;

namespace org.pdfclown.documents.contents.colorSpaces
{
  /**
    <summary>CIE-based ABC double-transformation-stage color space, where A, B and C represent the
    L*, a* and b* components of a CIE 1976 L*a*b* space [PDF:1.6:4.5.4].</summary>
  */
  [PDF(VersionEnum.PDF11)]
  public sealed class LabColorSpace
    : CIEBasedColorSpace
  {
    #region dynamic
    #region constructors
    //TODO:IMPL new element constructor!

    internal LabColorSpace(
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
      {return 3;}
    }

    public override Color DefaultColor
    {
      get
      {
        IList<Interval<double>> ranges = Ranges;
        return new LabColor(
          ranges[0].Low,
          ranges[1].Low,
          ranges[2].Low
          );
      }
    }

    public override Color GetColor(
      IList<PdfDirectObject> components,
      IContentContext context
      )
    {return new LabColor(components);}

    public override drawing::Brush GetPaint(
      Color color
      )
    {
      // FIXME: temporary hack
      return new drawing::SolidBrush(drawing::Color.Black);
    }

    /**
      <summary>Gets the (inclusive) ranges of the color components.</summary>
      <remarks>Component values falling outside the specified range are adjusted
      to the nearest valid value.</remarks>
    */
    //TODO:generalize to all the color spaces!
    public IList<Interval<double>> Ranges
    {
      get
      {
        IList<Interval<double>> ranges = new List<Interval<double>>();
        {
          // 1. L* component.
          ranges.Add(
            new Interval<double>(0d, 100d)
            );

          PdfArray rangesObject = (PdfArray)Dictionary[PdfName.Range];
          if(rangesObject == null)
          {
            // 2. a* component.
            ranges.Add(
              new Interval<double>(-100d, 100d)
              );
            // 3. b* component.
            ranges.Add(
              new Interval<double>(-100d, 100d)
              );
          }
          else
          {
            // 2/3. a*/b* components.
            for(
              int index = 0,
                length = rangesObject.Count;
              index < length;
              index += 2
              )
            {
              ranges.Add(
                new Interval<double>(
                  ((IPdfNumber)rangesObject[index]).RawValue,
                  ((IPdfNumber)rangesObject[index+1]).RawValue
                  )
                );
            }
          }
        }
        return ranges;
      }
    }
    #endregion
    #endregion
    #endregion
  }
}