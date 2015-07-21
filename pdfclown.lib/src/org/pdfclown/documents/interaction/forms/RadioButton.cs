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
using org.pdfclown.documents;
using org.pdfclown.documents.interaction.annotations;
using org.pdfclown.objects;
using org.pdfclown.util;

using System;

namespace org.pdfclown.documents.interaction.forms
{
  /**
    <summary>Radio button field [PDF:1.6:8.6.3].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class RadioButton
    : ButtonField
  {
    #region dynamic
    #region constructors
    /**
      <summary>Creates a new radiobutton within the given document context.</summary>
    */
    public RadioButton(
      string name,
      DualWidget[] widgets,
      string value
      ) : base(name, widgets[0])
    {
      Flags = EnumUtils.Mask(
        EnumUtils.Mask(Flags, FlagsEnum.Radio, true),
        FlagsEnum.NoToggleToOff,
        true
        );

      FieldWidgets fieldWidgets = Widgets;
      for(int index = 1, length = widgets.Length; index < length; index++)
      {fieldWidgets.Add(widgets[index]);}

      Value = value;
    }

    internal RadioButton(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets whether all the field buttons can be deselected at the same time.</summary>
    */
    public bool Toggleable
    {
      get
      {return (Flags & FlagsEnum.NoToggleToOff) != FlagsEnum.NoToggleToOff;}
      set
      {Flags = EnumUtils.Mask(Flags, FlagsEnum.NoToggleToOff, !value);}
    }

    public override object Value
    {
      get
      {return base.Value;}
      set
      {
        /*
          NOTE: The parent field's V entry holds a name object corresponding to the appearance state
          of whichever child field is currently in the on state; the default value for this entry is
          Off.
        */
        PdfName selectedWidgetName = new PdfName((string)value);
        bool selected = false;
        // Selecting the current appearance state for each widget...
        foreach(Widget widget in Widgets)
        {
          PdfName currentState;
          if(((DualWidget)widget).WidgetName.Equals(value)) // Selected state.
          {
            selected = true;
            currentState = selectedWidgetName;
          }
          else // Unselected state.
          {currentState = PdfName.Off;}

          widget.BaseDataObject[PdfName.AS] = currentState;
        }
        // Select the current widget!
        BaseDataObject[PdfName.V] = (selected ? selectedWidgetName : null);
      }
    }
    #endregion
    #endregion
    #endregion
  }
}