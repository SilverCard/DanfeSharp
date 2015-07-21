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

using org.pdfclown.bytes;
using org.pdfclown.files;
using org.pdfclown.tokens;

using System;

namespace org.pdfclown.objects
{
  /**
    <summary>PDF boolean object [PDF:1.6:3.2.1].</summary>
  */
  public sealed class PdfBoolean
    : PdfSimpleObject<bool>
  {
    #region static
    #region fields
    public static readonly PdfBoolean False = new PdfBoolean(false);
    public static readonly PdfBoolean True = new PdfBoolean(true);
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the object equivalent to the given value.</summary>
    */
    public static PdfBoolean Get(
      bool? value
      )
    {return value.HasValue ? (value.Value ? True : False) : null;}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    private PdfBoolean(
      bool value
      )
    {RawValue = value;}
    #endregion

    #region interface
    #region public
    public override PdfObject Accept(
      IVisitor visitor,
      object data
      )
    {return visitor.Visit(this, data);}

    public bool BooleanValue
    {
      get
      {return (bool)Value;}
    }

    public override int CompareTo(
      PdfDirectObject obj
      )
    {throw new NotImplementedException();}

    public override void WriteTo(
      IOutputStream stream,
      File context
      )
    {stream.Write(RawValue ? Keyword.True : Keyword.False);}
    #endregion
    #endregion
    #endregion
  }
}