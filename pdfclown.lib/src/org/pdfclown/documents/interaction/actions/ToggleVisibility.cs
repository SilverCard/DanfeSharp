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
using org.pdfclown.documents.interaction.forms;
using org.pdfclown.files;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;

namespace org.pdfclown.documents.interaction.actions
{
  /**
    <summary>'Toggle the visibility of one or more annotations on the screen' action [PDF:1.6:8.5.3].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class ToggleVisibility
    : Action
  {
    #region dynamic
    #region constructors
    /**
      <summary>Creates a new action within the given document context.</summary>
    */
    public ToggleVisibility(
      Document context,
      ICollection<PdfObjectWrapper> objects,
      bool visible
      ) : base(context, PdfName.Hide)
    {
      Objects = objects;
      Visible = visible;
    }

    internal ToggleVisibility(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the annotations (or associated form fields) to be affected.</summary>
    */
    public ICollection<PdfObjectWrapper> Objects
    {
      get
      {
        List<PdfObjectWrapper> objects = new List<PdfObjectWrapper>();
        {
          PdfDirectObject objectsObject = BaseDataObject[PdfName.T];
          FillObjects(objectsObject, objects);
        }
        return objects;
      }
      set
      {
        PdfArray objectsDataObject = new PdfArray();
        foreach(PdfObjectWrapper item in value)
        {
          if(item is Annotation)
          {
            objectsDataObject.Add(
              item.BaseObject
              );
          }
          else if(item is Field)
          {
            objectsDataObject.Add(
              new PdfTextString(((Field)item).FullName)
              );
          }
          else
          {
            throw new ArgumentException(
              "Invalid 'Hide' action target type (" + item.GetType().Name + ").\n"
                + "It MUST be either an annotation or a form field."
              );
          }
        }
        BaseDataObject[PdfName.T] = objectsDataObject;
      }
    }

    /**
      <summary>Gets/Sets whether to show the annotations.</summary>
    */
    public bool Visible
    {
      get
      {
        PdfBoolean hideObject = (PdfBoolean)BaseDataObject[PdfName.H];
        return hideObject != null
          ? !hideObject.BooleanValue
          : false;
      }
      set
      {BaseDataObject[PdfName.H] = PdfBoolean.Get(!value);}
    }
    #endregion

    #region private
    private void FillObjects(
      PdfDataObject objectObject,
      ICollection<PdfObjectWrapper> objects
      )
    {
      PdfDataObject objectDataObject = PdfObject.Resolve(objectObject);
      if(objectDataObject is PdfArray) // Multiple objects.
      {
        foreach(PdfDirectObject itemObject in (PdfArray)objectDataObject)
        {FillObjects(itemObject,objects);}
      }
      else // Single object.
      {
        if(objectDataObject is PdfDictionary) // Annotation.
        {
          objects.Add(
            Annotation.Wrap((PdfReference)objectObject)
            );
        }
        else if(objectDataObject is PdfTextString) // Form field (associated to widget annotations).
        {
          objects.Add(
            Document.Form.Fields[
              (string)((PdfTextString)objectDataObject).Value
              ]
            );
        }
        else // Invalid object type.
        {
          throw new Exception(
            "Invalid 'Hide' action target type (" + objectDataObject.GetType().Name + ").\n"
              + "It should be either an annotation or a form field."
            );
        }
      }
    }
    #endregion
    #endregion
    #endregion
  }
}