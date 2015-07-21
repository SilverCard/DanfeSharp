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
using org.pdfclown.documents;
using org.pdfclown.files;
using org.pdfclown.objects;
using org.pdfclown.util;

using System;
using System.IO;
using System.Net;

namespace org.pdfclown.documents.files
{
  /**
    <summary>Extended reference to the contents of another file [PDF:1.6:3.10.2].</summary>
  */
  [PDF(VersionEnum.PDF11)]
  public sealed class FullFileSpecification
    : FileSpecification
  {
    #region types
    /**
      <summary>Standard file system.</summary>
    */
    public enum StandardFileSystemEnum
    {
      /**
        <summary>Generic platform file system.</summary>
      */
      Native,
      /**
        <summary>Uniform resource locator.</summary>
      */
      URL
    }
    #endregion

    #region dynamic
    #region constructors
    internal FullFileSpecification(
      Document context,
      string path
      ) : base(
        context,
        new PdfDictionary(
          new PdfName[]
          {PdfName.Type},
          new PdfDirectObject[]
          {PdfName.Filespec}
          )
        )
    {SetPath(path);}

    internal FullFileSpecification(
      EmbeddedFile embeddedFile,
      string filename
      ) : this(embeddedFile.Document, filename)
    {EmbeddedFile = embeddedFile;}

    internal FullFileSpecification(
      Document context,
      Uri url
      ) : this(context, url.ToString())
    {FileSystem = StandardFileSystemEnum.URL;}

    internal FullFileSpecification(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the related files.</summary>
    */
    public RelatedFiles Dependencies
    {
      get
      {return GetDependencies(PdfName.F);}
      set
      {SetDependencies(PdfName.F,value);}
    }

    /**
      <summary>Gets/Sets the description of the file.</summary>
    */
    public string Description
    {
      get
      {return (string)PdfSimpleObject<object>.GetValue(BaseDictionary[PdfName.Desc]);}
      set
      {BaseDictionary[PdfName.Desc] = new PdfTextString(value);}
    }

    /**
      <summary>Gets/Sets the embedded file corresponding to this file.</summary>
    */
    public EmbeddedFile EmbeddedFile
    {
      get
      {return GetEmbeddedFile(PdfName.F);}
      set
      {SetEmbeddedFile(PdfName.F,value);}
    }

    /**
      <summary>Gets/Sets the file system to be used to interpret this file specification.</summary>
      <returns>Either <see cref="StandardFileSystemEnum"/> (standard file system) or
      <see cref="String"/> (custom file system).</returns>
    */
    public object FileSystem
    {
      get
      {
        PdfName fileSystemObject = (PdfName)BaseDictionary[PdfName.FS];
        StandardFileSystemEnum? standardFileSystem = StandardFileSystemEnumExtension.Get(fileSystemObject);
        return standardFileSystem.HasValue ? standardFileSystem.Value : fileSystemObject.Value;
      }
      set
      {
        PdfName fileSystemObject;
        if(value is StandardFileSystemEnum)
        {fileSystemObject = ((StandardFileSystemEnum)value).GetCode();}
        else if(value is string)
        {fileSystemObject = new PdfName((string)value);}
        else
          throw new ArgumentException("MUST be either StandardFileSystemEnum (standard file system) or String (custom file system)");

        BaseDictionary[PdfName.FS] = fileSystemObject;
      }
    }

    public override bytes::IInputStream GetInputStream(
      )
    {
      if(PdfName.URL.Equals(BaseDictionary[PdfName.FS])) // Remote resource [PDF:1.7:3.10.4].
      {
        Uri fileUrl;
        try
        {fileUrl = new Uri(Path);}
        catch(Exception e)
        {throw new Exception("Failed to instantiate URL for " + Path, e);}
        WebClient webClient = new WebClient();
        try
        {return new bytes::Buffer(webClient.OpenRead(fileUrl));}
        catch(Exception e)
        {throw new Exception("Failed to open input stream for " + Path, e);}
      }
      else // Local resource [PDF:1.7:3.10.1].
        return base.GetInputStream();
    }

    public override bytes::IOutputStream GetOutputStream(
      )
    {
      if(PdfName.URL.Equals(BaseDictionary[PdfName.FS])) // Remote resource [PDF:1.7:3.10.4].
      {
        Uri fileUrl;
        try
        {fileUrl = new Uri(Path);}
        catch(Exception e)
        {throw new Exception("Failed to instantiate URL for " + Path, e);}
        WebClient webClient = new WebClient();
        try
        {return new bytes::Stream(webClient.OpenWrite(fileUrl));}
        catch(Exception e)
        {throw new Exception("Failed to open output stream for " + Path, e);}
      }
      else // Local resource [PDF:1.7:3.10.1].
        return base.GetOutputStream();
    }

    /**
      <summary>Gets/Sets the identifier of the file.</summary>
    */
    public FileIdentifier ID
    {
      get
      {return FileIdentifier.Wrap(BaseDictionary[PdfName.ID]);}
      set
      {BaseDictionary[PdfName.ID] = value.BaseObject;}
    }

    public override string Path
    {
      get
      {return GetPath(PdfName.F);}
    }

    public void SetPath(
      string value
      )
    {SetPath(PdfName.F,value);}

    /**
      <summary>Gets/Sets whether the referenced file is volatile (changes frequently with time).
      </summary>
    */
    public bool Volatile
    {
      get
      {return (bool)PdfSimpleObject<object>.GetValue(BaseDictionary[PdfName.V], false);}
      set
      {BaseDictionary[PdfName.V] = PdfBoolean.Get(value);}
    }
    #endregion

    #region private
    private PdfDictionary BaseDictionary
    {
      get
      {return (PdfDictionary)BaseDataObject;}
    }

    /**
      <summary>Gets the related files associated to the given key.</summary>
    */
    private RelatedFiles GetDependencies(
      PdfName key
      )
    {
      PdfDictionary dependenciesObject = (PdfDictionary)BaseDictionary[PdfName.RF];
      if(dependenciesObject == null)
        return null;

      return RelatedFiles.Wrap(dependenciesObject[key]);
    }

    /**
      <summary>Gets the embedded file associated to the given key.</summary>
    */
    private EmbeddedFile GetEmbeddedFile(
      PdfName key
      )
    {
      PdfDictionary embeddedFilesObject = (PdfDictionary)BaseDictionary[PdfName.EF];
      if(embeddedFilesObject == null)
        return null;

      return EmbeddedFile.Wrap(embeddedFilesObject[key]);
    }

    /**
      <summary>Gets the path associated to the given key.</summary>
    */
    private string GetPath(
      PdfName key
      )
    {return (string)PdfSimpleObject<object>.GetValue(BaseDictionary[key]);}

    /**
      <see cref="GetDependencies(PdfName)"/>
    */
    private void SetDependencies(
      PdfName key,
      RelatedFiles value
      )
    {
      PdfDictionary dependenciesObject = (PdfDictionary)BaseDictionary[PdfName.RF];
      if(dependenciesObject == null)
      {BaseDictionary[PdfName.RF] = dependenciesObject = new PdfDictionary();}

      dependenciesObject[key] = value.BaseObject;
    }

    /**
      <see cref="GetEmbeddedFile(PdfName)"/>
    */
    private void SetEmbeddedFile(
      PdfName key,
      EmbeddedFile value
      )
    {
      PdfDictionary embeddedFilesObject = (PdfDictionary)BaseDictionary[PdfName.EF];
      if(embeddedFilesObject == null)
      {BaseDictionary[PdfName.EF] = embeddedFilesObject = new PdfDictionary();}

      embeddedFilesObject[key] = value.BaseObject;
    }

    /**
      <see cref="GetPath(PdfName)"/>
    */
    private void SetPath(
      PdfName key,
      string value
      )
    {BaseDictionary[key] = new PdfString(value);}
    #endregion
    #endregion
    #endregion
  }

  internal static class StandardFileSystemEnumExtension
  {
    private static readonly BiDictionary<FullFileSpecification.StandardFileSystemEnum,PdfName> codes;

    static StandardFileSystemEnumExtension(
      )
    {
      codes = new BiDictionary<FullFileSpecification.StandardFileSystemEnum,PdfName>();
      codes[FullFileSpecification.StandardFileSystemEnum.Native] = null;
      codes[FullFileSpecification.StandardFileSystemEnum.URL] = PdfName.URL;
    }

    public static FullFileSpecification.StandardFileSystemEnum? Get(
      PdfName code
      )
    {return codes.GetKey(code);}

    public static PdfName GetCode(
      this FullFileSpecification.StandardFileSystemEnum standardFileSystem
      )
    {return codes[standardFileSystem];}
  }
}