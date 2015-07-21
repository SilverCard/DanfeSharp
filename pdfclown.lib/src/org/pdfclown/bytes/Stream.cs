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

using org.pdfclown.tokens;
using org.pdfclown.util;
using org.pdfclown.util.io;

using System;
using System.IO;
using text = System.Text;

namespace org.pdfclown.bytes
{
  /**
    <summary>Generic stream.</summary>
  */
  public sealed class Stream
    : IInputStream,
      IOutputStream
  {
    #region dynamic
    #region fields
    private System.IO.Stream stream;

    private ByteOrderEnum byteOrder = ByteOrderEnum.BigEndian;
    #endregion

    #region constructors
    public Stream(
      System.IO.Stream stream
      )
    {this.stream = stream;}
    #endregion

    #region interface
    #region public
    #region IInputStream
    public ByteOrderEnum ByteOrder
    {
      get
      {return byteOrder;}
      set
      {byteOrder = value;}
    }

    public override int GetHashCode(
      )
    {return stream.GetHashCode();}

    public long Position
    {
      get
      {return stream.Position;}
      set
      {stream.Position = value;}
    }

    public void Read(
      byte[] data
      )
    {stream.Read(data, 0, data.Length);}

    public void Read(
      byte[] data,
      int offset,
      int count
      )
    {stream.Read(data, offset, count);}

    public int ReadByte(
      )
    {return stream.ReadByte();}

    public int ReadInt(
      )
    {
      byte[] data = new byte[sizeof(int)];
      Read(data);
      return ConvertUtils.ByteArrayToInt(data, 0, byteOrder);
    }

    public int ReadInt(
      int length
      )
    {
      byte[] data = new byte[length];
      Read(data);
      return ConvertUtils.ByteArrayToNumber(data, 0, length, byteOrder);
    }

    public string ReadLine(
      )
    {
      text::StringBuilder buffer = new text::StringBuilder();
      while(true)
      {
        int c = stream.ReadByte();
        if(c == -1)
          if(buffer.Length == 0)
            return null;
          else
            break;
        else if(c == '\r'
          || c == '\n')
          break;

        buffer.Append((char)c);
      }
      return buffer.ToString();
    }

    public short ReadShort(
      )
    {
      byte[] data = new byte[sizeof(short)];
      Read(data);
      return (short)ConvertUtils.ByteArrayToNumber(data,0,data.Length,byteOrder);
    }

    public string ReadString(
      int length
      )
    {
      text::StringBuilder buffer = new text::StringBuilder();
      int c;

      while((length--) > 0)
      {
        c = stream.ReadByte();
        if(c == -1)
          break;

        buffer.Append((char)c);
      }

      return buffer.ToString();
    }

    public sbyte ReadSignedByte(
      )
    {throw new NotImplementedException();}

    public ushort ReadUnsignedShort(
      )
    {
      byte[] data = new byte[sizeof(ushort)];
      Read(data);
      return (ushort)ConvertUtils.ByteArrayToNumber(data, 0, data.Length, byteOrder);
    }

    public void Seek(
      long offset
      )
    {stream.Seek(offset, SeekOrigin.Begin);}

    public void Skip(
      long offset
      )
    {stream.Seek(offset, SeekOrigin.Current);}

    #region IDataWrapper
    public byte[] ToByteArray(
      )
    {
      byte[] data = new byte[stream.Length];
      {
        stream.Position = 0;
        stream.Read(data, 0, data.Length);
      }
      return data;
    }
    #endregion

    #region IStream
    public long Length
    {get{return stream.Length;}}

    #region IDisposable
    public void Dispose(
      )
    {
      if(stream != null)
      {
        stream.Dispose();
        stream = null;
      }
      GC.SuppressFinalize(this);
    }
    #endregion
    #endregion
    #endregion

    #region IOutputStream
    public void Write(
      byte[] data
      )
    {stream.Write(data, 0, data.Length);}

    public void Write(
      byte[] data,
      int offset,
      int length
      )
    {stream.Write(data, offset, length);}

    public void Write(
      string data
      )
    {Write(Encoding.Pdf.Encode(data));}

    public void Write(
      IInputStream data
      )
    {
      // TODO:IMPL bufferize!!!
      byte[] baseData = new byte[data.Length];
      // Force the source pointer to the BOF (as we must copy the entire content)!
      data.Position = 0;
      // Read source content!
      data.Read(baseData, 0, baseData.Length);
      // Write target content!
      Write(baseData);
    }
    #endregion
    #endregion
    #endregion
    #endregion
  }
}