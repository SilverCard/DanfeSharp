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
using org.pdfclown.documents.contents.colorSpaces;
using org.pdfclown.documents.contents.composition;
using org.pdfclown.documents.contents.xObjects;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;

namespace org.pdfclown.documents.interaction.annotations
{
  /**
    <summary>Appearance characteristics [PDF:1.6:8.4.5].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class AppearanceCharacteristics
    : PdfObjectWrapper<PdfDictionary>
  {
    #region types
    /**
      <summary>Caption position relative to its icon [PDF:1.6:8.4.5].</summary>
    */
    public enum CaptionPositionEnum
    {
      /**
        <summary>Caption only (no icon).</summary>
      */
      CaptionOnly = 0,
      /**
        <summary>No caption (icon only).</summary>
      */
      NoCaption = 1,
      /**
        <summary>Caption below the icon.</summary>
      */
      Below = 2,
      /**
        <summary>Caption above the icon.</summary>
      */
      Above = 3,
      /**
        <summary>Caption to the right of the icon.</summary>
      */
      Right = 4,
      /**
        <summary>Caption to the left of the icon.</summary>
      */
      Left = 5,
      /**
        <summary>Caption overlaid directly on the icon.</summary>
      */
      Overlaid = 6
    };

    /**
      <summary>Icon fit [PDF:1.6:8.6.6].</summary>
    */
    public class IconFitObject
      : PdfObjectWrapper<PdfDictionary>
    {
      #region types
      /**
        <summary>Scaling mode [PDF:1.6:8.6.6].</summary>
      */
      public enum ScaleModeEnum
      {
        /**
          <summary>Always scale.</summary>
        */
        Always,
        /**
          <summary>Scale only when the icon is bigger than the annotation box.</summary>
        */
        Bigger,
        /**
          <summary>Scale only when the icon is smaller than the annotation box.</summary>
        */
        Smaller,
        /**
          <summary>Never scale.</summary>
        */
        Never
      };

      /**
        <summary>Scaling type [PDF:1.6:8.6.6].</summary>
      */
      public enum ScaleTypeEnum
      {
        /**
          <summary>Scale the icon to fill the annotation box exactly,
          without regard to its original aspect ratio.</summary>
        */
        Anamorphic,
        /**
          <summary>Scale the icon to fit the width or height of the annotation box,
          while maintaining the icon's original aspect ratio.</summary>
        */
        Proportional
      };
      #endregion

      #region static
      #region fields
      private static readonly Dictionary<ScaleModeEnum,PdfName> ScaleModeEnumCodes;
      private static readonly Dictionary<ScaleTypeEnum,PdfName> ScaleTypeEnumCodes;
      #endregion

      #region constructors
      static IconFitObject()
      {
        ScaleModeEnumCodes = new Dictionary<ScaleModeEnum,PdfName>();
        ScaleModeEnumCodes[ScaleModeEnum.Always] = PdfName.A;
        ScaleModeEnumCodes[ScaleModeEnum.Bigger] = PdfName.B;
        ScaleModeEnumCodes[ScaleModeEnum.Smaller] = PdfName.S;
        ScaleModeEnumCodes[ScaleModeEnum.Never] = PdfName.N;

        ScaleTypeEnumCodes = new Dictionary<ScaleTypeEnum,PdfName>();
        ScaleTypeEnumCodes[ScaleTypeEnum.Anamorphic] = PdfName.A;
        ScaleTypeEnumCodes[ScaleTypeEnum.Proportional] = PdfName.P;
      }
      #endregion

      #region interface
      #region private
      /**
        <summary>Gets the code corresponding to the given value.</summary>
      */
      private static PdfName ToCode(
        ScaleModeEnum value
        )
      {return ScaleModeEnumCodes[value];}

      /**
        <summary>Gets the code corresponding to the given value.</summary>
      */
      private static PdfName ToCode(
        ScaleTypeEnum value
        )
      {return ScaleTypeEnumCodes[value];}

      /**
        <summary>Gets the scaling mode corresponding to the given value.</summary>
      */
      private static ScaleModeEnum ToScaleModeEnum(
        PdfName value
        )
      {
        foreach(KeyValuePair<ScaleModeEnum,PdfName> scaleMode in ScaleModeEnumCodes)
        {
          if(scaleMode.Value.Equals(value))
            return scaleMode.Key;
        }
        return ScaleModeEnum.Always;
      }

      /**
        <summary>Gets the scaling type corresponding to the given value.</summary>
      */
      private static ScaleTypeEnum ToScaleTypeEnum(
        PdfName value
        )
      {
        foreach(KeyValuePair<ScaleTypeEnum,PdfName> scaleType in ScaleTypeEnumCodes)
        {
          if(scaleType.Value.Equals(value))
            return scaleType.Key;
        }
        return ScaleTypeEnum.Proportional;
      }
      #endregion
      #endregion
      #endregion

      #region dynamic
      #region constructors
      public IconFitObject(
        Document context
        ) : base(context, new PdfDictionary())
      {}

      internal IconFitObject(
        PdfDirectObject baseObject
        ) : base(baseObject)
      {}
      #endregion

      #region interface
      #region public
      /**
        <summary>Gets/Sets whether not to take into consideration the line width of the border.</summary>
      */
      public bool BorderExcluded
      {
        get
        {
          PdfBoolean borderExcludedObject = (PdfBoolean)BaseDataObject[PdfName.FB];
          return borderExcludedObject != null
            ? borderExcludedObject.RawValue
            : false;
        }
        set
        {BaseDataObject[PdfName.FB] = PdfBoolean.Get(value);}
      }

      /**
        <summary>Gets/Sets the circumstances under which the icon should be scaled inside the annotation box.</summary>
      */
      public ScaleModeEnum ScaleMode
      {
        get
        {return ToScaleModeEnum((PdfName)BaseDataObject[PdfName.SW]);}
        set
        {BaseDataObject[PdfName.SW] = ToCode(value);}
      }

      /**
        <summary>Gets/Sets the type of scaling to use.</summary>
      */
      public ScaleTypeEnum ScaleType
      {
        get
        {return ToScaleTypeEnum((PdfName)BaseDataObject[PdfName.S]);}
        set
        {BaseDataObject[PdfName.S] = ToCode(value);}
      }

      /**
        <summary>Gets/Sets the horizontal alignment of the icon inside the annotation box.</summary>
      */
      public XAlignmentEnum XAlignment
      {
        get
        {
          /*
            NOTE: 'A' entry may be undefined.
          */
          PdfArray alignmentObject = (PdfArray)BaseDataObject[PdfName.A];
          if(alignmentObject == null)
            return XAlignmentEnum.Center;

          switch((int)Math.Round(((IPdfNumber)alignmentObject[0]).RawValue/.5))
          {
            case 0: return XAlignmentEnum.Left;
            case 2: return XAlignmentEnum.Right;
            default: return XAlignmentEnum.Center;
          }
        }
        set
        {
          /*
            NOTE: 'A' entry may be undefined.
          */
          PdfArray alignmentObject = (PdfArray)BaseDataObject[PdfName.A];
          if(alignmentObject == null)
          {
            alignmentObject = new PdfArray(
              new PdfDirectObject[]
              {
                PdfReal.Get(0.5),
                PdfReal.Get(0.5)
              }
              );
            BaseDataObject[PdfName.A] = alignmentObject;
          }

          double objectValue;
          switch(value)
          {
            case XAlignmentEnum.Left: objectValue = 0; break;
            case XAlignmentEnum.Right: objectValue = 1; break;
            default: objectValue = 0.5; break;
          }
          alignmentObject[0] = PdfReal.Get(objectValue);
        }
      }

      /**
        <summary>Gets/Sets the vertical alignment of the icon inside the annotation box.</summary>
      */
      public YAlignmentEnum YAlignment
      {
        get
        {
          /*
            NOTE: 'A' entry may be undefined.
          */
          PdfArray alignmentObject = (PdfArray)BaseDataObject[PdfName.A];
          if(alignmentObject == null)
            return YAlignmentEnum.Middle;

          switch((int)Math.Round(((IPdfNumber)alignmentObject[1]).RawValue/.5))
          {
            case 0: return YAlignmentEnum.Bottom;
            case 2: return YAlignmentEnum.Top;
            default: return YAlignmentEnum.Middle;
          }
        }
        set
        {
          /*
            NOTE: 'A' entry may be undefined.
          */
          PdfArray alignmentObject = (PdfArray)BaseDataObject[PdfName.A];
          if(alignmentObject == null)
          {
            alignmentObject = new PdfArray(
              new PdfDirectObject[]
              {
                PdfReal.Get(0.5),
                PdfReal.Get(0.5)
              }
              );
            BaseDataObject[PdfName.A] = alignmentObject;
          }

          double objectValue;
          switch(value)
          {
            case YAlignmentEnum.Bottom: objectValue = 0; break;
            case YAlignmentEnum.Top: objectValue = 1; break;
            default: objectValue = 0.5; break;
          }
          alignmentObject[1] = PdfReal.Get(objectValue);
        }
      }
      #endregion
      #endregion
      #endregion
    }

