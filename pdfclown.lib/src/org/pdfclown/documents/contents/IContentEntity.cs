/*
  Copyright 2007-2010 Stefano Chizzolini. http://www.pdfclown.org

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

using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;
using org.pdfclown.documents.contents.objects;
using org.pdfclown.documents.contents.xObjects;

namespace org.pdfclown.documents.contents
{
  /**
    <summary>
      <para>Generic content entity.</para>
      <para>It provides common ways to convert any content into content stream objects.</para>
    </summary>
  */
  public interface IContentEntity
  {
    /**
      <summary>Converts this entity to its equivalent inline (dependent) object representation
      [PDF:1.6:4.8.6].</summary>
      <remarks>
        <para>This method creates and shows an inline object within the target content context,
        returning it.</para>
        <para>Due to its direct-content nature (opposite to the indirect-content nature of
        external objects), the resulting object should be shown only one time in order not to
        wastefully duplicate its data.</para>
      </remarks>
      <param name="composer">Target content composer.</param>
      <returns>The inline object representing the entity.</returns>
    */
    ContentObject ToInlineObject(
      PrimitiveComposer composer
      );

    /**
      <summary>Converts this entity to its equivalent external (independent) object representation
      [PDF:1.6:4.7].</summary>
      <remarks>
        <para>This method creates an external object within the target document, returning it.
        To show it in a content context (eg: a page), then it must be applied in an appropriate manner
        (see PrimitiveComposer object).</para>
      </remarks>
      <param name="context">Target document.</param>
      <returns>The external object representing the entity.</returns>
    */
    xObjects.XObject ToXObject(
      Document context
      );
  }
}