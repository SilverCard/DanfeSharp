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

using org.pdfclown.bytes.filters;
using org.pdfclown.objects;
using org.pdfclown.tokens;
using org.pdfclown.util;
using org.pdfclown.util.io;

using System;
using System.IO;
using text = System.Text;

namespace org.pdfclown.bytes
{
//TODO:IMPL Substitute System.Array static class invocations with System.Buffer static class invocations (better performance)!!!
  /**
    <summary>Byte buffer.</summary>
  */
  public sealed class Buffer
    : IBuffer
  {
    #region static
    #region fields
    /**
      <summary>Default buffer capacity.</summary>
    */
    private const int DefaultCapacity = 1 << 8;
    #endregion
    #endregion

    #region dynamic
    #region events
    public event EventHandler OnChange;
    #endregion

    #region fields
    /**
      <summary>Inner buffer where data are stored.</summary>
    */
    private byte[] data;
    /**
      <summary>Number of bytes actually used in the buffer.</summary>
    */
    private int length;
    /**
      <summary>Pointer position within the buffer.</summary>
    */
    private int position = 0;

    private ByteOrderEnum byteOrder = ByteOrderEnum.BigEndian;

    private bool dirty;
    #endregion

    #region constructors
    public Buffer(
      ) : this(0)
    {}

    public Buffer(
      int capacity
      )
    {
      if(capacity < 1)
      {capacity = DefaultCapacity;}

      this.data = new byte[capacity];
      this.length = 0;
    }

    public Buffer(
      byte[] data
      )
    {
      this.data = data;
      this.length = data.Length;
    }

    public Buffer(
      System.IO.Stream data
      ) : this((int)data.Length)
    {Append(data);}
    #endregion

    #region interface
    #region public
    #region IBuffer
    public IBuffer Append(
      byte data
      )
    {
      EnsureCapacity(1);
      this.data[this.length++] = data;
      NotifyChange();
      return this;
    }

    public IBuffer Append(
      byte[] data
      )
    {return Append(data, 0, data.Length);}

    public IBuffer Append(
      byte[] data,
      int offset,
      int length
      )
    {
      EnsureCapacity(length);
      Array.Copy(data, offset, this.data, this.length, length);
      this.length += length;
      NotifyChange();
      return this;
    }

    public IBuffer Append(
      string data
      )
    {return Append(Encoding.Pdf.Encode(data));}

    public IBuffer Append(
      IInputStream data
      )
    {return Append(data.ToByteArray(), 0, (int)data.Length);}

    public IBuffer Append(
      System.IO.Stream data
      )
    {
      byte[] array = new byte[data.Length];
      {
        data.Position = 0;
        data.Read(array, 0, array.Length);
      }
      return Append(array);
    }

    public int Capacity
    {
      get
      {return data.Length;}
    }

    public IBuffer Clone(
      )
    {
      IBuffer clone = new Buffer(Capacity);
      clone.Append(data);
      return clone;
    }

    public void Decode(
      Filter filter,
      PdfDictionary parameters
      )
    {
      data = filter.Decode(data,0,length,parameters);
      length = data.Length;
    }

    public void Delete(
      int index,
      int length
      )
    {
      // Shift left the trailing data block to override the deleted data!
      Array.Copy(this.data, index + length, this.data, index, this.length - (index + length));
      this.length -= length;
      NotifyChange();
    }

    public bool Dirty
    {
      get
      {return dirty;}
      set
      {dirty = value;}
    }

    public byte[] Encode(
      Filter filter,
      PdfDictionary parameters
      )
    {return filter.Encode(data, 0, length, parameters);}

    public int GetByte(
      int index
      )
    {return data[index];}

    public byte[] GetByteArray(
      int index,
      int length
      )
    {
      byte[] data = new byte[length];
      Array.Copy(this.data, index, data, 0, length);
      return data;
    }

    public string GetString(
      int index,
      int length
      )
    {return Encoding.Pdf.Decode(data, index, length);}

    public void Insert(
      int index,
      byte[] data
      )
    {Insert(index, data, 0, data.Length);}

    public void Insert(
      int index,
      byte[] data,
      int offset,
      int length
      )
    {
      EnsureCapacity(length);
      // Shift right the existing data block to make room for new data!
      Array.Copy(this.data, index, this.data, index + length, this.length - index);
      // Insert additional data!
      Array.Copy(data, offset, this.data, index, length);
      this.length += length;
      NotifyChange();
    }

    public void Insert(
      int index,
      string data
      )
    {Insert(index, Encoding.Pdf.Encode(data));}

    public void Insert(
      int index,
      IInputStream data
      )
    {Insert(index, data.ToByteArray());}

    public void Replace(
      int index,
      byte[] data
      )
    {
      Array.Copy(data, 0, this.data, index, data.Length);
      NotifyChange();
    }

    public void Replace(
      int index,
      byte[] data,
      int offset,
      int length
      )
    {
      Array.Copy(data, offset, this.data, index, data.Length);
      NotifyChange();
    }

    public void Replace(
      int index,
      string data
      )
    {Replace(index, Encoding.Pdf.Encode(data));}

    public void Replace(
      int index,
      IInputStream data
      )
    {Replace(index, data.ToByteArray());}

    public void SetLength(
      int value
      )
    {
      length = value;
      NotifyChange();
    }

    public void WriteTo(
      IOutputStream stream
      )
    {stream.Write(data, 0, length);}

    #region IInputStream
    public ByteOrderEnum ByteOrder
    {
      get
      {return byteOrder;}
      set
      {byteOrder = value;}
    }

    /* int GetHashCode() uses inherited implementation. */

    public long Position
    {
      get
      {return position;}
      set
      {
        if(value < 0)
        {value = 0;}
        else if(value > data.Length)
        {value = data.Length;}

        position = (int)value;
      }
    }

    public void Read(
      byte[] data
      )
    {Read(data, 0, data.Length);}

    public void Read(
      byte[] data,
      int offset,
      int length
      )
    {
      Array.Copy(this.data, position, data, offset, length);
      position += length;
    }

    public int ReadByte(
      )
    {
      if(position >= data.Length)
        return -1; //TODO:harmonize with other Read*() method EOF exceptions!!!

      return data[position++];
    }

    public int ReadInt(
      )
    {
      int value = ConvertUtils.ByteArrayToInt(data, position, byteOrder);
      position += sizeof(int);
      return value;
    }

    public int ReadInt(
      int length
      )
    {
      int value = ConvertUtils.ByteArrayToNumber(data, position, length, byteOrder);
      position += length;
      return value;
    }

    public string ReadLine(
      )
    {
      if(position >= data.Length)
        throw new EndOfStreamException();

      text::StringBuilder buffer = new text::StringBuilder();
      while(position < data.Length)
      {
        int c = data[position++];
        if(c == '\r'
          || c == '\n')
          break;

        buffer.Append((char)c);
      }
      return buffer.ToString();
    }

    public short ReadShort(
      )
    {
      short value = (short)ConvertUtils.ByteArrayToNumber(data, position, sizeof(short), byteOrder);
      position += sizeof(short);
      return value;
    }

    public string ReadString(
      int length
      )
    {
      string data = Encoding.Pdf.Decode(this.data, position, length);
      position += length;
      return data;
    }

    public sbyte ReadSignedByte(
      )
    {
      if(position >= data.Length)
        throw new EndOfStreamException();

      return (sbyte)data[position++];
    }

    public ushort ReadUnsignedShort(
      )
    {
      ushort value = (ushort)ConvertUtils.ByteArrayToNumber(data, position, sizeof(ushort), byteOrder);
      position += sizeof(ushort);
      return value;
    }

    public void Seek(
      long offset
      )
    {Position = offset;}

    public void Skip(
      long offset
      )
    {Position = position + offset;}

    #region IDataWrapper
    public byte[] ToByteArray(
      )
    {
      byte[] data = new byte[this.length];
      Array.Copy(this.data, 0, data, 0, this.length);
      return data;
    }
    #endregion

    #region IStream
    public long Length
    {
      get
      {return length;}
    }

    #region IDisposable
    public void Dispose(
      )
    {}
    #endregion
    #endregion
    #endregion
    #endregion

    #region IOutputStream
    public void Write(
      byte[] data
      )
    {Append(data);}

    public void Write(
      byte[] data,
      int offset,
      int length
      )
    {Append(data, offset, length);}

    public void Write(
      string data
      )
    {Append(data);}

    public void Write(
      IInputStream data
      )
    {Append(data);}
    #endregion
    #endregion

    #region private
    /**
      <summary>Check whether the buffer has sufficient room for
      adding data.</summary>
    */
    private void EnsureCapacity(
      int additionalLength
      )
    {
      int minCapacity = this.length + additionalLength;
      // Is additional data within the buffer capacity?
      if(minCapacity <= this.data.Length)
        return;

      // Additional data exceed buffer capacity.
      // Reallocate the buffer!
      byte[] data = new byte[
        Math.Max(
          this.data.Length << 1, // 1 order of magnitude greater than current capacity.
          minCapacity // Minimum capacity required.
          )
        ];
      Array.Copy(this.data, 0, data, 0, this.length);
      this.data = data;
    }

    private void NotifyChange(
      )
    {
      if(dirty || OnChange == null)
        return;

      dirty = true;
      OnChange(this, null);
    }
    #endregion
    #endregion
    #endregion
  }
}