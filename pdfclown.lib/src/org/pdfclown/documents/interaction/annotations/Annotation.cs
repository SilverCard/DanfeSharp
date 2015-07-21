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
using org.pdfclown.documents.contents;
using org.pdfclown.documents.contents.colorSpaces;
using org.pdfclown.documents.contents.layers;
using org.pdfclown.documents.interaction.actions;
using org.pdfclown.files;
using org.pdfclown.objects;
using org.pdfclown.util;

using System;
using System.Drawing;

namespace org.pdfclown.documents.interaction.annotations
{
  /**
    <summary>Annotation [PDF:1.6:8.4].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public class Annotation
    : PdfObjectWrapper<PdfDictionary>,
      ILayerable
  {
    #region types
    /**
      <summary>Field flags [PDF:1.6:8.4.2].</summary>
    */
    [Flags]
    public enum FlagsEnum
    {
      /**
        <summary>Hide the annotation, both on screen and on print,
        if it does not belong to one of the standard annotation types
        and no annotation handler is available.</summary>
      */
      Invisible = 0x1,
      /**
        <summary>Hide the annotation, both on screen and on print
        (regardless of its annotation type or whether an annotation handler is available).</summary>
      */
      Hidden = 0x2,
      /**
        <summary>Print the annotation when the page is printed.</summary>
      */
      Print = 0x4,
      /**
        <summary>Do not scale the annotation's appearance to match the magnification of the page.</summary>
      */
      NoZoom = 0x8,
      /**
        <summary>Do not rotate the annotation's appearance to match the rotation of the page.</summary>
      */
      NoRotate = 0x10,
      /**
        <summary>Hide the annotation on the screen.</summary>
      */
      NoView = 0x20,
      /**
        <summary>Do not allow the annotation to interact with the user.</summary>
      */
      ReadOnly = 0x40,
      /**
        <summary>Do not allow the annotation to be deleted or its properties to be modified by the user.</summary>
      */
      Locked = 0x80,
      /**
        <summary>Invert the interpretation of the NoView flag.</summary>
      */
      ToggleNoView = 0x100
    }
    #endregion

