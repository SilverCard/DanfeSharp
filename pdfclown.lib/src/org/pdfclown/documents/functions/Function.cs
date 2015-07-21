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

using org.pdfclown.files;
using org.pdfclown.objects;
using org.pdfclown.util.math;

using System;
using System.Collections.Generic;

namespace org.pdfclown.documents.functions
{
  /**
    <summary>Function [PDF:1.6:3.9].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public abstract class Function
    : PdfObjectWrapper<PdfDataObject>
  {
    #region types
    /**
      <summary>Default intervals callback.</summary>
    */
    protected delegate IList<Interval<T>> DefaultIntervalsCallback<T>(
      IList<Interval<T>> intervals
      ) where T : IComparable<T>;
    #endregion

    #region static
    #region fields
    private const int FunctionType0 = 0;
    private const int FunctionType2 = 2;
    private const int FunctionType3 = 3;
    private const int FunctionType4 = 4;
    #endregion

    #region interface
    #region public
    /**
      <summary>Wraps a function base object into a function object.</summary>
      <param name="baseObject">Function base object.</param>
      <returns>Function object associated to the base object.</returns>
    */
    public static Function Wrap(
      PdfDirectObject baseObject
      )
    {
      if(baseObject == null)
        return null;

      PdfDataObject dataObject = baseObject.Resolve();
      PdfDictionary dictionary = GetDictionary(dataObject);
      int functionType = ((PdfInteger)dictionary[PdfName.FunctionType]).RawValue;
      switch(functionType)
      {
        case FunctionType0:
          return new Type0Function(baseObject);
        case FunctionType2:
          return new Type2Function(baseObject);
        case FunctionType3:
          return new Type3Function(baseObject);
        case FunctionType4:
          return new Type4Function(baseObject);
        default:
          throw new NotSupportedException("Function type " + functionType + " unknown.");
      }
    }
    #endregion

    #region private
    /**
      <summary>Gets a function's dictionary.</summary>
      <param name="functionDataObject">Function data object.</param>
    */
    private static PdfDictionary GetDictionary(
      PdfDataObject functionDataObject
      )
    {
      if(functionDataObject is PdfDictionary)
        return (PdfDictionary)functionDataObject;
      else // MUST be PdfStream.
        return ((PdfStream)functionDataObject).Header;
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    protected Function(
      Document context,
      PdfDataObject baseDataObject
      ) : base(context, baseDataObject)
    {}

    protected Function(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the result of the calculation applied by this function
      to the specified input values.</summary>
      <param name="inputs">Input values.</param>
     */
    public abstract double[] Calculate(
      double[] inputs
      );

    /**
      <summary>Gets the result of the calculation applied by this function
      to the specified input values.</summary>
      <param name="inputs">Input values.</param>
     */
    public IList<PdfDirectObject> Calculate(
      IList<PdfDirectObject> inputs
      )
    {
      IList<PdfDirectObject> outputs = new List<PdfDirectObject>();
      {
        double[] inputValues = new double[inputs.Count];
        for(
          int index = 0,
            length = inputValues.Length;
          index < length;
          index++
          )
        {inputValues[index] = ((IPdfNumber)inputs[index]).RawValue;}
        double[] outputValues = Calculate(inputValues);
        for(
          int index = 0,
            length = outputValues.Length;
          index < length;
          index++
          )
        {outputs.Add(PdfReal.Get(outputValues[index]));}
      }
      return outputs;
    }

    /**
      <summary>Gets the (inclusive) domains of the input values.</summary>
      <remarks>Input values outside the declared domains are clipped to the nearest boundary value.</remarks>
    */
    public IList<Interval<double>> Domains
    {
      get
      {return GetIntervals<double>(PdfName.Domain, null);}
    }

    /**
      <summary>Gets the number of input values (parameters) of this function.</summary>
    */
    public int InputCount
    {
      get
      {return ((PdfArray)Dictionary[PdfName.Domain]).Count / 2;}
    }

    /**
      <summary>Gets the number of output values (results) of this function.</summary>
    */
    public int OutputCount
    {
      get
      {
        PdfArray rangesObject = (PdfArray)Dictionary[PdfName.Range];
        return rangesObject == null ? 1 : rangesObject.Count / 2;
      }
    }

    /**
      <summary>Gets the (inclusive) ranges of the output values.</summary>
      <remarks>Output values outside the declared ranges are clipped to the nearest boundary value;
      if this entry is absent, no clipping is done.</remarks>
      <returns><code>null</code> in case of unbounded ranges.</returns>
    */
    public IList<Interval<double>> Ranges
    {
      get
      {return GetIntervals<double>(PdfName.Range, null);}
    }
    #endregion

    #region protected
    /**
      <summary>Gets this function's dictionary.</summary>
    */
    protected PdfDictionary Dictionary
    {
      get
      {return GetDictionary(BaseDataObject);}
    }

    /**
      <summary>Gets the intervals corresponding to the specified key.</summary>
    */
    protected  IList<Interval<T>> GetIntervals<T>(
      PdfName key,
      DefaultIntervalsCallback<T> defaultIntervalsCallback
      ) where T : IComparable<T>
    {
      IList<Interval<T>> intervals;
      {
        PdfArray intervalsObject = (PdfArray)Dictionary[key];
        if(intervalsObject == null)
        {
          intervals = (defaultIntervalsCallback == null
            ? null
            : defaultIntervalsCallback(new List<Interval<T>>()));
        }
        else
        {
          intervals = new List<Interval<T>>();
          for(
            int index = 0,
              length = intervalsObject.Count;
            index < length;
            index += 2
            )
          {
            intervals.Add(
              new Interval<T>(
                (T)((IPdfNumber)intervalsObject[index]).Value,
                (T)((IPdfNumber)intervalsObject[index+1]).Value
                )
              );
          }
        }
      }
      return intervals;
    }
    #endregion
    #endregion
    #endregion
  }
}