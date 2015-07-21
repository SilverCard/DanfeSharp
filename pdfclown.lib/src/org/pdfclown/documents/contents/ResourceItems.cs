/*
  Copyright 2010-2012 Stefano Chizzolini. http://www.pdfclown.org

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
using System.Collections;
using System.Collections.Generic;

namespace org.pdfclown.documents.contents
{
  /**
    <summary>Collection of a specific resource type.</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public abstract class ResourceItems<TValue>
    : PdfObjectWrapper<PdfDictionary>,
      IDictionary<PdfName,TValue>
    where TValue : PdfObjectWrapper
  {
    #region dynamic
    #region constructors
    protected ResourceItems(
      Document context
      ) : base(context, new PdfDictionary())
    {}

    internal ResourceItems(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      Gets the key associated to a given value.
    */
    public PdfName GetKey(
      TValue value
      )
    {return BaseDataObject.GetKey(value.BaseObject);}

    #region IDictionary
    public void Add(
      PdfName key,
      TValue value
      )
    {BaseDataObject.Add(key,value.BaseObject);}

    public bool ContainsKey(
      PdfName key
      )
    {return BaseDataObject.ContainsKey(key);}

    public ICollection<PdfName> Keys
    {
      get
      {return BaseDataObject.Keys;}
    }

    public bool Remove(
      PdfName key
      )
    {return BaseDataObject.Remove(key);}

    public TValue this[
      PdfName key
      ]
    {
      get
      {return Wrap(BaseDataObject[key]);}
      set
      {BaseDataObject[key] = value.BaseObject;}
    }

    public bool TryGetValue(
      PdfName key,
      out TValue value
      )
    {return ((value = this[key]) != null || ContainsKey(key));}

    public ICollection<TValue> Values
    {
      get
      {
        ICollection<TValue> values;
        {
          // Get the low-level objects!
          ICollection<PdfDirectObject> valueObjects = BaseDataObject.Values;
          // Populating the high-level collection...
          values = new List<TValue>(valueObjects.Count);
          foreach(PdfDirectObject valueObject in valueObjects)
          {values.Add(Wrap(valueObject));}
        }
        return values;
      }
    }

    #region ICollection
    void ICollection<KeyValuePair<PdfName,TValue>>.Add(
      KeyValuePair<PdfName,TValue> entry
      )
    {Add(entry.Key,entry.Value);}

    public void Clear(
      )
    {BaseDataObject.Clear();}

    bool ICollection<KeyValuePair<PdfName,TValue>>.Contains(
      KeyValuePair<PdfName,TValue> entry
      )
    {return entry.Value.BaseObject.Equals(BaseDataObject[entry.Key]);}

    public void CopyTo(
      KeyValuePair<PdfName,TValue>[] entries,
      int index
      )
    {throw new NotImplementedException();}

    public int Count
    {
      get
      {return BaseDataObject.Count;}
    }

    public bool IsReadOnly
    {
      get
      {return false;}
    }

    public bool Remove(
      KeyValuePair<PdfName,TValue> entry
      )
    {
      return BaseDataObject.Remove(
        new KeyValuePair<PdfName,PdfDirectObject>(
          entry.Key,
          entry.Value.BaseObject
          )
        );
    }

    #region IEnumerable<KeyValuePair<PdfName,TValue>>
    IEnumerator<KeyValuePair<PdfName,TValue>> IEnumerable<KeyValuePair<PdfName,TValue>>.GetEnumerator(
      )
    {
      foreach(PdfName key in Keys)
      {yield return new KeyValuePair<PdfName,TValue>(key,this[key]);}
    }

    #region IEnumerable
    IEnumerator IEnumerable.GetEnumerator(
      )
    {return ((IEnumerable<KeyValuePair<PdfName,TValue>>)this).GetEnumerator();}
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion

    #region protected
    /**
      <summary>Wraps a base object within its corresponding high-level representation.</summary>
    */
    protected abstract TValue Wrap(
      PdfDirectObject baseObject
      );
    #endregion
    #endregion
    #endregion
  }
}