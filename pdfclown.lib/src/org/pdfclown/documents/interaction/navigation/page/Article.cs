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
using org.pdfclown.documents.interchange.metadata;
using org.pdfclown.objects;
using org.pdfclown.util;

using System;
using System.Collections.Generic;

namespace org.pdfclown.documents.interaction.navigation.page
{
  /**
    <summary>Article thread [PDF:1.7:8.3.2].</summary>
  */
  [PDF(VersionEnum.PDF11)]
  public sealed class Article
    : PdfObjectWrapper<PdfDictionary>
  {
    #region static
    #region interface
    #region public
    public static Article Wrap(
      PdfDirectObject baseObject
      )
    {return baseObject != null ? new Article(baseObject) : null;}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public Article(
      Document context
      ) : base(
        context,
        new PdfDictionary(
          new PdfName[]
          {PdfName.Type},
          new PdfDirectObject[]
          {PdfName.Thread}
          )
        )
    {context.Articles.Add(this);}

    private Article(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Deletes this thread removing also its reference in the document's collection.</summary>
    */
    public override bool Delete(
      )
    {
      // Shallow removal (references):
      // * reference in document
      Document.Articles.Remove(this);

      // Deep removal (indirect object).
      return base.Delete();
    }
  
    /**
      <summary>Gets the beads associated to this thread.</summary>
    */
    public ArticleElements Elements
    {
      get
      {return ArticleElements.Wrap(BaseObject);}
    }

    /**
      <summary>Gets/Sets common article metadata.</summary>
    */
    public Information Information
    {
      get
      {return Information.Wrap(BaseDataObject.Get<PdfDictionary>(PdfName.I));}
      set
      {BaseDataObject[PdfName.I] = PdfObjectWrapper.GetBaseObject(value);}
    }
    #endregion
    #endregion
    #endregion
  }
}