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
using org.pdfclown.tokens;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace org.pdfclown.documents.contents.fonts
{
  /**
    <summary>CFF file format parser [CFF:1.0].</summary>
  */
  internal sealed class CffParser
  {
    #region types
    /**
      <summary>Dictionary [CFF:1.0:4].</summary>
    */
    private sealed class Dict
      : IDictionary<int,IList<object>>
    {
      public enum OperatorEnum
      {
        Charset = 15,
        CharStrings = 17,
        CharstringType = 6 + OperatorValueEscape,
        Encoding = 16
      }

      private const int OperatorValueEscape = 12 << 8;

      public static string GetOperatorName(
        OperatorEnum value
        )
      {
        switch(value)
        {
          case OperatorEnum.Charset:
            return "charset";
          default:
            return value.ToString();
        }
      }

      public static Dict Parse(
        byte[] data
        )
      {return Parse(new bytes.Buffer(data));}

      public static Dict Parse(
        IInputStream stream
        )
      {
        IDictionary<int,IList<object>> entries = new Dictionary<int,IList<object>>();
        IList<object> operands = null;
        while(true)
        {
          int b0 = stream.ReadByte();
          if(b0 == -1)
            break;

          if(b0 >= 0 && b0 <= 21) // Operator.
          {
            int operator_ = b0;
            if(b0 == 12) // 2-byte operator.
            {operator_ = operator_ << 8 + stream.ReadByte();}

            /*
              NOTE: In order to resiliently support unknown operators on parsing, parsed operators
              are not directly mapped to OperatorEnum.
            */
            entries[operator_] = operands;
            operands = null;
          }
          else // Operand.
          {
            if(operands == null)
            {operands = new List<object>();}

            if(b0 == 28) // 3-byte integer.
            {operands.Add(stream.ReadByte() << 8 + stream.ReadByte());}
            else if(b0 == 29) // 5-byte integer.
            {operands.Add(stream.ReadByte() << 24 + stream.ReadByte() << 16 + stream.ReadByte() << 8 + stream.ReadByte());}
            else if(b0 == 30) // Variable-length real.
            {
              StringBuilder operandBuilder = new StringBuilder();
              bool ended = false;
              do
              {
                int b = stream.ReadByte();
                int[] nibbles = {(b >> 4) & 0xf, b & 0xf};
                foreach(int nibble in nibbles)
                {
                  switch (nibble)
                  {
                    case 0x0:
                    case 0x1:
                    case 0x2:
                    case 0x3:
                    case 0x4:
                    case 0x5:
                    case 0x6:
                    case 0x7:
                    case 0x8:
                    case 0x9:
                      operandBuilder.Append(nibble);
                      break;
                    case 0xa: // Decimal point.
                      operandBuilder.Append(".");
                      break;
                    case 0xb: // Positive exponent.
                      operandBuilder.Append("E");
                      break;
                    case 0xc: // Negative exponent.
                      operandBuilder.Append("E-");
                      break;
                    case 0xd: // Reserved.
                      break;
                    case 0xe: // Minus.
                      operandBuilder.Append("-");
                      break;
                    case 0xf: // End of number.
                      ended = true;
                      break;
                  }
                }
              } while(!ended);
              operands.Add(
                Double.Parse(
                  operandBuilder.ToString(),
                  NumberStyles.Float
                  )
                );
            }
            else if (b0 >= 32 && b0 <= 246) // 1-byte integer.
            {operands.Add(b0 - 139);}
            else if (b0 >= 247 && b0 <= 250) // 2-byte positive integer.
            {operands.Add((b0 - 247) << 8 + stream.ReadByte() + 108);}
            else if (b0 >= 251 && b0 <= 254) // 2-byte negative integer.
            {operands.Add(-(b0 - 251) << 8 - stream.ReadByte() - 108);}
            else // Reserved.
            { /* NOOP */ }
          }
        }
        return new Dict(entries);
      }

      private readonly IDictionary<int,IList<object>> entries;

      private Dict(
        IDictionary<int,IList<object>> entries
        )
      {this.entries = entries;}

      public void Add(
        int key,
        IList<object> value
        )
      {throw new NotSupportedException();}

      public bool ContainsKey(
        int key
        )
      {return entries.ContainsKey(key);}

      public ICollection<int> Keys
      {
        get
        {return entries.Keys;}
      }

      public bool Remove(
        int key
        )
      {throw new NotSupportedException();}

      public IList<object> this[
        int key
        ]
      {
        get
        {IList<object> value; entries.TryGetValue(key,out value); return value;}
        set
        {throw new NotSupportedException();}
      }

      public bool TryGetValue(
        int key,
        out IList<object> value
        )
      {return entries.TryGetValue(key,out value);}

      public ICollection<IList<object>> Values
      {
        get
        {return entries.Values;}
      }

      void ICollection<KeyValuePair<int,IList<object>>>.Add(
        KeyValuePair<int,IList<object>> keyValuePair
        )
      {throw new NotSupportedException();}

      public void Clear(
        )
      {throw new NotSupportedException();}

      bool ICollection<KeyValuePair<int,IList<object>>>.Contains(
        KeyValuePair<int,IList<object>> keyValuePair
        )
      {return entries.Contains(keyValuePair);}

      public void CopyTo(
        KeyValuePair<int,IList<object>>[] keyValuePairs,
        int index
        )
      {throw new NotImplementedException();}

      public int Count
      {
        get
        {return entries.Count;}
      }

      public bool IsReadOnly
      {
        get
        {return true;}
      }

      public bool Remove(
        KeyValuePair<int,IList<object>> keyValuePair
        )
      {throw new NotSupportedException();}

      IEnumerator<KeyValuePair<int,IList<object>>> IEnumerable<KeyValuePair<int,IList<object>>>.GetEnumerator(
        )
      {return entries.GetEnumerator();}

      IEnumerator IEnumerable.GetEnumerator(
        )
      {return ((IEnumerable<KeyValuePair<int,IList<object>>>)this).GetEnumerator();}

      public object Get(
        OperatorEnum operator_,
        int operandIndex
        )
      {return Get(operator_, operandIndex, null);}

      public object Get(
        OperatorEnum operator_,
        int operandIndex,
        int? defaultValue
        )
      {
        IList<object> operands = this[(int)operator_];
        return operands != null ? operands[operandIndex] : defaultValue;
      }
    }

    /**
      <summary>Array of variable-sized objects [CFF:1.0:5].</summary>
    */
    private sealed class Index
      : IList<byte[]>
    {
      public static Index Parse(
        byte[] data
        )
      {return Parse(new bytes.Buffer(data));}

      public static Index Parse(
        IInputStream stream
        )
      {
        byte[][] data = new byte[stream.ReadUnsignedShort()][];
        {
          int[] offsets = new int[data.Length + 1];
          int offSize = stream.ReadByte();
          for (int index = 0, count = offsets.Length; index < count; index++)
          {offsets[index] = stream.ReadInt(offSize);}
          for (int index = 0, count = data.Length; index < count; index++)
          {stream.Read(data[index] = new byte[offsets[index + 1] - offsets[index]]);}
        }
        return new Index(data);
      }

      public static Index Parse(
        IInputStream stream,
        int offset
        )
      {
        stream.Position = offset;
        return Parse(stream);
      }

      private readonly byte[][] data;

      private Index(
        byte[][] data
        )
      {this.data = data;}

      public int IndexOf(
        byte[] item
        )
      {throw new NotImplementedException();}

      public void Insert(
        int index,
        byte[] item
        )
      {throw new NotSupportedException();}

      public void RemoveAt(
        int index
        )
      {throw new NotSupportedException();}

      public byte[] this[
        int index
        ]
      {
        get
        {return data[index];}
        set
        {throw new NotSupportedException();}
      }

      public void Add(
        byte[] item
        )
      {throw new NotSupportedException();}

      public void Clear(
        )
      {throw new NotSupportedException();}

      public bool Contains(
        byte[] item
        )
      {throw new NotImplementedException();}

      public void CopyTo(
        byte[][] items,
        int index
        )
      {throw new NotImplementedException();}

      public int Count
      {
        get
        {return data.Length;}
      }

      public bool IsReadOnly
      {
        get
        {return true;}
      }

      public bool Remove(
        byte[] item
        )
      {throw new NotSupportedException();}

      public IEnumerator<byte[]> GetEnumerator(
        )
      {
        for(int index = 0, length = Count; index < length; index++)
        {yield return this[index];}
      }
  
      IEnumerator IEnumerable.GetEnumerator(
        )
      {return this.GetEnumerator();}
    }

    /**
      <summary>Predefined charsets [CFF:1.0:12,C].</summary>
    */
    private enum StandardCharsetEnum
    {
      ISOAdobe = 0,
      Expert = 1,
      ExpertSubset = 2
    }
    #endregion

    #region static
    #region fields
    /**
      <summary>Standard charset maps.</summary>
    */
    private static readonly IDictionary<StandardCharsetEnum,IDictionary<int,int>> StandardCharsets;

    /**
      <summary>Standard Strings [CFF:1.0:10] represent commonly occurring strings allocated to
      predefined SIDs.</summary>
    */
    private static readonly IList<string> StandardStrings;
    #endregion

    #region constructors
    static CffParser()
    {
      StandardCharsets = new Dictionary<StandardCharsetEnum,IDictionary<int,int>>();
      foreach(StandardCharsetEnum charset in Enum.GetValues(typeof(StandardCharsetEnum)))
      {
        IDictionary<int,int> charsetMap = new Dictionary<int,int>();
        {
          StreamReader stream = null;
          try
          {
            // Open the resource!
            stream = new StreamReader(
              Assembly.GetExecutingAssembly().GetManifestResourceStream("fonts.cff." + charset.ToString() + "Charset")
              );
            // Parsing the resource...
            String line;
            while((line = stream.ReadLine()) != null)
            {
              string[] lineItems = line.Split(',');
              charsetMap[Int32.Parse(lineItems[0])] = GlyphMapping.NameToCode(lineItems[1]).Value;
            }
          }
          catch(Exception e)
          {throw;}
          finally
          {
            if(stream != null)
            {stream.Close();}
          }
        }
      }

      StandardStrings = new List<string>();
      {
        StreamReader stream = null;
        try
        {
          // Open the resource!
          stream = new StreamReader(
            Assembly.GetExecutingAssembly().GetManifestResourceStream("fonts.cff.StandardStrings")
            );
          // Parsing the resource...
          string line;
          while((line = stream.ReadLine()) != null)
          {StandardStrings.Add(line);}
        }
        catch(Exception e)
        {throw;}
        finally
        {
          if(stream != null)
          {stream.Close();}
        }
      }
    }
    #endregion

    #region interface
    #region private
    /**
      <summary>Gets the charset corresponding to the given value.</summary>
    */
    private static StandardCharsetEnum? GetStandardCharset(
      int? value
      )
    {
      if(!value.HasValue)
        return StandardCharsetEnum.ISOAdobe;
      else if(!Enum.IsDefined(typeof(StandardCharsetEnum),value.Value))
        return null;
      else
        return (StandardCharsetEnum)value.Value;
    }

    private static string ToString(
      byte[] data
      )
    {return Charset.ISO88591.GetString(data);}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region fields
    public IDictionary<int,int> glyphIndexes;

    private readonly IInputStream fontData;
    private Index stringIndex;
    #endregion

    #region constructors
    internal CffParser(
      IInputStream fontData
      )
    {
      this.fontData = fontData;

      Load();
    }
    #endregion

    #region interface
    #region private
    /**
      <summary>Loads the font data.</summary>
    */
    private void Load(
      )
    {
      try
      {
        ParseHeader();
        Index nameIndex = Index.Parse(fontData);
        Index topDictIndex = Index.Parse(fontData);
        stringIndex = Index.Parse(fontData);
        #pragma warning disable 0219
        Index globalSubrIndex = Index.Parse(fontData);

        string fontName = ToString(nameIndex[0]);
        #pragma warning restore 0219
        Dict topDict = Dict.Parse(topDictIndex[0]);

  //      int encodingOffset = topDict.get(Dict.OperatorEnum.Encoding, 0, 0).intValue();
        //TODO: encoding

        #pragma warning disable 0219
        int charstringType = (int)topDict.Get(Dict.OperatorEnum.CharstringType, 0, 2);
        #pragma warning restore 0219
        int charStringsOffset = (int)topDict.Get(Dict.OperatorEnum.CharStrings, 0);
        Index charStringsIndex = Index.Parse(fontData, charStringsOffset);

        int charsetOffset = (int)topDict.Get(Dict.OperatorEnum.Charset, 0, 0);
        StandardCharsetEnum? charset = GetStandardCharset(charsetOffset);
        if(charset.HasValue)
        {
          glyphIndexes = new Dictionary<int,int>(StandardCharsets[charset.Value]);
        }
        else
        {
          glyphIndexes = new Dictionary<int,int>();
          fontData.Position = charsetOffset;
          int charsetFormat = fontData.ReadByte();
          for (int index = 1, count = charStringsIndex.Count; index <= count;)
          {
            switch(charsetFormat)
            {
              case 0:
                glyphIndexes[index++] = ToUnicode(fontData.ReadUnsignedShort());
                break;
              case 1:
              case 2:
              {
                int first = fontData.ReadUnsignedShort();
                int nLeft = (charsetFormat == 1 ? fontData.ReadByte() : fontData.ReadUnsignedShort());
                for (int rangeItemIndex = first, rangeItemEndIndex = first + nLeft; rangeItemIndex <= rangeItemEndIndex; rangeItemIndex++)
                {glyphIndexes[index++] = ToUnicode(rangeItemIndex);}
              }
                break;
            }
          }
        }
      }
      catch(Exception e)
      {throw;}
    }

    /**
      <summary>Gets the string corresponding to the specified identifier.</summary>
      <param name="id">SID (String ID).</param>
    */
    private string GetString(
      int id
      )
    {
      return id < StandardStrings.Count
        ? StandardStrings[id]
        : ToString(stringIndex[id - StandardStrings.Count]);
    }

    private void ParseHeader(
      )
    {
      fontData.Seek(2);
      int hdrSize = fontData.ReadByte();
      // Skip to the end of the header!
      fontData.Seek(hdrSize);
    }

    private int ToUnicode(
      int sid
      )
    {
      /*
       * FIXME: avoid Unicode resolution at this stage -- names should be kept to allow subsequent
       * character substitution (see font differences) in case of custom (non-unicode) encodings.
       */
      int? code = GlyphMapping.NameToCode(GetString(sid));
      if(!code.HasValue)
      {
        //custom code
        code = sid; // really bad
      }
      return code.Value;
    }
    #endregion
    #endregion
    #endregion
  }
}