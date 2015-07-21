/*
  Copyright 2007-2012 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.files;

using System;
using System.Collections;
using System.Collections.Generic;

namespace org.pdfclown.objects
{
  /**
    <summary>Abstract tree [PDF:1.7:3.8.5].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public abstract class Tree<TKey,TValue>
    : PdfObjectWrapper<PdfDictionary>,
      IDictionary<TKey,TValue>
    where TKey : PdfDirectObject, IPdfSimpleObject
    where TValue : PdfObjectWrapper
  {
    /*
      NOTE: This implementation is an adaptation of the B-tree algorithm described in "Introduction
      to Algorithms" [1], 2nd ed (Cormen, Leiserson, Rivest, Stein) published by MIT Press/McGraw-Hill.
      PDF trees represent a special subset of B-trees whereas actual keys are concentrated in leaf
      nodes and proxied by boundary limits across their paths. This simplifies some handling but
      requires keeping node limits updated whenever a change occurs in the leaf nodes composition.

      [1] http://en.wikipedia.org/wiki/Introduction_to_Algorithms
    */
    #region types
    /**
      Node children.
    */
    private sealed class Children
    {
      public sealed class InfoImpl
      {
        private static readonly InfoImpl KidsInfo = new InfoImpl(1, TreeLowOrder);
        private static readonly InfoImpl PairsInfo = new InfoImpl(2, TreeLowOrder); // NOTE: Paired children are combinations of 2 contiguous items.

        public static InfoImpl Get(
          PdfName typeName
          )
        {return typeName.Equals(PdfName.Kids) ? KidsInfo : PairsInfo;}

        /** Number of (contiguous) children defining an item. */
        public int ItemCount;
        /** Maximum number of children. */
        public int MaxCount;
        /** Minimum number of children. */
        public int MinCount;

        public InfoImpl(
          int itemCount,
          int lowOrder
          )
        {
          ItemCount = itemCount;
          MinCount = itemCount * lowOrder;
          MaxCount = MinCount * 2;
        }
      }

      /**
        <summary>Gets the given node's children.</summary>
        <param name="node">Parent node.</param>
        <param name="pairs">Pairs key.</param>
      */
      public static Children Get(
        PdfDictionary node,
        PdfName pairsKey
        )
      {
        PdfName childrenTypeName;
        if(node.ContainsKey(PdfName.Kids))
        {childrenTypeName = PdfName.Kids;}
        else if(node.ContainsKey(pairsKey))
        {childrenTypeName = pairsKey;}
        else
          throw new Exception("Malformed tree node.");

        PdfArray children = (PdfArray)node.Resolve(childrenTypeName);
        return new Children(node, children, childrenTypeName);
      }

      /** Children's collection */
      public readonly PdfArray Items;
      /** Node's children info. */
      public readonly InfoImpl Info;
      /** Parent node. */
      public readonly PdfDictionary Parent;
      /** Node's children type. */
      public readonly PdfName TypeName;

      private Children(
        PdfDictionary parent,
        PdfArray items,
        PdfName typeName
        )
      {
        Parent = parent;
        Items = items;
        TypeName = typeName;
        Info = InfoImpl.Get(typeName);
      }

      /**
        <summary>Gets whether the collection size has reached its maximum.</summary>
      */
      public bool IsFull(
        )
      {return Items.Count >= Info.MaxCount;}

      /**
        <summary>Gets whether this collection represents a leaf node.</summary>
      */
      public bool IsLeaf(
        )
      {return !TypeName.Equals(PdfName.Kids);}

      /**
        <summary>Gets whether the collection size is more than its maximum.</summary>
      */
      public bool IsOversized(
        )
      {return Items.Count > Info.MaxCount;}

      /**
        <summary>Gets whether the collection size is less than its minimum.</summary>
      */
      public bool IsUndersized(
        )
      {return Items.Count < Info.MinCount;}

      /**
        <summary>Gets whether the collection size is within the order limits.</summary>
      */
      public bool IsValid(
        )
      {return !(IsUndersized() || IsOversized());}
    }

    private class Enumerator
      : IEnumerator<KeyValuePair<TKey,TValue>>
    {
      #region dynamic
      #region fields
      /**
        <summary>Current named object.</summary>
      */
      private KeyValuePair<TKey,TValue>? current;

      /**
        <summary>Current level index.</summary>
      */
      private int levelIndex = 0;
      /**
        <summary>Stacked levels.</summary>
      */
      private Stack<object[]> levels = new Stack<object[]>();

      /**
        <summary>Current child tree nodes.</summary>
      */
      private PdfArray kids;
      /**
        <summary>Current names.</summary>
      */
      private PdfArray names;
      /**
        <summary>Current container.</summary>
      */
      private PdfIndirectObject container;

      /**
        <summary>Name tree.</summary>
      */
      private Tree<TKey,TValue> tree;
      #endregion

      #region constructors
      internal Enumerator(
        Tree<TKey,TValue> tree
        )
      {
        this.tree = tree;

        container = tree.Container;
        PdfDictionary rootNode = tree.BaseDataObject;
        PdfDirectObject kidsObject =  rootNode[PdfName.Kids];
        if(kidsObject == null) // Leaf node.
        {
          PdfDirectObject namesObject = rootNode[PdfName.Names];
          if(namesObject is PdfReference)
          {container = ((PdfReference)namesObject).IndirectObject;}
          names = (PdfArray)namesObject.Resolve();
        }
        else // Intermediate node.
        {
          if(kidsObject is PdfReference)
          {container = ((PdfReference)kidsObject).IndirectObject;}
          kids = (PdfArray)kidsObject.Resolve();
        }
      }
      #endregion

      #region interface
      #region public
      #region IEnumerator<KeyValuePair<TKey,TValue>>
      KeyValuePair<TKey,TValue> IEnumerator<KeyValuePair<TKey,TValue>>.Current
      {
        get
        {return current.Value;}
      }

      #region IEnumerator
      public object Current
      {
        get
        {return ((IEnumerator<KeyValuePair<TKey,TValue>>)this).Current;}
      }

      public bool MoveNext(
        )
      {return (current = GetNext()) != null;}

      public void Reset(
        )
      {throw new NotSupportedException();}
      #endregion

      #region IDisposable
      public void Dispose(
        )
      {}
      #endregion
      #endregion
      #endregion

      #region private
      private KeyValuePair<TKey,TValue>? GetNext(
        )
      {
        /*
          NOTE: Algorithm:
          1. [Vertical, down] We have to go downward the name tree till we reach
          a names collection (leaf node).
          2. [Horizontal] Then we iterate across the names collection.
          3. [Vertical, up] When leaf-nodes scan is complete, we go upward solving
          parent nodes, repeating step 1.
        */
        while(true)
        {
          if(names == null)
          {
            if(kids == null
              || kids.Count == levelIndex) // Kids subtree complete.
            {
              if(levels.Count == 0)
                return null;

              // 3. Go upward one level.
              // Restore current level!
              object[] level = levels.Pop();
              container = (PdfIndirectObject)level[0];
              kids = (PdfArray)level[1];
              levelIndex = ((int)level[2]) + 1; // Next node (partially scanned level).
            }
            else // Kids subtree incomplete.
            {
              // 1. Go downward one level.
              // Save current level!
              levels.Push(new object[]{container,kids,levelIndex});

              // Move downward!
              PdfReference kidReference = (PdfReference)kids[levelIndex];
              container = kidReference.IndirectObject;
              PdfDictionary kid = (PdfDictionary)kidReference.DataObject;
              PdfDirectObject kidsObject = kid[PdfName.Kids];
              if(kidsObject == null) // Leaf node.
              {
                PdfDirectObject namesObject = kid[PdfName.Names];
                if(namesObject is PdfReference)
                {container = ((PdfReference)namesObject).IndirectObject;}
                names = (PdfArray)namesObject.Resolve();
                kids = null;
              }
              else // Intermediate node.
              {
                if(kidsObject is PdfReference)
                {container = ((PdfReference)kidsObject).IndirectObject;}
                kids = (PdfArray)kidsObject.Resolve();
              }
              levelIndex = 0; // First node (new level).
            }
          }
          else
          {
            if(names.Count == levelIndex) // Names complete.
            {names = null;}
            else // Names incomplete.
            {
              // 2. Object found.
              TKey key = (TKey)names[levelIndex];
              TValue value = tree.WrapValue(names[levelIndex + 1]);
              levelIndex += 2;

              return new KeyValuePair<TKey,TValue>(key, value);
            }
          }
        }
      }
      #endregion
      #endregion
      #endregion
    }

    private interface IFiller<TObject>
    {
      void Add(
        PdfArray names,
        int offset
        );

      ICollection<TObject> Collection
      {
        get;
      }
    }

    private class KeysFiller
      : IFiller<TKey>
    {
      private ICollection<TKey> keys = new List<TKey>();

      public void Add(
        PdfArray names,
        int offset
        )
      {keys.Add((TKey)names[offset]);}

      public ICollection<TKey> Collection
      {
        get
        {return keys;}
      }
    }

    private class ValuesFiller
      : IFiller<TValue>
    {
      private Tree<TKey,TValue> tree;
      private ICollection<TValue> values = new List<TValue>();

      internal ValuesFiller(
        Tree<TKey,TValue> tree
        )
      {this.tree = tree;}

      public void Add(
        PdfArray names,
        int offset
        )
      {values.Add(tree.WrapValue(names[offset + 1]));}

      public ICollection<TValue> Collection
      {
        get
        {return values;}
      }
    }
    #endregion

    #region static
    #region fields
    /**
      Minimum number of items in non-root nodes.
      Note that the tree (high) order is assumed twice as much (<see cref="Children.Info.Info(int, int)"/>.
    */
    private static readonly int TreeLowOrder = 5;
    #endregion
    #endregion

    #region dynamic
    #region fields
    private PdfName pairsKey;
    #endregion

    #region constructors
    protected Tree(
      Document context
      ) : base(context, new PdfDictionary())
    {Initialize();}

    protected Tree(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {Initialize();}
    #endregion

    #region interface
    #region public
    /**
      Gets the key associated to the specified value.
    */
    public TKey GetKey(
      TValue value
      )
    {
      /*
        NOTE: Current implementation doesn't support bidirectional maps, to say that the only
        currently-available way to retrieve a key from a value is to iterate the whole map (really
        poor performance!).
      */
      foreach(KeyValuePair<TKey,TValue> entry in this)
      {
        if(entry.Value.Equals(value))
          return entry.Key;
      }
      return null;
    }

    #region IDictionary
    public virtual void Add(
      TKey key,
      TValue value
      )
    {Add(key, value, false);}

    public virtual bool ContainsKey(
      TKey key
      )
    {
      /*
        NOTE: Here we assume that any named entry has a non-null value.
      */
      return this[key] != null;
    }

    public virtual ICollection<TKey> Keys
    {
      get
      {
        KeysFiller filler = new KeysFiller();
        Fill(filler, BaseDataObject);

        return filler.Collection;
      }
    }

    public virtual bool Remove(
      TKey key
      )
    {
      PdfDictionary node = BaseDataObject;
      Stack<PdfReference> nodeReferenceStack = new Stack<PdfReference>();
      while(true)
      {
        Children nodeChildren = Children.Get(node, pairsKey);
        if(nodeChildren.IsLeaf()) // Leaf node.
        {
          int low = 0, high = nodeChildren.Items.Count - nodeChildren.Info.ItemCount;
          while(true)
          {
            if(low > high) // No match.
              return false;

            int mid = (mid = ((low + high) / 2)) - (mid % 2);
            int comparison = key.CompareTo(nodeChildren.Items[mid]);
            if(comparison < 0) // Key before.
            {high = mid - 2;}
            else if(comparison > 0) // Key after.
            {low = mid + 2;}
            else // Key matched.
            {
              // We got it!
              nodeChildren.Items.RemoveAt(mid + 1); // Removes value.
              nodeChildren.Items.RemoveAt(mid); // Removes key.
              if(mid == 0 || mid == nodeChildren.Items.Count) // Limits changed.
              {
                // Update key limits!
                UpdateNodeLimits(nodeChildren);

                // Updating key limits on ascendants...
                PdfReference rootReference = (PdfReference)BaseObject;
                PdfReference nodeReference;
                while(nodeReferenceStack.Count > 0 && !(nodeReference = nodeReferenceStack.Pop()).Equals(rootReference))
                {
                  PdfArray parentChildren = (PdfArray)nodeReference.Parent;
                  int nodeIndex = parentChildren.IndexOf(nodeReference);
                  if(nodeIndex == 0 || nodeIndex == parentChildren.Count - 1)
                  {
                    PdfDictionary parent = (PdfDictionary)parentChildren.Parent;
                    UpdateNodeLimits(parent, parentChildren, PdfName.Kids);
                  }
                  else
                    break;
                }
              }
              return true;
            }
          }
        }
        else // Intermediate node.
        {
          int low = 0, high = nodeChildren.Items.Count - nodeChildren.Info.ItemCount;
          while(true)
          {
            if(low > high) // Outside the limit range.
              return false;

            int mid = (low + high) / 2;
            PdfReference kidReference = (PdfReference)nodeChildren.Items[mid];
            PdfDictionary kid = (PdfDictionary)kidReference.DataObject;
            PdfArray limits = (PdfArray)kid.Resolve(PdfName.Limits);
            if(key.CompareTo(limits[0]) < 0) // Before the lower limit.
            {high = mid - 1;}
            else if(key.CompareTo(limits[1]) > 0) // After the upper limit.
            {low = mid + 1;}
            else // Limit range matched.
            {
              Children kidChildren = Children.Get(kid, pairsKey);
              if(kidChildren.IsUndersized())
              {
                /*
                  NOTE: Rebalancing is required as minimum node size invariant is violated.
                */
                PdfDictionary leftSibling = null;
                Children leftSiblingChildren = null;
                if(mid > 0)
                {
                  leftSibling = (PdfDictionary)nodeChildren.Items.Resolve(mid - 1);
                  leftSiblingChildren = Children.Get(leftSibling, pairsKey);
                }
                PdfDictionary rightSibling = null;
                Children rightSiblingChildren = null;
                if(mid < nodeChildren.Items.Count - 1)
                {
                  rightSibling = (PdfDictionary)nodeChildren.Items.Resolve(mid + 1);
                  rightSiblingChildren = Children.Get(rightSibling, pairsKey);
                }

                if(leftSiblingChildren != null && !leftSiblingChildren.IsUndersized())
                {
                  // Move the last child subtree of the left sibling to be the first child subtree of the kid!
                  for(int index = 0, endIndex = leftSiblingChildren.Info.ItemCount; index < endIndex; index++)
                  {
                    int itemIndex = leftSiblingChildren.Items.Count - 1;
                    PdfDirectObject item = leftSiblingChildren.Items[itemIndex];
                    leftSiblingChildren.Items.RemoveAt(itemIndex);
                    kidChildren.Items.Insert(0, item);
                  }
                  // Update left sibling's key limits!
                  UpdateNodeLimits(leftSiblingChildren);
                }
                else if(rightSiblingChildren != null && !rightSiblingChildren.IsUndersized())
                {
                  // Move the first child subtree of the right sibling to be the last child subtree of the kid!
                  for(int index = 0, endIndex = rightSiblingChildren.Info.ItemCount; index < endIndex; index++)
                  {
                    int itemIndex = 0;
                    PdfDirectObject item = rightSiblingChildren.Items[itemIndex];
                    rightSiblingChildren.Items.RemoveAt(itemIndex);
                    kidChildren.Items.Add(item);
                  }
                  // Update right sibling's key limits!
                  UpdateNodeLimits(rightSiblingChildren);
                }
                else
                {
                  if(leftSibling != null)
                  {
                    // Merging with the left sibling...
                    for(int index = leftSiblingChildren.Items.Count; index-- > 0;)
                    {
                      PdfDirectObject item = leftSiblingChildren.Items[index];
                      leftSiblingChildren.Items.RemoveAt(index);
                      kidChildren.Items.Insert(0, item);
                    }
                    nodeChildren.Items.RemoveAt(mid - 1);
                    leftSibling.Reference.Delete();
                  }
                  else if(rightSibling != null)
                  {
                    // Merging with the right sibling...
                    for(int index = rightSiblingChildren.Items.Count; index-- > 0;)
                    {
                      int itemIndex = 0;
                      PdfDirectObject item = rightSiblingChildren.Items[itemIndex];
                      rightSiblingChildren.Items.RemoveAt(itemIndex);
                      kidChildren.Items.Add(item);
                    }
                    nodeChildren.Items.RemoveAt(mid + 1);
                    rightSibling.Reference.Delete();
                  }
                  if(nodeChildren.Items.Count == 1)
                  {
                    // Collapsing root...
                    nodeChildren.Items.RemoveAt(0);
                    for(int index = kidChildren.Items.Count; index-- > 0;)
                    {
                      int itemIndex = 0;
                      PdfDirectObject item = kidChildren.Items[itemIndex];
                      kidChildren.Items.RemoveAt(itemIndex);
                      nodeChildren.Items.Add(item);
                    }
                    kid.Reference.Delete();
                    kid = node;
                    kidReference = kid.Reference;
                    kidChildren = nodeChildren;
                  }
                }
                // Update key limits!
                UpdateNodeLimits(kidChildren);
              }
              // Go down one level!
              nodeReferenceStack.Push(kidReference);
              node = kid;
              break;
            }
          }
        }
      }
    }

    public virtual TValue this[
      TKey key
      ]
    {
      get
      {
        PdfDictionary parent = BaseDataObject;
        while(true)
        {
          Children children = Children.Get(parent, pairsKey);
          if(children.IsLeaf()) // Leaf node.
          {
            int low = 0, high = children.Items.Count - children.Info.ItemCount;
            while(true)
            {
              if(low > high)
                return null;

              int mid = (mid = ((low + high) / 2)) - (mid % 2);
              int comparison = key.CompareTo(children.Items[mid]);
              if(comparison < 0)
              {high = mid - 2;}
              else if(comparison > 0)
              {low = mid + 2;}
              else
              {
                // We got it!
                return WrapValue(children.Items[mid + 1]);
              }
            }
          }
          else // Intermediate node.
          {
            int low = 0, high = children.Items.Count - children.Info.ItemCount;
            while(true)
            {
              if(low > high)
                return null;

              int mid = (low + high) / 2;
              PdfDictionary kid = (PdfDictionary)children.Items.Resolve(mid);
              PdfArray limits = (PdfArray)kid.Resolve(PdfName.Limits);
              if(key.CompareTo(limits[0]) < 0)
              {high = mid - 1;}
              else if(key.CompareTo(limits[1]) > 0)
              {low = mid + 1;}
              else
              {
                // Go down one level!
                parent = kid;
                break;
              }
            }
          }
        }
      }
      set
      {Add(key, value, true);}
    }

    public virtual bool TryGetValue(
      TKey key,
      out TValue value
      )
    {
      value = this[key];
      return value != null;
    }

    public virtual ICollection<TValue> Values
    {
      get
      {
        ValuesFiller filler = new ValuesFiller(this);
        Fill(filler, BaseDataObject);
        return filler.Collection;
      }
    }

    #region ICollection
    void ICollection<KeyValuePair<TKey,TValue>>.Add(
      KeyValuePair<TKey,TValue> keyValuePair
      )
    {Add(keyValuePair.Key,keyValuePair.Value);}

    public virtual void Clear(
      )
    {Clear(BaseDataObject);}

    bool ICollection<KeyValuePair<TKey,TValue>>.Contains(
      KeyValuePair<TKey,TValue> keyValuePair
      )
    {return keyValuePair.Value.Equals(this[keyValuePair.Key]);}

    public virtual void CopyTo(
      KeyValuePair<TKey,TValue>[] keyValuePairs,
      int index
      )
    {throw new NotImplementedException();}

    public virtual int Count
    {
      get
      {return GetCount(BaseDataObject);}
    }

    public virtual bool IsReadOnly
    {
      get
      {return false;}
    }

    public virtual bool Remove(
      KeyValuePair<TKey,TValue> keyValuePair
      )
    {throw new NotSupportedException();}

    #region IEnumerable<KeyValuePair<TKey,TValue>>
    public virtual IEnumerator<KeyValuePair<TKey,TValue>> GetEnumerator(
      )
    {return new Enumerator(this);}

    #region IEnumerable
    IEnumerator IEnumerable.GetEnumerator(
      )
    {return this.GetEnumerator();}
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion

    #region protected
    /**
      <summary>Gets the name of the key-value pairs entries.</summary>
    */
    protected abstract PdfName PairsKey
    {
      get;
    }

    /**
      <summary>Wraps a base object within its corresponding high-level representation.</summary>
    */
    protected abstract TValue WrapValue(
      PdfDirectObject baseObject
      );
    #endregion

    #region private
    /**
      <summary>Adds an entry into the tree.</summary>
      <param name="key">New entry's key.</param>
      <param name="value">New entry's value.</param>
      <param name="overwrite">Whether the entry is allowed to replace an existing one having the same
      key.</param>
    */
    private void Add(
      TKey key,
      TValue value,
      bool overwrite
      )
    {
      // Get the root node!
      PdfDictionary root = BaseDataObject;

      // Ensuring the root node isn't full...
      {
        Children rootChildren = Children.Get(root, pairsKey);
        if(rootChildren.IsFull())
        {
          // Transfer the root contents into the new leaf!
          PdfDictionary leaf = (PdfDictionary)new PdfDictionary().Swap(root);
          PdfArray rootChildrenObject = new PdfArray(new PdfDirectObject[]{File.Register(leaf)});
          root[PdfName.Kids] = rootChildrenObject;
          // Split the leaf!
          SplitFullNode(
            rootChildrenObject,
            0, // Old root's position within new root's kids.
            rootChildren.TypeName
            );
        }
      }

      // Set the entry under the root node!
      Add(key, value, overwrite, root);
    }

    /**
      <summary>Adds an entry under the given tree node.</summary>
      <param name="key">New entry's key.</param>
      <param name="value">New entry's value.</param>
      <param name="overwrite">Whether the entry is allowed to replace an existing one having the same
      key.</param>
      <param name="nodeReference">Current node reference.</param>
    */
    private void Add(
      TKey key,
      TValue value,
      bool overwrite,
      PdfDictionary node
      )
    {
      Children children = Children.Get(node, pairsKey);
      if(children.IsLeaf()) // Leaf node.
      {
        int childrenSize = children.Items.Count;
        int low = 0, high = childrenSize - children.Info.ItemCount;
        while(true)
        {
          if(low > high)
          {
            // Insert the entry!
            children.Items.Insert(low, key);
            children.Items.Insert(++low, value.BaseObject);
            break;
          }

          int mid = (mid = ((low + high) / 2)) - (mid % 2);
          if(mid >= childrenSize)
          {
            // Append the entry!
            children.Items.Add(key);
            children.Items.Add(value.BaseObject);
            break;
          }

          int comparison = key.CompareTo(children.Items[mid]);
          if(comparison < 0) // Before.
          {high = mid - 2;}
          else if(comparison > 0) // After.
          {low = mid + 2;}
          else // Matching entry.
          {
            if(!overwrite)
              throw new ArgumentException("Key '" + key + "' already exists.", "key");

            // Overwrite the entry!
            children.Items[mid] = key;
            children.Items[++mid] = value.BaseObject;
            break;
          }
        }

        // Update the key limits!
        UpdateNodeLimits(children);
      }
      else // Intermediate node.
      {
        int low = 0, high = children.Items.Count - children.Info.ItemCount;
        while(true)
        {
          bool matched = false;
          int mid = (low + high) / 2;
          PdfReference kidReference = (PdfReference)children.Items[mid];
          PdfDictionary kid = (PdfDictionary)kidReference.DataObject;
          PdfArray limits = (PdfArray)kid.Resolve(PdfName.Limits);
          if(key.CompareTo(limits[0]) < 0) // Before the lower limit.
          {high = mid - 1;}
          else if(key.CompareTo(limits[1]) > 0) // After the upper limit.
          {low = mid + 1;}
          else // Limit range matched.
          {matched = true;}

          if(matched // Limit range matched.
            || low > high) // No limit range match.
          {
            Children kidChildren = Children.Get(kid, pairsKey);
            if(kidChildren.IsFull())
            {
              // Split the node!
              SplitFullNode(
                children.Items,
                mid,
                kidChildren.TypeName
                );
              // Is the key before the split node?
              if(key.CompareTo(((PdfArray)kid.Resolve(PdfName.Limits))[0]) < 0)
              {
                kidReference = (PdfReference)children.Items[mid];
                kid = (PdfDictionary)kidReference.DataObject;
              }
            }

            Add(key, value, overwrite, kid);
            // Update the key limits!
            UpdateNodeLimits(children);
            break;
          }
        }
      }
    }

    /**
      <summary>Removes all the given node's children.</summary>
      <remarks>
        <para>As this method doesn't apply balancing, it's suitable for clearing root nodes only.
        </para>
        <para>Removal affects only tree nodes: referenced objects are preserved to avoid inadvertently
        breaking possible references to them from somewhere else.</para>
      </remarks>
      <param name="node">Current node.</param>
    */
    private void Clear(
      PdfDictionary node
      )
    {
      Children children = Children.Get(node, pairsKey);
      if(!children.IsLeaf())
      {
        foreach(PdfDirectObject child in children.Items)
        {
          Clear((PdfDictionary)child.Resolve());
          File.Unregister((PdfReference)child);
        }
        node[pairsKey] = node[children.TypeName];
        node.Remove(children.TypeName); // Recycles the array as the intermediate node transforms to leaf.
      }
      children.Items.Clear();
      node.Remove(PdfName.Limits);
    }

    private void Fill<TObject>(
      IFiller<TObject> filler,
      PdfDictionary node
      )
    {
      PdfArray kidsObject = (PdfArray)node.Resolve(PdfName.Kids);
      if(kidsObject == null) // Leaf node.
      {
        PdfArray namesObject = (PdfArray)node.Resolve(PdfName.Names);
        for(
          int index = 0,
            length = namesObject.Count;
          index < length;
          index += 2
          )
        {filler.Add(namesObject,index);}
      }
      else // Intermediate node.
      {
        foreach(PdfDirectObject kidObject in kidsObject)
        {Fill(filler, (PdfDictionary)kidObject.Resolve());}
      }
    }

    /**
      <summary>Gets the given node's entries count.</summary>
      <param name="node">Current node.</param>
    */
    private int GetCount(
      PdfDictionary node
      )
    {
      PdfArray children = (PdfArray)node.Resolve(PdfName.Names);
      if(children != null) // Leaf node.
      {return (children.Count / 2);}
      else // Intermediate node.
      {
        children = (PdfArray)node.Resolve(PdfName.Kids);
        int count = 0;
        foreach(PdfDirectObject child in children)
        {count += GetCount((PdfDictionary)child.Resolve());}
        return count;
      }
    }

    private void Initialize(
      )
    {
      pairsKey = PairsKey;

      PdfDictionary baseDataObject = BaseDataObject;
      if(baseDataObject.Count == 0)
      {
        baseDataObject.Updateable = false;
        baseDataObject[pairsKey] = new PdfArray(); // NOTE: Initial root is by definition a leaf node.
        baseDataObject.Updateable = true;
      }
    }

    /**
      <summary>Splits a full node.</summary>
      <remarks>A new node is inserted at the full node's position, receiving the lower half of its
      children.</remarks>
      <param name="nodes">Parent nodes.</param>
      <param name="fullNodeIndex">Full node's position among the parent nodes.</param>
      <param name="childrenTypeName">Full node's children type.</param>
    */
    private void SplitFullNode(
      PdfArray nodes,
      int fullNodeIndex,
      PdfName childrenTypeName
      )
    {
      // Get the full node!
      PdfDictionary fullNode = (PdfDictionary)nodes.Resolve(fullNodeIndex);
      PdfArray fullNodeChildren = (PdfArray)fullNode.Resolve(childrenTypeName);

      // Create a new (sibling) node!
      PdfDictionary newNode = new PdfDictionary();
      PdfArray newNodeChildren = new PdfArray();
      newNode[childrenTypeName] = newNodeChildren;
      // Insert the new node just before the full!
      nodes.Insert(fullNodeIndex,File.Register(newNode)); // NOTE: Nodes MUST be indirect objects.

      // Transferring exceeding children to the new node...
      for(int index = 0, length = Children.InfoImpl.Get(childrenTypeName).MinCount; index < length; index++)
      {
        PdfDirectObject removedChild = fullNodeChildren[0];
        fullNodeChildren.RemoveAt(0);
        newNodeChildren.Add(removedChild);
      }

      // Update the key limits!
      UpdateNodeLimits(newNode, newNodeChildren, childrenTypeName);
      UpdateNodeLimits(fullNode, fullNodeChildren, childrenTypeName);
    }

    /**
      <summary>Sets the key limits of the given node.</summary>
      <param name="children">Node children.</param>
    */
    private void UpdateNodeLimits(
      Children children
      )
    {UpdateNodeLimits(children.Parent, children.Items, children.TypeName);}

    /**
      <summary>Sets the key limits of the given node.</summary>
      <param name="node">Node to update.</param>
      <param name="children">Node children.</param>
      <param name="childrenTypeName">Node's children type.</param>
    */
    private void UpdateNodeLimits(
      PdfDictionary node,
      PdfArray children,
      PdfName childrenTypeName
      )
    {
      PdfDirectObject lowLimit, highLimit;
      if(childrenTypeName.Equals(PdfName.Kids))
      {
        // Non-leaf root node?
        if(node == BaseDataObject)
          return; // NOTE: Non-leaf root nodes DO NOT specify limits.

        lowLimit = ((PdfArray)((PdfDictionary)children.Resolve(0)).Resolve(PdfName.Limits))[0];
        highLimit = ((PdfArray)((PdfDictionary)children.Resolve(children.Count-1)).Resolve(PdfName.Limits))[1];
      }
      else if(childrenTypeName.Equals(pairsKey))
      {
        lowLimit = children[0];
        highLimit = children[children.Count-2];
      }
      else // NOTE: Should NEVER happen.
        throw new NotSupportedException(childrenTypeName + " is NOT a supported child type.");

      PdfArray limits = (PdfArray)node[PdfName.Limits];
      if(limits != null)
      {
        limits[0] = lowLimit;
        limits[1] = highLimit;
      }
      else
      {
        node[PdfName.Limits] = new PdfArray(
          new PdfDirectObject[]
          {
            lowLimit,
            highLimit
          }
          );
      }
    }
    #endregion
    #endregion
    #endregion
  }
}