/*
  Copyright 2007-2011 Stefano Chizzolini. http://www.pdfclown.org

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
using xObjects = org.pdfclown.documents.contents.xObjects;
using org.pdfclown.objects;

using System.Collections.Generic;

namespace org.pdfclown.documents.contents.objects
{
  /**
    <summary>'Paint the specified XObject' operation [PDF:1.6:4.7].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class PaintXObject
    : Operation,
      IResourceReference<xObjects::XObject>
  {
    #region static
    #region fields
    public static readonly string OperatorKeyword = "Do";
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public PaintXObject(
      PdfName name
      ) : base(OperatorKeyword,name)
    {}

    public PaintXObject(
      IList<PdfDirectObject> operands
      ) : base(OperatorKeyword,operands)
    {}
    #endregion
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the scanner for the contents of the painted external object.</summary>
      <param name="context">Scanning context.</param>
    */
    public ContentScanner GetScanner(
      ContentScanner context
      )
    {
      xObjects::XObject xObject = GetXObject(context.ContentContext);
      return xObject is xObjects::FormXObject
        ? new ContentScanner((xObjects::FormXObject)xObject, context)
        : null;
    }

    /**
      <summary>Gets the <see cref="xObjects::XObject">external object</see> resource to be painted.
      </summary>
      <param name="context">Content context.</param>
    */
    public xObjects::XObject GetXObject(
      IContentContext context
      )
    {return GetResource(context);}

    #region IResourceReference
    public xObjects::XObject GetResource(
      IContentContext context
      )
    {return context.Resources.XObjects[Name];}

    public PdfName Name
    {
      get
      {return (PdfName)operands[0];}
      set
      {operands[0] = value;}
    }
    #endregion
    #endregion
    #endregion
  }
}