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

using org.pdfclown.documents.interaction.actions;
using org.pdfclown.documents.interaction.navigation.document;
using org.pdfclown.files;
using org.pdfclown.objects;

using system = System;
using System.Collections.Generic;

namespace org.pdfclown.documents
{
  /**
    <summary>Document actions [PDF:1.6:8.5.2].</summary>
  */
  [PDF(VersionEnum.PDF14)]
  public sealed class DocumentActions
    : PdfObjectWrapper<PdfDictionary>
  {
    #region dynamic
    #region constructors
    public DocumentActions(
      Document context
      ) : base(context, new PdfDictionary())
    {}

    internal DocumentActions(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the action to be performed after printing the document.</summary>
    */
    public Action AfterPrint
    {
      get
      {return Action.Wrap(BaseDataObject[PdfName.DP]);}
      set
      {BaseDataObject[PdfName.DP] = value.BaseObject;}
    }

    /**
      <summary>Gets/Sets the action to be performed after saving the document.</summary>
    */
    public Action AfterSave
    {
      get
      {return Action.Wrap(BaseDataObject[PdfName.DS]);}
      set
      {BaseDataObject[PdfName.DS] = value.BaseObject;}
    }

    /**
      <summary>Gets/Sets the action to be performed before printing the document.</summary>
    */
    public Action BeforePrint
    {
      get
      {return Action.Wrap(BaseDataObject[PdfName.WP]);}
      set
      {BaseDataObject[PdfName.WP] = value.BaseObject;}
    }

    /**
      <summary>Gets/Sets the action to be performed before saving the document.</summary>
    */
    public Action BeforeSave
    {
      get
      {return Action.Wrap(BaseDataObject[PdfName.WS]);}
      set
      {BaseDataObject[PdfName.WS] = value.BaseObject;}
    }

    /**
      <summary>Gets/Sets the action to be performed before closing the document.</summary>
    */
    public Action OnClose
    {
      get
      {return Action.Wrap(BaseDataObject[PdfName.DC]);}
      set
      {BaseDataObject[PdfName.DC] = value.BaseObject;}
    }

    /**
      <summary>Gets/Sets the destination to be displayed or the action to be performed
      after opening the document.</summary>
    */
    public PdfObjectWrapper OnOpen
    {
      get
      {
        PdfDirectObject onOpenObject = Document.BaseDataObject[PdfName.OpenAction];
        if(onOpenObject is PdfDictionary) // Action (dictionary).
          return Action.Wrap(onOpenObject);
        else // Destination (array).
          return Destination.Wrap(onOpenObject);
      }
      set
      {
        if(!(value is Action
          || value is LocalDestination))
          throw new system::ArgumentException("Value MUST be either an Action or a LocalDestination.");

        Document.BaseDataObject[PdfName.OpenAction] = value.BaseObject;
      }
    }
    #endregion
    #endregion
    #endregion
  }
}