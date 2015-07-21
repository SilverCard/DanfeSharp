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

using org.pdfclown.documents;
using org.pdfclown.objects;

using System;

namespace org.pdfclown.documents.interaction.navigation.document
{
  /**
    <summary>Remote interaction target [PDF:1.6:8.2.1].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class RemoteDestination
    : Destination
  {
    #region dynamic
    #region constructors
    public RemoteDestination(
      Document context,
      int pageIndex
      ) : this(
        context,
        pageIndex,
        ModeEnum.Fit,
        null,
        null
        )
    {}

    public RemoteDestination(
      Document context,
      int pageIndex,
      ModeEnum mode,
      object location,
      double? zoom
      ) : base(
        context,
        pageIndex,
        mode,
        location,
        zoom
        )
    {}

    internal RemoteDestination(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the index of the target page.</summary>
    */
    public override object Page
    {
      get
      {return ((PdfInteger)BaseDataObject[0]).IntValue;}
      set
      {
        if(!(value is Int32))
          throw new ArgumentException("It MUST be an integer number.");

        BaseDataObject[0] = PdfInteger.Get((int)value);
      }
    }
    #endregion
    #endregion
    #endregion
  }
}