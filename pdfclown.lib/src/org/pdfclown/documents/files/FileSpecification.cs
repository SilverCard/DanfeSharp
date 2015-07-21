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

using bytes = org.pdfclown.bytes;
using org.pdfclown.files;
using org.pdfclown.objects;

using System;
using io = System.IO;

namespace org.pdfclown.documents.files
{
  /**
    <summary>Reference to the contents of another file (file specification) [PDF:1.6:3.10.2].</summary>
  */
  [PDF(VersionEnum.PDF11)]
  public abstract class FileSpecification
    : PdfObjectWrapper<PdfDirectObject>,
      IPdfNamedObjectWrapper
  {
    #region static
    #region public
    /**
      <summary>Creates a new reference to an external file.</summary>
      <param name="context">Document context.</param>
      <param name="path">File path.</param>
    */
    public static SimpleFileSpecification Get(
      Document context,
      string path
      )
    {return (SimpleFileSpecification)Get(context, path, false);}

    /**
      <summary>Creates a new reference to a file.</summary>
      <param name="context">Document context.</param>
      <param name="path">File path.</param>
      <param name="full">Whether the reference is able to support extended dependencies.</param>
    */
    public static FileSpecification Get(
      Document context,
      string path,
      bool full
      )
    {
      return full
        ? (FileSpecification)new FullFileSpecification(context, path)
        : (FileSpecification)new SimpleFileSpecification(context, path);
    }

    /**
      <summary>Creates a new reference to an embedded file.</summary>
      <param name="embeddedFile">Embedded file corresponding to the reference.</param>
      <param name="filename">Name corresponding to the reference.</param>
    */
    public static FullFileSpecification Get(
      EmbeddedFile embeddedFile,
      string filename
      )
    {return new FullFileSpecification(embeddedFile, filename);}

    /**
      <summary>Creates a new reference to a remote file.</summary>
      <param name="context">Document context.</param>
      <param name="url">Remote file location.</param>
    */
    public static FullFileSpecification Get(
      Document context,
      Uri url
      )
    {return new FullFileSpecification(context, url);}

    /**
      <summary>Instantiates an existing file reference.</summary>
      <param name="baseObject">Base object.</param>
    */
    public static FileSpecification Wrap(
      PdfDirectObject baseObject
      )
    {
      if(baseObject == null)
        return null;

      PdfDataObject baseDataObject = baseObject.Resolve();
      if(baseDataObject is PdfString)
        return new SimpleFileSpecification(baseObject);
      else if(baseDataObject is PdfDictionary)
        return new FullFileSpecification(baseObject);
      else
        return null;
    }
    #endregion
    #endregion

    #region dynamic
    #region constructors
    protected FileSpecification(
      Document context,
      PdfDirectObject baseDataObject
      ) : base(context, baseDataObject)
    {}

    protected FileSpecification(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the file absolute path.</summary>
    */
    public string GetAbsolutePath(
      )
    {
      string path = Path;
      if(!io::Path.IsPathRooted(path)) // Path needs to be resolved.
      {
        string basePath = Document.File.Path;
        if(basePath != null)
        {path = io::Path.Combine(io::Path.GetDirectoryName(basePath), path);}
      }
      return path;
    }

    /**
      <summary>Gets an input stream to read from the file.</summary>
    */
    public virtual bytes::IInputStream GetInputStream(
      )
    {
      return new bytes::Stream(
        new io::FileStream(
          GetAbsolutePath(),
          io::FileMode.Open,
          io::FileAccess.Read
          )
        );
    }

    /**
      <summary>Gets an output stream to write into the file.</summary>
    */
    public virtual bytes::IOutputStream GetOutputStream(
      )
    {
      return new bytes::Stream(
        new io::FileStream(
          GetAbsolutePath(),
          io::FileMode.Create,
          io::FileAccess.Write
          )
        );
    }

    /**
      <summary>Gets the file path.</summary>
    */
    public abstract string Path
    {
      get;
    }

    #region IPdfNamedObjectWrapper
    public PdfString Name
    {
      get
      {return RetrieveName();}
    }

    public PdfDirectObject NamedBaseObject
    {
      get
      {return RetrieveNamedBaseObject();}
    }
    #endregion
    #endregion
    #endregion
    #endregion
  }
}

