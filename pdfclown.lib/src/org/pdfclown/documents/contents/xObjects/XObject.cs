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

using org.pdfclown.documents;
using org.pdfclown.documents.contents.layers;
using org.pdfclown.files;
using org.pdfclown.objects;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace org.pdfclown.documents.contents.xObjects
{
  /**
    <summary>External graphics object whose contents are defined by a self-contained content stream,
    separate from the content stream in which it is used [PDF:1.6:4.7].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public abstract class XObject
    : PdfObjectWrapper<PdfStream>,
      ILayerable
  {
    #region static
    #region interface
    #region public
    /**
      <summary>Wraps an external object reference into an external object.</summary>
      <param name="baseObject">External object base object.</param>
      <returns>External object associated to the reference.</returns>
    */
    public static XObject Wrap(
      PdfDirectObject baseObject
      )
    {
      if(baseObject == null)
        return null;

      PdfName subtype = (PdfName)((PdfStream)baseObject.Resolve()).Header[PdfName.Subtype];
      if(subtype.Equals(PdfName.Form))
        return FormXObject.Wrap(baseObject);
      else if(subtype.Equals(PdfName.Image))
        return ImageXObject.Wrap(baseObject);
      else
        return null;
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    /**
      <summary>Creates a new external object inside the document.</summary>
    */
    protected XObject(
      Document context
      ) : this(context, new PdfStream())
    {}

    /**
      <summary>Creates a new external object inside the document.</summary>
    */
    protected XObject(
      Document context,
      PdfStream baseDataObject
      ) : base(context, baseDataObject)
    {baseDataObject.Header[PdfName.Type] = PdfName.XObject;}

    /**
      <summary>Instantiates an existing external object.</summary>
    */
    protected XObject(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the mapping from external-object space to user space.</summary>
    */
    public abstract Matrix Matrix
    {
      get;
      set;
    }

    /**
      <summary>Gets/Sets the external object size.</summary>
    */
    public abstract SizeF Size
    {
      get;
      set;
    }

    #region ILayerable
    public LayerEntity Layer
    {
      get
      {return (LayerEntity)PropertyList.Wrap(BaseDataObject.Header[PdfName.OC]);}
      set
      {BaseDataObject.Header[PdfName.OC] = value.BaseObject;}
    }
    #endregion
    #endregion
    #endregion
    #endregion
  }
}