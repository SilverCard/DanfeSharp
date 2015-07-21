/*
  Copyright 2006-2012 Stefano Chizzolini. http://www.pdfclown.org

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

using tokens = org.pdfclown.tokens;
using org.pdfclown.util.parsers;

using System;
using System.Globalization;
using System.Text;

namespace org.pdfclown.objects
{
  /**
    <summary>PDF date object [PDF:1.6:3.8.3].</summary>
  */
  public sealed class PdfDate
    : PdfString
  {
    #region static
    #region fields
    private const string FormatString = "yyyyMMddHHmmsszzz";
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the object equivalent to the given value.</summary>
    */
    public static PdfDate Get(
      DateTime? value
      )
    {return value.HasValue ? new PdfDate(value.Value) : null;}

    /**
      <summary>Converts a PDF date literal into its corresponding date.</summary>
      <exception cref="org.pdfclown.util.parsers.ParseException">Thrown when date literal parsing fails.</exception>
    */
    public static DateTime ToDate(
      string value
      )
    {
      // 1. Normalization.
      StringBuilder dateBuilder = new StringBuilder();
      try
      {
        int length = value.Length;
        // Year (YYYY).
        dateBuilder.Append(value.Substring(2, 4)); // NOTE: Skips the "D:" prefix; Year is mandatory.
        // Month (MM).
        dateBuilder.Append(length < 8 ? "01" : value.Substring(6, 2));
        // Day (DD).
        dateBuilder.Append(length < 10 ? "01" : value.Substring(8, 2));
        // Hour (HH).
        dateBuilder.Append(length < 12 ? "00" : value.Substring(10, 2));
        // Minute (mm).
        dateBuilder.Append(length < 14 ? "00" : value.Substring(12, 2));
        // Second (SS).
        dateBuilder.Append(length < 16 ? "00" : value.Substring(14, 2));
        // Local time / Universal Time relationship (O).
        dateBuilder.Append(length < 17 || value.Substring(16, 1).Equals("Z") ? "+" : value.Substring(16, 1));
        // UT Hour offset (HH').
        dateBuilder.Append(length < 19 ? "00" : value.Substring(17, 2));
        // UT Minute offset (mm').
        dateBuilder.Append(":").Append(length < 22 ? "00" : value.Substring(20, 2));
      }
      catch(Exception exception)
      {throw new ParseException("Failed to normalize the date string.", exception);}

      // 2. Parsing.
      try
      {
        return DateTime.ParseExact(
          dateBuilder.ToString(),
          FormatString,
          new CultureInfo("en-US")
          );
      }
      catch(Exception exception)
      {throw new ParseException("Failed to parse the date string.", exception);}
    }
    #endregion

    #region private
    private static string Format(
      DateTime value
      )
    {return ("D:" + value.ToString(FormatString).Replace(':','\'') + "'");}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public PdfDate(
      DateTime value
      )
    {Value = value;}
    #endregion

    #region interface
    #region public
    public override PdfObject Accept(
      IVisitor visitor,
      object data
      )
    {return visitor.Visit(this, data);}

    public override SerializationModeEnum SerializationMode {
      get
      {return base.SerializationMode;}
      set
      {/* NOOP: Serialization MUST be kept literal. */}
    }

    public override object Value
    {
      get
      // FIXME: proper call to base.StringValue could NOT be done due to an unexpected Mono runtime SIGSEGV (TOO BAD).
//            {return ToDate(base.StringValue);}
      {return ToDate((string)base.Value);}
      protected set
      {RawValue = tokens::Encoding.Pdf.Encode(Format((DateTime)value));}
    }
    #endregion
    #endregion
    #endregion
  }
}
