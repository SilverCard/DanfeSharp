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
using org.pdfclown.objects;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace org.pdfclown.documents.files
{
  /**
    <summary>Embedded files referenced by another one (dependencies) [PDF:1.6:3.10.3].</summary>
  */
  [PDF(VersionEnum.PDF13)]
  public sealed class RelatedFiles
    : PdfObjectWrapper<PdfArray>,
      IDictionary<string,EmbeddedFile>
  {
    #region static
    #region interface
    #region public
    public static RelatedFiles Wrap(
      PdfDirectObject baseObject
      )
    {return baseObject != null ? new RelatedFiles(baseObject) : null;}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public RelatedFiles(
      Document context
      ) : base(context, new PdfArray())
    {}

    private RelatedFiles(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    #region IDictionary
    public void Add(
      string key,
      EmbeddedFile value
      )
    {
      PdfArray itemPairs = BaseDataObject;
      // New entry.
      itemPairs.Add(new PdfTextString(key));
      itemPairs.Add(value.BaseObject);
    }

    public bool ContainsKey(
      string key
      )
    {
      PdfArray itemPairs = BaseDataObject;
      for(
        int index = 0,
          length = itemPairs.Count;
        index < length;
        index += 2
        )
      {
        if(((PdfTextString)itemPairs[index]).Value.Equals(key))
          return true;
      }
      return false;
    }

    public ICollection<string> Keys
    {
      get
      {
        List<string> keys = new List<string>();
        PdfArray itemPairs = BaseDataObject;
        for(
          int index = 0,
            length = itemPairs.Count;
          index < length;
          index += 2
          )
        {keys.Add((string)((PdfTextString)itemPairs[index]).Value);}
        return keys;
      }
    }

    public bool Remove(
      string key
      )
    {
      PdfArray itemPairs = BaseDataObject;
      for(
        int index = 0,
          length = itemPairs.Count;
        index < length;
        index += 2
        )
      {
        if(((PdfTextString)itemPairs[index]).Value.Equals(key))
        {
          itemPairs.RemoveAt(index); // Key removed.
          itemPairs.RemoveAt(index); // Value removed.
          return true;
        }
      }
      return false;
    }

    public EmbeddedFile this[
      string key
      ]
    {
      get
      {
        PdfArray itemPairs = BaseDataObject;
        for(
          int index = 0,
            length = itemPairs.Count;
          index < length;
          index += 2
          )
        {
          if(((PdfTextString)itemPairs[index]).Value.Equals(key))
            return EmbeddedFile.Wrap(itemPairs[index+1]);
        }
        return null;
      }
      set
      {
        PdfArray itemPairs = BaseDataObject;
        for(
          int index = 0,
            length = itemPairs.Count;
          index < length;
          index += 2
          )
        {
          // Already existing entry?
          if(((PdfTextString)itemPairs[index]).Value.Equals(key))
          {
            itemPairs[index+1] = value.BaseObject;
            return;
          }
        }
        // New entry.
        itemPairs.Add(new PdfTextString(key));
        itemPairs.Add(value.BaseObject);
      }
    }

    public bool TryGetValue(
      string key,
      out EmbeddedFile value
      )
    {
      value = this[key];
      if(value == null)
        return ContainsKey(key);
      else
        return true;
    }

    public ICollection<EmbeddedFile> Values
    {
      get
      {
        List<EmbeddedFile> values = new List<EmbeddedFile>();
        PdfArray itemPairs = BaseDataObject;
        for(
          int index = 1,
            length = itemPairs.Count;
          index < length;
          index += 2
          )
        {values.Add(EmbeddedFile.Wrap(itemPairs[index]));}
        return values;
      }
    }

    #region ICollection
    void ICollection<KeyValuePair<string,EmbeddedFile>>.Add(
      KeyValuePair<string,EmbeddedFile> entry
      )
    {Add(entry.Key,entry.Value);}

    public void Clear(
      )
    {BaseDataObject.Clear();}

    bool ICollection<KeyValuePair<string,EmbeddedFile>>.Contains(
      KeyValuePair<string,EmbeddedFile> entry
      )
    {return entry.Value.Equals(this[entry.Key]);}

    public void CopyTo(
      KeyValuePair<string,EmbeddedFile>[] entries,
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
      KeyValuePair<string,EmbeddedFile> entry
      )
    {throw new NotImplementedException();}

    #region IEnumerable<KeyValuePair<string,EmbeddedFile>>
    IEnumerator<KeyValuePair<string,EmbeddedFile>> IEnumerable<KeyValuePair<string,EmbeddedFile>>.GetEnumerator(
      )
    {
      PdfArray itemPairs = BaseDataObject;
      for(
        int index = 0,
          length = itemPairs.Count;
        index < length;
        index += 2
        )
      {
        yield return new KeyValuePair<string,EmbeddedFile>(
          (string)((PdfTextString)itemPairs[index]).Value,
          EmbeddedFile.Wrap(itemPairs[index+1])
          );
      }
    }

    #region IEnumerable
    IEnumerator IEnumerable.GetEnumerator(
      )
    {return ((IEnumerable<KeyValuePair<string,EmbeddedFile>>)this).GetEnumerator();}
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion
  }
}