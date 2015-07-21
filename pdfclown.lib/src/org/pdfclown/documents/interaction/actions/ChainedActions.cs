/*
  Copyright 2008-2011 Stefano Chizzolini. http://www.pdfclown.org

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

using org.pdfclown;
using org.pdfclown.documents;
using org.pdfclown.files;
using org.pdfclown.objects;

using System;
using System.Collections;
using System.Collections.Generic;

namespace org.pdfclown.documents.interaction.actions
{
  /**
    <summary>Chained actions [PDF:1.6:8.5.1].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class ChainedActions
    : PdfObjectWrapper<PdfDataObject>,
      IList<Action>
  {
    /*
      NOTE: Chained actions may be either singular or multiple (within an array).
      This implementation hides such a complexity to the user, smoothly exposing
      just the most general case (array) yet preserving its internal state.
    */
    #region dynamic
    #region fields
    /**
      Parent action.
    */
    private Action parent;
    #endregion

    #region constructors
    internal ChainedActions(
      PdfDirectObject baseObject,
      Action parent
      ) : base(baseObject)
    {this.parent = parent;}
    #endregion

    #region interface
    #region public
    public override object Clone(
      Document context
      )
    {throw new NotImplementedException();} // TODO:verify

    /**
      <summary>Gets the parent action.</summary>
    */
    public Action Parent
    {get{return parent;}}

    #region IList
    public int IndexOf(
      Action value
      )
    {
      PdfDataObject baseDataObject = BaseDataObject;
      if(baseDataObject is PdfDictionary) // Single action.
        return (((Action)value).BaseObject.Equals(BaseObject) ? 0 : -1);
      else // Multiple actions.
        return ((PdfArray)baseDataObject).IndexOf(((Action)value).BaseObject);
    }

    public void Insert(
      int index,
      Action value
      )
    {EnsureArray().Insert(index,value.BaseObject);}

    public void RemoveAt(
      int index
      )
    {EnsureArray().RemoveAt(index);}

    public Action this[
      int index
      ]
    {
      get
      {
        PdfDataObject baseDataObject = BaseDataObject;
        if(baseDataObject is PdfDictionary) // Single action.
        {
          if(index != 0)
            throw new ArgumentException("Index: " + index + ", Size: 1");

          return Action.Wrap(BaseObject);
        }
        else // Multiple actions.
          return Action.Wrap(((PdfArray)baseDataObject)[index]);
      }
      set
      {EnsureArray()[index] = value.BaseObject;}
    }

    #region ICollection
    public void Add(
      Action value
      )
    {EnsureArray().Add(value.BaseObject);}

    public void Clear(
      )
    {EnsureArray().Clear();}

    public bool Contains(
      Action value
      )
    {
      PdfDataObject baseDataObject = BaseDataObject;
      if(baseDataObject is PdfDictionary) // Single action.
        return ((Action)value).BaseObject.Equals(BaseObject);
      else // Multiple actions.
        return ((PdfArray)baseDataObject).Contains(((Action)value).BaseObject);
    }

    public void CopyTo(
      Action[] values,
      int index
      )
    {throw new NotImplementedException();}

    public int Count
    {
      get
      {
        PdfDataObject baseDataObject = BaseDataObject;
        if(baseDataObject is PdfDictionary) // Single action.
          return 1;
        else // Multiple actions.
          return ((PdfArray)baseDataObject).Count;
      }
    }

    public bool IsReadOnly
    {get{return false;}}

    public bool Remove(
      Action value
      )
    {return EnsureArray().Remove(((Action)value).BaseObject);}

    #region IEnumerable<Action>
    IEnumerator<Action> IEnumerable<Action>.GetEnumerator(
      )
    {
      for(
        int index = 0,
          length = Count;
        index < length;
        index++
        )
      {yield return this[index];}
    }

    #region IEnumerable
    IEnumerator IEnumerable.GetEnumerator(
      )
    {return ((IEnumerable<Action>)this).GetEnumerator();}
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion

    #region private
    private PdfArray EnsureArray(
      )
    {
      PdfDataObject baseDataObject = BaseDataObject;
      if(baseDataObject is PdfDictionary) // Single action.
      {
        PdfArray actionsArray = new PdfArray();
        actionsArray.Add(BaseObject);
        BaseObject = actionsArray;
        parent.BaseDataObject[PdfName.Next] = actionsArray;

        baseDataObject = actionsArray;
      }
      return (PdfArray)baseDataObject;
    }
    #endregion
    #endregion
    #endregion
  }
}