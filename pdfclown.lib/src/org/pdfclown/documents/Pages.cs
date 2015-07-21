/*
  Copyright 2006-2012 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.util.collections.generic;

using System;
using System.Collections;
using System.Collections.Generic;

namespace org.pdfclown.documents
{
  /**
    <summary>Document pages collection [PDF:1.6:3.6.2].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class Pages
    : PdfObjectWrapper<PdfDictionary>,
      IExtList<Page>,
      IList<Page>
  {
    #region types
    private class Enumerator
      : IEnumerator<Page>
    {
      /**
        <summary>Collection size.</summary>
      */
      private int count;
      /**
        <summary>Index of the next item.</summary>
      */
      private int index = 0;

      /**
        <summary>Current page.</summary>
      */
      private Page current;

      /**
        <summary>Current level index.</summary>
      */
      private int levelIndex = 0;
      /**
        <summary>Stacked level indexes.</summary>
      */
      private Stack<int> levelIndexes = new Stack<int>();

      /**
        <summary>Current child tree nodes.</summary>
      */
      private PdfArray kids;
      /**
        <summary>Current parent tree node.</summary>
      */
      private PdfDictionary parent;

      internal Enumerator(
        Pages pages
        )
      {
        count = pages.Count;
        parent = pages.BaseDataObject;
        kids = (PdfArray)parent.Resolve(PdfName.Kids);
      }

      Page IEnumerator<Page>.Current
      {
        get
        {return current;}
      }

      public object Current
      {
        get
        {return ((IEnumerator<Page>)this).Current;}
      }

      public bool MoveNext(
        )
      {
        if(index == count)
          return false;

        /*
          NOTE: As stated in [PDF:1.6:3.6.2], page retrieval is a matter of diving
          inside a B-tree.
          This is a special adaptation of the get() algorithm necessary to keep
          a low overhead throughout the page tree scan (using the get() method
          would have implied a nonlinear computational cost).
        */
        /*
          NOTE: Algorithm:
          1. [Vertical, down] We have to go downward the page tree till we reach
          a page (leaf node).
          2. [Horizontal] Then we iterate across the page collection it belongs to,
          repeating step 1 whenever we find a subtree.
          3. [Vertical, up] When leaf-nodes scan is complete, we go upward solving
          parent nodes, repeating step 2.
        */
        while(true)
        {
          // Did we complete current page-tree-branch level?
          if(kids.Count == levelIndex) // Page subtree complete.
          {
            // 3. Go upward one level.
            // Restore node index at the current level!
            levelIndex = levelIndexes.Pop() + 1; // Next node (partially scanned level).
            // Move upward!
            parent = (PdfDictionary)parent.Resolve(PdfName.Parent);
            kids = (PdfArray)parent.Resolve(PdfName.Kids);
          }
          else // Page subtree incomplete.
          {
            PdfReference kidReference = (PdfReference)kids[levelIndex];
            PdfDictionary kid = (PdfDictionary)kidReference.DataObject;
            // Is current kid a page object?
            if(kid[PdfName.Type].Equals(PdfName.Page)) // Page object.
            {
              // 2. Page found.
              index++; // Absolute page index.
              levelIndex++; // Current level node index.

              current = Page.Wrap(kidReference);
              return true;
            }
            else // Page tree node.
            {
              // 1. Go downward one level.
              // Save node index at the current level!
              levelIndexes.Push(levelIndex);
              // Move downward!
              parent = kid;
              kids = (PdfArray)parent.Resolve(PdfName.Kids);
              levelIndex = 0; // First node (new level).
            }
          }
        }
      }

      public void Reset(
        )
      {throw new NotSupportedException();}

      public void Dispose(
        )
      {}
    }
    #endregion

    /*
      TODO:IMPL A B-tree algorithm should be implemented to optimize the inner layout
      of the page tree (better insertion/deletion performance). In this case, it would
      be necessary to keep track of the modified tree nodes for incremental update.
    */
    #region dynamic
    #region constructors
    internal Pages(
      Document context
      ) : base(
        context,
        new PdfDictionary(
          new PdfName[3]
          {
            PdfName.Type,
            PdfName.Kids,
            PdfName.Count
          },
          new PdfDirectObject[3]
          {
            PdfName.Pages,
            new PdfArray(),
            PdfInteger.Default
          }
          )
        )
    {}

    internal Pages(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    #region IExtList<Page>
    public IList<Page> GetRange(
      int index,
      int count
      )
    {
      return GetSlice(
        index,
        index + count
        );
    }

    public IList<Page> GetSlice(
      int fromIndex,
      int toIndex
      )
    {
      List<Page> pages = new List<Page>(toIndex - fromIndex);
      int i = fromIndex;
      while(i < toIndex)
      {pages.Add(this[i++]);}

      return pages;
    }

    public void InsertAll<TVar>(
      int index,
      ICollection<TVar> pages
      )
      where TVar : Page
    {CommonAddAll(index,pages);}

    #region IExtCollection<Page>
    public void AddAll<TVar>(
      ICollection<TVar> pages
      )
      where TVar : Page
    {CommonAddAll(-1,pages);}

    public void RemoveAll<TVar>(
      ICollection<TVar> pages
      )
      where TVar : Page
    {
      /*
        NOTE: The interface contract doesn't prescribe any relation among the removing-collection's
        items, so we cannot adopt the optimized approach of the add*(...) methods family,
        where adding-collection's items are explicitly ordered.
      */
      foreach(Page page in pages)
      {Remove(page);}
    }

    public int RemoveAll(
      Predicate<Page> match
      )
    {
      /*
        NOTE: Removal is indirectly fulfilled through an intermediate collection
        in order not to interfere with the enumerator execution.
      */
      List<Page> removingPages = new List<Page>();
      foreach(Page page in this)
      {
        if(match(page))
        {removingPages.Add(page);}
      }

      RemoveAll(removingPages);

      return removingPages.Count;
    }
    #endregion
    #endregion

    #region IList<Page>
    public int IndexOf(
      Page page
      )
    {return page.Index;}

    public void Insert(
      int index,
      Page page
      )
    {CommonAddAll(index,(ICollection<Page>)new Page[]{page});}

    public void RemoveAt(
      int index
      )
    {Remove(this[index]);}

    public Page this[
      int index
      ]
    {
      get
      {
        /*
          NOTE: As stated in [PDF:1.6:3.6.2], to retrieve pages is a matter of diving
          inside a B-tree. To keep it as efficient as possible, this implementation
          does NOT adopt recursion to deepen its search, opting for an iterative
          strategy instead.
        */
        int pageOffset = 0;
        PdfDictionary parent = BaseDataObject;
        PdfArray kids = (PdfArray)parent.Resolve(PdfName.Kids);
        for(
          int i = 0;
          i < kids.Count;
          i++
          )
        {
          PdfReference kidReference = (PdfReference)kids[i];
          PdfDictionary kid = (PdfDictionary)kidReference.DataObject;
          // Is current kid a page object?
          if(kid[PdfName.Type].Equals(PdfName.Page)) // Page object.
          {
            // Did we reach the searched position?
            if(pageOffset == index) // Vertical scan (we finished).
              // We got it!
              return Page.Wrap(kidReference);
            else // Horizontal scan (go past).
              // Cumulate current page object count!
              pageOffset++;
          }
          else // Page tree node.
          {
            // Does the current subtree contain the searched page?
            if(((PdfInteger)kid[PdfName.Count]).RawValue + pageOffset > index) // Vertical scan (deepen the search).
            {
              // Go down one level!
              parent = kid;
              kids = (PdfArray)parent.Resolve(PdfName.Kids);
              i = -1;
            }
            else // Horizontal scan (go past).
            {
              // Cumulate current subtree count!
              pageOffset += ((PdfInteger)kid[PdfName.Count]).RawValue;
            }
          }
        }

        return null;
      }
      set
      {
        RemoveAt(index);
        Insert(index,value);
      }
    }

    #region ICollection<Page>
    public void Add(
      Page page
      )
    {CommonAddAll(-1,(ICollection<Page>)new Page[]{page});}

    public void Clear(
      )
    {throw new NotImplementedException();}

    public bool Contains(
      Page page
      )
    {throw new NotImplementedException();}

    public void CopyTo(
      Page[] pages,
      int index
      )
    {throw new NotImplementedException();}

    public int Count
    {get{return ((PdfInteger)BaseDataObject[PdfName.Count]).RawValue;}}

    public bool IsReadOnly
    {get{return false;}}

    public bool Remove(
      Page page
      )
    {
      PdfDictionary pageData = page.BaseDataObject;
      // Get the parent tree node!
      PdfDirectObject parent = pageData[PdfName.Parent];
      PdfDictionary parentData = (PdfDictionary)parent.Resolve();
      // Get the parent's page collection!
      PdfDirectObject kids = parentData[PdfName.Kids];
      PdfArray kidsData = (PdfArray)kids.Resolve();
      // Remove the page!
      kidsData.Remove(page.BaseObject);

      // Unbind the page from its parent!
      pageData[PdfName.Parent] = null;

      // Decrementing the pages counters...
      do
      {
        // Get the page collection counter!
        PdfInteger countObject = (PdfInteger)parentData[PdfName.Count];
        // Decrement the counter at the current level!
        parentData[PdfName.Count] = PdfInteger.Get(countObject.IntValue-1);

        // Iterate upward!
        parent = parentData[PdfName.Parent];
        parentData = (PdfDictionary)PdfObject.Resolve(parent);
      } while(parent != null);

      return true;
    }

    #region IEnumerable<Page>
    public IEnumerator<Page> GetEnumerator(
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

    #region private
    /**
      Add a collection of pages at the specified position.
      <param name="index">Addition position. To append, use value -1.</param>
      <param name="pages">Collection of pages to add.</param>
    */
    private void CommonAddAll<TPage>(
      int index,
      ICollection<TPage> pages
      ) where TPage : Page
    {
      PdfDirectObject parent;
      PdfDictionary parentData;
      PdfDirectObject kids;
      PdfArray kidsData;
      int offset;
      // Append operation?
      if(index == -1) // Append operation.
      {
        // Get the parent tree node!
        parent = BaseObject;
        parentData = BaseDataObject;
        // Get the parent's page collection!
        kids = parentData[PdfName.Kids];
        kidsData = (PdfArray)PdfObject.Resolve(kids);
        offset = 0; // Not used.
      }
      else // Insert operation.
      {
        // Get the page currently at the specified position!
        Page pivotPage = this[index];
        // Get the parent tree node!
        parent = pivotPage.BaseDataObject[PdfName.Parent];
        parentData = (PdfDictionary)parent.Resolve();
        // Get the parent's page collection!
        kids = parentData[PdfName.Kids];
        kidsData = (PdfArray)kids.Resolve();
        // Get the insertion's relative position within the parent's page collection!
        offset = kidsData.IndexOf(pivotPage.BaseObject);
      }

      // Adding the pages...
      foreach(Page page in pages)
      {
        // Append?
        if(index == -1) // Append.
        {
          // Append the page to the collection!
          kidsData.Add(page.BaseObject);
        }
        else // Insert.
        {
          // Insert the page into the collection!
          kidsData.Insert(
            offset++,
            page.BaseObject
            );
        }
        // Bind the page to the collection!
        page.BaseDataObject[PdfName.Parent] = parent;
      }

      // Incrementing the pages counters...
      do
      {
        // Get the page collection counter!
        PdfInteger countObject = (PdfInteger)parentData[PdfName.Count];
        // Increment the counter at the current level!
        parentData[PdfName.Count] = PdfInteger.Get(countObject.IntValue+pages.Count);

        // Iterate upward!
        parent = parentData[PdfName.Parent];
        parentData = (PdfDictionary)PdfObject.Resolve(parent);
      } while(parent != null);
    }
    #endregion
    #endregion
    #endregion
  }
}