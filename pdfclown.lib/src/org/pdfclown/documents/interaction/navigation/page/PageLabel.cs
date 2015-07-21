/*
  Copyright 2012 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.util;

using System;
using System.Collections.Generic;

namespace org.pdfclown.documents.interaction.navigation.page
{
  /**
    <summary>Page label range [PDF:1.7:8.3.1].</summary>
    <remarks>It represents a series of consecutive pages' visual identifiers using the same
    numbering system.</remarks>
  */
  [PDF(VersionEnum.PDF13)]
  public sealed class PageLabel
    : PdfObjectWrapper<PdfDictionary>
  {
    #region types
    public enum NumberStyleEnum
    {
      /**
        <summary>Decimal arabic numerals.</summary>
      */
      ArabicNumber,
      /**
        <summary>Upper-case roman numerals.</summary>
      */
      UCaseRomanNumber,
      /**
        <summary>Lower-case roman numerals.</summary>
      */
      LCaseRomanNumber,
      /**
        <summary>Upper-case letters (A to Z for the first 26 pages, AA to ZZ for the next 26, and so
        on).</summary>
      */
      UCaseLetter,
      /**
        <summary>Lower-case letters (a to z for the first 26 pages, aa to zz for the next 26, and so
        on).</summary>
      */
      LCaseLetter
    };
    #endregion

    #region static
    #region fields
    private static readonly int DefaultNumberBase = 1;
    #endregion

    #region interface
    /**
      <summary>Gets an existing page label range.</summary>
      <param name="baseObject">Base object to wrap.</param>
    */
    public static PageLabel Wrap(
      PdfDirectObject baseObject
      )
    {return baseObject != null ? new PageLabel(baseObject) : null;}
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public PageLabel(
      Document context,
      NumberStyleEnum numberStyle
      ) : this(context, null, numberStyle, DefaultNumberBase)
    {}

    public PageLabel(
      Document context,
      String prefix,
      NumberStyleEnum numberStyle,
      int numberBase
      ) : base(
        context,
        new PdfDictionary(
          new PdfName[]
          {PdfName.Type},
          new PdfDirectObject[]
          {PdfName.PageLabel}
          )
        )
    {
      Prefix = prefix;
      NumberStyle = numberStyle;
      NumberBase = numberBase;
    }

    private PageLabel(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the value of the numeric suffix for the first page label in this range.
      Subsequent pages are numbered sequentially from this value.</summary>
    */
    public int NumberBase
    {
      get
      {return (int)PdfSimpleObject<object>.GetValue(BaseDataObject[PdfName.St], DefaultNumberBase);}
      set
      {BaseDataObject[PdfName.St] = value <= DefaultNumberBase ? null : PdfInteger.Get(value);}
    }

    /**
      <summary>Gets/Sets the numbering style to be used for the numeric suffix of each page label in
      this range.</summary>
      <remarks>If no style is defined, the numeric suffix isn't displayed at all.</remarks>
    */
    public NumberStyleEnum NumberStyle
    {
      get
      {return NumberStyleEnumExtension.Get((PdfName)BaseDataObject[PdfName.S]);}
      set
      {BaseDataObject[PdfName.S] = value.GetCode();}
    }

    /**
      <summary>Gets/Sets the label prefix for page labels in this range.</summary>
    */
    public string Prefix
    {
      get
      {return (string)PdfSimpleObject<object>.GetValue(BaseDataObject[PdfName.P]);}
      set
      {BaseDataObject[PdfName.P] = value != null ? new PdfTextString(value) : null;}
    }
    #endregion
    #endregion
    #endregion
  }

  internal static class NumberStyleEnumExtension
  {
    private static readonly BiDictionary<PageLabel.NumberStyleEnum,PdfName> codes;

    static NumberStyleEnumExtension()
    {
      codes = new BiDictionary<PageLabel.NumberStyleEnum,PdfName>();
      codes[PageLabel.NumberStyleEnum.ArabicNumber] = PdfName.D;
      codes[PageLabel.NumberStyleEnum.UCaseRomanNumber] = PdfName.R;
      codes[PageLabel.NumberStyleEnum.LCaseRomanNumber] = PdfName.r;
      codes[PageLabel.NumberStyleEnum.UCaseLetter] = PdfName.A;
      codes[PageLabel.NumberStyleEnum.LCaseLetter] = PdfName.a;
    }

    public static PageLabel.NumberStyleEnum Get(
      PdfName name
      )
    {
      if(name == null)
        throw new ArgumentNullException();

      PageLabel.NumberStyleEnum? numberStyle = codes.GetKey(name);
      if(!numberStyle.HasValue)
        throw new NotSupportedException("Page layout unknown: " + name);

      return numberStyle.Value;
    }

    public static PdfName GetCode(
      this PageLabel.NumberStyleEnum numberStyle
      )
    {return codes[numberStyle];}
  }
}