/*
  Copyright 2011-2012 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.documents.contents.objects;
using org.pdfclown.objects;
using org.pdfclown.tokens;

using System;
using System.Collections.Generic;

namespace org.pdfclown.documents.contents.tokens
{
  /**
    <summary>Content stream parser [PDF:1.6:3.7.1].</summary>
  */
  public sealed class ContentParser
    : BaseParser
  {
    #region dynamic
    #region constructors
    internal ContentParser(
      bytes::IInputStream stream
      ) : base(stream)
    {}

    public ContentParser(
      byte[] data
      ) : base(data)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Parses the next content object [PDF:1.6:4.1].</summary>
    */
    public ContentObject ParseContentObject(
      )
    {
      Operation operation = ParseOperation();
      if(operation is PaintXObject) // External object.
        return new XObject((PaintXObject)operation);
      else if(operation is PaintShading) // Shading.
        return new Shading((PaintShading)operation);
      else if(operation is BeginSubpath
        || operation is DrawRectangle) // Path.
        return ParsePath(operation);
      else if(operation is BeginText) // Text.
        return new Text(
          ParseContentObjects()
          );
      else if(operation is SaveGraphicsState) // Local graphics state.
        return new LocalGraphicsState(
          ParseContentObjects()
          );
      else if(operation is BeginMarkedContent) // Marked-content sequence.
        return new MarkedContent(
          (BeginMarkedContent)operation,
          ParseContentObjects()
          );
      else if(operation is BeginInlineImage) // Inline image.
        return ParseInlineImage();
      else // Single operation.
        return operation;
    }

    /**
      <summary>Parses the next content objects.</summary>
    */
    public IList<ContentObject> ParseContentObjects(
      )
    {
      List<ContentObject> contentObjects = new List<ContentObject>();
      while(MoveNext())
      {
        ContentObject contentObject = ParseContentObject();
        // Multiple-operation graphics object end?
        if(contentObject is EndText // Text.
          || contentObject is RestoreGraphicsState // Local graphics state.
          || contentObject is EndMarkedContent // End marked-content sequence.
          || contentObject is EndInlineImage) // Inline image.
          return contentObjects;

        contentObjects.Add(contentObject);
      }
      return contentObjects;
    }

    /**
      <summary>Parses the next operation.</summary>
    */
    public Operation ParseOperation(
      )
    {
      string operator_ = null;
      List<PdfDirectObject> operands = new List<PdfDirectObject>();
      // Parsing the operation parts...
      do
      {
        switch(TokenType)
        {
          case TokenTypeEnum.Keyword:
            operator_ = (string)Token;
            break;
          default:
            operands.Add((PdfDirectObject)ParsePdfObject());
            break;
        }
      } while(operator_ == null && MoveNext());
      return Operation.Get(operator_,operands);
    }

    public override PdfDataObject ParsePdfObject(
      )
    {
      switch(TokenType)
      {
        case TokenTypeEnum.Literal:
          if(Token is string)
            return new PdfString(
              Encoding.Pdf.Encode((string)Token),
              PdfString.SerializationModeEnum.Literal
              );
          break;
        case TokenTypeEnum.Hex:
          return new PdfString(
            (string)Token,
            PdfString.SerializationModeEnum.Hex
            );
      }
      return base.ParsePdfObject();
    }
    #endregion

    #region private
    private InlineImage ParseInlineImage(
      )
    {
      /*
        NOTE: Inline images use a peculiar syntax that's an exception to the usual rule
        that the data in a content stream is interpreted according to the standard PDF syntax
        for objects.
      */
      InlineImageHeader header;
      {
        List<PdfDirectObject> operands = new List<PdfDirectObject>();
        // Parsing the image entries...
        while(MoveNext()
          && TokenType != TokenTypeEnum.Keyword) // Not keyword (i.e. end at image data beginning (ID operator)).
        {operands.Add((PdfDirectObject)ParsePdfObject());}
        header = new InlineImageHeader(operands);
      }

      InlineImageBody body;
      {
        bytes::IInputStream stream = Stream;
        MoveNext();
        bytes::Buffer data = new bytes::Buffer();
        byte prevByte = 0;
        while(true)
        {
          byte curByte = (byte)stream.ReadByte();
          if(prevByte == 'E' && curByte == 'I')
            break;

          data.Append(prevByte = curByte);
        }
        body = new InlineImageBody(data);
      }

      return new InlineImage(
        header,
        body
        );
    }

    private Path ParsePath(
      Operation beginOperation
      )
    {
      /*
        NOTE: Paths do not have an explicit end operation, so we must infer it
        looking for the first non-painting operation.
      */
      IList<ContentObject> operations = new List<ContentObject>();
      {
        operations.Add(beginOperation);
        long position = Position;
        bool closeable = false;
        while(MoveNext())
        {
          Operation operation = ParseOperation();
          // Multiple-operation graphics object closeable?
          if(operation is PaintPath) // Painting operation.
          {closeable = true;}
          else if(closeable) // Past end (first non-painting operation).
          {
            Seek(position); // Rolls back to the last path-related operation.

            break;
          }

          operations.Add(operation);
          position = Position;
        }
      }
      return new Path(operations);
    }
    #endregion
    #endregion
    #endregion
  }
}