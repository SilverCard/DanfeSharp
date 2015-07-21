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
    <summary>Sampled function using a sequence of sample values to provide an approximation for
    functions whose domains and ranges are bounded [PDF:1.6:3.9.1].</summary>
    <remarks>The samples are organized as an m-dimensional table in which each entry has n components.
    </remarks>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class Type0Function
    : Function
  {
    #region types
    public enum InterpolationOrderEnum
    {
      /**
        Linear spline interpolation.
      */
      Linear = 1,
      /**
        Cubic spline interpolation.
      */
      Cubic = 3
    }
    #endregion

    #region dynamic
    #region constructors
    //TODO:implement function creation!

    internal Type0Function(
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
      <summary>Gets the linear mapping of input values into the domain of the function's sample table.</summary>
    */
    public IList<Interval<int>> DomainEncodes
    {
      get
      {
        return GetIntervals<int>(
          PdfName.Encode,
          delegate(IList<Interval<int>> intervals)
          {
            foreach(int sampleCount in SampleCounts)
            {intervals.Add(new Interval<int>(0, sampleCount-1));}
            return intervals;
          }
          );
      }
    }

    /**
      <summary>Gets the order of interpolation between samples.</summary>
    */
    public InterpolationOrderEnum Order
    {
      get
      {
        PdfInteger interpolationOrderObject = (PdfInteger)Dictionary[PdfName.Order];
        return (interpolationOrderObject == null
          ? InterpolationOrderEnum.Linear
          : (InterpolationOrderEnum)interpolationOrderObject.RawValue);
      }
    }

    /**
      <summary>Gets the linear mapping of sample values into the ranges of the function's output values.</summary>
    */
    public IList<Interval<double>> RangeDecodes
    {
      get
      {return GetIntervals<double>(PdfName.Decode, null);}
    }

    /**
      <summary>Gets the number of bits used to represent each sample.</summary>
    */
    public int SampleBitsCount
    {
      get
      {return ((PdfInteger)Dictionary[PdfName.BitsPerSample]).RawValue;}
    }

    /**
      <summary>Gets the number of samples in each input dimension of the sample table.</summary>
    */
    public IList<int> SampleCounts
    {
      get
      {
        List<int> sampleCounts = new List<int>();
        {
          PdfArray sampleCountsObject = (PdfArray)Dictionary[PdfName.Size];
          foreach(PdfDirectObject sampleCountObject in sampleCountsObject)
          {sampleCounts.Add(((PdfInteger)sampleCountObject).RawValue);}
        }
        return sampleCounts;
      }
    }
    #endregion
    #endregion
    #endregion
  }
}