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

using org.pdfclown.bytes;
using org.pdfclown.files;
using tokens = org.pdfclown.tokens;
using org.pdfclown.util;

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace org.pdfclown.objects
{
  /**
    <summary>PDF string object [PDF:1.6:3.2.3].</summary>
    <remarks>
      <para>A string object consists of a series of bytes.</para>
      <para>String objects can be serialized in two ways:</para>
      <list type="bullet">
        <item>as a sequence of literal characters (plain form)</item>
        <item>as a sequence of hexadecimal digits (hexadecimal form)</item>
      </list>
    </remarks>
  */
  public class PdfString
    : PdfSimpleObject<byte[]>,
      IDataWrapper
  {
    /*
      NOTE: String objects are internally represented as unescaped sequences of bytes.
      Escaping is applied on serialization only.
    */
    #region types
    /**
      <summary>String serialization mode.</summary>
    */
    public enum SerializationModeEnum
    {
      /**
        Plain form.
      */
      Literal,
      /**
        Hexadecimal form.
      */
      Hex
    };
    #endregion

    #region static
    #region fields
    public static readonly PdfString Default = new PdfString("");

    private const byte BackspaceCode = 8;
    private const byte CarriageReturnCode = 13;
    private const byte FormFeedCode = 12;
    private const byte HorizontalTabCode = 9;
    private const byte LineFeedCode = 10;

    private const byte HexLeftDelimiterCode = 60;
    private const byte HexRightDelimiterCode = 62;
    private const byte LiteralEscapeCode = 92;
    private const byte LiteralLeftDelimiterCode = 40;
    private const byte LiteralRightDelimiterCode = 41;
    #endregion
    #endregion

    #region dynamic
    #region fields
    private SerializationModeEnum serializationMode = SerializationModeEnum.Literal;
    #endregion

    #region constructors
    public PdfString(
      byte[] rawValue
      )
    {RawValue = rawValue;}

    public PdfString(
      string value
      )
    {Value = value;}

    public PdfString(
      byte[] rawValue,
      SerializationModeEnum serializationMode
      )
    {
      SerializationMode = serializationMode;
      RawValue = rawValue;
    }

    public PdfString(
      string value,
      SerializationModeEnum serializationMode
      )
    {
      SerializationMode = serializationMode;
      Value = value;
    }

    protected PdfString(
      )
    {}
    #endregion

    #region interface
    #region public
    public override PdfObject Accept(
      IVisitor visitor,
      object data
      )
    {return visitor.Visit(this, data);}

    public override int CompareTo(
      PdfDirectObject obj
      )
    {
      if(!(obj is PdfString))
        throw new ArgumentException("Object MUST be a PdfString");

      return String.CompareOrdinal(StringValue, ((PdfString)obj).StringValue);
    }

    /**
      <summary>Gets/Sets the serialization mode.</summary>
    */
    public virtual SerializationModeEnum SerializationMode
    {
      get
      {return serializationMode;}
      set
      {serializationMode = value;}
    }

    public string StringValue
    {
      get
      {return (string)Value;}
    }

    public byte[] ToByteArray(
      )
    {return (byte[])RawValue.Clone();}

    public override string ToString(
      )
    {
      switch(serializationMode)
      {
        case SerializationModeEnum.Hex:
          return "<" + base.ToString() + ">";
        case SerializationModeEnum.Literal:
          return "(" + base.ToString() + ")";
        default:
          throw new NotImplementedException();
      }
    }

    public override object Value
    {
      get
      {
        switch(serializationMode)
        {
          case SerializationModeEnum.Literal:
            return tokens::Encoding.Pdf.Decode(RawValue);
          case SerializationModeEnum.Hex:
            return ConvertUtils.ByteArrayToHex(RawValue);
          default:
            throw new NotImplementedException(serializationMode + " serialization mode is not implemented.");
        }
      }
      protected set
      {
        switch(serializationMode)
        {
          case SerializationModeEnum.Literal:
            RawValue = tokens::Encoding.Pdf.Encode((string)value);
            break;
          case SerializationModeEnum.Hex:
            RawValue = ConvertUtils.HexToByteArray((string)value);
            break;
          default:
            throw new NotImplementedException(serializationMode + " serialization mode is not implemented.");
        }
      }
    }

    public override void WriteTo(
      IOutputStream stream,
      files.File context
      )
    {
      MemoryStream buffer = new MemoryStream();
      {
        byte[] rawValue = RawValue;
        switch(serializationMode)
        {
          case SerializationModeEnum.Literal:
            buffer.WriteByte(LiteralLeftDelimiterCode);
            /*
              NOTE: Literal lexical conventions prescribe that the following reserved characters
              are to be escaped when placed inside string character sequences:
                - \n Line feed (LF)
                - \r Carriage return (CR)
                - \t Horizontal tab (HT)
                - \b Backspace (BS)
                - \f Form feed (FF)
                - \( Left parenthesis
                - \) Right parenthesis
                - \\ Backslash
            */
            for(
              int index = 0;
              index < rawValue.Length;
              index++
              )
            {
              byte valueByte = rawValue[index];
              switch(valueByte)
              {
                case LineFeedCode:
                  buffer.WriteByte(LiteralEscapeCode); valueByte = 110; break;
                case CarriageReturnCode:
                  buffer.WriteByte(LiteralEscapeCode); valueByte = 114; break;
                case HorizontalTabCode:
                  buffer.WriteByte(LiteralEscapeCode); valueByte = 116; break;
                case BackspaceCode:
                  buffer.WriteByte(LiteralEscapeCode); valueByte = 98; break;
                case FormFeedCode:
                  buffer.WriteByte(LiteralEscapeCode); valueByte = 102; break;
                case LiteralLeftDelimiterCode:
                case LiteralRightDelimiterCode:
                case LiteralEscapeCode:
                  buffer.WriteByte(LiteralEscapeCode); break;
              }
              buffer.WriteByte(valueByte);
            }
            buffer.WriteByte(LiteralRightDelimiterCode);
            break;
          case SerializationModeEnum.Hex:
            buffer.WriteByte(HexLeftDelimiterCode);
            byte[] value = tokens::Encoding.Pdf.Encode(ConvertUtils.ByteArrayToHex(rawValue));
            buffer.Write(value,0,value.Length);
            buffer.WriteByte(HexRightDelimiterCode);
            break;
          default:
            throw new NotImplementedException();
        }
      }
      stream.Write(buffer.ToArray());
    }
    #endregion
    #endregion
    #endregion
  }
}