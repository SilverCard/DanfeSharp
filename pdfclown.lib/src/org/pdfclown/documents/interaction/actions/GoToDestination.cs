/*
  Copyright 2010-2012 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.documents;
using org.pdfclown.documents.interaction.navigation.document;
using org.pdfclown.objects;

using System;

namespace org.pdfclown.documents.interaction.actions
{
  /**
    <summary>Abstract 'go to destination' action.</summary>
  */
  [PDF(VersionEnum.PDF11)]
  public abstract class GoToDestination<T>
    : Action,
      IGoToAction
    where T : Destination
  {
    #region dynamic
    #region constructors
    protected GoToDestination(
      Document context,
      PdfName actionType,
      T destination
      ) : base(context, actionType)
    {Destination = destination;}

    protected GoToDestination(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the destination to jump to.</summary>
    */
    public T Destination
    {
      get
      {return Document.ResolveName<T>(BaseDataObject[PdfName.D]);}
      set
      {
        if(value == null)
          throw new ArgumentException("Destination MUST be defined.");

        BaseDataObject[PdfName.D] = value.NamedBaseObject;
      }
    }
    #endregion
    #endregion
    #endregion
  }
}