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
    <summary>Interactive form fields [PDF:1.6:8.6.1].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class Fields
    : PdfObjectWrapper<PdfArray>,
      IDictionary<string,Field>
  {
    #region dynamic
    #region constructors
    public Fields(
      Document context
      ) : base(context, new PdfArray())
    {}

    internal Fields(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    public void Add(
      Field value
      )
    {BaseDataObject.Add(value.BaseObject);}

    #region IDictionary
    public void Add(
      string key,
      Field value
      )
    {throw new NotImplementedException();}

    public bool ContainsKey(
      string key
      )
    //TODO: avoid getter (use raw matching).
    {return this[key] != null;}

    public ICollection<string> Keys
    {
      get
      {
        throw new NotImplementedException();
      //TODO: retrieve all the full names (keys)!!!
      }
    }

    public bool Remove(
      string key
      )
    {
      Field field = this[key];
      if(field == null)
        return false;

      PdfArray fieldObjects;
      {
        PdfReference fieldParentReference = (PdfReference)field.BaseDataObject[PdfName.Parent];
        if(fieldParentReference == null)
        {fieldObjects = BaseDataObject;}
        else
        {fieldObjects = (PdfArray)((PdfDictionary)fieldParentReference.DataObject).Resolve(PdfName.Kids);}
      }
      return fieldObjects.Remove(field.BaseObject);
    }

    public Field this[
      string key
      ]
    {
      get
      {
        /*
          TODO: It is possible for different field dictionaries to have the SAME fully qualified field
          name if they are descendants of a common ancestor with that name and have no
          partial field names (T entries) of their own. Such field dictionaries are different
          representations of the same underlying field; they should differ only in properties
          that specify their visual appearance. In particular, field dictionaries with the same
          fully qualified field name must have the same field type (FT), value (V), and default
          value (DV).
         */
        PdfReference valueFieldReference = null;
        {
          IEnumerator partialNamesIterator = key.Split('.').GetEnumerator();
          IEnumerator<PdfDirectObject> fieldObjectsIterator = BaseDataObject.GetEnumerator();
          while(partialNamesIterator.MoveNext())
          {
            string partialName = (string)partialNamesIterator.Current;
            valueFieldReference = null;
            while(fieldObjectsIterator != null
              && fieldObjectsIterator.MoveNext())
            {
              PdfReference fieldReference = (PdfReference)fieldObjectsIterator.Current;
              PdfDictionary fieldDictionary = (PdfDictionary)fieldReference.DataObject;
              PdfTextString fieldName = (PdfTextString)fieldDictionary[PdfName.T];
              if(fieldName != null && fieldName.Value.Equals(partialName))
              {
                valueFieldReference = fieldReference;
                PdfArray kidFieldObjects = (PdfArray)fieldDictionary.Resolve(PdfName.Kids);
                fieldObjectsIterator = (kidFieldObjects == null ? null : kidFieldObjects.GetEnumerator());
                break;
              }
            }
            if(valueFieldReference == null)
              break;
          }
        }
        return Field.Wrap(valueFieldReference);
      }
      set
      {
        throw new NotImplementedException();
    /*
    TODO:put the field into the correct position, based on the full name (key)!!!
    */
      }
    }

    public bool TryGetValue(
      string key,
      out Field value
      )
    {
      value = this[key];
      return (value != null
        || ContainsKey(key));
    }

    public ICollection<Field> Values
    {
      get
      {
        IList<Field> values = new List<Field>();
        RetrieveValues(BaseDataObject, values);
        return values;
      }
    }

    #region ICollection
    void ICollection<KeyValuePair<string,Field>>.Add(
      KeyValuePair<string,Field> entry
      )
    {Add(entry.Key,entry.Value);}

    public void Clear(
      )
    {BaseDataObject.Clear();}

    bool ICollection<KeyValuePair<string,Field>>.Contains(
      KeyValuePair<string,Field> entry
      )
    {throw new NotImplementedException();}

    public void CopyTo(
      KeyValuePair<string,Field>[] entries,
      int index
      )
    {throw new NotImplementedException();}

    public int Count
    {
      get
      {return Values.Count;}
    }

    public bool IsReadOnly
    {
      get
      {return false;}
    }

    public bool Remove(
      KeyValuePair<string,Field> entry
      )
    {throw new NotImplementedException();}

    #region IEnumerable<KeyValuePair<string,Field>>
    IEnumerator<KeyValuePair<string,Field>> IEnumerable<KeyValuePair<string,Field>>.GetEnumerator(
      )
    {throw new NotImplementedException();}

    #region IEnumerable
    IEnumerator IEnumerable.GetEnumerator(
      )
    {return ((IEnumerable<KeyValuePair<string,Field>>)this).GetEnumerator();}
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion

    #region private
    private void RetrieveValues(
      PdfArray fieldObjects,
      IList<Field> values
      )
    {
      foreach(PdfDirectObject fieldObject in fieldObjects)
      {
        PdfReference fieldReference = (PdfReference)fieldObject;
        PdfArray kidReferences = (PdfArray)((PdfDictionary)fieldReference.DataObject).Resolve(PdfName.Kids);
        PdfDictionary kidObject;
        if(kidReferences == null)
        {kidObject = null;}
        else
        {kidObject = (PdfDictionary)((PdfReference)kidReferences[0]).DataObject;}
        // Terminal field?
        if(kidObject == null // Merged single widget annotation.
          || (!kidObject.ContainsKey(PdfName.FT) // Multiple widget annotations.
            && kidObject.ContainsKey(PdfName.Subtype)
            && kidObject[PdfName.Subtype].Equals(PdfName.Widget)))
        {values.Add(Field.Wrap(fieldReference));}
        else // Non-terminal field.
        {RetrieveValues(kidReferences, values);}
      }
    }
    #endregion
    #endregion
    #endregion
  }
}