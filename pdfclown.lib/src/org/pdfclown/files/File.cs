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
using org.pdfclown.documents;
using org.pdfclown.documents.contents;
using org.pdfclown.objects;
using org.pdfclown.tokens;

using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.IO;

namespace org.pdfclown.files
{
  /**
    <summary>PDF file representation.</summary>
  */
  public sealed class File
    : IDisposable
  {
    #region types
    /**
      <summary>File configuration.</summary>
    */
    public sealed class ConfigurationImpl
    {
      private string realFormat;

      private readonly File file;

      internal ConfigurationImpl(
        File file
        )
      {this.file = file;}

      /**
        <summary>Gets the file associated with this configuration.</summary>
      */
      public File File
      {
        get
        {return file;}
      }

      /**
        <summary>Gets/Sets the format applied to real number serialization.</summary>
      */
      public string RealFormat
      {
        get
        {
          if(realFormat == null)
          {SetRealFormat(5);}
          return realFormat;
        }
        set
        {realFormat = value;}
      }

      /**
        <param name="decimalPlacesCount">Number of digits in decimal places.</param>
        <seealso cref="RealFormat"/>
      */
      public void SetRealFormat(
        int decimalPlacesCount
        )
      {realFormat = "0." + new string('#', decimalPlacesCount);}
    }

    private sealed class ImplicitContainer
      : PdfIndirectObject
    {
      public ImplicitContainer(
        File file,
        PdfDataObject dataObject
        ) : base(file, dataObject, new XRefEntry(int.MinValue, int.MinValue))
      {}
    }
    #endregion

    #region static
    #region fields
    private static Random hashCodeGenerator = new Random();
    #endregion
    #endregion

    #region dynamic
    #region fields
    private ConfigurationImpl configuration;
    private readonly Document document;
    private readonly int hashCode = hashCodeGenerator.Next();
    private readonly IndirectObjects indirectObjects;
    private string path;
    private Reader reader;
    private readonly PdfDictionary trailer;
    private readonly Version version;

    private Cloner cloner;
    #endregion

    #region constructors
    public File(
      )
    {
      Initialize();

      version = VersionEnum.PDF14.GetVersion();
      trailer = PrepareTrailer(new PdfDictionary());
      indirectObjects = new IndirectObjects(this, null);
      document = new Document(this);
    }

    public File(
      string path
      ) : this(
        new bytes.Stream(
          new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read
            )
          )
        )
    {this.path = path;}

    public File(
      IInputStream stream
      )
    {
      Initialize();

      reader = new Reader(stream, this);

      Reader.FileInfo info = reader.ReadInfo();
      version = info.Version;
      trailer = PrepareTrailer(info.Trailer);
      if(trailer.ContainsKey(PdfName.Encrypt)) // Encrypted file.
        throw new NotImplementedException("Encrypted files are currently not supported.");

      indirectObjects = new IndirectObjects(this, info.XrefEntries);
      document = new Document(trailer[PdfName.Root]);
      document.Configuration.XrefMode = (PdfName.XRef.Equals(trailer[PdfName.Type])
        ? Document.ConfigurationImpl.XRefModeEnum.Compressed
        : Document.ConfigurationImpl.XRefModeEnum.Plain);
    }
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the default cloner.</summary>
    */
    public Cloner Cloner
    {
      get
      {
        if(cloner == null)
        {cloner = new Cloner(this);}

        return cloner;
      }
      set
      {cloner = value;}
    }

    /**
      <summary>Gets the file configuration.</summary>
    */
    public ConfigurationImpl Configuration
    {
      get
      {return configuration;}
    }

    /**
      <summary>Gets the high-level representation of the file content.</summary>
    */
    public Document Document
    {
      get
      {return document;}
    }

    public override int GetHashCode(
      )
    {return hashCode;}

    /**
      <summary>Gets the identifier of this file.</summary>
    */
    public FileIdentifier ID
    {
      get
      {return FileIdentifier.Wrap(Trailer[PdfName.ID]);}
    }

    /**
      <summary>Gets the indirect objects collection.</summary>
    */
    public IndirectObjects IndirectObjects
    {
      get
      {return indirectObjects;}
    }

