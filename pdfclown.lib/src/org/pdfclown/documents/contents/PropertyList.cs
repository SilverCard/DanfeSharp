/*
  Copyright 2010-2012 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.documents.contents.fonts;
using org.pdfclown.documents.contents.layers;
using org.pdfclown.files;
using org.pdfclown.objects;

using System;

namespace org.pdfclown.documents.contents
{
  /**
    <summary>Private information meaningful to the program (application or plugin extension)
    creating the marked content [PDF:1.6:10.5.1].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public class PropertyList
    : PdfObjectWrapper<PdfDictionary>
  {
    #region static
    #region interface
    #region public
    /**
      <summary>Wraps the specified base object into a property list object.</summary>
      <param name="baseObject">Base object of a property list object.</param>
      <returns>Property list object corresponding to the base object.</returns>
    */
    public static PropertyList Wrap(
      PdfDirectObject baseObject
      )
    {
      if(baseObject == null)
        return null;

      PdfName type = (PdfName)((PdfDictionary)baseObject.Resolve())[PdfName.Type];
      if(Layer.TypeName.Equals(type))
        return Layer.Wrap(baseObject);
      else if(LayerMembership.TypeName.Equals(type))
        return LayerMembership.Wrap(baseObject);
      else
        return new PropertyList(baseObject);
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public PropertyList(
      Document context,
      PdfDictionary baseDataObject
      ) : base(context, baseDataObject)
    {}

    protected PropertyList(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #endregion
    #endregion
  }
}