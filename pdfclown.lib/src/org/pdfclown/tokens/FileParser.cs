/*
  Copyright 2011 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.util.parsers;

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace org.pdfclown.tokens
{
  /**
    <summary>PDF file parser [PDF:1.7:3.2,3.4].</summary>
  */
  public sealed class FileParser
    : BaseParser
  {
    #region types
    public struct Reference
    {
      public readonly int GenerationNumber;
      public readonly int ObjectNumber;

      internal Reference(
        int objectNumber,
        int generationNumber
        )
      {
        this.ObjectNumber = objectNumber;
        this.GenerationNumber = generationNumber;
      }
    }
    #endregion

    #region static
    #region fields
    private static readonly int EOFMarkerChunkSize = 1024; // [PDF:1.6:H.3.18].
    #endregion
    #endregion

    #region dynamic
    #region fields
    private files.File file;
    #endregion

    #region constructors
    internal FileParser(
      IInputStream stream,
      files.File file
      ) : base(stream)
    {this.file = file;}
    #endregion

    #region interface
    #region public
    public override bool MoveNext(
      )
    {
      bool moved = base.MoveNext();
      if(moved)
      {
        switch(TokenType)
        {
          case TokenTypeEnum.Integer:
          {
            /*
              NOTE: We need to verify whether indirect reference pattern is applicable:
              ref :=  { int int 'R' }
            */
            IInputStream stream = Stream;
            long baseOffset = stream.Position; // Backs up the recovery position.

            // 1. Object number.
            int objectNumber = (int)Token;
            // 2. Generation number.
            base.MoveNext();
            if(TokenType == TokenTypeEnum.Integer)
            {
              int generationNumber = (int)Token;
              // 3. Reference keyword.
              base.MoveNext();
              if(TokenType == TokenTypeEnum.Keyword
                && Token.Equals(Keyword.Reference))
              {Token = new Reference(objectNumber,generationNumber);}
            }
            if(!(Token is Reference))
            {
              // Rollback!
              stream.Seek(baseOffset);
              Token = objectNumber;
              TokenType = TokenTypeEnum.Integer;
            }
          } break;
        }
      }
      return moved;
    }

    public override PdfDataObject ParsePdfObject(
      )
    {
      switch(TokenType)
      {
        case TokenTypeEnum.Keyword:
          if(Token is Reference)
            return new PdfReference(
              (Reference)Token,
              file
              );
          break;
      }

      PdfDataObject pdfObject = base.ParsePdfObject();
      if(pdfObject is PdfDictionary)
      {
        IInputStream stream = Stream;
        int oldOffset = (int)stream.Position;
        MoveNext();
        // Is this dictionary the header of a stream object [PDF:1.6:3.2.7]?
        if((TokenType == TokenTypeEnum.Keyword)
          && Token.Equals(Keyword.BeginStream))
        {
          PdfDictionary streamHeader = (PdfDictionary)pdfObject;

          // Keep track of current position!
          /*
            NOTE: Indirect reference resolution is an outbound call which affects the stream pointer position,
            so we need to recover our current position after it returns.
          */
          long position = stream.Position;
          // Get the stream length!
          int length = ((PdfInteger)streamHeader.Resolve(PdfName.Length)).IntValue;
          // Move to the stream data beginning!
          stream.Seek(position); SkipEOL();

          // Copy the stream data to the instance!
          byte[] data = new byte[length];
          stream.Read(data);

          MoveNext(); // Postcondition (last token should be 'endstream' keyword).

        Object streamType = streamHeader[PdfName.Type];
        if(PdfName.ObjStm.Equals(streamType)) // Object stream [PDF:1.6:3.4.6].
          return new ObjectStream(
            streamHeader,
            new bytes.Buffer(data)
            );
        else if(PdfName.XRef.Equals(streamType)) // Cross-reference stream [PDF:1.6:3.4.7].
          return new XRefStream(
            streamHeader,
            new bytes.Buffer(data)
            );
        else // Generic stream.
          return new PdfStream(
            streamHeader,
            new bytes.Buffer(data)
            );
        }
        else // Stand-alone dictionary.
        {stream.Seek(oldOffset);} // Restores postcondition (last token should be the dictionary end).
      }
      return pdfObject;
    }

    /**
      <summary>Retrieves the PDF version of the file [PDF:1.6:3.4.1].</summary>
    */
    public string RetrieveVersion(
      )
    {
      IInputStream stream = Stream;
      stream.Seek(0);
      string header = stream.ReadString(10);
      if(!header.StartsWith(Keyword.BOF))
        throw new ParseException("PDF header not found.",stream.Position);

      return header.Substring(Keyword.BOF.Length,3);
    }

    /**
      <summary>Retrieves the starting position of the last xref-table section [PDF:1.6:3.4.4].</summary>
    */
    public long RetrieveXRefOffset(
      )
    {
      IInputStream stream = Stream;
      long streamLength = stream.Length;
      int chunkSize = (int)Math.Min(streamLength, EOFMarkerChunkSize);

      // Move back before 'startxref' keyword!
      long position = streamLength - chunkSize;
      stream.Seek(position);

      // Get 'startxref' keyword position!
      int index = stream.ReadString(chunkSize).LastIndexOf(Keyword.StartXRef);
      if(index < 0)
        throw new ParseException("'" + Keyword.StartXRef + "' keyword not found.", stream.Position);

      // Go past the startxref keyword!
      stream.Seek(position + index); MoveNext();

      // Go to the xref offset!
      MoveNext();
      if(TokenType != TokenTypeEnum.Integer)
        throw new ParseException("'" + Keyword.StartXRef + "' value invalid.", stream.Position);

      return (int)Token;
    }
    #endregion
    #endregion
    #endregion
  }
}