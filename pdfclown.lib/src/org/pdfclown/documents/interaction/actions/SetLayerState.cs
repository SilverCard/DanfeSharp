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
using org.pdfclown.documents.contents.layers;
using org.pdfclown.objects;
using org.pdfclown.util;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace org.pdfclown.documents.interaction.actions
{
  /**
    <summary>'Set the state of one or more optional content groups' action [PDF:1.6:8.5.3].</summary>
  */
  [PDF(VersionEnum.PDF15)]
  public sealed class SetLayerState
    : Action
  {
    #region types
    public enum StateModeEnum
    {
      On,
      Off,
      Toggle
    }

    public class LayerState
    {
      internal class LayersImpl
        : Collection<Layer>
      {
        internal LayerState parentState;

        protected override void ClearItems(
          )
        {
          // Low-level definition.
          LayerStates baseStates = BaseStates;
          if(baseStates != null)
          {
            int itemIndex = baseStates.GetBaseIndex(parentState)
              + 1; // Name object offset.
            for(int count = Count; count > 0; count--)
            {baseStates.BaseDataObject.RemoveAt(itemIndex);}
          }
          // High-level definition.
          base.ClearItems();
        }

        protected override void InsertItem(
          int index,
          Layer item
          )
        {
          // High-level definition.
          base.InsertItem(index, item);
          // Low-level definition.
          LayerStates baseStates = BaseStates;
          if(baseStates != null)
          {
            int baseIndex = baseStates.GetBaseIndex(parentState);
            int itemIndex = baseIndex
              + 1 // Name object offset.
              + index; // Layer object offset.
            baseStates.BaseDataObject[itemIndex] = item.BaseObject;
          }
        }

        protected override void RemoveItem(
          int index
          )
        {
          // High-level definition.
          base.RemoveItem(index);
          // Low-level definition.
          LayerStates baseStates = BaseStates;
          if(baseStates != null)
          {
            int baseIndex = baseStates.GetBaseIndex(parentState);
            int itemIndex = baseIndex
              + 1 // Name object offset.
              + index; // Layer object offset.
            baseStates.BaseDataObject.RemoveAt(itemIndex);
          }
        }

        protected override void SetItem(
          int index,
          Layer item
          )
        {
          RemoveItem(index);
          InsertItem(index, item);
        }

        private LayerStates BaseStates
        {
          get
          {return parentState != null ? parentState.baseStates : null;}
        }
      }

      private readonly LayersImpl layers;
      private StateModeEnum mode;

      private LayerStates baseStates;

      public LayerState(
        StateModeEnum mode
        ) : this(mode, new LayersImpl(), null)
      {}

      internal LayerState(
        StateModeEnum mode,
        LayersImpl layers,
        LayerStates baseStates
        )
      {
        this.mode = mode;
        this.layers = layers;
        this.layers.parentState = this;
        Attach(baseStates);
      }

      public override bool Equals(
        object obj
        )
      {
        if(!(obj is LayerState))
          return false;

        LayerState state = (LayerState)obj;
        if(!state.Mode.Equals(Mode)
          || state.Layers.Count != Layers.Count)
          return false;

        IEnumerator<Layer> layerIterator = Layers.GetEnumerator();
        IEnumerator<Layer> stateLayerIterator = state.Layers.GetEnumerator();
        while(layerIterator.MoveNext())
        {
          stateLayerIterator.MoveNext();
          if(!layerIterator.Current.Equals(stateLayerIterator.Current))
            return false;
        }
        return true;
      }

      public IList<Layer> Layers
      {
        get
        {return layers;}
      }

      public StateModeEnum Mode
      {
        get
        {return mode;}
        set
        {
          mode = value;
  
          if(baseStates != null)
          {
            int baseIndex = baseStates.GetBaseIndex(this);
            baseStates.BaseDataObject[baseIndex] = value.GetName();
          }
        }
      }

      public override int GetHashCode(
        )
      {return mode.GetHashCode() ^ layers.Count;}

      internal void Attach(
        LayerStates baseStates
        )
      {this.baseStates = baseStates;}

      internal void Detach(
        )
      {baseStates = null;}
    }

    public class LayerStates
      : PdfObjectWrapper<PdfArray>,
        IList<LayerState>
    {
      private IList<LayerState> items;

      public LayerStates(
        ) : base(new PdfArray())
      {}

      internal LayerStates(
        PdfDirectObject baseObject
        ) : base(baseObject)
      {Initialize();}

      #region IList<LayerState>
      public int IndexOf(
        LayerState item
        )
      {return items.IndexOf(item);}

      public void Insert(
        int index,
        LayerState item
        )
      {
        int baseIndex = GetBaseIndex(index);
        if(baseIndex == -1)
        {Add(item);}
        else
        {
          PdfArray baseDataObject = BaseDataObject;
          // Low-level definition.
          baseDataObject.Insert(baseIndex++, item.Mode.GetName());
          foreach(Layer layer in item.Layers)
          {baseDataObject.Insert(baseIndex++, layer.BaseObject);}
          // High-level definition.
          items.Insert(index, item);
          item.Attach(this);
        }
      }

      public void RemoveAt(
        int index
        )
      {
        LayerState layerState;
        // Low-level definition.
        {
          int baseIndex = GetBaseIndex(index);
          if(baseIndex == -1)
            throw new IndexOutOfRangeException();

          PdfArray baseDataObject = BaseDataObject;
          bool done = false;
          for(int baseCount = baseDataObject.Count; baseIndex < baseCount;)
          {
            if(baseDataObject[baseIndex] is PdfName)
            {
              if(done)
                break;

              done = true;
            }
            baseDataObject.RemoveAt(baseIndex);
          }
        }
        // High-level definition.
        {
          layerState = items[index];
          items.RemoveAt(index);
          layerState.Detach();
        }
      }

      public LayerState this[
        int index
        ]
      {
        get
        {return items[index];}
        set
        {
          RemoveAt(index);
          Insert(index, value);
        }
      }

      #region ICollection<LayerState>
      public void Add(
        LayerState item
        )
      {
        PdfArray baseDataObject = BaseDataObject;
        // Low-level definition.
        baseDataObject.Add(item.Mode.GetName());
        foreach(Layer layer in item.Layers)
        {baseDataObject.Add(layer.BaseObject);}
        // High-level definition.
        items.Add(item);
        item.Attach(this);
      }

      public void Clear(
        )
      {
        // Low-level definition.
        BaseDataObject.Clear();
        // High-level definition.
        foreach(LayerState item in items)
        {item.Detach();}
        items.Clear();
      }

      public bool Contains(
        LayerState item
        )
      {return items.Contains(item);}

      public void CopyTo(
        LayerState[] items,
        int index
        )
      {throw new NotImplementedException();}

      public int Count
      {
        get
        {return items.Count;}
      }

      public bool IsReadOnly
      {
        get
        {return false;}
      }

      public bool Remove(
        LayerState item
        )
      {
        int index = IndexOf(item);
        if(index == -1)
          return false;

        RemoveAt(index);
        return true;
      }

      #region IEnumerable<LayerState>
      public IEnumerator<LayerState> GetEnumerator(
        )
      {return items.GetEnumerator();}

      #region IEnumerable
      IEnumerator IEnumerable.GetEnumerator(
        )
      {return this.GetEnumerator();}
      #endregion
      #endregion
      #endregion
      #endregion

      /**
        <summary>Gets the position of the initial base item corresponding to the specified layer
        state index.</summary>
        <param name="index">Layer state index.</param>
        <returns>-1, in case <code>index</code> is outside the available range.</returns>
      */
      internal int GetBaseIndex(
        int index
        )
      {
        int baseIndex = -1;
        {
          PdfArray baseDataObject = BaseDataObject;
          int layerStateIndex = -1;
          for(
            int baseItemIndex = 0,
              baseItemCount = baseDataObject.Count;
            baseItemIndex < baseItemCount;
            baseItemIndex++
            )
          {
            if(baseDataObject[baseItemIndex] is PdfName)
            {
              layerStateIndex++;
              if(layerStateIndex == index)
              {
                baseIndex = baseItemIndex;
                break;
              }
            }
          }
        }
        return baseIndex;
      }

      /**
        <summary>Gets the position of the initial base item corresponding to the specified layer
        state.</summary>
        <param name="item">Layer state.</param>
        <returns>-1, in case <code>item</code> has no match.</returns>
      */
      internal int GetBaseIndex(
        LayerState item
        )
      {
        int baseIndex = -1;
        {
          PdfArray baseDataObject = BaseDataObject;
          for(
            int baseItemIndex = 0,
              baseItemCount = baseDataObject.Count;
            baseItemIndex < baseItemCount;
            baseItemIndex++
            )
          {
            PdfDirectObject baseItem = baseDataObject[baseItemIndex];
            if(baseItem is PdfName
              && baseItem.Equals(item.Mode.GetName()))
            {
              foreach(Layer layer in item.Layers)
              {
                if(++baseItemIndex >= baseItemCount)
                  break;

                baseItem = baseDataObject[baseItemIndex];
                if(baseItem is PdfName
                  || !baseItem.Equals(layer.BaseObject))
                  break;
              }
            }
          }
        }
        return baseIndex;
      }

      private void Initialize(
        )
      {
        items = new List<LayerState>();
        PdfArray baseDataObject = BaseDataObject;
        StateModeEnum? mode = null;
        LayerState.LayersImpl layers = null;
        for(
          int baseIndex = 0,
            baseCount = baseDataObject.Count;
          baseIndex < baseCount;
          baseIndex++
          )
        {
          PdfDirectObject baseObject = baseDataObject[baseIndex];
          if(baseObject is PdfName)
          {
            if(mode.HasValue)
            {items.Add(new LayerState(mode.Value, layers, this));}
            mode = StateModeEnumExtension.Get((PdfName)baseObject);
            layers = new LayerState.LayersImpl();
          }
          else
          {layers.Add(Layer.Wrap(baseObject));}
        }
        if(mode.HasValue)
        {items.Add(new LayerState(mode.Value, layers, this));}
      }
    }
    #endregion

    #region dynamic
    #region constructors
    /**
      <summary>Creates a new action within the given document context.</summary>
    */
    public SetLayerState(
      Document context
      ) : base(context, PdfName.SetOCGState)
    {States = new LayerStates();}

    internal SetLayerState(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    public LayerStates States
    {
      get
      {return new LayerStates(BaseDataObject[PdfName.State]);}
      set
      {BaseDataObject[PdfName.State] = value.BaseObject;}
    }
    #endregion
    #endregion
    #endregion
  }

  internal static class StateModeEnumExtension
  {
    private static readonly BiDictionary<SetLayerState.StateModeEnum,PdfName> codes;

    static StateModeEnumExtension()
    {
      codes = new BiDictionary<SetLayerState.StateModeEnum,PdfName>();
      codes[SetLayerState.StateModeEnum.On] = PdfName.ON;
      codes[SetLayerState.StateModeEnum.Off] = PdfName.OFF;
      codes[SetLayerState.StateModeEnum.Toggle] = PdfName.Toggle;
    }

    public static SetLayerState.StateModeEnum Get(
      PdfName name
      )
    {
      if(name == null)
        throw new ArgumentNullException("name");

      SetLayerState.StateModeEnum? stateMode = codes.GetKey(name);
      if(!stateMode.HasValue)
        throw new NotSupportedException("State mode unknown: " + name);

      return stateMode.Value;
    }

    public static PdfName GetName(
      this SetLayerState.StateModeEnum stateMode
      )
    {return codes[stateMode];}
  }
}