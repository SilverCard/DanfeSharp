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

namespace org.pdfclown.objects
{
  /**
    <summary>Abstract PDF direct object.</summary>
  */
  public abstract class PdfDirectObject
    : PdfDataObject,
      IComparable<PdfDirectObject>
  {
    #region static
    #region fields
    private static readonly byte[] NullChunk = Encoding.Pdf.Encode(Keyword.Null);
    #endregion

    #region interface
    #region internal
    /**
      <summary>Ensures that the given direct object is properly represented as string.</summary>
      <remarks>This method is useful to force null pointers to be expressed as PDF null objects.</remarks>
    */
    internal static string ToString(
      PdfDirectObject obj
      )
    {return (obj == null ? Keyword.Null : obj.ToString());}

    /**
      <summary>Ensures that the given direct object is properly serialized.</summary>
      <remarks>This method is useful to force null pointers to be expressed as PDF null objects.</remarks>
    */
    internal static void WriteTo(
      IOutputStream stream,
      File context,
      PdfDirectObject obj
      )
    {
      if(obj == null)
      {stream.Write(NullChunk);}
      else
      {obj.WriteTo(stream, context);}
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    protected PdfDirectObject(
      )
    {}
    #endregion

    #region public
    #region IComparable
    public abstract int CompareTo(
      PdfDirectObject obj
      );
    #endregion
    #endregion
    #endregion
  }
}