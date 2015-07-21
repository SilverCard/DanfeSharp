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
    <summary>External object shown in a content stream context [PDF:1.6:4.7].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class XObject
    : GraphicsObject,
      IResourceReference<xObjects::XObject>
  {
    #region static
    #region fields
    public static readonly string BeginOperatorKeyword = PaintXObject.OperatorKeyword;
    public static readonly string EndOperatorKeyword = BeginOperatorKeyword;
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public XObject(
      PaintXObject operation
      ) : base(operation)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the scanner for this object's contents.</summary>
      <param name="context">Scanning context.</param>
    */
    public ContentScanner GetScanner(
      ContentScanner context
      )
    {return Operation.GetScanner(context);}

    #region IResourceReference
    public xObjects::XObject GetResource(
      IContentContext context
      )
    {return Operation.GetResource(context);}

    public PdfName Name
    {
      get
      {return Operation.Name;}
      set
      {Operation.Name = value;}
    }
    #endregion
    #endregion

    #region private
    private PaintXObject Operation
    {
      get
      {return (PaintXObject)Objects[0];}
    }
    #endregion
    #endregion
    #endregion
  }
}