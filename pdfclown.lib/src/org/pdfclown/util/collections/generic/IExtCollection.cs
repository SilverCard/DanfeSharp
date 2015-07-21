/*
  Copyright 2006-2010 Stefano Chizzolini. http://www.pdfclown.org

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
  /**
    <summary>Extension collection interface.</summary>
  */
  public interface IExtCollection<T>
  {
    /**
      <summary>Appends all of the specified-collection's items to the end of the list.</summary>
      <param name="items">Collection of items to append.</param>
    */
    void AddAll<TVar>(
      ICollection<TVar> items
      )
      where TVar : T;

    /**
      <summary>Removes all of the specified-collection's items from the list.</summary>
      <param name="items">Collection of items to remove.</param>
    */
    void RemoveAll<TVar>(
      ICollection<TVar> items
      )
      where TVar : T;

    /**
      <summary>Removes all the items that match the conditions defined by the specified
      predicate.</summary>
      <param name="match">The <see cref="Predicate"/> delegate that defines the conditions of the items
      to remove.</param>
      <returns>The number of items removed from the collection.</returns>
    */
    int RemoveAll(
      Predicate<T> match
      );
  }
}