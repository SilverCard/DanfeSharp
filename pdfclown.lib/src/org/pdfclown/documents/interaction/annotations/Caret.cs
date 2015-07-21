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
    <summary>Caret annotation [PDF:1.6:8.4.5].</summary>
    <remarks>It displays a visual symbol that indicates the presence of text edits.</remarks>
  */
  [PDF(VersionEnum.PDF15)]
  public sealed class Caret
    : Annotation
  {
    #region types
    /**
      <summary>Symbol type [PDF:1.6:8.4.5].</summary>
    */
    public enum SymbolTypeEnum
    {
      /**
        <summary>New paragraph.</summary>
      */
      NewParagraph,
      /**
        <summary>None.</summary>
      */
      None
    };
    #endregion

    #region static
    #region fields
    private static readonly Dictionary<SymbolTypeEnum,PdfName> SymbolTypeEnumCodes;
    #endregion

    #region constructors
    static Caret()
    {
      SymbolTypeEnumCodes = new Dictionary<SymbolTypeEnum,PdfName>();
      SymbolTypeEnumCodes[SymbolTypeEnum.NewParagraph] = PdfName.P;
      SymbolTypeEnumCodes[SymbolTypeEnum.None] = PdfName.None;
    }
    #endregion

    #region interface
    #region private
    /**
      <summary>Gets the code corresponding to the given value.</summary>
    */
    private static PdfName ToCode(
      SymbolTypeEnum value
      )
    {return SymbolTypeEnumCodes[value];}

    /**
      <summary>Gets the symbol type corresponding to the given value.</summary>
    */
    private static SymbolTypeEnum ToSymbolTypeEnum(
      PdfName value
      )
    {
      foreach(KeyValuePair<SymbolTypeEnum,PdfName> symbolType in SymbolTypeEnumCodes)
      {
        if(symbolType.Value.Equals(value))
          return symbolType.Key;
      }
      return SymbolTypeEnum.None;
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public Caret(
      Page page,
      RectangleF box,
      string text
      ) : base(page, PdfName.Caret, box, text)
    {}

    internal Caret(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the symbol to be used in displaying the annotation.</summary>
    */
    public SymbolTypeEnum SymbolType
    {
      get
      {return ToSymbolTypeEnum((PdfName)BaseDataObject[PdfName.Sy]);}
      set
      {BaseDataObject[PdfName.Sy] = ToCode(value);}
    }
    #endregion
    #endregion
    #endregion
  }
}