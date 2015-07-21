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

using org.pdfclown.objects;
using org.pdfclown.util.math;

using System;
using System.Collections.Generic;

namespace org.pdfclown.documents.functions
{
  /**
    <summary>Stitching function producing a single new 1-input function from the combination of the
    subdomains of <see cref="Functions">several 1-input functions</see> [PDF:1.6:3.9.3].</summary>
  */
  [PDF(VersionEnum.PDF13)]
  public sealed class Type3Function
    : Function
  {
    #region dynamic
    #region constructors
    //TODO:implement function creation!

    internal Type3Function(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    public override double[] Calculate(
      double[] inputs
      )
    {
      // FIXME: Auto-generated method stub
      return null;
    }

    /**
      <summary>Gets the <see cref="Domains">domain</see> partition bounds whose resulting intervals
      are respectively applied to each <see cref="Functions">function</see>.</summary>
    */
    public IList<double> DomainBounds
    {
      get
      {
        IList<double> domainBounds = new List<double>();
        {
          PdfArray domainBoundsObject = (PdfArray)Dictionary.Resolve(PdfName.Bounds);
          foreach(PdfDirectObject domainBoundObject in domainBoundsObject)
          {domainBounds.Add(((IPdfNumber)domainBoundObject).RawValue);}
        }
        return domainBounds;
      }
    }

    /**
      <summary>Gets the mapping of each <see cref="DomainBounds">subdomain</see> into the domain
      of the corresponding <see cref="Functions">function</see>.</summary>
    */
    public IList<Interval<double>> DomainEncodes
    {
      get
      {return GetIntervals<double>(PdfName.Encode, null);}
    }

    /**
      <summary>Gets the 1-input functions making up this stitching function.</summary>
      <remarks>The output dimensionality of all functions must be the same.</remarks>
    */
    public Functions Functions
    {
      get
      {return new Functions(Dictionary[PdfName.Functions], this);}
    }
    #endregion
    #endregion
    #endregion
  }
}