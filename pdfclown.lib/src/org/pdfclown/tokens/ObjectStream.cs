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

using org.pdfclown.bytes;
using org.pdfclown.files;
using org.pdfclown.objects;

using System;
using System.Collections;
using System.Collections.Generic;

namespace org.pdfclown.tokens
{
  /**
    <summary>Object stream containing a sequence of PDF objects [PDF:1.6:3.4.6].</summary>
    <remarks>The purpose of object streams is to allow a greater number of PDF objects
    to be compressed, thereby substantially reducing the size of PDF files.
    The objects in the stream are referred to as compressed objects.</remarks>
  */
  public sealed class ObjectStream
    : PdfStream,
      IDictionary<int,PdfDataObject>, IDisposable
  {
    #region types
    private sealed class ObjectEntry
    {
      internal PdfDataObject dataObject;
      internal int offset;

      private FileParser parser;

      private ObjectEntry(
        FileParser parser
        )
      {this.parser = parser;}

      public ObjectEntry(
        int offset,
        FileParser parser
        ) : this(parser)
      {
        this.dataObject = null;
        this.offset = offset;
      }

      public ObjectEntry(
        PdfDataObject dataObject,
        FileParser parser
        ) : this(parser)
      {
        this.dataObject = dataObject;
        this.offset = -1; // Undefined -- to set on stream serialization.
      }

      public PdfDataObject DataObject
      {
        get
        {
          if(dataObject == null)
          {
            parser.Seek(offset); parser.MoveNext();
            dataObject = parser.ParsePdfObject();
          }
          return dataObject;
        }
      }
    }
    #endregion

    #region dynamic
    #region fields
    /**
      <summary>Compressed objects map.</summary>
      <remarks>This map is initially populated with offset values;
      when a compressed object is required, its offset is used to retrieve it.</remarks>
    */
    private IDictionary<int,ObjectEntry> entries;
    private FileParser parser;
    #endregion

    #region constructors
    public ObjectStream(
      ) : base(new PdfDictionary(new PdfName[]{PdfName.Type}, new PdfDirectObject[]{PdfName.ObjStm}))
    {}

    public ObjectStream(
      PdfDictionary header,
      IBuffer body
      ) : base(header, body)
    {}
    #endregion

    #region interface
    #region public
    public override PdfObject Accept(
      IVisitor visitor,
      object data
      )
    {return visitor.Visit(this, data);}

    /**
      <summary>Gets/Sets the object stream extended by this one.</summary>
      <remarks>Both streams are considered part of a collection of object streams  whose links form
      a directed acyclic graph.</remarks>
    */
    public ObjectStream BaseStream
    {
      get
      {return (ObjectStream)Header.Resolve(PdfName.Extends);}
      set
      {Header[PdfName.Extends] = value.Reference;}
    }

    public override void WriteTo(
      IOutputStream stream,
      File context
      )
    {
      if(entries != null)
      {Flush(stream);}

      base.WriteTo(stream, context);
    }

    #region IDictionary
    public void Add(
      int key,
      PdfDataObject value
      )
    {Entries.Add(key, new ObjectEntry(value, parser));}

    public bool ContainsKey(
      int key
      )
    {return Entries.ContainsKey(key);}

    public ICollection<int> Keys
    {
      get
      {return Entries.Keys;}
    }

    public bool Remove(
      int key
      )
    {return Entries.Remove(key);}

    public PdfDataObject this[
      int key
      ]
    {
      get
      {
        ObjectEntry entry = Entries[key];
        return (entry != null ? entry.DataObject : null);
      }
      set
      {Entries[key] = new ObjectEntry(value, parser);}
    }

    public bool TryGetValue(
      int key,
      out PdfDataObject value
      )
    {
      value = this[key];
      return (value != null
        || ContainsKey(key));
    }

    public ICollection<PdfDataObject> Values
    {
      get
      {
        IList<PdfDataObject> values = new List<PdfDataObject>();
        foreach(int key in Entries.Keys)
        {values.Add(this[key]);}
        return values;
      }
    }

    #region ICollection
    void ICollection<KeyValuePair<int,PdfDataObject>>.Add(
      KeyValuePair<int,PdfDataObject> entry
      )
    {Add(entry.Key, entry.Value);}

    public void Clear(
      )
    {
      if(entries == null)
      {entries = new Dictionary<int,ObjectEntry>();}
      else
      {entries.Clear();}
    }

    bool ICollection<KeyValuePair<int,PdfDataObject>>.Contains(
      KeyValuePair<int,PdfDataObject> entry
      )
    {return ((ICollection<KeyValuePair<int,PdfDataObject>>)Entries).Contains(entry);}

    public void CopyTo(
      KeyValuePair<int,PdfDataObject>[] entries,
      int index
      )
    {throw new NotImplementedException();}

    public int Count
    {
      get
      {return Entries.Count;}
    }

    public bool IsReadOnly
    {
      get
      {return false;}
    }

    public bool Remove(
      KeyValuePair<int,PdfDataObject> entry
      )
    {
      PdfDataObject value;
      if(TryGetValue(entry.Key, out value)
        && value.Equals(entry.Value))
        return Entries.Remove(entry.Key);
      else
        return false;
    }

    #region IEnumerable<KeyValuePair<int,PdfDataObject>>
    IEnumerator<KeyValuePair<int,PdfDataObject>> IEnumerable<KeyValuePair<int,PdfDataObject>>.GetEnumerator(
      )
    {
      foreach(int key in Keys)
      {yield return new KeyValuePair<int,PdfDataObject>(key,this[key]);}
    }

    #region IEnumerable
    IEnumerator IEnumerable.GetEnumerator(
      )
    {return ((IEnumerable<KeyValuePair<int,PdfDataObject>>)this).GetEnumerator();}
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion

    #region private
    private IDictionary<int,ObjectEntry> Entries
    {
      get
      {
        if(entries == null)
        {
          entries = new Dictionary<int,ObjectEntry>();

          IBuffer body = Body;
          if(body.Length > 0)
          {
            parser = new FileParser(Body, File);
            int baseOffset = ((PdfInteger)Header[PdfName.First]).IntValue;
            for(
              int index = 0,
                length = ((PdfInteger)Header[PdfName.N]).IntValue;
              index < length;
              index++
              )
            {
              int objectNumber = ((PdfInteger)parser.ParsePdfObject(1)).IntValue;
              int objectOffset = baseOffset + ((PdfInteger)parser.ParsePdfObject(1)).IntValue;
              entries[objectNumber] = new ObjectEntry(objectOffset, parser);
            }
          }
        }
        return entries;
      }
    }

    /**
      <summary>Serializes the object stream entries into the stream body.</summary>
    */
    private void Flush(
      IOutputStream stream
      )
    {
      // 1. Body.
      int dataByteOffset;
      {
        // Serializing the entries into the stream buffer...
        IBuffer indexBuffer = new bytes.Buffer();
        IBuffer dataBuffer = new bytes.Buffer();
        IndirectObjects indirectObjects = File.IndirectObjects;
        int objectIndex = -1;
        File context = File;
        foreach(KeyValuePair<int,ObjectEntry> entry in Entries)
        {
          int objectNumber = entry.Key;

          // Update the xref entry!
          XRefEntry xrefEntry = indirectObjects[objectNumber].XrefEntry;
          xrefEntry.Offset = ++objectIndex;

          /*
            NOTE: The entry offset MUST be updated only after its serialization, in order not to
            interfere with its possible data-object retrieval from the old serialization.
          */
          int entryValueOffset = (int)dataBuffer.Length;

          // Index.
          indexBuffer
            .Append(objectNumber.ToString()).Append(Chunk.Space) // Object number.
            .Append(entryValueOffset.ToString()).Append(Chunk.Space); // Byte offset (relative to the first one).

          // Data.
          entry.Value.DataObject.WriteTo(dataBuffer, context);
          entry.Value.offset = entryValueOffset;
        }

        // Get the stream buffer!
        IBuffer body = Body;

        // Delete the old entries!
        body.SetLength(0);

        // Add the new entries!
        body.Append(indexBuffer);
        dataByteOffset = (int)body.Length;
        body.Append(dataBuffer);
      }

      // 2. Header.
      {
        PdfDictionary header = Header;
        header[PdfName.N] = PdfInteger.Get(Entries.Count);
        header[PdfName.First] = PdfInteger.Get(dataByteOffset);
      }
    }
    #endregion
    #endregion
    #endregion

    public void Dispose()
    {
        if (parser != null) parser.Dispose();
    }
  }
}