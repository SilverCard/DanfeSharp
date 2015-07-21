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
using org.pdfclown.files;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;
using io = System.IO;
using System.Text;

namespace org.pdfclown.tokens
{
  /**
    <summary>PDF file writer.</summary>
  */
  public abstract class Writer
  {
    #region static
    #region fields
    private static readonly byte[] BOFChunk = Encoding.Pdf.Encode(Keyword.BOF);
    private static readonly byte[] EOFChunk = Encoding.Pdf.Encode(Symbol.LineFeed + Keyword.EOF + Symbol.CarriageReturn + Symbol.LineFeed);
    private static readonly byte[] HeaderBinaryHintChunk = new byte[]{(byte)Symbol.LineFeed,(byte)Symbol.Percent,(byte)0x80,(byte)0x80,(byte)0x80,(byte)0x80,(byte)Symbol.LineFeed}; // NOTE: Arbitrary binary characters (code >= 128) for ensuring proper behavior of file transfer applications [PDF:1.6:3.4.1].
    private static readonly byte[] StartXRefChunk = Encoding.Pdf.Encode(Keyword.StartXRef + Symbol.LineFeed);
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets a new writer instance for the specified file.</summary>
      <param name="file">File to serialize.</param>
      <param name="stream">Target stream.</param>
    */
    public static Writer Get(
      File file,
      IOutputStream stream
      )
    {
      // Which cross-reference table mode?
      switch(file.Document.Configuration.XrefMode)
      {
        case Document.ConfigurationImpl.XRefModeEnum.Plain:
          return new PlainWriter(file, stream);
        case Document.ConfigurationImpl.XRefModeEnum.Compressed:
          return new CompressedWriter(file, stream);
        default:
          throw new NotSupportedException();
      }
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region fields
    protected readonly File file;
    protected readonly IOutputStream stream;
    #endregion

    #region constructors
    protected Writer(
      File file,
      IOutputStream stream
      )
    {
      this.file = file;
      this.stream = stream;
    }
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the file to serialize.</summary>
    */
    public File File
    {
      get
      {return file;}
    }

    /**
      <summary>Gets the target stream.</summary>
    */
    public IOutputStream Stream
    {
      get
      {return stream;}
    }

    /**
      <summary>Serializes the <see cref="File">file</see> to the <see cref="Stream">target stream</see>.</summary>
      <param name="mode">Serialization mode.</param>
     */
    public void Write(
      SerializationModeEnum mode
      )
    {
      switch(mode)
      {
        case SerializationModeEnum.Incremental:
          if(file.Reader == null)
            goto case SerializationModeEnum.Standard;

          WriteIncremental();
          break;
        case SerializationModeEnum.Standard:
          WriteStandard();
          break;
        case SerializationModeEnum.Linearized:
          WriteLinearized();
          break;
      }
    }
    #endregion

    #region protected
    /**
      <summary>Updates the specified trailer.</summary>
      <remarks>This method has to be called just before serializing the trailer object.</remarks>
    */
    protected void UpdateTrailer(
      PdfDictionary trailer,
      IOutputStream stream
      )
    {
      // File identifier update.
      FileIdentifier identifier = FileIdentifier.Wrap(trailer[PdfName.ID]);
      if(identifier == null)
      {trailer[PdfName.ID] = (identifier = new FileIdentifier()).BaseObject;}
      identifier.Update(this);
    }

    /**
      <summary>Serializes the beginning of the file [PDF:1.6:3.4.1].</summary>
    */
    protected void WriteHeader(
      )
    {
      stream.Write(BOFChunk);
      stream.Write(file.Document.Version.ToString()); // NOTE: Document version represents the actual (possibly-overridden) file version.
      stream.Write(HeaderBinaryHintChunk);
    }

    /**
      <summary>Serializes the PDF file as incremental update [PDF:1.6:3.4.5].</summary>
    */
    protected abstract void WriteIncremental(
      );

    /**
      <summary>Serializes the PDF file linearized [PDF:1.6:F].</summary>
    */
    protected abstract void WriteLinearized(
      );

    /**
      <summary>Serializes the PDF file compactly [PDF:1.6:3.4].</summary>
    */
    protected abstract void WriteStandard(
      );

    /**
      <summary>Serializes the end of the file [PDF:1.6:3.4.4].</summary>
      <param name="startxref">Byte offset from the beginning of the file to the beginning
        of the last cross-reference section.</param>
    */
    protected void WriteTail(
      long startxref
      )
    {
      stream.Write(StartXRefChunk);
      stream.Write(startxref.ToString());
      stream.Write(EOFChunk);
    }
    #endregion
    #endregion
    #endregion
  }
}
