/*
  Copyright 2010-2011 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.objects;
using org.pdfclown.util;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace org.pdfclown.documents.contents.fonts
{
  /**
    <summary>Character map [PDF:1.6:5.6.4].</summary>
  */
  internal sealed class CMap
  {
    #region static
    #region interface
    /**
      <summary>Gets the character map extracted from the given data.</summary>
      <param name="stream">Character map data.</param>
    */
    public static IDictionary<ByteArray,int> Get(
      bytes::IInputStream stream
      )
    {
      CMapParser parser = new CMapParser(stream);
      return parser.Parse();
    }

    /**
      <summary>Gets the character map extracted from the given encoding object.</summary>
      <param name="encodingObject">Encoding object.</param>
    */
    public static IDictionary<ByteArray,int> Get(
      PdfDataObject encodingObject
      )
    {
      if(encodingObject == null)
        return null;

      if(encodingObject is PdfName) // Predefined CMap.
        return Get((PdfName)encodingObject);
      else if(encodingObject is PdfStream) // Embedded CMap file.
        return Get((PdfStream)encodingObject);
      else
        throw new NotSupportedException("Unknown encoding object type: " + encodingObject.GetType().Name);
    }

    /**
      <summary>Gets the character map extracted from the given data.</summary>
      <param name="stream">Character map data.</param>
    */
    public static IDictionary<ByteArray,int> Get(
      PdfStream stream
      )
    {return Get(stream.Body);}

    /**
      <summary>Gets the character map corresponding to the given name.</summary>
      <param name="name">Predefined character map name.</param>
      <returns>null, in case no name matching occurs.</returns>
    */
    public static IDictionary<ByteArray,int> Get(
      PdfName name
      )
    {return Get((string)name.Value);}

    /**
      <summary>Gets the character map corresponding to the given name.</summary>
      <param name="name">Predefined character map name.</param>
      <returns>null, in case no name matching occurs.</returns>
    */
    public static IDictionary<ByteArray,int> Get(
      string name
      )
    {
      IDictionary<ByteArray,int> cmap;
      {
        Stream cmapResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("fonts.cmap." + name);
        if(cmapResourceStream == null)
          return null;

        cmap = Get(new bytes::Buffer(cmapResourceStream));
      }
      return cmap;
    }
    #endregion
    #endregion

    #region constructors
    private CMap(
      )
    {}
    #endregion
  }
}