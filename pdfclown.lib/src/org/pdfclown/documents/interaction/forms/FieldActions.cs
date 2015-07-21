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
using org.pdfclown.documents.interaction.actions;
using org.pdfclown.documents.interaction.annotations;
using org.pdfclown.objects;

using system = System;

namespace org.pdfclown.documents.interaction.forms
{
  /**
    <summary>Form field actions [PDF:1.6:8.5.2].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class FieldActions
    : PdfObjectWrapper<PdfDictionary>
  {
    #region dynamic
    #region constructors
    public FieldActions(
      Document context
      ) : base(context, new PdfDictionary())
    {}

    internal FieldActions(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets a JavaScript action to be performed to recalculate the value
      of this field when that of another field changes.</summary>
    */
    public JavaScript OnCalculate
    {
      get
      {return (JavaScript)Action.Wrap(BaseDataObject[PdfName.C]);}
      set
      {BaseDataObject[PdfName.C] = value.BaseObject;}
    }

    /**
      <summary>Gets/Sets a JavaScript action to be performed when the user types a keystroke
      into a text field or combo box or modifies the selection in a scrollable list box.</summary>
    */
    public JavaScript OnChange
    {
      get
      {return (JavaScript)Action.Wrap(BaseDataObject[PdfName.K]);}
      set
      {BaseDataObject[PdfName.K] = value.BaseObject;}
    }

    /**
      <summary>Gets/Sets a JavaScript action to be performed before the field is formatted
      to display its current value.</summary>
      <remarks>This action can modify the field's value before formatting.</remarks>
    */
    public JavaScript OnFormat
    {
      get
      {return (JavaScript)Action.Wrap(BaseDataObject[PdfName.F]);}
      set
      {BaseDataObject[PdfName.F] = value.BaseObject;}
    }

    /**
      <summary>Gets/Sets a JavaScript action to be performed when the field's value is changed.</summary>
      <remarks>This action can check the new value for validity.</remarks>
    */
    public JavaScript OnValidate
    {
      get
      {return (JavaScript)Action.Wrap(BaseDataObject[PdfName.V]);}
      set
      {BaseDataObject[PdfName.V] = value.BaseObject;}
    }
    #endregion
    #endregion
    #endregion
  }
}