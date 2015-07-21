/*
  Copyright 2011-2013 Stefano Chizzolini. http://www.pdfclown.org

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

using org.pdfclown.objects;

using System;

namespace org.pdfclown.documents.contents.layers
{
  /**
    <summary>Optional content group collection.</summary>
  */
  [PDF(VersionEnum.PDF15)]
  public sealed class Layers
    : Array<ILayerNode>,
      ILayerNode
  {
    #region types
    private delegate int EvaluateNode(
      int currentNodeIndex,
      int currentBaseIndex
      );

    private class ItemWrapper
      : IWrapper<ILayerNode>
    {
      public ILayerNode Wrap(
        PdfDirectObject baseObject
        )
      {return LayerNode.Wrap(baseObject);}
    }
    #endregion

    #region static
    #region fields
    private static readonly ItemWrapper Wrapper = new ItemWrapper();
    #endregion

    #region interface
    #region public
    public static Layers Wrap(
      PdfDirectObject baseObject
      )
    {return baseObject != null ? new Layers(baseObject) : null;}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public Layers(
      Document context
      ) : this(context, null)
    {}

    public Layers(
      Document context,
      string title
      ) : base(context, Wrapper)
    {Title = title;}

    private Layers(
      PdfDirectObject baseObject
      ) : base(Wrapper, baseObject)
    {}
    #endregion

    #region interface
    #region public
    public override int Count
    {
      get
      {
        return Evaluate(delegate(
          int currentNodeIndex,
          int currentBaseIndex
          )
        {
          if(currentBaseIndex == -1)
            return currentNodeIndex;
          else
            return -1;
        }) + 1;
      }
    }

    public override int IndexOf(
      ILayerNode item
      )
    {return GetNodeIndex(base.IndexOf(item));}

    public override void Insert(
      int index,
      ILayerNode item
      )
    {base.Insert(GetBaseIndex(index), item);}

    public override void RemoveAt(
      int index
      )
    {
      int baseIndex = GetBaseIndex(index);
      ILayerNode removedItem = base[baseIndex];
      base.RemoveAt(baseIndex);
      if(removedItem is Layer
        && baseIndex < base.Count)
      {
        /*
          NOTE: Sublayers MUST be removed as well.
        */
        if(BaseDataObject.Resolve(baseIndex) is PdfArray)
        {BaseDataObject.RemoveAt(baseIndex);}
      }
    }

    public override ILayerNode this[
      int index
      ]
    {
      get
      {return base[GetBaseIndex(index)];}
      set
      {base[GetBaseIndex(index)] = value;}
    }

    public override string ToString(
      )
    {return Title;}

    #region ILayerNode
    Layers ILayerNode.Layers
    {
      get
      {return this;}
    }

    public string Title
    {
      get
      {
        if(BaseDataObject.Count == 0)
          return null;

        PdfDirectObject firstObject = BaseDataObject[0];
        return firstObject is PdfTextString ? (string)((PdfTextString)firstObject).Value : null;
      }
      set
      {
        PdfTextString titleObject = PdfTextString.Get(value);
        PdfArray baseDataObject = BaseDataObject;
        PdfDirectObject firstObject = (baseDataObject.Count == 0 ? null : baseDataObject[0]);
        if(firstObject is PdfTextString)
        {
          if(titleObject != null)
          {baseDataObject[0] = titleObject;}
          else
          {baseDataObject.RemoveAt(0);}
        }
        else if(titleObject != null)
        {baseDataObject.Insert(0, titleObject);}
      }
    }
    #endregion
    #endregion

    #region private
    /**
      <summary>Gets the positional information resulting from the collection evaluation.</summary>
      <param name="evaluator">Expression used to evaluate the positional matching.</param>
    */
    private int Evaluate(
      EvaluateNode evaluateNode
      )
    {
      /*
        NOTE: Layer hierarchies are represented through a somewhat flatten structure which needs
        to be evaluated in order to match nodes in their actual place.
      */
      PdfArray baseDataObject = BaseDataObject;
      int nodeIndex = -1;
      bool groupAllowed = true;
      for(
        int baseIndex = 0,
          baseLength = base.Count;
        baseIndex < baseLength;
        baseIndex++
        )
      {
        PdfDataObject itemDataObject = baseDataObject.Resolve(baseIndex);
        if(itemDataObject is PdfDictionary
          || (itemDataObject is PdfArray && groupAllowed))
        {
          nodeIndex++;
          int evaluation = evaluateNode(nodeIndex, baseIndex);
          if(evaluation > -1)
            return evaluation;
        }
        groupAllowed = !(itemDataObject is PdfDictionary);
      }
      return evaluateNode(nodeIndex, -1);
    }

    private int GetBaseIndex(
      int nodeIndex
      )
    {
      return Evaluate(delegate(
        int currentNodeIndex,
        int currentBaseIndex
        )
      {
        if(currentNodeIndex == nodeIndex)
          return currentBaseIndex;
        else
          return -1;
      });
    }

    private int GetNodeIndex(
      int baseIndex
      )
    {
      return Evaluate(delegate(
        int currentNodeIndex,
        int currentBaseIndex
        )
      {
        if(currentBaseIndex == baseIndex)
          return currentNodeIndex;
        else
          return -1;
      });
    }
    #endregion
    #endregion
    #endregion
  }
}