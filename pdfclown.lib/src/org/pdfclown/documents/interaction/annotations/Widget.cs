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
using org.pdfclown.documents.interaction.forms;
using org.pdfclown.objects;
using org.pdfclown.util;

using System;
using System.Collections.Generic;
using System.Drawing;

namespace org.pdfclown.documents.interaction.annotations
{
  /**
    <summary>Widget annotation [PDF:1.6:8.4.5].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public class Widget
    : Annotation
  {
    #region types
    /**
      <summary>Highlighting mode [PDF:1.6:8.4.5].</summary>
    */
    public enum HighlightModeEnum
    {
      /**
        <summary>No highlighting.</summary>
      */
      None,
      /**
        <summary>Invert the contents of the annotation rectangle.</summary>
      */
      Invert,
      /**
        <summary>Invert the annotation's border.</summary>
      */
      Outline,
      /**
        <summary>Display the annotation's down appearance.</summary>
      */
      Push,
      /**
        <summary>Same as Push (which is preferred).</summary>
      */
      Toggle
    };
    #endregion

    #region static
    #region fields
    private static readonly Dictionary<HighlightModeEnum,PdfName> HighlightModeEnumCodes;
    #endregion

    #region constructors
    static Widget()
    {
      HighlightModeEnumCodes = new Dictionary<HighlightModeEnum,PdfName>();
      HighlightModeEnumCodes[HighlightModeEnum.None] = PdfName.N;
      HighlightModeEnumCodes[HighlightModeEnum.Invert] = PdfName.I;
      HighlightModeEnumCodes[HighlightModeEnum.Outline] = PdfName.O;
      HighlightModeEnumCodes[HighlightModeEnum.Push] = PdfName.P;
      HighlightModeEnumCodes[HighlightModeEnum.Toggle] = PdfName.T;
    }
    #endregion

    #region interface
    #region public
    public static Widget Wrap(
      PdfDirectObject baseObject,
      Field field
      )
    {
      return field is CheckBox
          || field is RadioButton
        ? new DualWidget(baseObject)
        : new Widget(baseObject);
    }
    #endregion

    #region private
    /**
      <summary>Gets the code corresponding to the given value.</summary>
    */
    private static PdfName ToCode(
      HighlightModeEnum value
      )
    {return HighlightModeEnumCodes[value];}

    /**
      <summary>Gets the highlighting mode corresponding to the given value.</summary>
    */
    private static HighlightModeEnum ToHighlightModeEnum(
      PdfName value
      )
    {
      foreach(KeyValuePair<HighlightModeEnum,PdfName> mode in HighlightModeEnumCodes)
      {
        if(mode.Value.Equals(value))
          return mode.Key;
      }
      return HighlightModeEnum.Invert;
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public Widget(
      Page page,
      RectangleF box
      ) : base(page, PdfName.Widget, box, null)
    {Flags = EnumUtils.Mask(Flags, FlagsEnum.Print, true);}

    internal protected Widget(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    public override AnnotationActions Actions
    {
      get
      {
        PdfDirectObject actionsObject = BaseDataObject[PdfName.AA];
        return actionsObject != null ? new WidgetActions(this, actionsObject) : null;
      }
      set
      {base.Actions = value;}
    }

    /**
      <summary>Gets/Sets the annotation's appearance characteristics to be used for its visual
      presentation on the page.</summary>
    */
    public AppearanceCharacteristics AppearanceCharacteristics
    {
      get
      {return AppearanceCharacteristics.Wrap(BaseDataObject.Get<PdfDictionary>(PdfName.MK));}
      set
      {BaseDataObject[PdfName.MK] = value.BaseObject;}
    }

    /**
      <summary>Gets/Sets the annotation's highlighting mode, the visual effect to be used
      when the mouse button is pressed or held down inside its active area.</summary>
    */
    public HighlightModeEnum HighlightMode
    {
      get
      {return ToHighlightModeEnum((PdfName)BaseDataObject[PdfName.H]);}
      set
      {BaseDataObject[PdfName.H] = ToCode(value);}
    }
    #endregion
    #endregion
    #endregion
  }
}