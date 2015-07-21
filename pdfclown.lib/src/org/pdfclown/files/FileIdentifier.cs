/*
  Copyright 2012 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.documents.interchange.metadata;
using org.pdfclown.tokens;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace org.pdfclown.files
{
  /**
    <summary>File identifier [PDF:1.7:10.3].</summary>
  */
  public sealed class FileIdentifier
    : PdfObjectWrapper<PdfArray>
  {
    #region static
    #region public
    /**
      <summary>Gets an existing file identifier.</summary>
      <param name="baseObject">Base object to wrap.</param>
    */
    public static FileIdentifier Wrap(
      PdfDirectObject baseObject
      )
    {return baseObject != null ? new FileIdentifier(baseObject) : null;}
    #endregion

    #region private
    private static void Digest(
      BinaryWriter buffer,
      object value
      )
    {buffer.Write(value.ToString());}

    private static PdfArray CreateBaseDataObject(
      )
    {return new PdfArray(PdfString.Default, PdfString.Default);}
    #endregion
    #endregion

    #region dynamic
    #region constructors
    /**
      <summary>Creates a new direct file identifier.</summary>
    */
    public FileIdentifier(
      ) : this(CreateBaseDataObject())
    {}

    /**
      <summary>Creates a new indirect file identifier.</summary>
    */
    public FileIdentifier(
      File context
      ) : base(context, CreateBaseDataObject())
    {}

    /**
      <summary>Instantiates an existing file identifier.</summary>
      <param nme="baseObject">Base object.</param>
    */
    private FileIdentifier(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the permanent identifier based on the contents of the file at the time it was
      originally created.</summary>
    */
    public string BaseID
    {
      get
      {return (string)((PdfString)BaseDataObject[0]).Value;}
    }

    /**
      <summary>Gets the changing identifier based on the file's contents at the time it was last
      updated.</summary>
    */
    public string VersionID
    {
      get
      {return (string)((PdfString)BaseDataObject[1]).Value;}
    }

    /**
      <summary>Computes a new version identifier based on the file's contents.</summary>
      <remarks>This method is typically invoked internally during file serialization.</remarks>
      <param name="writer">File serializer.</param>
    */
    public void Update(
      Writer writer
      )
    {
      /*
        NOTE: To help ensure the uniqueness of file identifiers, it is recommended that they are
        computed by means of a message digest algorithm such as MD5 [PDF:1.7:10.3].
      */
      using (MD5 md5 = MD5.Create())
      {
        using(BinaryWriter buffer = new BinaryWriter(new MemoryStream(), Charset.ISO88591))
        {
          File file = writer.File;
          try
          {
            // File identifier computation is fulfilled with this information:
            // a) Current time.
            Digest(buffer, DateTime.Now.Ticks);

            // b) File location.
            if(file.Path != null)
            {Digest(buffer, file.Path);}

            // c) File size.
            Digest(buffer, writer.Stream.Length);

            // d) Entries in the document information dictionary.
            foreach(KeyValuePair<PdfName,PdfDirectObject> informationObjectEntry in file.Document.Information.BaseDataObject)
            {
              Digest(buffer, informationObjectEntry.Key);
              Digest(buffer, informationObjectEntry.Value);
            }
          }
          catch(Exception e)
          {throw new Exception("File identifier digest failed.", e);}

          /*
            NOTE: File identifier is an array of two byte strings [PDF:1.7:10.3]:
             1) a permanent identifier based on the contents of the file at the time it was
                originally created. It does not change when the file is incrementally updated;
             2) a changing identifier based on the file's contents at the time it was last updated.
            When a file is first written, both identifiers are set to the same value. If both
            identifiers match when a file reference is resolved, it is very likely that the correct
            file has been found. If only the first identifier matches, a different version of the
            correct file has been found.
          */
          PdfString versionID = new PdfString(
            md5.ComputeHash(((MemoryStream)buffer.BaseStream).ToArray()),
            PdfString.SerializationModeEnum.Hex
            );
          BaseDataObject[1] = versionID;
          if(BaseDataObject[0].Equals(PdfString.Default))
          {BaseDataObject[0] = versionID;}
        }
      }
    }
    #endregion
    #endregion
    #endregion
  }
}