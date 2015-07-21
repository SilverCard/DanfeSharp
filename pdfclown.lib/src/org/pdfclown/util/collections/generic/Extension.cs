/*
  Copyright 2010 Stefano Chizzolini. http://www.pdfclown.org

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
using System.Collections.Generic;

namespace org.pdfclown.util.collections.generic
{
  public static class Extension
  {
    public static void AddAll<T>(
      this ICollection<T> collection,
      IEnumerable<T> enumerable
      )
    {
      foreach(T item in enumerable)
      {collection.Add(item);}
    }

    public static void RemoveAll<T>(
      this ICollection<T> collection,
      IEnumerable<T> enumerable
      )
    {
      foreach(T item in enumerable)
      {collection.Remove(item);}
    }

    /**
      <summary>Sets all the specified entries into this dictionary.</summary>
      <remarks>The effect of this call is equivalent to that of calling the indexer on this dictionary
      once for each entry in the specified enumerable.</remarks>
    */
    public static void SetAll<TKey,TValue>(
      this IDictionary<TKey,TValue> dictionary,
      IEnumerable<KeyValuePair<TKey,TValue>> enumerable
      )
    {
      foreach(KeyValuePair<TKey,TValue> entry in enumerable)
      {dictionary[entry.Key] = entry.Value;}
    }
  }
}

