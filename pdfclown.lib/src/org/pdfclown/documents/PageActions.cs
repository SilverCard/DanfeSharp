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
using org.pdfclown.files;
using org.pdfclown.objects;

using system = System;
using System.Collections.Generic;

namespace org.pdfclown.documents
{
  /**
    <summary>Page actions [PDF:1.6:8.5.2].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class PageActions
    : PdfObjectWrapper<PdfDictionary>
  {
    #region dynamic
    #region constructors
    public PageActions(
      Document context
      ) : base(context, new PdfDictionary())
    {}

    internal PageActions(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the action to be performed when the page is closed.</summary>
    */
    public Action OnClose
    {
      get
      {return Action.Wrap(BaseDataObject[PdfName.C]);}
      set
      {BaseDataObject[PdfName.C] = value.BaseObject;}
    }

    /**
      <summary>Gets/Sets the action to be performed when the page is opened.</summary>
    */
    public Action OnOpen
    {
      get
      {return Action.Wrap(BaseDataObject[PdfName.O]);}
      set
      {BaseDataObject[PdfName.O] = value.BaseObject;}
    }
    #endregion
    #endregion
    #endregion
  }
}