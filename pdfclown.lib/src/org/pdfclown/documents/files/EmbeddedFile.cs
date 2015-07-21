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

using bytes = org.pdfclown.bytes;
using org.pdfclown.documents;
using org.pdfclown.objects;

using System;
using System.IO;

namespace org.pdfclown.documents.files
{
  /**
    <summary>Embedded file [PDF:1.6:3.10.3].</summary>
  */
  [PDF(VersionEnum.PDF13)]
  public sealed class EmbeddedFile
    : PdfObjectWrapper<PdfStream>
  {
    #region static
    #region interface
    #region public
    /**
      <summary>Creates a new embedded file inside the document.</summary>
      <param name="context">Document context.</param>
      <param name="path">Path of the file to embed.</param>
    */
    public static EmbeddedFile Get(
      Document context,
      string path
      )
    {
      return new EmbeddedFile(
        context,
        new bytes.Stream(
          new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read
            )
          )
        );
    }

    /**
      <summary>Creates a new embedded file inside the document.</summary>
      <param name="context">Document context.</param>
      <param name="stream">File stream to embed.</param>
    */
    public static EmbeddedFile Get(
      Document context,
      bytes::IInputStream stream
      )
    {return new EmbeddedFile(context, stream);}

    /**
      <summary>Instantiates an existing embedded file.</summary>
      <param name="baseObject">Base object.</param>
    */
    public static EmbeddedFile Wrap(
      PdfDirectObject baseObject
      )
    {return baseObject != null ? new EmbeddedFile(baseObject) : null;}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    private EmbeddedFile(
      Document context,
      bytes::IInputStream stream
      ) : base(
        context,
        new PdfStream(
          new PdfDictionary(
            new PdfName[]{PdfName.Type},
            new PdfDirectObject[]{PdfName.EmbeddedFile}
            ),
          new bytes::Buffer(stream.ToByteArray())
          )
        )
    {}

    private EmbeddedFile(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the creation date of this file.</summary>
    */
    public DateTime? CreationDate
    {
      get
      {
        PdfDate dateObject = (PdfDate)GetInfo(PdfName.CreationDate);
        return dateObject != null ? (DateTime?)dateObject.Value : null;
      }
      set
      {SetInfo(PdfName.CreationDate, PdfDate.Get(value));}
    }

    /**
      <summary>Gets the data contained within this file.</summary>
    */
    public bytes::IBuffer Data
    {
      get
      {return BaseDataObject.Body;}
    }

    /**
      <summary>Gets/Sets the MIME media type name of this file [RFC 2046].</summary>
    */
    public string MimeType
    {
      get
      {
        PdfName subtype = (PdfName)BaseDataObject.Header[PdfName.Subtype];
        return subtype != null ? (string)subtype.Value : null;
      }
      set
      {BaseDataObject.Header[PdfName.Subtype] = new PdfName(value);}
    }

    /**
      <summary>Gets/Sets the modification date of this file.</summary>
    */
    public DateTime? ModificationDate
    {
      get
      {
        PdfDate dateObject = (PdfDate)GetInfo(PdfName.ModDate);
        return (DateTime?)(dateObject != null ? dateObject.Value : null);
      }
      set
      {SetInfo(PdfName.ModDate, PdfDate.Get(value));}
    }

    /**
      <summary>Gets/Sets the size of this file, in bytes.</summary>
    */
    public int Size
    {
      get
      {
        PdfInteger sizeObject = (PdfInteger)GetInfo(PdfName.Size);
        return sizeObject != null ? sizeObject.IntValue : 0;
      }
      set
      {SetInfo(PdfName.Size, PdfInteger.Get(value));}
    }
    #endregion

    #region private
    /**
      <summary>Gets the file parameter associated to the specified key.</summary>
      <param name="key">Parameter key.</param>
    */
    private PdfDirectObject GetInfo(
      PdfName key
      )
    {return Params[key];}

    /**
      <summary>Gets the file parameters.</summary>
    */
    private PdfDictionary Params
    {
      get
      {return BaseDataObject.Header.Resolve<PdfDictionary>(PdfName.Params);}
    }

    /**
      <see cref="GetInfo(PdfName)"/>
    */
    private void SetInfo(
      PdfName key,
      PdfDirectObject value
      )
    {Params[key] = value;}
    #endregion
    #endregion
    #endregion
  }
}