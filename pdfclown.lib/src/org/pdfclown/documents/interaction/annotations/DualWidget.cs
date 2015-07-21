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
using org.pdfclown.documents.contents.xObjects;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;
using System.Drawing;

namespace org.pdfclown.documents.interaction.annotations
{
  /**
    <summary>Dual-state widget annotation.</summary>
    <remarks>As its name implies, it has two states: on and off.</remarks>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class DualWidget
    : Widget
  {
    #region dynamic
    #region constructors
    /**
      <param name="widgetName">Widget name. It corresponds to the on-state name.</param>
    */
    public DualWidget(
      Page page,
      RectangleF box,
      string widgetName
      ) : base(page,box)
    {
      // Initialize the on-state appearance!
      /*
        NOTE: This is necessary to keep the reference to the on-state name.
      */
      Appearance appearance = new Appearance(page.Document);
      Appearance = appearance;
      AppearanceStates normalAppearance = appearance.Normal;
      normalAppearance[new PdfName(widgetName)] = new FormXObject(page.Document, box.Size);
    }

    internal DualWidget(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    public string WidgetName
    {
      get
      {
        foreach(KeyValuePair<PdfName,FormXObject> normalAppearanceEntry in Appearance.Normal)
        {
          PdfName key = normalAppearanceEntry.Key;
          if(!key.Equals(PdfName.Off)) // 'On' state.
            return (string)key.Value;
        }
        return null; // NOTE: It MUST NOT happen (on-state should always be defined).
      }
    }
    #endregion
    #endregion
    #endregion
  }
}