    #region static
    #region interface
    #region public
    /**
      <summary>Wraps an annotation base object into an annotation object.</summary>
      <param name="baseObject">Annotation base object.</param>
      <returns>Annotation object associated to the base object.</returns>
    */
    public static Annotation Wrap(
      PdfDirectObject baseObject
      )
    {
      if(baseObject == null)
        return null;

      PdfName annotationType = (PdfName)((PdfDictionary)baseObject.Resolve())[PdfName.Subtype];
      if(annotationType.Equals(PdfName.Text))
        return new Note(baseObject);
      else if(annotationType.Equals(PdfName.Link))
        return new Link(baseObject);
      else if(annotationType.Equals(PdfName.FreeText))
        return new CalloutNote(baseObject);
      else if(annotationType.Equals(PdfName.Line))
        return new Line(baseObject);
      else if(annotationType.Equals(PdfName.Square))
        return new Rectangle(baseObject);
      else if(annotationType.Equals(PdfName.Circle))
        return new Ellipse(baseObject);
      else if(annotationType.Equals(PdfName.Polygon))
        return new Polygon(baseObject);
      else if(annotationType.Equals(PdfName.PolyLine))
        return new Polyline(baseObject);
      else if(annotationType.Equals(PdfName.Highlight)
        || annotationType.Equals(PdfName.Underline)
        || annotationType.Equals(PdfName.Squiggly)
        || annotationType.Equals(PdfName.StrikeOut))
        return new TextMarkup(baseObject);
      else if(annotationType.Equals(PdfName.Stamp))
        return new RubberStamp(baseObject);
      else if(annotationType.Equals(PdfName.Caret))
        return new Caret(baseObject);
      else if(annotationType.Equals(PdfName.Ink))
        return new Scribble(baseObject);
      else if(annotationType.Equals(PdfName.Popup))
        return new Popup(baseObject);
      else if(annotationType.Equals(PdfName.FileAttachment))
        return new FileAttachment(baseObject);
      else if(annotationType.Equals(PdfName.Sound))
        return new Sound(baseObject);
      else if(annotationType.Equals(PdfName.Movie))
        return new Movie(baseObject);
      else if(annotationType.Equals(PdfName.Widget))
        return new Widget(baseObject);
      else if(annotationType.Equals(PdfName.Screen))
        return new Screen(baseObject);
//TODO
//     else if(annotationType.Equals(PdfName.PrinterMark)) return new PrinterMark(baseObject);
//     else if(annotationType.Equals(PdfName.TrapNet)) return new TrapNet(baseObject);
//     else if(annotationType.Equals(PdfName.Watermark)) return new Watermark(baseObject);
//     else if(annotationType.Equals(PdfName.3DAnnotation)) return new 3DAnnotation(baseObject);
      else // Other annotation type.
        return new Annotation(baseObject);
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    protected Annotation(
      Page page,
      PdfName subtype,
      RectangleF box,
      string text
      ) : base(
        page.Document,
        new PdfDictionary(
          new PdfName[]
          {
            PdfName.Type,
            PdfName.Subtype,
            PdfName.Border
          },
          new PdfDirectObject[]
          {
            PdfName.Annot,
            subtype,
            new PdfArray(new PdfDirectObject[]{PdfInteger.Default, PdfInteger.Default, PdfInteger.Default}) // NOTE: Hide border by default.
          }
          )
        )
    {
      page.Annotations.Add(this);
      Box = box;
      Text = text;
    }

    protected Annotation(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets action to be performed when the annotation is activated.</summary>
    */
    [PDF(VersionEnum.PDF11)]
    public virtual actions.Action Action
    {
      get
      {return actions.Action.Wrap(BaseDataObject[PdfName.A]);}
      set
      {BaseDataObject[PdfName.A] = PdfObjectWrapper.GetBaseObject(value);}
    }

    /**
      <summary>Gets/Sets the annotation's behavior in response to various trigger events.</summary>
    */
    [PDF(VersionEnum.PDF12)]
    public virtual AnnotationActions Actions
    {
      get
      {return new AnnotationActions(this, BaseDataObject.Get<PdfDictionary>(PdfName.AA));}
      set
      {BaseDataObject[PdfName.AA] = PdfObjectWrapper.GetBaseObject(value);}
    }

    /**
      <summary>Gets/Sets the appearance specifying how the annotation is presented visually on the page.</summary>
    */
    [PDF(VersionEnum.PDF12)]
    public Appearance Appearance
    {
      get
      {return Appearance.Wrap(BaseDataObject.Get<PdfDictionary>(PdfName.AP));}
      set
      {BaseDataObject[PdfName.AP] = PdfObjectWrapper.GetBaseObject(value);}
    }

    /**
      <summary>Gets/Sets the border style.</summary>
    */
    [PDF(VersionEnum.PDF11)]
    public Border Border
    {
      get
      {return new Border(BaseDataObject.Get<PdfDictionary>(PdfName.BS));}
      set
      {
        BaseDataObject[PdfName.BS] = PdfObjectWrapper.GetBaseObject(value);
        if(value != null)
        {BaseDataObject.Remove(PdfName.Border);}
      }
    }

    /**
      <summary>Gets/Sets the location of the annotation on the page in default user space units.
      </summary>
    */
    public RectangleF Box
    {
      get
      {
        org.pdfclown.objects.Rectangle box = org.pdfclown.objects.Rectangle.Wrap(BaseDataObject[PdfName.Rect]);
        return new RectangleF(
          (float)box.Left,
          (float)(GetPageHeight() - box.Top),
          (float)box.Width,
          (float)box.Height
          );
      }
      set
      {
        BaseDataObject[PdfName.Rect] = new org.pdfclown.objects.Rectangle(
          value.X,
          GetPageHeight() - value.Y,
          value.Width,
          value.Height
          ).BaseDataObject;
      }
    }

    /**
      <summary>Gets/Sets the annotation color.</summary>
    */
    [PDF(VersionEnum.PDF11)]
    public DeviceColor Color
    {
      get
      {return DeviceColor.Get((PdfArray)BaseDataObject[PdfName.C]);}
      set
      {BaseDataObject[PdfName.C] = PdfObjectWrapper.GetBaseObject(value);}
    }

    /**
      <summary>Deletes this annotation removing also its reference on the page.</summary>
    */
    public override bool Delete(
      )
    {
      // Shallow removal (references):
      // * reference on page
      Page.Annotations.Remove(this);

      // Deep removal (indirect object).
      return base.Delete();
    }

    /**
      <summary>Gets/Sets the annotation flags.</summary>
    */
    [PDF(VersionEnum.PDF11)]
    public FlagsEnum Flags
    {
      get
      {
        PdfInteger flagsObject = (PdfInteger)BaseDataObject[PdfName.F];
        return flagsObject == null
          ? 0
          : (FlagsEnum)Enum.ToObject(typeof(FlagsEnum), flagsObject.RawValue);
      }
      set
      {BaseDataObject[PdfName.F] = PdfInteger.Get((int)value);}
    }

    /**
      <summary>Gets/Sets the date and time when the annotation was most recently modified.</summary>
    */
    [PDF(VersionEnum.PDF11)]
    public DateTime? ModificationDate
    {
      get
      {
        /*
          NOTE: Despite PDF date being the preferred format, loose formats are tolerated by the spec.
        */
        PdfDirectObject modificationDateObject = BaseDataObject[PdfName.M];
        return (DateTime?)(modificationDateObject is PdfDate ? ((PdfDate)modificationDateObject).Value : null);
      }
      set
      {BaseDataObject[PdfName.M] = PdfDate.Get(value);}
    }

    /**
      <summary>Gets/Sets the annotation name.</summary>
      <remarks>The annotation name uniquely identifies the annotation among all the annotations on its page.</remarks>
    */
    [PDF(VersionEnum.PDF14)]
    public string Name
    {
      get
      {return (string)PdfSimpleObject<Object>.GetValue(BaseDataObject[PdfName.NM]);}
      set
      {BaseDataObject[PdfName.NM] = PdfTextString.Get(value);}
    }

    /**
      <summary>Gets/Sets the associated page.</summary>
    */
    [PDF(VersionEnum.PDF13)]
    public Page Page
    {
      get
      {return Page.Wrap(BaseDataObject[PdfName.P]);}
    }

    /**
      <summary>Gets/Sets whether to print the annotation when the page is printed.</summary>
    */
    [PDF(VersionEnum.PDF11)]
    public bool Printable
    {
      get
      {return (Flags & FlagsEnum.Print) == FlagsEnum.Print;}
      set
      {Flags = EnumUtils.Mask(Flags, FlagsEnum.Print, value);}
    }

    /**
      <summary>Gets/Sets the annotation text.</summary>
      <remarks>Depending on the annotation type, the text may be either directly displayed
      or (in case of non-textual annotations) used as alternate description.</remarks>
    */
    public string Text
    {
      get
      {return (string)PdfSimpleObject<Object>.GetValue(BaseDataObject[PdfName.Contents]);}
      set
      {
        if(value == null)
        {BaseDataObject.Remove(PdfName.Contents);}
        else
        {BaseDataObject[PdfName.Contents] = PdfTextString.Get(value);}
      }
    }

    /**
      <summary>Gets/Sets whether the annotation is visible.</summary>
    */
    [PDF(VersionEnum.PDF11)]
    public bool Visible
    {
      get
      {return (Flags & FlagsEnum.Hidden) != FlagsEnum.Hidden;}
      set
      {Flags = EnumUtils.Mask(Flags, FlagsEnum.Hidden, !value);}
    }

    #region ILayerable
    [PDF(VersionEnum.PDF15)]
    public LayerEntity Layer
    {
      get
      {return (LayerEntity)PropertyList.Wrap(BaseDataObject[PdfName.OC]);}
      set
      {BaseDataObject[PdfName.OC] = PdfObjectWrapper.GetBaseObject(value);}
    }
    #endregion
    #endregion

    #region private
    private float GetPageHeight(
      )
    {
      Page page = Page;
      return (page != null
          ? page.Box.Height
          : Document.GetSize().Height);
    }
    #endregion
    #endregion
    #endregion
  }
}