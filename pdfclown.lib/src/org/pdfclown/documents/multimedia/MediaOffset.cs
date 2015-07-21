/*
  Copyright 2012 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.documents.contents.colorSpaces;
using org.pdfclown.documents.interaction;
using actions = org.pdfclown.documents.interaction.actions;
using org.pdfclown.files;
using org.pdfclown.objects;

using System;

namespace org.pdfclown.documents.multimedia
{
  /**
    <summary>Media offset [PDF:1.7:9.1.5].</summary>
  */
  [PDF(VersionEnum.PDF15)]
  public abstract class MediaOffset
    : PdfObjectWrapper<PdfDictionary>
  {
    #region types
    /**
      <summary>Media offset frame [PDF:1.7:9.1.5].</summary>
    */
    public sealed class Frame
      : MediaOffset
    {
      public Frame(
        Document context,
        int value
        ) : base(context, PdfName.F)
      {Value = value;}

      internal Frame(
        PdfDirectObject baseObject
        ) : base(baseObject)
      {}

      /**
        <summary>Gets/Sets the (zero-based) frame within a media object.</summary>
      */
      public override object Value
      {
        get
        {return ((PdfInteger)BaseDataObject[PdfName.F]).IntValue;}
        set
        {
          int intValue = (int)value;
          if(intValue < 0)
            throw new ArgumentException("MUST be non-negative.");

          BaseDataObject[PdfName.F] = PdfInteger.Get(intValue);
        }
      }
    }

    /**
      <summary>Media offset marker [PDF:1.7:9.1.5].</summary>
    */
    public sealed class Marker
      : MediaOffset
    {
      public Marker(
        Document context,
        string value
        ) : base(context, PdfName.M)
      {Value = value;}

      internal Marker(
        PdfDirectObject baseObject
        ) : base(baseObject)
      {}

      /**
        <summary>Gets a named offset within a media object.</summary>
      */
      public override object Value
      {
        get
        {return ((PdfTextString)BaseDataObject[PdfName.M]).StringValue;}
        set
        {BaseDataObject[PdfName.M] = PdfTextString.Get(value);}
      }
    }

    /**
      <summary>Media offset time [PDF:1.7:9.1.5].</summary>
    */
    public sealed class Time
      : MediaOffset
    {
      public Time(
        Document context,
        double value
        ) : base(context, PdfName.T)
      {BaseDataObject[PdfName.T] = new Timespan(value).BaseObject;}

      internal Time(
        PdfDirectObject baseObject
        ) : base(baseObject)
      {}

      /**
        <summary>Gets/Sets the temporal offset (in seconds).</summary>
      */
      public override object Value
      {
        get
        {return Timespan.Time;}
        set
        {Timespan.Time = (double)value;}
      }

      private Timespan Timespan
      {
        get
        {return new Timespan(BaseDataObject[PdfName.T]);}
      }
    }
    #endregion

    #region static
    #region interface
    #region public
    public static MediaOffset Wrap(
      PdfDirectObject baseObject
      )
    {
      if(baseObject == null)
        return null;

      PdfDictionary dataObject = (PdfDictionary)baseObject.Resolve();
      PdfName offsetType = (PdfName)dataObject[PdfName.S];
      if(offsetType == null
        || (dataObject.ContainsKey(PdfName.Type)
            && !dataObject[PdfName.Type].Equals(PdfName.MediaOffset)))
        return null;

      if(offsetType.Equals(PdfName.F))
        return new Frame(baseObject);
      else if(offsetType.Equals(PdfName.M))
        return new Marker(baseObject);
      else if(offsetType.Equals(PdfName.T))
        return new Time(baseObject);
      else
        throw new NotSupportedException();
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    protected MediaOffset(
      Document context,
      PdfName subtype
      ) : base(
        context,
        new PdfDictionary(
          new PdfName[]
          {
            PdfName.Type,
            PdfName.S
          },
          new PdfDirectObject[]
          {
            PdfName.MediaOffset,
            subtype
          }
          )
        )
    {}

    protected MediaOffset(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the offset value.</summary>
    */
    public abstract object Value
    {
      get;
      set;
    }
    #endregion
    #endregion
    #endregion
  }
}