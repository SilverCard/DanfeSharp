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

using org.pdfclown.documents;
using org.pdfclown.files;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;

namespace org.pdfclown.documents.interaction.viewer
{
  /**
    <summary>Viewer preferences [PDF:1.6:8.1].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class ViewerPreferences
    : PdfObjectWrapper<PdfDictionary>
  {
    #region types
    /**
      <summary>Predominant reading order for text [PDF:1.6:8.1].</summary>
    */
    public enum DirectionEnum
    {
      /**
        <summary>Left to right.</summary>
      */
      LeftToRight,
      /**
        <summary>Right to left.</summary>
      */
      RightToLeft
    };
    #endregion

    #region static
    #region fields
    private static readonly Dictionary<DirectionEnum,PdfName> DirectionEnumCodes;
    #endregion

    #region constructors
    static ViewerPreferences()
    {
      DirectionEnumCodes = new Dictionary<DirectionEnum,PdfName>();
      DirectionEnumCodes[DirectionEnum.LeftToRight] = PdfName.L2R;
      DirectionEnumCodes[DirectionEnum.RightToLeft] = PdfName.R2L;
    }
    #endregion

    #region interface
    #region public
    public static ViewerPreferences Wrap(
      PdfDirectObject baseObject
      )
    {return baseObject != null ? new ViewerPreferences(baseObject) : null;}
    #endregion

    #region private
    /**
      <summary>Gets the code corresponding to the given value.</summary>
    */
    private static PdfName ToCode(
      DirectionEnum value
      )
    {return DirectionEnumCodes[value];}

    /**
      <summary>Gets the direction corresponding to the given value.</summary>
    */
    private static DirectionEnum ToDirectionEnum(
      PdfName value
      )
    {
      foreach(KeyValuePair<DirectionEnum,PdfName> direction in DirectionEnumCodes)
      {
        if(direction.Value.Equals(value))
          return direction.Key;
      }
      return DirectionEnum.LeftToRight;
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public ViewerPreferences(
      Document context
      ) : base(context, new PdfDictionary())
    {}

    private ViewerPreferences(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    public bool CenterWindow
    {
      get
      {return (bool)Get(PdfName.CenterWindow, false);}
      set
      {BaseDataObject[PdfName.CenterWindow] = PdfBoolean.Get(value);}
    }

    public DirectionEnum Direction
    {
      get
      {return ToDirectionEnum((PdfName)BaseDataObject[PdfName.Direction]);}
      set
      {BaseDataObject[PdfName.Direction] = ToCode(value);}
    }

    public bool DisplayDocTitle
    {
      get
      {return (bool)Get(PdfName.DisplayDocTitle, false);}
      set
      {BaseDataObject[PdfName.DisplayDocTitle] = PdfBoolean.Get(value);}
    }

    public bool FitWindow
    {
      get
      {return (bool)Get(PdfName.FitWindow, false);}
      set
      {BaseDataObject[PdfName.FitWindow] = PdfBoolean.Get(value);}
    }

    public bool HideMenubar
    {
      get
      {return (bool)Get(PdfName.HideMenubar, false);}
      set
      {BaseDataObject[PdfName.HideMenubar] = PdfBoolean.Get(value);}
    }

    public bool HideToolbar
    {
      get
      {return (bool)Get(PdfName.HideToolbar, false);}
      set
      {BaseDataObject[PdfName.HideToolbar] = PdfBoolean.Get(value);}
    }

    public bool HideWindowUI
    {
      get
      {return (bool)Get(PdfName.HideWindowUI, false);}
      set
      {BaseDataObject[PdfName.HideWindowUI] = PdfBoolean.Get(value);}
    }
    #endregion

    #region private
    private object Get(
      PdfName key,
      object defaultValue
      )
    {return PdfSimpleObject<object>.GetValue(BaseDataObject[key], defaultValue);}
    #endregion
    #endregion
    #endregion
  }
}