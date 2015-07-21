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
using org.pdfclown.documents.interaction.annotations;
using org.pdfclown.files;
using org.pdfclown.objects;

using System;
using System.Collections;
using System.Collections.Generic;

namespace org.pdfclown.documents.interaction.forms
{
  /**
    <summary>Field option [PDF:1.6:8.6.3].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class ChoiceItem
    : PdfObjectWrapper<PdfDirectObject>
  {
    #region fields
    private ChoiceItems items;
    #endregion

    #region constructors
    public ChoiceItem(
      string value
      ) : base(new PdfTextString(value))
    {}

    public ChoiceItem(
      Document context,
      string value,
      string text
      ) : base(
        context,
        new PdfArray(
          new PdfDirectObject[]
          {
            new PdfTextString(value),
            new PdfTextString(text)
          }
          )
        )
    {}

    internal ChoiceItem(
      PdfDirectObject baseObject,
      ChoiceItems items
      ) : base(baseObject)
    {Items = items;}
    #endregion

    #region interface
    #region public
    public override object Clone(
      Document context
      )
    {throw new NotImplementedException();}

    //TODO:make the class immutable (to avoid needing wiring it up to its collection...)!!!
    /**
      <summary>Gets/Sets the displayed text.</summary>
    */
    public string Text
    {
      get
      {
        PdfDirectObject baseDataObject = BaseDataObject;
        if(baseDataObject is PdfArray) // <value,text> pair.
          return (string)((PdfTextString)((PdfArray)baseDataObject)[1]).Value;
        else // Single text string.
          return (string)((PdfTextString)baseDataObject).Value;
      }
      set
      {
        PdfDirectObject baseDataObject = BaseDataObject;
        if(baseDataObject is PdfTextString)
        {
          PdfDirectObject oldBaseDataObject = baseDataObject;

          BaseObject = baseDataObject = new PdfArray(
              new PdfDirectObject[]{oldBaseDataObject}
              );
          ((PdfArray)baseDataObject).Add(PdfTextString.Default);

          if(items != null)
          {
            // Force list update!
            /*
              NOTE: This operation is necessary in order to substitute
              the previous base object with the new one within the list.
            */
            PdfArray itemsObject = items.BaseDataObject;
            itemsObject[itemsObject.IndexOf(oldBaseDataObject)] = baseDataObject;
          }
        }

        ((PdfArray)baseDataObject)[1] = new PdfTextString(value);
      }
    }

    /**
      <summary>Gets/Sets the export value.</summary>
    */
    public string Value
    {
      get
      {
        PdfDirectObject baseDataObject = BaseDataObject;
        if(baseDataObject is PdfArray) // <value,text> pair.
          return (string)((PdfTextString)((PdfArray)baseDataObject)[0]).Value;
        else // Single text string.
          return (string)((PdfTextString)baseDataObject).Value;
      }
      set
      {
        PdfDirectObject baseDataObject = BaseDataObject;
        if(baseDataObject is PdfArray) // <value,text> pair.
        {((PdfArray)baseDataObject)[0] = new PdfTextString(value);}
        else // Single text string.
        {BaseObject = new PdfTextString(value);}
      }
    }
    #endregion

    #region internal
    internal ChoiceItems Items
    {
      set
      {
        if(items != null)
          throw new ArgumentException("Item already associated to another choice field.");

        items = value;
      }
    }
    #endregion
    #endregion
  }
}