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
using org.pdfclown.util.math;

using System;
using System.Collections.Generic;

namespace org.pdfclown.documents.contents.layers
{
  /**
    <summary>Optional content group [PDF:1.7:4.10.1].</summary>
  */
  [PDF(VersionEnum.PDF15)]
  public sealed class Layer
    : LayerEntity,
      ILayerNode
  {
    #region types
    /**
      <summary>Sublayers location within a configuration structure.</summary>
    */
    private class LayersLocation
    {
      /**
        <summary>Sublayers ordinal position within the parent sublayers.</summary>
      */
      public int Index;
      /**
        <summary>Parent layer object.</summary>
      */
      public PdfDirectObject ParentLayerObject;
      /**
        <summary>Parent sublayers object.</summary>
      */
      public PdfArray ParentLayersObject;
      /**
        <summary>Upper levels.</summary>
      */
      public Stack<Object[]> Levels;

      public LayersLocation(
        PdfDirectObject parentLayerObject,
        PdfArray parentLayersObject,
        int index,
        Stack<Object[]> levels
        )
      {
        ParentLayerObject = parentLayerObject;
        ParentLayersObject = parentLayersObject;
        Index = index;
        Levels = levels;
      }
    }

    /**
      <summary>Layer state.</summary>
    */
    internal enum StateEnum
    {
      /**
        <summary>Active.</summary>
      */
      On,
      /**
        <summary>Inactive.</summary>
      */
      Off
    }
    #endregion

    #region static
    #region fields
    public static readonly PdfName TypeName = PdfName.OCG;

    private static readonly PdfName MembershipName = new PdfName("D-OCMD");
    #endregion

    #region interface
    #region public
    public static new Layer Wrap(
      PdfDirectObject baseObject
      )
    {return baseObject != null ? new Layer(baseObject) : null;}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public Layer(
      Document context,
      string title
      ) : base(context, PdfName.OCG)
    {
      Title = title;

      // Add this layer to the global collection!
      /*
        NOTE: Every layer MUST be included in the global collection [PDF:1.7:4.10.3].
      */
      LayerDefinition definition = context.Layer;
      if(definition == null)
      {context.Layer = definition = new LayerDefinition(context);}
      definition.AllLayersObject.Add(BaseObject);
    }

    private Layer(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the name of the type of content controlled by the group.</summary>
    */
    public string ContentType
    {
      get
      {return (string)PdfSimpleObject<object>.GetValue(GetUsageEntry(PdfName.CreatorInfo)[PdfName.Subtype]);}
      set
      {GetUsageEntry(PdfName.CreatorInfo)[PdfName.Subtype] = PdfName.Get(value);}
    }

    /**
      <summary>Gets/Sets the name of the application that created this layer.</summary>
    */
    public string Creator
    {
      get
      {return (string)PdfSimpleObject<object>.GetValue(GetUsageEntry(PdfName.CreatorInfo)[PdfName.Creator]);}
      set
      {GetUsageEntry(PdfName.CreatorInfo)[PdfName.Creator] = PdfTextString.Get(value);}
    }

    /**
      <summary>Gets/Sets whether this layer is visible when the document is saved by a viewer
      application to a format that does not support optional content.</summary>
    */
    public bool Exportable
    {
      get
      {return StateEnumExtension.Get((PdfName)GetUsageEntry(PdfName.Export)[PdfName.ExportState]).IsEnabled();}
      set
      {
        GetUsageEntry(PdfName.Export)[PdfName.ExportState] = StateEnumExtension.Get(value).GetName();
        DefaultConfiguration.SetUsageApplication(PdfName.Export, PdfName.Export, this, true);
      }
    }

    /**
      <summary>Gets/Sets the language (and possibly locale) of the content controlled by this layer.
      </summary>
    */
    public string Language
    {
      get
      {return (string)PdfSimpleObject<object>.GetValue(GetUsageEntry(PdfName.Language)[PdfName.Lang]);}
      set
      {GetUsageEntry(PdfName.Language)[PdfName.Lang] = PdfTextString.Get(value);}
    }

    /**
      <summary>Gets/Sets the sublayers.</summary>
    */
    public Layers Layers
    {
      get
      {
        LayersLocation location = FindLayersLocation();
        return Layers.Wrap(location.ParentLayersObject.Get<PdfArray>(location.Index));
      }
      set
      {
        LayersLocation location = FindLayersLocation();
        if(location.Index == location.ParentLayersObject.Count)
        {location.ParentLayersObject.Add(value.BaseObject);} // Appends new sublayers.
        else if(location.ParentLayersObject.Resolve(location.Index) is PdfArray)
        {location.ParentLayersObject[location.Index] = value.BaseObject;} // Substitutes old sublayers.
        else
        {location.ParentLayersObject.Insert(location.Index, value.BaseObject);} // Inserts new sublayers.
      }
    }

    /**
      <summary>Gets/Sets whether the default visibility of this layer cannot be changed through the
      user interface of a viewer application.</summary>
    */
    public bool Locked
    {
      get
      {return DefaultConfiguration.BaseDataObject.Resolve<PdfArray>(PdfName.Locked).Contains(BaseObject);}
      set
      {
        PdfArray lockedArrayObject = DefaultConfiguration.BaseDataObject.Resolve<PdfArray>(PdfName.Locked);
        if(!lockedArrayObject.Contains(BaseObject))
        {lockedArrayObject.Add(BaseObject);}
      }
    }

    public override LayerMembership Membership
    {
      get
      {
        LayerMembership membership;
        PdfDirectObject membershipObject = BaseDataObject[MembershipName];
        if(membershipObject == null)
        {
          membership = new LayerMembership(Document);
          membership.VisibilityPolicy = VisibilityPolicyEnum.AllOn; // NOTE: Forces visibility to depend on all the ascendant layers.
          BaseDataObject[MembershipName] = membershipObject = membership.BaseObject;
        }
        else
        {membership = LayerMembership.Wrap(membershipObject);}
        if(membership.VisibilityLayers.Count == 0)
        {
          membership.VisibilityLayers.Add(this);
          LayersLocation location = FindLayersLocation();
          if(location.ParentLayerObject != null)
          {membership.VisibilityLayers.Add(new Layer(location.ParentLayerObject));}
          foreach(Object[] level in location.Levels)
          {
            PdfDirectObject layerObject = (PdfDirectObject)level[2];
            if(layerObject != null)
            {membership.VisibilityLayers.Add(new Layer(layerObject));}
          }
        }
        return membership;
      }
    }

    /**
      <summary>Gets/Sets whether this layer is visible when the document is printed from a viewer
      application.</summary>
    */
    public bool Printable
    {
      get
      {return StateEnumExtension.Get((PdfName)GetUsageEntry(PdfName.Print)[PdfName.PrintState]).IsEnabled();}
      set
      {
        GetUsageEntry(PdfName.Print)[PdfName.PrintState] = StateEnumExtension.Get(value).GetName();
        DefaultConfiguration.SetUsageApplication(PdfName.Print, PdfName.Print, this, true);
      }
    }

    public override string ToString(
      )
    {return Title;}

    /**
      <summary>Gets/Sets whether this layer is visible when the document is opened in a viewer
      application.</summary>
    */
    public bool Viewable
    {
      get
      {return StateEnumExtension.Get((PdfName)GetUsageEntry(PdfName.View)[PdfName.ViewState]).IsEnabled();}
      set
      {
        GetUsageEntry(PdfName.View)[PdfName.ViewState] = StateEnumExtension.Get(value).GetName();
        DefaultConfiguration.SetUsageApplication(PdfName.View, PdfName.View, this, true);
      }
    }

    public override IList<Layer> VisibilityLayers
    {
      get
      {return Membership.VisibilityLayers;}
    }

    public override VisibilityPolicyEnum VisibilityPolicy
    {
      get
      {return Membership.VisibilityPolicy;}
      set
      {
        if(!value.Equals(Membership.VisibilityPolicy))
          throw new NotSupportedException("Single layers cannot manage custom state policies; use LayerMembership instead.");
      }
    }

    /**
      <summary>Gets/Sets whether this layer is initially visible.</summary>
    */
    public bool Visible
    {
      get
      {return DefaultConfiguration.IsVisible(this);}
      set
      {DefaultConfiguration.SetVisible(this, value);}
    }

    /**
      <summary>Gets/Sets the range of magnifications at which the content in this layer is best
      viewed.</summary>
    */
    public Interval<double> ZoomRange
    {
      get
      {
        PdfDictionary zoomDictionary = GetUsageEntry(PdfName.Zoom);
        IPdfNumber minObject = (IPdfNumber)zoomDictionary.Resolve(PdfName.min);
        IPdfNumber maxObject = (IPdfNumber)zoomDictionary.Resolve(PdfName.max);
        return new Interval<double>(
          minObject != null ? minObject.RawValue : 0,
          maxObject != null ? maxObject.RawValue : double.PositiveInfinity
          );
      }
      set
      {
        if(value != null)
        {
          PdfDictionary zoomDictionary = GetUsageEntry(PdfName.Zoom);
          zoomDictionary[PdfName.min] = (value.Low != 0 ? PdfReal.Get(value.Low) : null);
          zoomDictionary[PdfName.max] = (value.High != double.PositiveInfinity ? PdfReal.Get(value.High) : null);
        }
        else
        {Usage.Remove(PdfName.Zoom);}
        DefaultConfiguration.SetUsageApplication(PdfName.View, PdfName.Zoom, this, value != null);
      }
    }

    #region ILayerNode
    public string Title
    {
      get
      {return (string)((PdfTextString)BaseDataObject[PdfName.Name]).Value;}
      set
      {BaseDataObject[PdfName.Name] = new PdfTextString(value);}
    }
    #endregion
    #endregion

    #region private
    private LayerConfiguration DefaultConfiguration
    {
      get
      {return Document.Layer.DefaultConfiguration;}
    }

    /**
      <summary>Finds the location of the sublayers object in the default configuration; in case no
      sublayers object is associated to this object, its virtual position is indicated.</summary>
    */
    private LayersLocation FindLayersLocation(
      )
    {
      LayersLocation location = FindLayersLocation(DefaultConfiguration);
      if(location == null)
      {
        /*
          NOTE: In case the layer is outside the default structure, it's appended to the root
          collection.
        */
        /*
          TODO: anytime a layer is inserted into a collection, the layer tree must be checked to
          avoid duplicate: in case the layer is already in the tree, it must be moved to the new
          position along with its sublayers.
        */
        Layers rootLayers = DefaultConfiguration.Layers;
        rootLayers.Add(this);
        location = new LayersLocation(null, rootLayers.BaseDataObject, rootLayers.Count, new Stack<Object[]>());
      }
      return location;
    }

    /**
      <summary>Finds the location of the sublayers object in the specified configuration; in case no
      sublayers object is associated to this object, its virtual position is indicated.</summary>
      <param name="configuration">Configuration context.</param>
      <returns><code>null</code>, if this layer is outside the specified configuration.</returns>
    */
    private LayersLocation FindLayersLocation(
      LayerConfiguration configuration
      )
    {
      /*
        NOTE: As layers are only weakly tied to configurations, their sublayers have to be sought
        through the configuration structure tree.
      */
      PdfDirectObject levelLayerObject = null;
      PdfArray levelObject = configuration.Layers.BaseDataObject;
      IEnumerator<PdfDirectObject> levelIterator = levelObject.GetEnumerator();
      Stack<object[]> levelIterators = new Stack<object[]>();
      PdfDirectObject thisObject = BaseObject;
      PdfDirectObject currentLayerObject = null;
      while(true)
      {
        if(!levelIterator.MoveNext())
        {
          if(levelIterators.Count == 0)
            break;

          object[] levelItems = levelIterators.Pop();
          levelObject = (PdfArray)levelItems[0];
          levelIterator = (IEnumerator<PdfDirectObject>)levelItems[1];
          levelLayerObject = (PdfDirectObject)levelItems[2];
          currentLayerObject = null;
        }
        else
        {
          PdfDirectObject nodeObject = levelIterator.Current;
          PdfDataObject nodeDataObject = PdfObject.Resolve(nodeObject);
          if(nodeDataObject is PdfDictionary)
          {
            if(nodeObject.Equals(thisObject))
              /*
                NOTE: Sublayers are expressed as an array immediately following the parent layer node.
              */
              return new LayersLocation(levelLayerObject, levelObject, levelObject.IndexOf(thisObject) + 1, levelIterators);
  
            currentLayerObject = nodeObject;
          }
          else if(nodeDataObject is PdfArray)
          {
            levelIterators.Push(new object[]{levelObject, levelIterator, levelLayerObject});
            levelObject = (PdfArray)nodeDataObject;
            levelIterator = levelObject.GetEnumerator();
            levelLayerObject = currentLayerObject;
            currentLayerObject = null;
          }
        }
      }
      return null;
    }

    private PdfDictionary GetUsageEntry(
      PdfName key
      )
    {return Usage.Resolve<PdfDictionary>(key);}

    private PdfDictionary Usage
    {
      get
      {return BaseDataObject.Resolve<PdfDictionary>(PdfName.Usage);}
    }
    #endregion
    #endregion
    #endregion
  }

  internal static class StateEnumExtension
  {
    private static readonly BiDictionary<Layer.StateEnum,PdfName> codes;

    static StateEnumExtension()
    {
      codes = new BiDictionary<Layer.StateEnum,PdfName>();
      codes[Layer.StateEnum.On] = PdfName.ON;
      codes[Layer.StateEnum.Off] = PdfName.OFF;
    }

    public static Layer.StateEnum Get(
      PdfName name
      )
    {
      if(name == null)
        return Layer.StateEnum.On;

      Layer.StateEnum? state = codes.GetKey(name);
      if(!state.HasValue)
        throw new NotSupportedException("State unknown: " + name);

      return state.Value;
    }

    public static Layer.StateEnum Get(
      bool? enabled
      )
    {return !enabled.HasValue || enabled.Value ? Layer.StateEnum.On : Layer.StateEnum.Off;}

    public static PdfName GetName(
      this Layer.StateEnum state
      )
    {return codes[state];}

    public static bool IsEnabled(
      this Layer.StateEnum state
      )
    {return state == Layer.StateEnum.On;}
  }
}