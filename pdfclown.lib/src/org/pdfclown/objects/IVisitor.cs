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

using org.pdfclown.tokens;

using System.Collections.Generic;

namespace org.pdfclown.objects
{
  /**
    <summary>Visitor interface.</summary>
    <remarks>Implementations are expected to be functional (traversal results are propagated through
    return values rather than side effects) and external (responsibility for traversing the
    hierarchical structure is assigned to the 'visit' methods rather than the 'accept' counterparts).
    </remarks>
  */
  public interface IVisitor
  {
    /**
      <summary>Visits an object stream.</summary>
      <param name="object">Visited object.</param>
      <param name="data">Supplemental data.</param>
      <returns>Result object.</returns>
    */
    PdfObject Visit(
      ObjectStream obj,
      object data
      );

    /**
      <summary>Visits an object array.</summary>
      <param name="object">Visited object.</param>
      <param name="data">Supplemental data.</param>
      <returns>Result object.</returns>
    */
    PdfObject Visit(
      PdfArray obj,
      object data
      );

    /**
      <summary>Visits a boolean object.</summary>
      <param name="object">Visited object.</param>
      <param name="data">Supplemental data.</param>
      <returns>Result object.</returns>
    */
    PdfObject Visit(
      PdfBoolean obj,
      object data
      );

    /**
      <summary>Visits a data object.</summary>
      <param name="object">Visited object.</param>
      <param name="data">Supplemental data.</param>
      <returns>Result object.</returns>
    */
    PdfObject Visit(
      PdfDataObject obj,
      object data
      );

    /**
      <summary>Visits a date object.</summary>
      <param name="object">Visited object.</param>
      <param name="data">Supplemental data.</param>
      <returns>Result object.</returns>
    */
    PdfObject Visit(
      PdfDate obj,
      object data
      );

    /**
      <summary>Visits an object dictionary.</summary>
      <param name="object">Visited object.</param>
      <param name="data">Supplemental data.</param>
      <returns>Result object.</returns>
    */
    PdfObject Visit(
      PdfDictionary obj,
      object data
      );

    /**
      <summary>Visits an indirect object.</summary>
      <param name="object">Visited object.</param>
      <param name="data">Supplemental data.</param>
      <returns>Result object.</returns>
    */
    PdfObject Visit(
      PdfIndirectObject obj,
      object data
      );

    /**
      <summary>Visits an integer-number object.</summary>
      <param name="object">Visited object.</param>
      <param name="data">Supplemental data.</param>
      <returns>Result object.</returns>
    */
    PdfObject Visit(
      PdfInteger obj,
      object data
      );

    /**
      <summary>Visits a name object.</summary>
      <param name="object">Visited object.</param>
      <param name="data">Supplemental data.</param>
      <returns>Result object.</returns>
    */
    PdfObject Visit(
      PdfName obj,
      object data
      );

    /**
      <summary>Visits a real-number object.</summary>
      <param name="object">Visited object.</param>
      <param name="data">Supplemental data.</param>
      <returns>Result object.</returns>
    */
    PdfObject Visit(
      PdfReal obj,
      object data
      );

    /**
      <summary>Visits a reference object.</summary>
      <param name="object">Visited object.</param>
      <param name="data">Supplemental data.</param>
      <returns>Result object.</returns>
    */
    PdfObject Visit(
      PdfReference obj,
      object data
      );

    /**
      <summary>Visits a stream object.</summary>
      <param name="object">Visited object.</param>
      <param name="data">Supplemental data.</param>
      <returns>Result object.</returns>
    */
    PdfObject Visit(
      PdfStream obj,
      object data
      );

    /**
      <summary>Visits a string object.</summary>
      <param name="object">Visited object.</param>
      <param name="data">Supplemental data.</param>
      <returns>Result object.</returns>
    */
    PdfObject Visit(
      PdfString obj,
      object data
      );

    /**
      <summary>Visits a text string object.</summary>
      <param name="object">Visited object.</param>
      <param name="data">Supplemental data.</param>
      <returns>Result object.</returns>
    */
    PdfObject Visit(
      PdfTextString obj,
      object data
      );

    /**
      <summary>Visits a cross-reference stream object.</summary>
      <param name="object">Visited object.</param>
      <param name="data">Supplemental data.</param>
      <returns>Result object.</returns>
    */
    PdfObject Visit(
      XRefStream obj,
      object data
      );
  }
}