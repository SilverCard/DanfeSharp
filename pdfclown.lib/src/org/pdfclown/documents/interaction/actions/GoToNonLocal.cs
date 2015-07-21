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
using org.pdfclown.documents.files;
using org.pdfclown.documents.interaction.navigation.document;
using org.pdfclown.objects;

using System;

namespace org.pdfclown.documents.interaction.actions
{
  /**
    <summary>Abstract 'go to non-local destination' action.</summary>
  */
  [PDF(VersionEnum.PDF11)]
  public abstract class GotoNonLocal<T>
    : GoToDestination<T>
    where T : Destination
  {
    #region dynamic
    #region constructors
    protected GotoNonLocal(
      Document context,
      PdfName actionType,
      FileSpecification destinationFile,
      T destination
      ) : base(context, actionType, destination)
    {DestinationFile = destinationFile;}

    protected GotoNonLocal(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the file in which the destination is located.</summary>
    */
    public virtual FileSpecification DestinationFile
    {
      get
      {return FileSpecification.Wrap(BaseDataObject[PdfName.F]);}
      set
      {BaseDataObject[PdfName.F] = (value != null ? value.BaseObject : null);}
    }

    /**
      <summary>Gets/Sets the action options.</summary>
    */
    public OptionsEnum Options
    {
      get
      {
        OptionsEnum options = 0;
        PdfDirectObject optionsObject = BaseDataObject[PdfName.NewWindow];
        if(optionsObject != null
          && ((PdfBoolean)optionsObject).BooleanValue)
        {options |= OptionsEnum.NewWindow;}
        return options;
      }
      set
      {
        if((value & OptionsEnum.NewWindow) == OptionsEnum.NewWindow)
        {BaseDataObject[PdfName.NewWindow] = PdfBoolean.True;}
        else if((value & OptionsEnum.SameWindow) == OptionsEnum.SameWindow)
        {BaseDataObject[PdfName.NewWindow] = PdfBoolean.False;}
        else
        {BaseDataObject.Remove(PdfName.NewWindow);} // NOTE: Forcing the absence of this entry ensures that the viewer application should behave in accordance with the current user preference.
      }
    }
    #endregion
    #endregion
    #endregion
  }
}