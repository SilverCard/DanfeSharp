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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace org.pdfclown.documents.interchange.metadata
{
  /**
    <summary>Document information [PDF:1.6:10.2.1].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class Information
    : PdfObjectWrapper<PdfDictionary>,
      IDictionary<PdfName,object>
  {
    #region static
    #region interface
    #region public
    public static Information Wrap(
      PdfDirectObject baseObject
      )
    {return baseObject != null ? new Information(baseObject) : null;}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public Information(
      Document context
      ) : base(context, new PdfDictionary())
    {}

    private Information(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    public string Author
    {
      get
      {return (string)this[PdfName.Author];}
      set
      {this[PdfName.Author] = value;}
    }

    public DateTime? CreationDate
    {
      get
      {return (DateTime?)this[PdfName.CreationDate];}
      set
      {this[PdfName.CreationDate] = value;}
    }

    public string Creator
    {
      get
      {return (string)this[PdfName.Creator];}
      set
      {this[PdfName.Creator] = value;}
    }

    [PDF(VersionEnum.PDF11)]
    public string Keywords
    {
      get
      {return (string)this[PdfName.Keywords];}
      set
      {this[PdfName.Keywords] = value;}
    }

    [PDF(VersionEnum.PDF11)]
    public DateTime? ModificationDate
    {
      get
      {return (DateTime?)this[PdfName.ModDate];}
      set
      {this[PdfName.ModDate] = value;}
    }

    public string Producer
    {
      get
      {return (string)this[PdfName.Producer];}
      set
      {this[PdfName.Producer] = value;}
    }

    [PDF(VersionEnum.PDF11)]
    public string Subject
    {
      get
      {return (string)this[PdfName.Subject];}
      set
      {this[PdfName.Subject] = value;}
    }

    [PDF(VersionEnum.PDF11)]
    public string Title
    {
      get
      {return (string)this[PdfName.Title];}
      set
      {this[PdfName.Title] = value;}
    }

    #region IDictionary
    public void Add(
      PdfName key,
      object value
      )
    {
      OnChange(key);
      BaseDataObject.Add(key, PdfSimpleObject<object>.Get(value));
    }

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
    {
      OnChange(key);
      return BaseDataObject.Remove(key);
    }

    public object this[
      PdfName key
      ]
    {
      get
      {return PdfSimpleObject<object>.GetValue(BaseDataObject[key]);}
      set
      {
        OnChange(key);
        BaseDataObject[key] = PdfSimpleObject<object>.Get(value);
      }
    }

    public bool TryGetValue(
      PdfName key,
      out object value
      )
    {
      PdfDirectObject valueObject;
      if(BaseDataObject.TryGetValue(key, out valueObject))
      {
        value = PdfSimpleObject<object>.GetValue(valueObject);
        return true;
      }
      else
        value = null;
        return false;
    }

    public ICollection<object> Values
    {
      get
      {
        IList<object> values = new List<object>();
        foreach(PdfDirectObject item in BaseDataObject.Values)
        {values.Add(PdfSimpleObject<object>.GetValue(item));}
        return values;
      }
    }

    #region ICollection
    void ICollection<KeyValuePair<PdfName,object>>.Add(
      KeyValuePair<PdfName,object> entry
      )
    {Add(entry.Key,entry.Value);}

    public void Clear(
      )
    {
      BaseDataObject.Clear();
      ModificationDate = DateTime.Now;
    }

    bool ICollection<KeyValuePair<PdfName,object>>.Contains(
      KeyValuePair<PdfName,object> entry
      )
    {return entry.Value.Equals(this[entry.Key]);}

    public void CopyTo(
      KeyValuePair<PdfName,object>[] entries,
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
      KeyValuePair<PdfName,object> entry
      )
    {throw new NotImplementedException();}

    #region IEnumerable<KeyValuePair<PdfName,object>>
    IEnumerator<KeyValuePair<PdfName,object>> IEnumerable<KeyValuePair<PdfName,object>>.GetEnumerator(
      )
    {
      foreach(KeyValuePair<PdfName,PdfDirectObject> entry in BaseDataObject)
      {
        yield return new KeyValuePair<PdfName,object>(
          entry.Key,
          PdfSimpleObject<object>.GetValue(entry.Value)
          );
      }
    }

    #region IEnumerable
    IEnumerator IEnumerable.GetEnumerator(
      )
    {return ((IEnumerable<KeyValuePair<PdfName,object>>)this).GetEnumerator();}
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion

    #region private
    //TODO: Listen to baseDataObject's onChange notification?
    private void OnChange(
      PdfName key
      )
    {
      if(!BaseDataObject.Updated && !PdfName.ModDate.Equals(key))
      {ModificationDate = DateTime.Now;}
    }
    #endregion
    #endregion
    #endregion
  }
}