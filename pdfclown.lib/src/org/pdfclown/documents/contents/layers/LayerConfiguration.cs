/*
  Copyright 2011-2012 Stefano Chizzolini. http://www.pdfclown.org

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

using org.pdfclown.files;
using org.pdfclown.objects;
using org.pdfclown.util;

using System;

namespace org.pdfclown.documents.contents.layers
{
  /**
    <summary>Optional content configuration [PDF:1.7:4.10.3].</summary>
  */
  [PDF(VersionEnum.PDF15)]
  public class LayerConfiguration
    : PdfObjectWrapper<PdfDictionary>,
      ILayerConfiguration
  {
    /**
      <summary>Base state used to initialize the states of all the layers in a document when this
      configuration is applied.</summary>
    */
    internal enum BaseStateEnum
    {
      /**
        <summary>All the layers are visible.</summary>
      */
      On,
      /**
        <summary>All the layers are invisible.</summary>
      */
      Off,
      /**
        <summary>All the layers are left unchanged.</summary>
      */
      Unchanged
    }

    #region dynamic
    #region constructors
    public LayerConfiguration(
      Document context
      ) : base(context, new PdfDictionary())
    {}

    public LayerConfiguration(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    #region ILayerConfiguration
    public string Creator
    {
      get
      {return (string)PdfSimpleObject<object>.GetValue(BaseDataObject[PdfName.Creator]);}
      set
      {BaseDataObject[PdfName.Creator] = PdfTextString.Get(value);}
    }

    public Layers Layers
    {
      get
      {return Layers.Wrap(BaseDataObject.Get<PdfArray>(PdfName.Order));}
      set
      {BaseDataObject[PdfName.Order] = value.BaseObject;}
    }

    public ListModeEnum ListMode
    {
      get
      {return ListModeEnumExtension.Get((PdfName)BaseDataObject[PdfName.ListMode]);}
      set
      {BaseDataObject[PdfName.ListMode] = value.GetName();}
    }

    public Array<LayerGroup> OptionGroups
    {
      get
      {return Array<LayerGroup>.Wrap<LayerGroup>(BaseDataObject.Get<PdfArray>(PdfName.RBGroups));}
    }

    public string Title
    {
      get
      {return (string)PdfSimpleObject<object>.GetValue(BaseDataObject[PdfName.Name]);}
      set
      {BaseDataObject[PdfName.Name] = PdfTextString.Get(value);}
    }

    public bool? Visible
    {
      get
      {return BaseStateEnumExtension.Get((PdfName)BaseDataObject[PdfName.BaseState]).IsEnabled();}
      set
      {
        /*
          NOTE: Base state can be altered only in case of alternate configuration; default ones MUST
          be set to default state (that is ON).
        */
        if(!(BaseObject.Parent is PdfDictionary)) // Not the default configuration?
        {BaseDataObject[PdfName.BaseState] = BaseStateEnumExtension.Get(value).GetName();}
      }
    }
    #endregion
    #endregion

    #region internal
    internal bool IsVisible(
      Layer layer
      )
    {
      bool? visible = Visible;
      if(!visible.HasValue || visible.Value)
        return !OffLayersObject.Contains(layer.BaseObject);
      else
        return OnLayersObject.Contains(layer.BaseObject);
    }

    /**
      <summary>Sets the usage application for the specified factors.</summary>
      <param name="event_">Situation in which this usage application should be used. May be
        <see cref="PdfName.View">View</see>, <see cref="PdfName.Print">Print</see> or <see
        cref="PdfName.Export">Export</see>.</param>
      <param name="category">Layer usage entry to consider when managing the states of the layer.
      </param>
      <param name="layer">Layer which should have its state automatically managed based on its usage
        information.</param>
      <param name="retain">Whether this usage application has to be kept or removed.</param>
    */
    internal void SetUsageApplication(
      PdfName event_,
      PdfName category,
      Layer layer,
      bool retain
      )
    {
      bool matched = false;
      PdfArray usages = BaseDataObject.Resolve<PdfArray>(PdfName.AS);
      foreach(PdfDirectObject usage in usages)
      {
        PdfDictionary usageDictionary = (PdfDictionary)usage;
        if(usageDictionary[PdfName.Event].Equals(event_)
          && ((PdfArray)usageDictionary[PdfName.Category]).Contains(category))
        {
          PdfArray usageLayers = usageDictionary.Resolve<PdfArray>(PdfName.OCGs);
          if(usageLayers.Contains(layer.BaseObject))
          {
            if(!retain)
            {usageLayers.Remove(layer.BaseObject);}
          }
          else
          {
            if(retain)
            {usageLayers.Add(layer.BaseObject);}
          }
          matched = true;
        }
      }
      if(!matched && retain)
      {
        PdfDictionary usageDictionary = new PdfDictionary();
        {
          usageDictionary[PdfName.Event] = event_;
          usageDictionary.Resolve<PdfArray>(PdfName.Category).Add(category);
          usageDictionary.Resolve<PdfArray>(PdfName.OCGs).Add(layer.BaseObject);
        }
        usages.Add(usageDictionary);
      }
    }

    internal void SetVisible(
      Layer layer,
      bool value
      )
    {
      PdfDirectObject layerObject = layer.BaseObject;
      PdfArray offLayersObject = OffLayersObject;
      PdfArray onLayersObject = OnLayersObject;
      bool? visible = Visible;
      if(!visible.HasValue)
      {
        if(value && !onLayersObject.Contains(layerObject))
        {
          onLayersObject.Add(layerObject);
          offLayersObject.Remove(layerObject);
        }
        else if(!value && !offLayersObject.Contains(layerObject))
        {
          offLayersObject.Add(layerObject);
          onLayersObject.Remove(layerObject);
        }
      }
      else if(!visible.Value)
      {
        if(value && !onLayersObject.Contains(layerObject))
        {onLayersObject.Add(layerObject);}
      }
      else
      {
        if(!value && !offLayersObject.Contains(layerObject))
        {offLayersObject.Add(layerObject);}
      }
    }
    #endregion

    #region private
    /**
      <summary>Gets the collection of the layer objects whose state is set to OFF.</summary>
    */
    private PdfArray OffLayersObject
    {
      get
      {return BaseDataObject.Resolve<PdfArray>(PdfName.OFF);}
    }

    /**
      <summary>Gets the collection of the layer objects whose state is set to ON.</summary>
    */
    private PdfArray OnLayersObject
    {
      get
      {return BaseDataObject.Resolve<PdfArray>(PdfName.ON);}
    }
    #endregion
    #endregion
    #endregion
  }

  internal static class BaseStateEnumExtension
  {
    private static readonly BiDictionary<LayerConfiguration.BaseStateEnum,PdfName> codes;

    static BaseStateEnumExtension()
    {
      codes = new BiDictionary<LayerConfiguration.BaseStateEnum,PdfName>();
      codes[LayerConfiguration.BaseStateEnum.On] = PdfName.ON;
      codes[LayerConfiguration.BaseStateEnum.Off] = PdfName.OFF;
      codes[LayerConfiguration.BaseStateEnum.Unchanged] = PdfName.Unchanged;
    }

    public static LayerConfiguration.BaseStateEnum Get(
      PdfName name
      )
    {
      if(name == null)
        return LayerConfiguration.BaseStateEnum.On;

      LayerConfiguration.BaseStateEnum? baseState = codes.GetKey(name);
      if(!baseState.HasValue)
        throw new NotSupportedException("Base state unknown: " + name);

      return baseState.Value;
    }

    public static LayerConfiguration.BaseStateEnum Get(
      bool? enabled
      )
    {return enabled.HasValue ? (enabled.Value ? LayerConfiguration.BaseStateEnum.On : LayerConfiguration.BaseStateEnum.Off) : LayerConfiguration.BaseStateEnum.Unchanged;}

    public static PdfName GetName(
      this LayerConfiguration.BaseStateEnum baseState
      )
    {return codes[baseState];}

    public static bool? IsEnabled(
      this LayerConfiguration.BaseStateEnum baseState
      )
    {
      switch(baseState)
      {
        case LayerConfiguration.BaseStateEnum.On:
          return true;
        case LayerConfiguration.BaseStateEnum.Off:
          return false;
        case LayerConfiguration.BaseStateEnum.Unchanged:
          return null;
        default:
          throw new NotSupportedException();
      }
    }
  }
}