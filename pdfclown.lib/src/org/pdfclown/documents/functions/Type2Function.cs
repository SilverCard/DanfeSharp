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

using System;
using System.Collections.Generic;

namespace org.pdfclown.documents.functions
{
  /**
    <summary>Exponential interpolation of one input value and <code>n</code> output values
    [PDF:1.6:3.9.2].</summary>
    <remarks>Each input value <code>x</code> will return <code>n</code> values, given by <code>
    y[j] = C0[j] + x^N × (C1[j] − C0[j])</code>, for <code>0 ≤ j < n</code>, where <code>C0</code>
    and <code>C1</code> are the {@link #getBoundOutputValues() function results} when, respectively,
    <code>x = 0</code> and <code>x = 1</code>, and <code>N</code> is the {@link #getExponent()
    interpolation exponent}.</remarks>
  */
  [PDF(VersionEnum.PDF13)]
  public sealed class Type2Function
    : Function
  {
    #region dynamic
    #region constructors
    //TODO:implement function creation!

    internal Type2Function(
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
      <summary>Gets the output value pairs <code>(C0,C1)</code> for lower (<code>0.0</code>)
      and higher (<code>1.0</code>) input values.</summary>
    */
    public IList<double[]> BoundOutputValues
    {
      get
      {
        IList<double[]> outputBounds;
        {
          PdfArray lowOutputBoundsObject = (PdfArray)Dictionary[PdfName.C0];
          PdfArray highOutputBoundsObject = (PdfArray)Dictionary[PdfName.C1];
          if(lowOutputBoundsObject == null)
          {
            outputBounds = new List<double[]>();
            outputBounds.Add(new double[]{0,1});
          }
          else
          {
            outputBounds = new List<double[]>();
            IEnumerator<PdfDirectObject> lowOutputBoundsObjectIterator = lowOutputBoundsObject.GetEnumerator();
            IEnumerator<PdfDirectObject> highOutputBoundsObjectIterator = highOutputBoundsObject.GetEnumerator();
            while(lowOutputBoundsObjectIterator.MoveNext()
              && highOutputBoundsObjectIterator.MoveNext())
            {
              outputBounds.Add(
                new double[]
                {
                  ((IPdfNumber)lowOutputBoundsObjectIterator.Current).RawValue,
                  ((IPdfNumber)highOutputBoundsObjectIterator.Current).RawValue
                }
                );
            }
          }
        }
        return outputBounds;
      }
    }

    /**
      <summary>Gets the interpolation exponent.</summary>
    */
    public double Exponent
    {
      get
      {return ((IPdfNumber)Dictionary[PdfName.N]).RawValue;}
    }
    #endregion
    #endregion
    #endregion
  }
}