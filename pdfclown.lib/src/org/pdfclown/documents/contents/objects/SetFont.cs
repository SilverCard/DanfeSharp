/*
  Copyright 2007-2012 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.documents.contents.fonts;
using org.pdfclown.objects;

using System.Collections.Generic;

namespace org.pdfclown.documents.contents.objects
{
  /**
    <summary>'Set the text font' operation [PDF:1.6:5.2].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class SetFont
    : Operation,
      IResourceReference<Font>
  {
    #region static
    #region fields
    public static readonly string OperatorKeyword = "Tf";
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public SetFont(
      PdfName name,
      double size
      ) : base(OperatorKeyword, name, PdfReal.Get(size))
    {}

    public SetFont(
      IList<PdfDirectObject> operands
      ) : base(OperatorKeyword, operands)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the <see cref="Font">font</see> resource to be set.</summary>
      <param name="context">Content context.</param>
    */
    public Font GetFont(
      IContentContext context
      )
    {return GetResource(context);}

    public override void Scan(
      ContentScanner.GraphicsState state
      )
    {
      state.Font = GetFont(state.Scanner.ContentContext);
      state.FontSize = Size;
    }

    /**
      <summary>Gets/Sets the font size to be set.</summary>
    */
    public double Size
    {
      get
      {return ((IPdfNumber)operands[1]).RawValue;}
      set
      {operands[1] = PdfReal.Get(value);}
    }

    #region IResourceReference
    public Font GetResource(
      IContentContext context
      )
    {return context.Resources.Fonts[Name];}

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
    #endregion
  }
}