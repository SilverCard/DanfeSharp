/*
  Copyright 2008-2012 Stefano Chizzolini. http://www.pdfclown.org

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

using org.pdfclown.bytes;
using org.pdfclown.documents;
using org.pdfclown.objects;

using System;

namespace org.pdfclown.documents.interaction.actions
{
  /**
    <summary>'Cause a URI (Uniform Resource Identifier) to be resolved' action [PDF:1.6:8.5.3].</summary>
  */
  [PDF(VersionEnum.PDF11)]
  public sealed class GoToURI
    : Action,
      IGoToAction
  {
    #region dynamic
    #region constructors
    /**
      <summary>Creates a new action within the given document context.</summary>
    */
    public GoToURI(
      Document context,
      Uri uri
      ) : base(context, PdfName.URI)
    {URI = uri;}

    internal GoToURI(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the uniform resource identifier to resolve [RFC 2396].</summary>
    */
    public Uri URI
    {
      get
      {
        /*
          NOTE: 'URI' entry MUST exist.
        */
        return new Uri(
          (string)((PdfString)BaseDataObject[PdfName.URI]).Value
          );
      }
      set
      {BaseDataObject[PdfName.URI] = new PdfString(value.ToString());}
    }
    #endregion
    #endregion
    #endregion
  }
}