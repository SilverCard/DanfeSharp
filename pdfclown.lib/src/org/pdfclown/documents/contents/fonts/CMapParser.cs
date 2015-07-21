/*
  Copyright 2009-2011 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.util;
using org.pdfclown.util.math;
using org.pdfclown.util.parsers;

using System;
using System.Collections.Generic;
using System.Globalization;
using io = System.IO;
using System.Text;

namespace org.pdfclown.documents.contents.fonts
{
  /**
    <summary>CMap parser [PDF:1.6:5.6.4;CMAP].</summary>
  */
  internal sealed class CMapParser
    : PostScriptParser
  {
    #region static
    #region fields
    private static readonly string BeginBaseFontCharOperator = "beginbfchar";
    private static readonly string BeginBaseFontRangeOperator = "beginbfrange";
    private static readonly string BeginCIDCharOperator = "begincidchar";
    private static readonly string BeginCIDRangeOperator = "begincidrange";
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public CMapParser(
      io::Stream stream
      ) : this(new bytes::Buffer(stream))
    {}

    public CMapParser(
      bytes::IInputStream stream
      ) : base(stream)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Parses the character-code-to-unicode mapping [PDF:1.6:5.9.1].</summary>
    */
    public IDictionary<ByteArray,int> Parse(
      )
    {
      Stream.Position = 0;
      IDictionary<ByteArray,int> codes = new Dictionary<ByteArray,int>();
      {
        int itemCount = 0;
        while(MoveNext())
        {
          switch(TokenType)
          {
            case TokenTypeEnum.Keyword:
            {
              string operator_ = (string)Token;
              if(operator_.Equals(BeginBaseFontCharOperator)
                || operator_.Equals(BeginCIDCharOperator))
              {
                /*
                  NOTE: The first element on each line is the input code of the template font;
                  the second element is the code or name of the character.
                */
                for(
                  int itemIndex = 0;
                  itemIndex < itemCount;
                  itemIndex++
                  )
                {
                  MoveNext();
                  ByteArray inputCode = new ByteArray(ParseInputCode());
                  MoveNext();
                  codes[inputCode] = ParseUnicode();
                }
              }
              else if(operator_.Equals(BeginBaseFontRangeOperator)
                || operator_.Equals(BeginCIDRangeOperator))
              {
                /*
                  NOTE: The first and second elements in each line are the beginning and
                  ending valid input codes for the template font; the third element is
                  the beginning character code for the range.
                */
                for(
                  int itemIndex = 0;
                  itemIndex < itemCount;
                  itemIndex++
                  )
                {
                  // 1. Beginning input code.
                  MoveNext();
                  byte[] beginInputCode = ParseInputCode();
                  // 2. Ending input code.
                  MoveNext();
                  byte[] endInputCode = ParseInputCode();
                  // 3. Character codes.
                  MoveNext();
                  switch(TokenType)
                  {
                    case TokenTypeEnum.ArrayBegin:
                    {
                      byte[] inputCode = beginInputCode;
                      while(MoveNext()
                        && TokenType != TokenTypeEnum.ArrayEnd)
                      {
                        codes[new ByteArray(inputCode)] = ParseUnicode();
                        OperationUtils.Increment(inputCode);
                      }
                      break;
                    }
                    default:
                    {
                      byte[] inputCode = beginInputCode;
                      int charCode = ParseUnicode();
                      int endCharCode = charCode + (ConvertUtils.ByteArrayToInt(endInputCode) - ConvertUtils.ByteArrayToInt(beginInputCode));
                      while(true)
                      {
                        codes[new ByteArray(inputCode)] = charCode;
                        if(charCode == endCharCode)
                          break;

                        OperationUtils.Increment(inputCode);
                        charCode++;
                      }
                      break;
                    }
                  }
                }
              }
              break;
            }
            case TokenTypeEnum.Integer:
            {
              itemCount = (int)Token;
              break;
            }
          }
        }
      }
      return codes;
    }
    #endregion

    #region private
    /**
      <summary>Converts the current token into its input code value.</summary>
    */
    private byte[] ParseInputCode(
      )
    {return ConvertUtils.HexToByteArray((string)Token);}

    /**
      <summary>Converts the current token into its Unicode value.</summary>
    */
    private int ParseUnicode(
      )
    {
      switch(TokenType)
      {
        case TokenTypeEnum.Hex: // Character code in hexadecimal format.
          return Int32.Parse((string)Token,NumberStyles.HexNumber);
        case TokenTypeEnum.Integer: // Character code in plain format.
          return (int)Token;
        case TokenTypeEnum.Name: // Character name.
          return GlyphMapping.NameToCode((string)Token).Value;
        default:
          throw new Exception(
            "Hex string, integer or name expected instead of " + TokenType
            );
      }
    }
    #endregion
    #endregion
    #endregion
  }
}