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

using System;

namespace org.pdfclown.bytes
{
  /**
    <summary>
      <para>Buffer.</para>
      <para>Its pivotal concept is the array index.</para>
    </summary>
    <returns>This buffer.</returns>
  */
  public interface IBuffer
    : IInputStream,
      IOutputStream
  {
    /**
      <summary>Appends a byte to the buffer.</summary>
      <param name="data">Byte to copy.</param>
      <returns>This buffer.</returns>
    */
    IBuffer Append(
      byte data
      );

    /**
      <summary>Appends a byte array to the buffer.</summary>
      <param name="data">Byte array to copy.</param>
      <returns>This buffer.</returns>
    */
    IBuffer Append(
      byte[] data
      );

    /**
      <summary>Appends a byte range to the buffer.</summary>
      <param name="data">Byte array from which the byte range has to be copied.</param>
      <param name="offset">Location in the byte array at which copying begins.</param>
      <param name="length">Number of bytes to copy.</param>
      <returns>This buffer.</returns>
    */
    IBuffer Append(
      byte[] data,
      int offset,
      int length
      );

    /**
      <summary>Appends a string to the buffer.</summary>
      <param name="data">String to copy.</param>
      <returns>This buffer.</returns>
    */
    IBuffer Append(
      string data
      );

    /**
      <summary>Appends an IInputStream to the buffer.</summary>
      <param name="data">Source data to copy.</param>
      <returns>This buffer.</returns>
    */
    IBuffer Append(
      IInputStream data
      );

    /**
      <summary>Appends a stream to the buffer.</summary>
      <param name="data">Source data to copy.</param>
      <returns>This buffer.</returns>
    */
    IBuffer Append(
      System.IO.Stream data
      );

    /**
      <summary>Gets the allocated buffer size.</summary>
      <returns>Allocated buffer size.</returns>
    */
    int Capacity
    {
      get;
    }

    /**
      <summary>Gets a clone of the buffer.</summary>
      <returns>Deep copy of the buffer.</returns>
    */
    IBuffer Clone(
      );

    /**
      <summary>Applies the specified filter to decode the buffer.</summary>
      <param name="filter">Filter to use for decoding the buffer.</param>
      <param name="parameters">Decoding parameters.</param>
    */
    void Decode(
      Filter filter,
      PdfDictionary parameters
      );

    /**
      <summary>Deletes a byte chunk from the buffer.</summary>
      <param name="index">Location at which deletion has to begin.</param>
      <param name="length">Number of bytes to delete.</param>
    */
    void Delete(
      int index,
      int length
      );

    /**
      <summary>Gets/Sets whether this buffer has changed.</summary>
    */
    bool Dirty
    {
      get;
      set;
    }

    /**
      <summary>Applies the specified filter to encode the buffer.</summary>
      <param name="filter">Filter to use for encoding the buffer.</param>
      <param name="parameters">Encoding parameters.</param>
      <returns>Encoded buffer.</returns>
    */
    byte[] Encode(
      Filter filter,
      PdfDictionary parameters
      );

    /**
      <summary>Gets the byte at a specified location.</summary>
      <param name="index">A location in the buffer.</param>
      <returns>Byte at the specified location.</returns>
    */
    int GetByte(
      int index
      );

    /**
      <summary>Gets the byte range beginning at a specified location.</summary>
      <param name="index">Location at which the byte range has to begin.</param>
      <param name="length">Number of bytes to copy.</param>
      <returns>Byte range beginning at the specified location.</returns>
    */
    byte[] GetByteArray(
      int index,
      int length
      );

    /**
      <summary>Gets the string beginning at a specified location.</summary>
      <param name="index">Location at which the string has to begin.</param>
      <param name="length">Number of bytes to convert.</param>
      <returns>String beginning at the specified location.</returns>
    */
    string GetString(
      int index,
      int length
      );

    /**
      <summary>Inserts a byte array into the buffer.</summary>
      <param name="index">Location at which the byte array has to be inserted.</param>
      <param name="data">Byte array to insert.</param>
    */
    void Insert(
      int index,
      byte[] data
      );

    /**
      <summary>Inserts a byte range into the buffer.</summary>
      <param name="index">Location at which the byte range has to be inserted.</param>
      <param name="data">Byte array from which the byte range has to be copied.</param>
      <param name="offset">Location in the byte array at which copying begins.</param>
      <param name="length">Number of bytes to copy.</param>
    */
    void Insert(
      int index,
      byte[] data,
      int offset,
      int length
      );

    /**
      <summary>Inserts a string into the buffer.</summary>
      <param name="index">Location at which the string has to be inserted.</param>
      <param name="data">String to insert.</param>
    */
    void Insert(
      int index,
      string data
      );

    /**
      <summary>Inserts an IInputStream into the buffer.</summary>
      <param name="index">Location at which the IInputStream has to be inserted.</param>
      <param name="data">Source data to copy.</param>
    */
    void Insert(
      int index,
      IInputStream data
      );

    /**
      <summary>Notifies the dirtiness of the observed buffer.</summary>
    */
    event EventHandler OnChange;

    /**
      <summary>Replaces the buffer contents with a byte array.</summary>
      <param name="index">Location at which the byte array has to be copied.</param>
      <param name="data">Byte array to copy.</param>
    */
    void Replace(
      int index,
      byte[] data
      );

    /**
      <summary>Replaces the buffer contents with a byte range.</summary>
      <param name="index">Location at which the byte range has to be copied.</param>
      <param name="data">Byte array from which the byte range has to be copied.</param>
      <param name="offset">Location in the byte array at which copying begins.</param>
      <param name="length">Number of bytes to copy.</param>
    */
    void Replace(
      int index,
      byte[] data,
      int offset,
      int length
      );

    /**
      <summary>Replaces the buffer contents with a string.</summary>
      <param name="index">Location at which the string has to be copied.</param>
      <param name="data">String to copy.</param>
    */
    void Replace(
      int index,
      string data
      );

    /**
      <summary>Replaces the buffer contents with an IInputStream.</summary>
      <param name="index">Location at which the IInputStream has to be copied.</param>
      <param name="data">Source data to copy.</param>
    */
    void Replace(
      int index,
      IInputStream data
      );

    /**
      <summary>Sets the used buffer size.</summary>
      <param name="value">New length.</param>
    */
    void SetLength(
      int value
      );

    /**
      <summary>Writes the buffer data to a stream.</summary>
      <param name="stream">Target stream.</param>
    */
    void WriteTo(
      IOutputStream stream
      );
  }
}