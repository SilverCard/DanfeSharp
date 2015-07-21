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

using org.pdfclown.bytes;
using org.pdfclown.files;
using org.pdfclown.tokens;

using System;
using System.Collections;
using System.Collections.Generic;
using text = System.Text;

namespace org.pdfclown.objects
{
  /**
    <summary>PDF dictionary object [PDF:1.6:3.2.6].</summary>
  */
  public sealed class PdfDictionary
    : PdfDirectObject,
      IDictionary<PdfName,PdfDirectObject>
  {
    #region static
    #region fields
    private static readonly byte[] BeginDictionaryChunk = Encoding.Pdf.Encode(Keyword.BeginDictionary);
    private static readonly byte[] EndDictionaryChunk = Encoding.Pdf.Encode(Keyword.EndDictionary);
    #endregion
    #endregion

    #region dynamic
    #region fields
    internal IDictionary<PdfName,PdfDirectObject> entries;

    private PdfObject parent;
    private bool updateable = true;
    private bool updated;
    private bool virtual_;
    #endregion

    #region constructors
    public PdfDictionary(
      )
    {entries = new Dictionary<PdfName,PdfDirectObject>();}

    public PdfDictionary(
      int capacity
      )
    {entries = new Dictionary<PdfName,PdfDirectObject>(capacity);}

    public PdfDictionary(
      PdfName[] keys,
      PdfDirectObject[] values
      ) : this(values.Length)
    {
      Updateable = false;
      for(
        int index = 0;
        index < values.Length;
        index++
        )
      {this[keys[index]] = values[index];}
      Updateable = true;
    }

    public PdfDictionary(
      IDictionary<PdfName,PdfDirectObject> entries
      ) : this(entries.Count)
    {
      Updateable = false;
      foreach(KeyValuePair<PdfName,PdfDirectObject> entry in entries)
      {this[entry.Key] = (PdfDirectObject)Include(entry.Value);}
      Updateable = true;
    }
    #endregion

    #region interface
    #region public
    public override PdfObject Accept(
      IVisitor visitor,
      object data
      )
    {return visitor.Visit(this, data);}

    public override int CompareTo(
      PdfDirectObject obj
      )
    {throw new NotImplementedException();}

    /**
      <summary>Gets the value corresponding to the given key, forcing its instantiation as a direct
      object in case of missing entry.</summary>
      <param name="key">Key whose associated value is to be returned.</param>
    */
    public PdfDirectObject Get<T>(
      PdfName key
      ) where T : PdfDataObject, new()
    {return Get<T>(key, true);}

    /**
      <summary>Gets the value corresponding to the given key, forcing its instantiation in case of
      missing entry.</summary>
      <param name="key">Key whose associated value is to be returned.</param>
      <param name="direct">Whether the item has to be instantiated directly within its container
      instead of being referenced through an indirect object.</param>
    */
    public PdfDirectObject Get<T>(
      PdfName key,
      bool direct
      ) where T : PdfDataObject, new()
    {
      PdfDirectObject value = this[key];
      if(value == null)
      {
        /*
          NOTE: The null-object placeholder MUST NOT perturb the existing structure; therefore:
            - it MUST be marked as virtual in order not to unnecessarily serialize it;
            - it MUST be put into this dictionary without affecting its update status.
        */
        try
        {
          value = (PdfDirectObject)Include(direct
            ? (PdfDataObject)new T()
            : new PdfIndirectObject(File, new T(), new XRefEntry(0, 0)).Reference);
          entries[key] = value;
          value.Virtual = true;
        }
        catch(Exception e)
        {throw new Exception(typeof(T).Name + " failed to instantiate.", e);}
      }
      return value;
    }

    public override bool Equals(
      object @object
      )
    {
      return base.Equals(@object)
        || (@object != null
          && @object.GetType().Equals(GetType())
          && ((PdfDictionary)@object).entries.Equals(entries));
    }

    public override int GetHashCode(
      )
    {return entries.GetHashCode();}

    /**
      Gets the key associated to the specified value.
    */
    public PdfName GetKey(
      PdfDirectObject value
      )
    {
      /*
        NOTE: Current PdfDictionary implementation doesn't support bidirectional maps, to say that
        the only currently-available way to retrieve a key from a value is to iterate the whole map
        (really poor performance!).
      */
      foreach(KeyValuePair<PdfName,PdfDirectObject> entry in entries)
      {
        if(entry.Value.Equals(value))
          return entry.Key;
      }
      return null;
    }

    public override PdfObject Parent
    {
      get
      {return parent;}
      internal set
      {parent = value;}
    }

    /**
      <summary>Gets the dereferenced value corresponding to the given key.</summary>
      <remarks>This method takes care to resolve the value returned by <see cref="this[PdfName]">
      this[PdfName]</see>.</remarks>
      <param name="key">Key whose associated value is to be returned.</param>
      <returns>null, if the map contains no mapping for this key.</returns>
    */
    public PdfDataObject Resolve(
      PdfName key
      )
    {return Resolve(this[key]);}

    /**
      <summary>Gets the dereferenced value corresponding to the given key, forcing its instantiation
      in case of missing entry.</summary>
      <remarks>This method takes care to resolve the value returned by <see cref="Get(PdfName)"/>.
      </remarks>
      <param name="key">Key whose associated value is to be returned.</param>
      <returns>null, if the map contains no mapping for this key.</returns>
    */
    public T Resolve<T>(
      PdfName key
      ) where T : PdfDataObject, new()
    {return (T)Resolve(Get<T>(key));}

    public override PdfObject Swap(
      PdfObject other
      )
    {
      PdfDictionary otherDictionary = (PdfDictionary)other;
      IDictionary<PdfName,PdfDirectObject> otherEntries = otherDictionary.entries;
      // Update the other!
      otherDictionary.entries = this.entries;
      otherDictionary.Update();
      // Update this one!
      this.entries = otherEntries;
      this.Update();
      return this;
    }

    public override string ToString(
      )
    {
      text::StringBuilder buffer = new text::StringBuilder();
      {
        // Begin.
        buffer.Append("<< ");
        // Entries.
        foreach(KeyValuePair<PdfName,PdfDirectObject> entry in entries)
        {
          // Entry...
          // ...key.
          buffer.Append(entry.Key.ToString()).Append(" ");
          // ...value.
          buffer.Append(PdfDirectObject.ToString(entry.Value)).Append(" ");
        }
        // End.
        buffer.Append(">>");
      }
      return buffer.ToString();
    }

    public override bool Updateable
    {
      get
      {return updateable;}
      set
      {updateable = value;}
    }

    public override bool Updated
    {
      get
      {return updated;}
      protected internal set
      {updated = value;}
    }

    public override void WriteTo(
      IOutputStream stream,
      File context
      )
    {
      // Begin.
      stream.Write(BeginDictionaryChunk);
      // Entries.
      foreach(KeyValuePair<PdfName,PdfDirectObject> entry in entries)
      {
        PdfDirectObject value = entry.Value;
        if(value != null && value.Virtual)
          continue;

        // Entry...
        // ...key.
        entry.Key.WriteTo(stream, context); stream.Write(Chunk.Space);
        // ...value.
        PdfDirectObject.WriteTo(stream, context, value); stream.Write(Chunk.Space);
      }
      // End.
      stream.Write(EndDictionaryChunk);
    }

    #region IDictionary
    public void Add(
      PdfName key,
      PdfDirectObject value
      )
    {
      entries.Add(key, (PdfDirectObject)Include(value));
      Update();
    }

    public bool ContainsKey(
      PdfName key
      )
    {return entries.ContainsKey(key);}

    public ICollection<PdfName> Keys
    {
      get
      {return entries.Keys;}
    }

    public bool Remove(
      PdfName key
      )
    {
      PdfDirectObject oldValue = this[key];
      if(entries.Remove(key))
      {
        Exclude(oldValue);
        Update();
        return true;
      }
      return false;
    }

    public PdfDirectObject this[
      PdfName key
      ]
    {
      get
      {
        /*
          NOTE: This is an intentional violation of the official .NET Framework Class
          Library prescription (no exception is thrown anytime a key is not found --
          a null pointer is returned instead).
        */
        PdfDirectObject value;
        entries.TryGetValue(key,out value);
        return value;
      }
      set
      {
        if(value == null)
        {Remove(key);}
        else
        {
          PdfDirectObject oldValue = this[key];
          entries[key] = (PdfDirectObject)Include(value);
          Exclude(oldValue);
          Update();
        }
      }
    }

    public bool TryGetValue(
      PdfName key,
      out PdfDirectObject value
      )
    {return entries.TryGetValue(key,out value);}

    public ICollection<PdfDirectObject> Values
    {
      get
      {return entries.Values;}
    }

    #region ICollection
    void ICollection<KeyValuePair<PdfName,PdfDirectObject>>.Add(
      KeyValuePair<PdfName,PdfDirectObject> entry
      )
    {Add(entry.Key, entry.Value);}

    public void Clear(
      )
    {
      foreach(PdfName key in new List<PdfDirectObject>(entries.Keys))
      {Remove(key);}
    }

    bool ICollection<KeyValuePair<PdfName,PdfDirectObject>>.Contains(
      KeyValuePair<PdfName,PdfDirectObject> entry
      )
    {return ((ICollection<KeyValuePair<PdfName,PdfDirectObject>>)entries).Contains(entry);}

    public void CopyTo(
      KeyValuePair<PdfName,PdfDirectObject>[] entries,
      int index
      )
    {throw new NotImplementedException();}

    public int Count
    {
      get
      {return entries.Count;}
    }

    public bool IsReadOnly
    {
      get
      {return false;}
    }

    public bool Remove(
      KeyValuePair<PdfName,PdfDirectObject> entry
      )
    {
      if(entry.Value.Equals(this[entry.Key]))
        return Remove(entry.Key);
      else
        return false;
    }

    #region IEnumerable<KeyValuePair<PdfName,PdfDirectObject>>
    IEnumerator<KeyValuePair<PdfName,PdfDirectObject>> IEnumerable<KeyValuePair<PdfName,PdfDirectObject>>.GetEnumerator(
      )
    {return entries.GetEnumerator();}

    #region IEnumerable
    IEnumerator IEnumerable.GetEnumerator(
      )
    {return ((IEnumerable<KeyValuePair<PdfName,PdfDirectObject>>)this).GetEnumerator();}
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion

    #region protected
    protected internal override bool Virtual
    {
      get
      {return virtual_;}
      set
      {virtual_ = value;}
    }
    #endregion
    #endregion
    #endregion
  }
}