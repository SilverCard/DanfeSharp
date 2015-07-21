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

namespace org.pdfclown.files
{
  /**
    <summary>PDF file serialization mode [PDF:1.6:3.4].</summary>
  */
  public enum SerializationModeEnum
  {
    /**
      <summary>Standard complete file serialization [PDF:1.6:3.4].</summary>
      <remarks>It <i>writes the entire file</i>, generating a single-section cross-reference table
      and removing obsolete data structures.
      It reduces the serialization size, but it's more computationally-intensive (slower).</remarks>
    */
    Standard = 0,
    /**
      <summary>Standard incremental file serialization [PDF:1.6:2.2.7].</summary>
      <remarks>It <i>leaves original contents intact, appending changes to the end of the file</i>
      along with an additional cross-reference table section.
      It increases the serialization size, but it's faster.</remarks>
    */
    Incremental = 1,
    /**
      <summary>Linearized file serialization [PDF:1.6:F].</summary>
      <remarks>It organizes the file to enable <i>efficient incremental access in a network environment</i>.
      It increases the serialization size and it's more computationally-intensive (slower).</remarks>
    */
    Linearized = 2
  }
}