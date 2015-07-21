/*
  Copyright 2006-2012 Stefano Chizzolini. http://www.pdfclown.org

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

using org.pdfclown.files;

namespace org.pdfclown.objects
{
  /**
    <summary>PDF indirect object interface.</summary>
  */
  public interface IPdfIndirectObject
  {
    PdfObject Clone(
      File context
      );

    /**
      <summary>Gets/Sets the actual data associated to the indirect reference.</summary>
    */
    PdfDataObject DataObject
    {get;set;}

    /**
      <summary>Removes the object from its file context.</summary>
      <remarks>
        <para>The object is no more usable after this method returns.</para>
      </remarks>
    */
    void Delete(
      );

    /**
      <summary>Gets the indirect object associated to the indirect reference.</summary>
    */
    PdfIndirectObject IndirectObject
    {get;}

    /**
      <summary>Gets the indirect reference associated to the indirect object.</summary>
    */
    PdfReference Reference
    {get;}
  }
}