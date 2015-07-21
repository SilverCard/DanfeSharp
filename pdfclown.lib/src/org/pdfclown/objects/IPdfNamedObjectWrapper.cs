/*
  Copyright 2012 Stefano Chizzolini. http://www.pdfclown.org

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

namespace org.pdfclown.objects
{
  /**
    <summary>High-level representation of a PDF object that can be referenced also through a name.
    </summary>
    <remarks>
      <para>Some categories of objects in a PDF file can be also referred to by name rather than by
      object reference. The correspondence between names and objects is established by the document's
      name dictionary [PDF:1.6:3.6.3].</para>
      <para>The name's purpose is to provide a further level of referential abstraction especially
      for references across diverse PDF documents.</para>
    </remarks>
  */
  public interface IPdfNamedObjectWrapper
    : IPdfObjectWrapper
  {
    /**
      <summary>Gets the object name.</summary>
      <remarks>As names are tipically loosely-coupled with their corresponding PDF objects, name
      retrieval implies a costly reverse lookup into the document's name tree.</remarks>
    */
    PdfString Name
    {
      get;
    }

    /**
      <summary>Gets the object name, if available; otherwise, behaves like
      <see cref="IPdfObjectWrapper.BaseObject">BaseObject</see>.</summary>
    */
    PdfDirectObject NamedBaseObject
    {
      get;
    }
  }
}