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
using multimedia = org.pdfclown.documents.multimedia;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;
using System.Drawing;

namespace org.pdfclown.documents.interaction.annotations
{
  /**
    <summary>Sound annotation [PDF:1.6:8.4.5].</summary>
    <remarks>When the annotation is activated, the sound is played.</remarks>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class Sound
    : Annotation
  {
    #region types
    /**
      <summary>Icon to be used in displaying the annotation [PDF:1.6:8.4.5].</summary>
    */
    public enum IconTypeEnum
    {
      /**
        <summary>Speaker.</summary>
      */
      Speaker,
      /**
        <summary>Microphone.</summary>
      */
      Microphone
    };
    #endregion

    #region static
    #region fields
    private static readonly Dictionary<IconTypeEnum,PdfName> IconTypeEnumCodes;
    #endregion

    #region constructors
    static Sound()
    {
      IconTypeEnumCodes = new Dictionary<IconTypeEnum,PdfName>();
      IconTypeEnumCodes[IconTypeEnum.Speaker] = PdfName.Speaker;
      IconTypeEnumCodes[IconTypeEnum.Microphone] = PdfName.Mic;
    }
    #endregion

    #region interface
    #region private
    /**
      <summary>Gets the code corresponding to the given value.</summary>
    */
    private static PdfName ToCode(
      IconTypeEnum value
      )
    {return IconTypeEnumCodes[value];}

    /**
      <summary>Gets the icon type corresponding to the given value.</summary>
    */
    private static IconTypeEnum ToIconTypeEnum(
      PdfName value
      )
    {
      foreach(KeyValuePair<IconTypeEnum,PdfName> iconType in IconTypeEnumCodes)
      {
        if(iconType.Value.Equals(value))
          return iconType.Key;
      }
      return IconTypeEnum.Speaker;
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public Sound(
      Page page,
      RectangleF box,
      string text,
      multimedia::Sound content
      ) : base(page, PdfName.Sound, box, text)
    {Content = content;}

    internal Sound(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the icon to be used in displaying the annotation.</summary>
    */
    public IconTypeEnum IconType
    {
      get
      {return ToIconTypeEnum((PdfName)BaseDataObject[PdfName.Name]);}
      set
      {BaseDataObject[PdfName.Name] = ToCode(value);}
    }

    /**
      <summary>Gets/Sets the sound to be played.</summary>
    */
    public multimedia::Sound Content
    {
      get
      {
        return new multimedia::Sound(
          BaseDataObject[PdfName.Sound]
          );
      }
      set
      {
        if(value == null)
          throw new ArgumentException("Content MUST be defined.");

        BaseDataObject[PdfName.Sound] = value.BaseObject;
      }
    }
    #endregion
    #endregion
    #endregion
  }
}