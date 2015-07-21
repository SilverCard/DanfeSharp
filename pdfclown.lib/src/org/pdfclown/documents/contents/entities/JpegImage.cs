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

using bytes = org.pdfclown.bytes;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;
using org.pdfclown.documents.contents.objects;
using xObjects = org.pdfclown.documents.contents.xObjects;
using org.pdfclown.objects;
using org.pdfclown.util.io;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace org.pdfclown.documents.contents.entities
{
  /**
    <summary>JPEG image object [ISO 10918-1;JFIF:1.02].</summary>
  */
  public sealed class JpegImage
    : Image
  {
    #region dynamic
    #region constructors
    public JpegImage(
      System.IO.Stream stream
      ) : base(stream)
    {Load();}
    #endregion

    #region interface
    #region public
    public override ContentObject ToInlineObject(
      PrimitiveComposer composer
      )
    {
      return composer.Add(
        new InlineImage(
          new InlineImageHeader(
            new List<PdfDirectObject>(
              new PdfDirectObject[]
              {
                PdfName.W, PdfInteger.Get(Width),
                PdfName.H, PdfInteger.Get(Height),
                PdfName.CS, PdfName.RGB,
                PdfName.BPC, PdfInteger.Get(BitsPerComponent),
                PdfName.F, PdfName.DCT
              }
              )
            ),
          new InlineImageBody(
            new bytes::Buffer(Stream)
            )
          )
        );
    }

    public override xObjects::XObject ToXObject(
      Document context
      )
    {
      return new xObjects::ImageXObject(
        context,
        new PdfStream(
          new PdfDictionary(
            new PdfName[]
            {
              PdfName.Width,
              PdfName.Height,
              PdfName.BitsPerComponent,
              PdfName.ColorSpace,
              PdfName.Filter
            },
            new PdfDirectObject[]
            {
              PdfInteger.Get(Width),
              PdfInteger.Get(Height),
              PdfInteger.Get(BitsPerComponent),
              PdfName.DeviceRGB,
              PdfName.DCTDecode
            }
            ),
          new bytes::Buffer(Stream)
          )
        );
    }
    #endregion

    #region private
    private void Load(
      )
    {
      /*
        NOTE: Big-endian data expected.
      */
      System.IO.Stream stream = Stream;
      BigEndianBinaryReader streamReader = new BigEndianBinaryReader(stream);

      int index = 4;
      stream.Seek(index,SeekOrigin.Begin);
      byte[] markerBytes = new byte[2];
      while(true)
      {
        index += streamReader.ReadUInt16();
        stream.Seek(index,SeekOrigin.Begin);

        stream.Read(markerBytes,0,2);
        index += 2;

        // Frame header?
        if(markerBytes[0] == 0xFF
          && markerBytes[1] == 0xC0)
        {
          stream.Seek(2,SeekOrigin.Current);
          // Get the image bits per color component (sample precision)!
          BitsPerComponent = stream.ReadByte();
          // Get the image size!
          Height = streamReader.ReadUInt16();
          Width = streamReader.ReadUInt16();

          break;
        }
      }
    }
    #endregion
    #endregion
    #endregion
  }
}