    /**
      <summary>Annotation orientation [PDF:1.6:8.4.5].</summary>
    */
    public enum OrientationEnum
    {
      /**
        <summary>Upward.</summary>
      */
      Up = 0,
      /**
        <summary>Leftward.</summary>
      */
      Left = 90,
      /**
        <summary>Downward.</summary>
      */
      Down = 180,
      /**
        <summary>Rightward.</summary>
      */
      Right = 270
    };
    #endregion

    #region static
    #region interface
    #region public
    public static AppearanceCharacteristics Wrap(
      PdfDirectObject baseObject
      )
    {return baseObject != null ? new AppearanceCharacteristics(baseObject) : null;}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public AppearanceCharacteristics(
      Document context
      ) : base(context, new PdfDictionary())
    {}

    private AppearanceCharacteristics(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the widget annotation's alternate (down) caption,
      displayed when the mouse button is pressed within its active area
      (Pushbutton fields only).</summary>
    */
    public string AlternateCaption
    {
      get
      {
        PdfTextString alternateCaptionObject = (PdfTextString)BaseDataObject[PdfName.AC];
        return alternateCaptionObject != null ? (string)alternateCaptionObject.Value : null;
      }
      set
      {BaseDataObject[PdfName.AC] = new PdfTextString(value);}
    }

    /**
      <summary>Gets/Sets the widget annotation's alternate (down) icon definition,
      displayed when the mouse button is pressed within its active area
      (Pushbutton fields only).</summary>
    */
    public FormXObject AlternateIcon
    {
      get
      {return FormXObject.Wrap(BaseDataObject[PdfName.IX]);}
      set
      {BaseDataObject[PdfName.IX] = value.BaseObject;}
    }

    /**
      <summary>Gets/Sets the widget annotation's background color.</summary>
    */
    public DeviceColor BackgroundColor
    {
      get
      {return GetColor(PdfName.BG);}
      set
      {SetColor(PdfName.BG, value);}
    }

    /**
      <summary>Gets/Sets the widget annotation's border color.</summary>
    */
    public DeviceColor BorderColor
    {
      get
      {return GetColor(PdfName.BC);}
      set
      {SetColor(PdfName.BC, value);}
    }

    /**
      <summary>Gets/Sets the position of the caption relative to its icon (Pushbutton fields only).</summary>
    */
    public CaptionPositionEnum CaptionPosition
    {
      get
      {
        PdfInteger captionPositionObject = (PdfInteger)BaseDataObject[PdfName.TP];
        return captionPositionObject != null ? (CaptionPositionEnum)captionPositionObject.RawValue : CaptionPositionEnum.CaptionOnly;
      }
      set
      {BaseDataObject[PdfName.TP] = PdfInteger.Get((int)value);}
    }

    /**
      <summary>Gets/Sets the icon fit specifying how to display the widget annotation's icon
      within its annotation box (Pushbutton fields only).
      If present, the icon fit applies to all of the annotation's icons
      (normal, rollover, and alternate).</summary>
    */
    public IconFitObject IconFit
    {
      get
      {
        PdfDirectObject iconFitObject = BaseDataObject[PdfName.IF];
        return iconFitObject != null ? new IconFitObject(iconFitObject) : null;
      }
      set
      {BaseDataObject[PdfName.IF] = PdfObjectWrapper.GetBaseObject(value);}
    }

    /**
      <summary>Gets/Sets the widget annotation's normal caption,
      displayed when it is not interacting with the user (Button fields only).</summary>
    */
    public string NormalCaption
    {
      get
      {
        PdfTextString normalCaptionObject = (PdfTextString)BaseDataObject[PdfName.CA];
        return normalCaptionObject != null ? (string)normalCaptionObject.Value : null;
      }
      set
      {BaseDataObject[PdfName.CA] = PdfTextString.Get(value);}
    }

    /**
      <summary>Gets/Sets the widget annotation's normal icon definition,
      displayed when it is not interacting with the user (Pushbutton fields only).</summary>
    */
    public FormXObject NormalIcon
    {
      get
      {return FormXObject.Wrap(BaseDataObject[PdfName.I]);}
      set
      {BaseDataObject[PdfName.I] = PdfObjectWrapper.GetBaseObject(value);}
    }

    /**
      <summary>Gets/Sets the widget annotation's orientation.</summary>
    */
    public OrientationEnum Orientation
    {
      get
      {
        PdfInteger orientationObject = (PdfInteger)BaseDataObject[PdfName.R];
        return orientationObject != null ? (OrientationEnum)orientationObject.RawValue : OrientationEnum.Up;
      }
      set
      {BaseDataObject[PdfName.R] = PdfInteger.Get((int)value);}
    }

    /**
      <summary>Gets/Sets the widget annotation's rollover caption,
      displayed when the user rolls the cursor into its active area
      without pressing the mouse button (Pushbutton fields only).</summary>
    */
    public string RolloverCaption
    {
      get
      {
        PdfTextString rolloverCaptionObject = (PdfTextString)BaseDataObject[PdfName.RC];
        return rolloverCaptionObject != null ? (string)rolloverCaptionObject.Value : null;
      }
      set
      {BaseDataObject[PdfName.RC] = PdfTextString.Get(value);}
    }

    /**
      <summary>Gets/Sets the widget annotation's rollover icon definition,
      displayed when the user rolls the cursor into its active area
      without pressing the mouse button (Pushbutton fields only).</summary>
    */
    public FormXObject RolloverIcon
    {
      get
      {return FormXObject.Wrap(BaseDataObject[PdfName.RI]);}
      set
      {BaseDataObject[PdfName.RI] = PdfObjectWrapper.GetBaseObject(value);}
    }
    #endregion

    #region private
    private DeviceColor GetColor(
      PdfName key
      )
    {return DeviceColor.Get((PdfArray)BaseDataObject.Resolve(key));}

    private void SetColor(
      PdfName key,
      DeviceColor value
      )
    {BaseDataObject[key] = PdfObjectWrapper.GetBaseObject(value);}
    #endregion
    #endregion
    #endregion
  }
}