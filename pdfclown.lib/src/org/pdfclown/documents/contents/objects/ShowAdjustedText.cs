/*
  Copyright 2009-2012 Stefano Chizzolini. http://www.pdfclown.org

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

  redistributions retain the above copyright notice, license and disclaimer, along with
  Redistribution and use, with or without modification, are permitted provided that such
  this list of conditions.
*/

using org.pdfclown.bytes;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;
using System.IO;

namespace org.pdfclown.documents.contents.objects
{
  /**
    <summary>'Show one or more text strings, allowing individual glyph positioning'
    operation [PDF:1.6:5.3.2].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class ShowAdjustedText
    : ShowText
  {
    #region static
    #region fields
    public static readonly string OperatorKeyword = "TJ";
    #endregion
    #endregion

    #region dynamic
    #region constructors
    /**
      <param name="value">Each element can be either a byte array (encoded text) or a number.
        If the element is a byte array (encoded text), this operator shows the text glyphs.
        If it is a number (glyph adjustment), the operator adjusts the next glyph position by that amount.</param>
    */
    public ShowAdjustedText(
      IList<object> value
      ) : base(OperatorKeyword)
    {Value = value;}

    public ShowAdjustedText(
      IList<PdfDirectObject> operands
      ) : base(OperatorKeyword,operands)
    {}
    #endregion

    #region interface
    #region public
    public override byte[] Text
    {
      get
      {
        MemoryStream textStream = new MemoryStream();
        foreach(PdfDirectObject element in ((PdfArray)operands[0]))
        {
          if(element is PdfString)
          {
            byte[] elementValue = ((PdfString)element).RawValue;
            textStream.Write(elementValue,0,elementValue.Length);
          }
        }
        return textStream.ToArray();
      }
      set
      {Value = new List<object>(){(object)value};}
    }

    public override IList<object> Value
    {
      get
      {
        IList<object> value = new List<object>();
        foreach(PdfDirectObject element in ((PdfArray)operands[0]))
        {
          //TODO:horrible workaround to the lack of generic covariance...
          if(element is IPdfNumber)
          {
            value.Add(
              ((IPdfNumber)element).RawValue
              );
          }
          else if(element is PdfString)
          {
            value.Add(
              ((PdfString)element).RawValue
              );
          }
          else
            throw new NotSupportedException("Element type " + element.GetType().Name + " not supported.");
        }
        return value;
      }
      set
      {
        PdfArray elements = new PdfArray();
        operands[0] = elements;
        bool textItemExpected = true;
        foreach(object valueItem in value)
        {
          PdfDirectObject element;
          if(textItemExpected)
          {element = new PdfString((byte[])valueItem);}
          else
          {element = PdfReal.Get((double)valueItem);}
          elements.Add(element);

          textItemExpected = !textItemExpected;
        }
      }
    }
    #endregion
    #endregion
    #endregion
  }
}