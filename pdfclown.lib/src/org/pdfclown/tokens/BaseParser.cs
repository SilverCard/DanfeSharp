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

using org.pdfclown.bytes;
using org.pdfclown.objects;
using org.pdfclown.util.parsers;

using System;

namespace org.pdfclown.tokens
{
  /**
    <summary>Base PDF parser [PDF:1.7:3.2].</summary>
  */
  public class BaseParser
    : PostScriptParser
  {
    #region dynamic
    #region constructors
    protected BaseParser(
      IInputStream stream
      ) : base(stream)
    {}

    protected BaseParser(
      byte[] data
      ) : base(data)
    {}
    #endregion

    #region interface
    #region public
    public override bool MoveNext(
      )
    {
      bool moved;
      while(moved = base.MoveNext())
      {
        TokenTypeEnum tokenType = TokenType;
        if(tokenType == TokenTypeEnum.Comment)
          continue; // Comments are ignored.

        if(tokenType == TokenTypeEnum.Literal)
        {
          string literalToken = (string)Token;
          if(literalToken.StartsWith(Keyword.DatePrefix)) // Date.
          {
            /*
              NOTE: Dates are a weak extension to the PostScript language.
            */
            try
            {Token = PdfDate.ToDate(literalToken);}
            catch(ParseException)
            {/* NOOP: gently degrade to a common literal. */}
          }
        }
        break;
      }
      return moved;
    }

    /**
      <summary>Parses the current PDF object [PDF:1.6:3.2].</summary>
    */
    public virtual PdfDataObject ParsePdfObject(
      )
    {
      switch(TokenType)
      {
        case TokenTypeEnum.Integer:
          return PdfInteger.Get((int)Token);
        case TokenTypeEnum.Name:
          return new PdfName((string)Token,true);
        case TokenTypeEnum.DictionaryBegin:
        {
          PdfDictionary dictionary = new PdfDictionary();
          dictionary.Updateable = false;
          while(true)
          {
            // Key.
            MoveNext(); if(TokenType == TokenTypeEnum.DictionaryEnd) break;
            PdfName key = (PdfName)ParsePdfObject();
            // Value.
            MoveNext();
            PdfDirectObject value = (PdfDirectObject)ParsePdfObject();
            // Add the current entry to the dictionary!
            dictionary[key] = value;
          }
          dictionary.Updateable = true;
          return dictionary;
        }
        case TokenTypeEnum.ArrayBegin:
        {
          PdfArray array = new PdfArray();
          array.Updateable = false;
          while(true)
          {
            // Value.
            MoveNext(); if(TokenType == TokenTypeEnum.ArrayEnd) break;
            // Add the current item to the array!
            array.Add((PdfDirectObject)ParsePdfObject());
          }
          array.Updateable = true;
          return array;
        }
        case TokenTypeEnum.Literal:
          if(Token is DateTime)
            return PdfDate.Get((DateTime)Token);
          else
            return new PdfTextString(
              Encoding.Pdf.Encode((string)Token)
              );
        case TokenTypeEnum.Hex:
          return new PdfTextString(
            (string)Token,
            PdfString.SerializationModeEnum.Hex
            );
        case TokenTypeEnum.Real:
          return PdfReal.Get((double)Token);
        case TokenTypeEnum.Boolean:
          return PdfBoolean.Get((bool)Token);
        case TokenTypeEnum.Null:
          return null;
        default:
          throw new Exception("Unknown type: " + TokenType);
      }
    }

    /**
      <summary>Parses a PDF object after moving to the given token offset.</summary>
      <param name="offset">Number of tokens to skip before reaching the intended one.</param>
      <seealso cref="ParsePdfObject()"/>
    */
    public PdfDataObject ParsePdfObject(
      int offset
      )
    {
      MoveNext(offset);
      return ParsePdfObject();
    }
    #endregion
    #endregion
    #endregion
  }
}

