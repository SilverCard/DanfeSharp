/*
  Copyright 2007-2012 Stefano Chizzolini. http://www.pdfclown.org

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

using bytes = org.pdfclown.bytes;
using org.pdfclown.documents.contents.objects;
using org.pdfclown.documents.contents.tokens;
using org.pdfclown.files;
using org.pdfclown.objects;
using org.pdfclown.util.io;

using System;
using System.Collections;
using System.Collections.Generic;

namespace org.pdfclown.documents.contents
{
  /**
    <summary>Content stream [PDF:1.6:3.7.1].</summary>
    <remarks>During its loading, this content stream is parsed and its instructions
    are exposed as a list; in case of modifications, it's user responsability
    to call the <see cref="Flush()"/> method in order to serialize back the instructions
    into this content stream.</remarks>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class Contents
    : PdfObjectWrapper<PdfDataObject>,
      IList<ContentObject>
  {
    #region types
    /**
      <summary>Content stream wrapper.</summary>
    */
    private class ContentStream
      : bytes::IInputStream
    {
      private readonly PdfDataObject baseDataObject;

      private long basePosition;
      private bytes::IInputStream stream;
      private int streamIndex = -1;

      public ContentStream(
        PdfDataObject baseDataObject
        )
      {
        this.baseDataObject = baseDataObject;
        MoveNextStream();
      }

      public ByteOrderEnum ByteOrder
      {
        get
        {return stream.ByteOrder;}
        set
        {throw new NotSupportedException();}
      }

      public void Dispose(
        )
      {/* NOOP */}

      public long Length
      {
        get
        {
          if(baseDataObject is PdfStream) // Single stream.
            return ((PdfStream)baseDataObject).Body.Length;
          else // Array of streams.
          {
            long length = 0;
            foreach(PdfDirectObject stream in (PdfArray)baseDataObject)
            {length += ((PdfStream)((PdfReference)stream).DataObject).Body.Length;}
            return length;
          }
        }
      }

      public long Position
      {
        get
        {return basePosition + stream.Position;}
        set
        {Seek(value);}
      }

      public void Read(
        byte[] data
        )
      {throw new NotImplementedException();}

      public void Read(
        byte[] data,
        int offset,
        int length
        )
      {throw new NotImplementedException();}

      public int ReadByte(
        )
      {
        if((stream == null
          || stream.Position >= stream.Length)
          && !MoveNextStream())
          return -1; //TODO:harmonize with other Read*() method EOF exceptions!!!

        return stream.ReadByte();
      }

      public int ReadInt(
        )
      {throw new NotImplementedException();}

      public int ReadInt(
        int length
        )
      {throw new NotImplementedException();}

      public string ReadLine(
        )
      {throw new NotImplementedException();}

      public short ReadShort(
        )
      {throw new NotImplementedException();}

      public sbyte ReadSignedByte(
        )
      {throw new NotImplementedException();}

      public string ReadString(
        int length
        )
      {throw new NotImplementedException();}

      public ushort ReadUnsignedShort(
        )
      {throw new NotImplementedException();}

      public void Seek(
        long position
        )
      {
        while(true)
        {
          if(position < basePosition) //Before current stream.
          {
            if(!MovePreviousStream())
              throw new ArgumentException("Lower than acceptable.","position");
          }
          else if(position > basePosition + stream.Length) // After current stream.
          {
            if(!MoveNextStream())
              throw new ArgumentException("Higher than acceptable.","position");
          }
          else // At current stream.
          {
            stream.Seek(position - basePosition);
            break;
          }
        }
      }

      public void Skip(
        long offset
        )
      {
        while(true)
        {
          long position = stream.Position + offset;
          if(position < 0) //Before current stream.
          {
            offset += stream.Position;
            if(!MovePreviousStream())
              throw new ArgumentException("Lower than acceptable.","offset");
  
            stream.Position = stream.Length;
          }
          else if(position > stream.Length) // After current stream.
          {
            offset -= (stream.Length - stream.Position);
            if(!MoveNextStream())
              throw new ArgumentException("Higher than acceptable.","offset");
          }
          else // At current stream.
          {
            stream.Seek(position);
            break;
          }
        }
      }

      public byte[] ToByteArray(
        )
      {throw new NotImplementedException();}

      private bool MoveNextStream(
        )
      {
        // Is the content stream just a single stream?
        /*
          NOTE: A content stream may be made up of multiple streams [PDF:1.6:3.6.2].
        */
        if(baseDataObject is PdfStream) // Single stream.
        {
          if(streamIndex < 1)
          {
            streamIndex++;
  
            basePosition = (streamIndex == 0
              ? 0
              : basePosition + stream.Length);
  
            stream = (streamIndex < 1
              ? ((PdfStream)baseDataObject).Body
              : null);
          }
        }
        else // Multiple streams.
        {
          PdfArray streams = (PdfArray)baseDataObject;
          if(streamIndex < streams.Count)
          {
            streamIndex++;
  
            basePosition = (streamIndex == 0
              ? 0
              : basePosition + stream.Length);
  
            stream = (streamIndex < streams.Count
              ? ((PdfStream)streams.Resolve(streamIndex)).Body
              : null);
          }
        }
        if(stream == null)
          return false;
  
        stream.Position = 0;
        return true;
      }
  
      private bool MovePreviousStream(
        )
      {
        if(streamIndex == 0)
        {
          streamIndex--;
          stream = null;
        }
        if(streamIndex == -1)
          return false;
  
        streamIndex--;
        /* NOTE: A content stream may be made up of multiple streams [PDF:1.6:3.6.2]. */
        // Is the content stream just a single stream?
        if(baseDataObject is PdfStream) // Single stream.
        {
          stream = ((PdfStream)baseDataObject).Body;
          basePosition = 0;
        }
        else // Array of streams.
        {
          PdfArray streams = (PdfArray)baseDataObject;
  
          stream = ((PdfStream)((PdfReference)streams[streamIndex]).DataObject).Body;
          basePosition -= stream.Length;
        }
  
        return true;
      }
    }
    #endregion

    #region static
    #region interface
    #region public
    public static Contents Wrap(
      PdfDirectObject baseObject,
      IContentContext contentContext
      )
    {return baseObject != null ? new Contents(baseObject, contentContext) : null;}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region fields
    private IList<ContentObject> items;

    private IContentContext contentContext;
    #endregion

    #region constructors
    private Contents(
      PdfDirectObject baseObject,
      IContentContext contentContext
      ) : base(baseObject)
    {
      this.contentContext = contentContext;
      Load();
    }
    #endregion

    #region interface
    #region public
    public override object Clone(
      Document context
      )
    {throw new NotSupportedException();}

    /**
      <summary>Serializes the contents into the content stream.</summary>
    */
    public void Flush(
      )
    {
      PdfStream stream;
      PdfDataObject baseDataObject = BaseDataObject;
      // Are contents just a single stream object?
      if(baseDataObject is PdfStream) // Single stream.
      {stream = (PdfStream)baseDataObject;}
      else // Array of streams.
      {
        PdfArray streams = (PdfArray)baseDataObject;
        // No stream available?
        if(streams.Count == 0) // No stream.
        {
          // Add first stream!
          stream = new PdfStream();
          streams.Add( // Inserts the new stream into the content stream.
            File.Register(stream) // Inserts the new stream into the file.
            );
        }
        else // Streams exist.
        {
          // Eliminating exceeding streams...
          /*
            NOTE: Applications that consume or produce PDF files are not required to preserve
            the existing structure of the Contents array [PDF:1.6:3.6.2].
          */
          while(streams.Count > 1)
          {
            File.Unregister((PdfReference)streams[1]); // Removes the exceeding stream from the file.
            streams.RemoveAt(1); // Removes the exceeding stream from the content stream.
          }
          stream = (PdfStream)streams.Resolve(0);
        }
      }

      // Get the stream buffer!
      bytes::IBuffer buffer = stream.Body;
      // Delete old contents from the stream buffer!
      buffer.SetLength(0);
      // Serializing the new contents into the stream buffer...
      Document context = Document;
      foreach(ContentObject item in items)
      {item.WriteTo(buffer, context);}
    }

    public IContentContext ContentContext
    {get{return contentContext;}}

    #region IList
    public int IndexOf(
      ContentObject obj
      )
    {return items.IndexOf(obj);}

    public void Insert(
      int index,
      ContentObject obj
      )
    {items.Insert(index,obj);}

    public void RemoveAt(
      int index
      )
    {items.RemoveAt(index);}

    public ContentObject this[
      int index
      ]
    {
      get{return items[index];}
      set{items[index] = value;}
    }

    #region ICollection
    public void Add(
      ContentObject obj
      )
    {items.Add(obj);}

    public void Clear(
      )
    {items.Clear();}

    public bool Contains(
      ContentObject obj
      )
    {return items.Contains(obj);}

    public void CopyTo(
      ContentObject[] objs,
      int index
      )
    {items.CopyTo(objs,index);}

    public int Count
    {get{return items.Count;}}

    public bool IsReadOnly
    {get{return false;}}

    public bool Remove(
      ContentObject obj
      )
    {return items.Remove(obj);}

    #region IEnumerable<ContentObject>
    public IEnumerator<ContentObject> GetEnumerator(
      )
    {return items.GetEnumerator();}

    #region IEnumerable
    IEnumerator IEnumerable.GetEnumerator()
    {return ((IEnumerable<ContentObject>)this).GetEnumerator();}
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion

    #region private
    private void Load(
      )
    {
      ContentParser parser = new ContentParser(new ContentStream(BaseDataObject));
      items = parser.ParseContentObjects();
    }
    #endregion
    #endregion
    #endregion
  }
}
