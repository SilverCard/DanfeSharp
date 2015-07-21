/*
  Copyright 2009-2010 Stefano Chizzolini. http://www.pdfclown.org

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

using System;
using System.Linq;
using System.Text;

namespace org.pdfclown.util
{
  /**
    <summary>Byte array.</summary>
  */
  /*
    NOTE: This class is useful when applied as key for dictionaries using the default IEqualityComparer.
  */
  public struct ByteArray
  {
    public readonly byte[] Data; //TODO: yes, I know it's risky (temporary simplification)...

    public ByteArray(byte[] data)
    {Array.Copy(data,this.Data = new byte[data.Length],data.Length);}

    public override bool Equals(
      object obj
      )
    {
      return obj is ByteArray
        && Data.SequenceEqual(((ByteArray)obj).Data);
    }

    public override int GetHashCode(
      )
    {
      int hashCode = 0;
      for(int index = 0, length = Data.Length; index < length; index++)
      {hashCode ^= Data[index] << (8 * (index % 4));}
      return hashCode;
    }

    public override string ToString(
      )
    {
      StringBuilder builder = new StringBuilder("[");
      {
        foreach(byte datum in Data)
        {
          if(builder.Length > 1)
          {builder.Append(",");}

          builder.Append(datum & 0xFF);
        }
        builder.Append("]");
      }
      return builder.ToString();
    }
  }
}