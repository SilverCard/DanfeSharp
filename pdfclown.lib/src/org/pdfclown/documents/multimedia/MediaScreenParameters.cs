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
using org.pdfclown.util;

using System;
using drawing = System.Drawing;

namespace org.pdfclown.documents.multimedia
{
  /**
    <summary>Media screen parameters [PDF:1.7:9.1.5].</summary>
  */
  [PDF(VersionEnum.PDF15)]
  public sealed class MediaScreenParameters
    : PdfObjectWrapper<PdfDictionary>
  {
    #region types
    /**
      <summary>Media screen parameters viability.</summary>
    */
    public class Viability
      : PdfObjectWrapper<PdfDictionary>
    {
      public class FloatingWindowParametersObject
        : PdfObjectWrapper<PdfDictionary>
      {
        public enum LocationEnum
        {
          /**
            <summary>Upper-left corner.</summary>
          */
          UpperLeft,
          /**
            <summary>Upper center.</summary>
          */
          UpperCenter,
          /**
            <summary>Upper-right corner.</summary>
          */
          UpperRight,
          /**
            <summary>Center left.</summary>
          */
          CenterLeft,
          /**
            <summary>Center.</summary>
          */
          Center,
          /**
            <summary>Center right.</summary>
          */
          CenterRight,
          /**
            <summary>Lower-left corner.</summary>
          */
          LowerLeft,
          /**
            <summary>Lower center.</summary>
          */
          LowerCenter,
          /**
            <summary>Lower-right corner.</summary>
          */
          LowerRight
        }

        public enum OffscreenBehaviorEnum
        {
          /**
            <summary>Take no special action.</summary>
          */
          None,
          /**
            <summary>Move and/or resize the window so that it is on-screen.</summary>
          */
          Adapt,
          /**
            <summary>Consider the object to be non-viable.</summary>
          */
          NonViable
        }

        public enum RelatedWindowEnum
        {
          /**
            <summary>The document window.</summary>
          */
          Document,
          /**
            <summary>The application window.</summary>
          */
          Application,
          /**
            <summary>The full virtual desktop.</summary>
          */
          Desktop,
          /**
            <summary>The monitor specified by <see cref="MediaScreenParameters.Viability.MonitorSpecifier"/>.</summary>
          */
          Custom
        }

        public enum ResizeBehaviorEnum
        {
          /**
            <summary>Not resizable.</summary>
          */
          None,
          /**
            <summary>Resizable preserving its aspect ratio.</summary>
          */
          AspectRatioLocked,
          /**
            <summary>Resizable without preserving its aspect ratio.</summary>
          */
          Free
        }

        public FloatingWindowParametersObject(
          drawing::Size size
          ) : base(
            new PdfDictionary(
              new PdfName[]
              {PdfName.Type},
              new PdfDirectObject[]
              {PdfName.FWParams}
              )
            )
        {Size = size;}

        internal FloatingWindowParametersObject(
          PdfDirectObject baseObject
          ) : base(baseObject)
        {}

        /**
          <summary>Gets/Sets the location where the floating window should be positioned relative to
          the related window.</summary>
        */
        public LocationEnum? Location
        {
          get
          {return LocationEnumExtension.Get((PdfInteger)BaseDataObject[PdfName.P]);}
          set
          {BaseDataObject[PdfName.P] = (value.HasValue ? value.Value.GetCode() : null);}
        }

        /**
          <summary>Gets/Sets what should occur if the floating window is positioned totally or
          partially offscreen (that is, not visible on any physical monitor).</summary>
        */
        public OffscreenBehaviorEnum? OffscreenBehavior
        {
          get
          {return OffscreenBehaviorEnumExtension.Get((PdfInteger)BaseDataObject[PdfName.O]);}
          set
          {BaseDataObject[PdfName.O] = (value.HasValue ? value.Value.GetCode() : null);}
        }

        /**
          <summary>Gets/Sets the window relative to which the floating window should be positioned.
          </summary>
        */
        public RelatedWindowEnum? RelatedWindow
        {
          get
          {return RelatedWindowEnumExtension.Get((PdfInteger)BaseDataObject[PdfName.RT]);}
          set
          {BaseDataObject[PdfName.RT] = (value.HasValue ? value.Value.GetCode() : null);}
        }

        /**
          <summary>Gets/Sets how the floating window may be resized by a user.</summary>
        */
        public ResizeBehaviorEnum? ResizeBehavior
        {
          get
          {return ResizeBehaviorEnumExtension.Get((PdfInteger)BaseDataObject[PdfName.R]);}
          set
          {BaseDataObject[PdfName.R] = (value.HasValue ? value.Value.GetCode() : null);}
        }

        /**
          <summary>Gets/Sets the floating window's width and height, in pixels.</summary>
          <remarks>These values correspond to the dimensions of the rectangle in which the media
          will play, not including such items as title bar and resizing handles.</remarks>
        */
        public drawing::Size Size
        {
          get
          {
            PdfArray sizeObject = (PdfArray)BaseDataObject[PdfName.D];
            return new drawing::Size(((PdfInteger)sizeObject[0]).IntValue, ((PdfInteger)sizeObject[1]).IntValue);
          }
          set
          {BaseDataObject[PdfName.D] = new PdfArray(PdfInteger.Get(value.Width), PdfInteger.Get(value.Height));}
        }

        /**
          <summary>Gets/Sets whether the floating window should include user interface elements that
          allow a user to close it.</summary>
          <remarks>Meaningful only if <see cref="TitleBarVisible"/> is true.</remarks>
        */
        public bool Closeable
        {
          get
          {return (bool)PdfBoolean.GetValue(BaseDataObject[PdfName.UC], true);}
          set
          {BaseDataObject[PdfName.UC] = PdfBoolean.Get(value);}
        }

        /**
          <summary>Gets/Sets whether the floating window should have a title bar.</summary>
        */
        public bool TitleBarVisible
        {
          get
          {return (bool)PdfBoolean.GetValue(BaseDataObject[PdfName.T], true);}
          set
          {BaseDataObject[PdfName.T] = PdfBoolean.Get(value);}
        }

        //TODO: TT entry!
      }

      public enum WindowTypeEnum
      {
        /**
          <summary>A floating window.</summary>
        */
        Floating,
        /**
          <summary>A full-screen window that obscures all other windows.</summary>
        */
        FullScreen,
        /**
          <summary>A hidden window.</summary>
        */
        Hidden,
        /**
          <summary>The rectangle occupied by the {@link Screen screen annotation} associated with
          the media rendition.</summary>
        */
        Annotation
      }

      internal Viability(
        PdfDirectObject baseObject
        ) : base(baseObject)
      {}

      /**
        <summary>Gets/Sets the background color for the rectangle in which the media is being played.
        </summary>
        <remarks>This color is used if the media object does not entirely cover the rectangle or if
        it has transparent sections.</remarks>
      */
      public DeviceRGBColor BackgroundColor
      {
        get
        {return DeviceRGBColor.Get((PdfArray)BaseDataObject[PdfName.B]);}
        set
        {BaseDataObject[PdfName.B] = PdfObjectWrapper.GetBaseObject(value);}
      }

      /**
        <summary>Gets/Sets the opacity of the background color.</summary>
        <returns>A number in the range 0 to 1, where 0 means full transparency and 1 full opacity.
        </returns>
      */
      public double BackgroundOpacity
      {
        get
        {return (double)PdfReal.GetValue(BaseDataObject[PdfName.O], 1d);}
        set
        {
          if(value < 0)
          {value = 0;}
          else if(value > 1)
          {value = 1;}
          BaseDataObject[PdfName.O] = PdfReal.Get(value);
        }
      }

      /**
        <summary>Gets/Sets the options used in displaying floating windows.</summary>
      */
      public FloatingWindowParametersObject FloatingWindowParameters
      {
        get
        {return new FloatingWindowParametersObject(BaseDataObject.Get<PdfDictionary>(PdfName.F));}
        set
        {BaseDataObject[PdfName.F] = PdfObjectWrapper.GetBaseObject(value);}
      }

      /**
        <summary>Gets/Sets which monitor in a multi-monitor system a floating or full-screen window
        should appear on.</summary>
      */
      public MonitorSpecifierEnum? MonitorSpecifier
      {
        get
        {return MonitorSpecifierEnumExtension.Get((PdfInteger)BaseDataObject[PdfName.M]);}
        set
        {BaseDataObject[PdfName.M] = (value.HasValue ? value.Value.GetCode() : null);}
      }

      /**
        <summary>Gets/Sets the type of window that the media object should play in.</summary>
      */
      public WindowTypeEnum? WindowType
      {
        get
        {return WindowTypeEnumExtension.Get((PdfInteger)BaseDataObject[PdfName.W]);}
        set
        {BaseDataObject[PdfName.W] = (value.HasValue ? value.Value.GetCode() : null);}
      }
    }
    #endregion

    #region dynamic
    #region constructors
    public MediaScreenParameters(
      Document context
      ) : base(
        context,
        new PdfDictionary(
          new PdfName[]
          {PdfName.Type},
          new PdfDirectObject[]
          {PdfName.MediaScreenParams}
          )
        )
    {}

    internal MediaScreenParameters(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the preferred options the renderer should attempt to honor without affecting
      its viability.</summary>
    */
    public Viability Preferences
    {
      get
      {return new Viability(BaseDataObject.Get<PdfDictionary>(PdfName.BE));}
      set
      {BaseDataObject[PdfName.BE] = PdfObjectWrapper.GetBaseObject(value);}
    }
  
    /**
      <summary>Gets/Sets the minimum requirements the renderer must honor in order to be considered
      viable.</summary>
    */
    public Viability Requirements
    {
      get
      {return new Viability(BaseDataObject.Get<PdfDictionary>(PdfName.MH));}
      set
      {BaseDataObject[PdfName.MH] = PdfObjectWrapper.GetBaseObject(value);}
    }
    #endregion
    #endregion
    #endregion
  }

  internal static class LocationEnumExtension
  {
    private static readonly BiDictionary<MediaScreenParameters.Viability.FloatingWindowParametersObject.LocationEnum,PdfInteger> codes;

    static LocationEnumExtension()
    {
      codes = new BiDictionary<MediaScreenParameters.Viability.FloatingWindowParametersObject.LocationEnum,PdfInteger>();
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.LocationEnum.UpperLeft] = new PdfInteger(0);
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.LocationEnum.UpperCenter] = new PdfInteger(1);
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.LocationEnum.UpperRight] = new PdfInteger(2);
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.LocationEnum.CenterLeft] = new PdfInteger(3);
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.LocationEnum.Center] = new PdfInteger(4);
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.LocationEnum.CenterRight] = new PdfInteger(5);
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.LocationEnum.LowerLeft] = new PdfInteger(6);
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.LocationEnum.LowerCenter] = new PdfInteger(7);
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.LocationEnum.LowerRight] = new PdfInteger(8);
    }

    public static MediaScreenParameters.Viability.FloatingWindowParametersObject.LocationEnum? Get(
      PdfInteger code
      )
    {
      if(code == null)
        return MediaScreenParameters.Viability.FloatingWindowParametersObject.LocationEnum.Center;

      MediaScreenParameters.Viability.FloatingWindowParametersObject.LocationEnum? location = codes.GetKey(code);
      if(!location.HasValue)
        throw new NotSupportedException("Location unknown: " + code);

      return location;
    }

    public static PdfInteger GetCode(
      this MediaScreenParameters.Viability.FloatingWindowParametersObject.LocationEnum location
      )
    {return codes[location];}
  }

  internal static class OffscreenBehaviorEnumExtension
  {
    private static readonly BiDictionary<MediaScreenParameters.Viability.FloatingWindowParametersObject.OffscreenBehaviorEnum,PdfInteger> codes;

    static OffscreenBehaviorEnumExtension()
    {
      codes = new BiDictionary<MediaScreenParameters.Viability.FloatingWindowParametersObject.OffscreenBehaviorEnum,PdfInteger>();
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.OffscreenBehaviorEnum.None] = new PdfInteger(0);
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.OffscreenBehaviorEnum.Adapt] = new PdfInteger(1);
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.OffscreenBehaviorEnum.NonViable] = new PdfInteger(2);
    }

    public static MediaScreenParameters.Viability.FloatingWindowParametersObject.OffscreenBehaviorEnum? Get(
      PdfInteger code
      )
    {
      if(code == null)
        return MediaScreenParameters.Viability.FloatingWindowParametersObject.OffscreenBehaviorEnum.Adapt;

      MediaScreenParameters.Viability.FloatingWindowParametersObject.OffscreenBehaviorEnum? offscreenBehavior = codes.GetKey(code);
      if(!offscreenBehavior.HasValue)
        throw new NotSupportedException("Offscreen behavior unknown: " + code);

      return offscreenBehavior;
    }

    public static PdfInteger GetCode(
      this MediaScreenParameters.Viability.FloatingWindowParametersObject.OffscreenBehaviorEnum offscreenBehavior
      )
    {return codes[offscreenBehavior];}
  }

  internal static class RelatedWindowEnumExtension
  {
    private static readonly BiDictionary<MediaScreenParameters.Viability.FloatingWindowParametersObject.RelatedWindowEnum,PdfInteger> codes;

    static RelatedWindowEnumExtension()
    {
      codes = new BiDictionary<MediaScreenParameters.Viability.FloatingWindowParametersObject.RelatedWindowEnum,PdfInteger>();
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.RelatedWindowEnum.Document] = new PdfInteger(0);
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.RelatedWindowEnum.Application] = new PdfInteger(1);
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.RelatedWindowEnum.Desktop] = new PdfInteger(2);
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.RelatedWindowEnum.Custom] = new PdfInteger(3);
    }

    public static MediaScreenParameters.Viability.FloatingWindowParametersObject.RelatedWindowEnum? Get(
      PdfInteger code
      )
    {
      if(code == null)
        return MediaScreenParameters.Viability.FloatingWindowParametersObject.RelatedWindowEnum.Document;

      MediaScreenParameters.Viability.FloatingWindowParametersObject.RelatedWindowEnum? relatedWindow = codes.GetKey(code);
      if(!relatedWindow.HasValue)
        throw new NotSupportedException("Related window unknown: " + code);

      return relatedWindow;
    }

    public static PdfInteger GetCode(
      this MediaScreenParameters.Viability.FloatingWindowParametersObject.RelatedWindowEnum relatedWindow
      )
    {return codes[relatedWindow];}
  }

  internal static class ResizeBehaviorEnumExtension
  {
    private static readonly BiDictionary<MediaScreenParameters.Viability.FloatingWindowParametersObject.ResizeBehaviorEnum,PdfInteger> codes;

    static ResizeBehaviorEnumExtension()
    {
      codes = new BiDictionary<MediaScreenParameters.Viability.FloatingWindowParametersObject.ResizeBehaviorEnum,PdfInteger>();
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.ResizeBehaviorEnum.None] = new PdfInteger(0);
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.ResizeBehaviorEnum.AspectRatioLocked] = new PdfInteger(1);
      codes[MediaScreenParameters.Viability.FloatingWindowParametersObject.ResizeBehaviorEnum.Free] = new PdfInteger(2);
    }

    public static MediaScreenParameters.Viability.FloatingWindowParametersObject.ResizeBehaviorEnum? Get(
      PdfInteger code
      )
    {
      if(code == null)
        return MediaScreenParameters.Viability.FloatingWindowParametersObject.ResizeBehaviorEnum.None;

      MediaScreenParameters.Viability.FloatingWindowParametersObject.ResizeBehaviorEnum? resizeBehavior = codes.GetKey(code);
      if(!resizeBehavior.HasValue)
        throw new NotSupportedException("Resize behavior unknown: " + code);

      return resizeBehavior;
    }

    public static PdfInteger GetCode(
      this MediaScreenParameters.Viability.FloatingWindowParametersObject.ResizeBehaviorEnum resizeBehavior
      )
    {return codes[resizeBehavior];}
  }

  internal static class WindowTypeEnumExtension
  {
    private static readonly BiDictionary<MediaScreenParameters.Viability.WindowTypeEnum,PdfInteger> codes;

    static WindowTypeEnumExtension()
    {
      codes = new BiDictionary<MediaScreenParameters.Viability.WindowTypeEnum,PdfInteger>();
      codes[MediaScreenParameters.Viability.WindowTypeEnum.Floating] = new PdfInteger(0);
      codes[MediaScreenParameters.Viability.WindowTypeEnum.FullScreen] = new PdfInteger(1);
      codes[MediaScreenParameters.Viability.WindowTypeEnum.Hidden] = new PdfInteger(2);
      codes[MediaScreenParameters.Viability.WindowTypeEnum.Annotation] = new PdfInteger(3);
    }

    public static MediaScreenParameters.Viability.WindowTypeEnum? Get(
      PdfInteger code
      )
    {
      if(code == null)
        return MediaScreenParameters.Viability.WindowTypeEnum.Annotation;

      MediaScreenParameters.Viability.WindowTypeEnum? windowType = codes.GetKey(code);
      if(!windowType.HasValue)
        throw new NotSupportedException("Window type unknown: " + code);

      return windowType;
    }

    public static PdfInteger GetCode(
      this MediaScreenParameters.Viability.WindowTypeEnum windowType
      )
    {return codes[windowType];}
  }
}