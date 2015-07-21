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

using org.pdfclown.documents;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace org.pdfclown.objects
{
  /**
    <summary>Collection of sequentially-arranged object wrappers.</summary>
  */
  public class Array<TItem>
    : PdfObjectWrapper<PdfArray>,
      IList<TItem>
    where TItem : IPdfObjectWrapper
  {
    #region types
    /**
      <summary>Item instancer.</summary>
    */
    public interface IWrapper<T>
      where T : TItem
    {
      T Wrap(
        PdfDirectObject baseObject
        );
    }

    private class DefaultWrapper<T>
      : IWrapper<T>
      where T : TItem
    {
      private MethodInfo itemConstructor;

      internal DefaultWrapper(
        )
      {itemConstructor = typeof(TItem).GetMethod("Wrap", new Type[]{typeof(PdfDirectObject)});}

      public T Wrap(
        PdfDirectObject baseObject
        )
      {return (T)itemConstructor.Invoke(null, new object[]{baseObject});}
    }
    #endregion

    #region static
    #region interface
    #region public
    /**
      <summary>Wraps an existing base array using the default wrapper for wrapping its items.
      </summary>
      <param name="itemClass">Item class.</param>
      <param name="baseObject">Base array. MUST be a {@link PdfReference reference} every time
      available.</param>
    */
    public static Array<T> Wrap<T>(
      PdfDirectObject baseObject
      ) where T : TItem
    {return baseObject != null ? new Array<T>(baseObject) : null;}
  
    /**
      <summary>Wraps an existing base array using the specified wrapper for wrapping its items.
      </summary>
      <param name="itemWrapper">Item wrapper.</param>
      <param name="baseObject">Base array. MUST be a {@link PdfReference reference} every time
      available.</param>
    */
    public static Array<T> Wrap<T>(
      Array<T>.IWrapper<T> itemWrapper,
      PdfDirectObject baseObject
      ) where T : TItem
    {return baseObject != null ? new Array<T>(itemWrapper, baseObject) : null;}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region fields
    private IWrapper<TItem> itemWrapper;
    #endregion

    #region constructors
    /**
      <summary>Wraps a new base array using the default wrapper for wrapping its items.</summary>
      <param name="context">Document context.</param>
    */
    public Array(
      Document context
      ) : this(
        context,
        new PdfArray()
        )
    {}

    /**
      <summary>Wraps a new base array using the specified wrapper for wrapping its items.</summary>
      <param name="context">Document context.</param>
      <param name="itemWrapper">Item wrapper.</param>
    */
    public Array(
      Document context,
      IWrapper<TItem> itemWrapper
      ) : this(
        context,
        itemWrapper,
        new PdfArray()
        )
    {}

    /**
      <summary>Wraps the specified base array using the default wrapper for wrapping its items.</summary>
      <param name="context">Document context.</param>
      <param name="baseDataObject">Base array.</param>
    */
    public Array(
      Document context,
      PdfArray baseDataObject
      ) : this(
        context,
        new DefaultWrapper<TItem>(),
        baseDataObject
        )
    {}

    /**
      <summary>Wraps the specified base array using the specified wrapper for wrapping its items.</summary>
      <param name="context">Document context.</param>
      <param name="itemWrapper">Item wrapper.</param>
      <param name="baseDataObject">Base array.</param>
    */
    public Array(
      Document context,
      IWrapper<TItem> itemWrapper,
      PdfArray baseDataObject
      ) : base(context, baseDataObject)
    {this.itemWrapper = itemWrapper;}

    /**
      <summary>Wraps an existing base array using the default wrapper for wrapping its items.</summary>
      <param name="baseObject">Base array. MUST be a <see cref="PdfReference">reference</see>
      everytime available.</param>
    */
    protected Array(
      PdfDirectObject baseObject
      ) : this(
        new DefaultWrapper<TItem>(),
        baseObject
        )
    {}

    /**
      <summary>Wraps an existing base array using the specified wrapper for wrapping its items.</summary>
      <param name="itemWrapper">Item wrapper.</param>
      <param name="baseObject">Base array. MUST be a <see cref="PdfReference">reference</see>
      everytime available.</param>
    */
    protected Array(
      IWrapper<TItem> itemWrapper,
      PdfDirectObject baseObject
      ) : base(baseObject)
    {this.itemWrapper = itemWrapper;}
    #endregion

    #region interface
    #region public
    #region IList<TItem>
    public virtual int IndexOf(
      TItem item
      )
    {return BaseDataObject.IndexOf(item.BaseObject);}

    public virtual void Insert(
      int index,
      TItem item
      )
    {BaseDataObject.Insert(index, item.BaseObject);}

    public virtual void RemoveAt(
      int index
      )
    {BaseDataObject.RemoveAt(index);}

    public virtual TItem this[
      int index
      ]
    {
      get
      {return itemWrapper.Wrap(BaseDataObject[index]);}
      set
      {BaseDataObject[index] = value.BaseObject;}
    }

    #region ICollection<TItem>
    public virtual void Add(
      TItem item
      )
    {BaseDataObject.Add(item.BaseObject);}

    public virtual void Clear(
      )
    {
      int index = Count;
      while(index-- > 0)
      {RemoveAt(index);}
    }

    public virtual bool Contains(
      TItem item
      )
    {return BaseDataObject.Contains(item.BaseObject);}

    public virtual void CopyTo(
      TItem[] items,
      int index
      )
    {throw new NotImplementedException();}

    public virtual int Count
    {
      get
      {return BaseDataObject.Count;}
    }

    public virtual bool IsReadOnly
    {
      get
      {return false;}
    }

    public virtual bool Remove(
      TItem item
      )
    {return BaseDataObject.Remove(item.BaseObject);}

    #region IEnumerable<TItem>
    public virtual IEnumerator<TItem> GetEnumerator(
      )
    {
      for(int index = 0, length = Count; index < length; index++)
      {yield return this[index];}
    }

    #region IEnumerable
    IEnumerator IEnumerable.GetEnumerator(
      )
    {return this.GetEnumerator();}
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion
  }
}