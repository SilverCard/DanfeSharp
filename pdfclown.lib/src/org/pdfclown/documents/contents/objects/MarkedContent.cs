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
using org.pdfclown.objects;
using org.pdfclown.tokens;

using System.Collections.Generic;

namespace org.pdfclown.documents.contents.objects
{
  /**
    <summary>Marked-content sequence [PDF:1.6:10.5].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class MarkedContent
    : ContainerObject
  {
    #region static
    #region fields
    public static readonly string EndOperatorKeyword = EndMarkedContent.OperatorKeyword;

    private static readonly byte[] EndChunk = Encoding.Pdf.Encode(EndOperatorKeyword + Symbol.LineFeed);
    #endregion
    #endregion

    #region dynamic
    #region fields
    private BeginMarkedContent header;
    #endregion

    #region constructors
    public MarkedContent(
      BeginMarkedContent header
      ) : this(header, new List<ContentObject>())
    {}

    public MarkedContent(
      BeginMarkedContent header,
      IList<ContentObject> objects
      ) : base(objects)
    {this.header = header;}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets information about this marked-content sequence.</summary>
    */
    public override Operation Header
    {
      get
      {return header;}
      set
      {header = (BeginMarkedContent)value;}
    }

    public override void WriteTo(
      IOutputStream stream,
      Document context
      )
    {
      header.WriteTo(stream, context);
      base.WriteTo(stream, context);
      stream.Write(EndChunk);
    }
    #endregion
    #endregion
    #endregion
  }
}