    /**
      <summary>Gets/Sets the file path.</summary>
    */
    public string Path
    {
      get
      {return path;}
      set
      {path = value;}
    }

    /**
      <summary>Gets the data reader backing this file.</summary>
      <returns><code>null</code> in case of newly-created file.</returns>
    */
    public Reader Reader
    {
      get
      {return reader;}
    }

    /**
      <summary>Registers an <b>internal data object</b>.</summary>
    */
    public PdfReference Register(
      PdfDataObject obj
      )
    {return indirectObjects.Add(obj).Reference;}

    /**
      <summary>Serializes the file to the current file-system path using the <see
      cref="SerializationModeEnum.Standard">standard serialization mode</see>.</summary>
    */
    public void Save(
      )
    {Save(SerializationModeEnum.Standard);}

    /**
      <summary>Serializes the file to the current file-system path.</summary>
      <param name="mode">Serialization mode.</param>
    */
    public void Save(
      SerializationModeEnum mode
      )
    {
      if(!System.IO.File.Exists(path))
        throw new FileNotFoundException("No valid source path available.");

      /*
        NOTE: The document file cannot be directly overwritten as it's locked for reading by the
        open stream; its update is therefore delayed to its disposal, when the temporary file will
        overwrite it (see Dispose() method).
      */
      Save(TempPath, mode);
    }

    /**
      <summary>Serializes the file to the specified file system path.</summary>
      <param name="path">Target path.</param>
      <param name="mode">Serialization mode.</param>
    */
    public void Save(
      string path,
      SerializationModeEnum mode
      )
    {
      FileStream outputStream = new System.IO.FileStream(
        path,
        System.IO.FileMode.Create,
        System.IO.FileAccess.Write
        );
      Save(
        new bytes.Stream(outputStream),
        mode
        );
      outputStream.Flush();
      outputStream.Close();
    }

    /**
      <summary>Serializes the file to the specified stream.</summary>
      <remarks>It's caller responsibility to close the stream after this method ends.</remarks>
      <param name="stream">Target stream.</param>
      <param name="mode">Serialization mode.</param>
    */
    public void Save(
      IOutputStream stream,
      SerializationModeEnum mode
      )
    {
      if(Reader == null)
      {
        try
        {
          string assemblyTitle = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute))).Title;
          string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
          Document.Information.Producer = assemblyTitle + " " + assemblyVersion;
        }
        catch
        {/* NOOP */}
      }
  
      Writer writer = Writer.Get(this, stream);
      writer.Write(mode);
    }

    /**
      <summary>Gets the file trailer.</summary>
    */
    public PdfDictionary Trailer
    {
      get
      {return trailer;}
    }

    /**
      <summary>Unregisters an internal object.</summary>
    */
    public void Unregister(
      PdfReference reference
      )
    {indirectObjects.RemoveAt(reference.ObjectNumber);}

    /**
      <summary>Gets the file header version [PDF:1.6:3.4.1].</summary>
      <remarks>This property represents just the original file version; to get the actual version,
      use the <see cref="org.pdfclown.documents.Document.Version">Document.Version</see> method.
      </remarks>
    */
    public Version Version
    {
      get
      {return version;}
    }

    #region IDisposable
    public void Dispose(
      )
    {
      if(reader != null)
      {
        reader.Dispose();
        reader = null;

        /*
          NOTE: If the temporary file exists (see Save() method), it must overwrite the document file.
        */
        if(System.IO.File.Exists(TempPath))
        {
          System.IO.File.Delete(path);
          System.IO.File.Move(TempPath,path);
        }
      }

      GC.SuppressFinalize(this);
    }
    #endregion
    #endregion

    #region private
    private void Initialize(
      )
    {configuration = new ConfigurationImpl(this);}

    private PdfDictionary PrepareTrailer(
      PdfDictionary trailer
      )
    {return (PdfDictionary)new ImplicitContainer(this, trailer).DataObject;}

    private string TempPath
    {
      get
      {return (path == null ? null : path + ".tmp");}
    }
    #endregion
    #endregion
    #endregion
  }
}