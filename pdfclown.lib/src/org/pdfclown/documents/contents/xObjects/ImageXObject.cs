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

using org.pdfclown;
using org.pdfclown.documents;
using org.pdfclown.objects;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace org.pdfclown.documents.contents.xObjects
{
  /**
    <summary>Image external object [PDF:1.6:4.8.4].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class ImageXObject
    : XObject
  {
    #region static
    #region interface
    #region public
    public static new ImageXObject Wrap(
      PdfDirectObject baseObject
      )
    {return baseObject != null ? new ImageXObject(baseObject) : null;}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public ImageXObject(
      Document context,
      PdfStream baseDataObject
      ) : base(
        context,
        baseDataObject
        )
    {
      /*
        NOTE: It's caller responsability to adequately populate the stream
        header and body in order to instantiate a valid object; header entries like
        'Width', 'Height', 'ColorSpace', 'BitsPerComponent' MUST be defined
        appropriately.
      */
      baseDataObject.Header[PdfName.Subtype] = PdfName.Image;
    }

    private ImageXObject(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the number of bits per color component.</summary>
    */
    public int BitsPerComponent
    {
      get
      {return ((PdfInteger)BaseDataObject.Header[PdfName.BitsPerComponent]).RawValue;}
    }

    /**
      <summary>Gets the color space in which samples are specified.</summary>
    */
    public string ColorSpace
    {
      get
      {return ((PdfName)BaseDataObject.Header[PdfName.ColorSpace]).RawValue;}
    }

    public override Matrix Matrix
    {
      get
      {
        SizeF size = Size;

        /*
          NOTE: Image-space-to-user-space matrix is [1/w 0 0 1/h 0 0],
          where w and h are the width and height of the image in samples [PDF:1.6:4.8.3].
        */
        return new Matrix(
          1f / size.Width,
          0,
          0,
          1f / size.Height,
          0,
          0
          );
      }
      set
      {/* NOOP. */}
    }

    /**
      <summary>Gets the size of the image (in samples).</summary>
    */
    public override SizeF Size
    {
      get
      {
        PdfDictionary header = BaseDataObject.Header;

        return new SizeF(
          ((PdfInteger)header[PdfName.Width]).RawValue,
          ((PdfInteger)header[PdfName.Height]).RawValue
          );
      }
      set
      {throw new NotSupportedException();}
    }
    #endregion
    #endregion
    #endregion
  }
}