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
using org.pdfclown.documents.contents;
using org.pdfclown.documents.contents.composition;
using org.pdfclown.documents.contents.objects;
using org.pdfclown.files;
using org.pdfclown.objects;

using System;
using drawing = System.Drawing;
using System.Drawing.Drawing2D;

namespace org.pdfclown.documents.contents.xObjects
{
  /**
    <summary>Form external object [PDF:1.6:4.9].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class FormXObject
    : XObject,
      IContentContext
  {
    #region static
    #region interface
    #region public
    public static new FormXObject Wrap(
      PdfDirectObject baseObject
      )
    {return baseObject != null ? new FormXObject(baseObject) : null;}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    /**
      <summary>Creates a new form within the specified document context.</summary>
      <param name="context">Document where to place this form.</param>
      <param name="size">Form size.</param>
    */
    public FormXObject(
      Document context,
      drawing::SizeF size
      ) : this(context, new drawing::RectangleF(new drawing::PointF(0, 0), size))
    {}

    /**
      <summary>Creates a new form within the specified document context.</summary>
      <param name="context">Document where to place this form.</param>
      <param name="box">Form box.</param>
    */
    public FormXObject(
      Document context,
      drawing::RectangleF box
      ) : base(context)
    {
      BaseDataObject.Header[PdfName.Subtype] = PdfName.Form;
      Box = box;
    }

    private FormXObject(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    public override Matrix Matrix
    {
      get
      {
        /*
          NOTE: Form-space-to-user-space matrix is identity [1 0 0 1 0 0] by default,
          but may be adjusted by setting the Matrix entry in the form dictionary [PDF:1.6:4.9].
        */
        PdfArray matrix = (PdfArray)BaseDataObject.Header.Resolve(PdfName.Matrix);
        if(matrix == null)
          return new Matrix();
        else
          return new Matrix(
            ((IPdfNumber)matrix[0]).FloatValue,
            ((IPdfNumber)matrix[1]).FloatValue,
            ((IPdfNumber)matrix[2]).FloatValue,
            ((IPdfNumber)matrix[3]).FloatValue,
            ((IPdfNumber)matrix[4]).FloatValue,
            ((IPdfNumber)matrix[5]).FloatValue
            );
      }
      set
      {
        BaseDataObject.Header[PdfName.Matrix] = value != null
          ? new PdfArray(
            PdfReal.Get(value.Elements[0]),
            PdfReal.Get(value.Elements[1]),
            PdfReal.Get(value.Elements[2]),
            PdfReal.Get(value.Elements[3]),
            PdfReal.Get(value.Elements[4]),
            PdfReal.Get(value.Elements[5])
            )
          : null;
      }
    }

    public override drawing::SizeF Size
    {
      get
      {
        PdfArray box = (PdfArray)BaseDataObject.Header.Resolve(PdfName.BBox);
        return new drawing::SizeF(
          ((IPdfNumber)box[2]).FloatValue,
          ((IPdfNumber)box[3]).FloatValue
          );
      }
      set
      {
        PdfArray boxObject = (PdfArray)BaseDataObject.Header.Resolve(PdfName.BBox);
        boxObject[2] = PdfReal.Get(value.Width);
        boxObject[3] = PdfReal.Get(value.Height);
      }
    }
    #endregion

    #region internal
    #region IContentContext
    public drawing::RectangleF Box
    {
      get
      {return Rectangle.Wrap(BaseDataObject.Header[PdfName.BBox]).ToRectangleF();}
      set
      {BaseDataObject.Header[PdfName.BBox] = new Rectangle(value).BaseDataObject;}
    }

    public Contents Contents
    {
      get
      {return Contents.Wrap(BaseObject, this);}
    }

    public void Render(
      drawing::Graphics context,
      drawing::SizeF size
      )
    {
      ContentScanner scanner = new ContentScanner(Contents);
      scanner.Render(context, size);
    }

    public Resources Resources
    {
      get
      {return Resources.Wrap(BaseDataObject.Header.Get<PdfDictionary>(PdfName.Resources));}
      set
      {BaseDataObject.Header[PdfName.Resources] = PdfObjectWrapper.GetBaseObject(value);}
    }

    public RotationEnum Rotation
    {
      get
      {return RotationEnum.Downward;}
    }

    #region IContentEntity
    public ContentObject ToInlineObject(
      PrimitiveComposer composer
      )
    {throw new NotImplementedException();}

    public XObject ToXObject(
      Document context
      )
    {return (XObject)Clone(context);}
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion
  }
}