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

using org.pdfclown.objects;

using System;
using System.Collections;
using System.Collections.Generic;

namespace org.pdfclown.documents.contents.layers
{
  /**
    <summary>Optional content membership [PDF:1.7:4.10.1].</summary>
  */
  [PDF(VersionEnum.PDF15)]
  public sealed class LayerMembership
    : LayerEntity
  {
    #region types
    /**
      <summary>Layers whose states determine the visibility of content controlled by a membership.</summary>
    */
    private class VisibilityLayersImpl
      : PdfObjectWrapper<PdfDirectObject>,
        IList<Layer>
    {
      #region fields
      private LayerMembership membership;
      #endregion

      #region constructors
      internal VisibilityLayersImpl(
        LayerMembership membership
        ) : base(membership.BaseDataObject[PdfName.OCGs])
      {this.membership = membership;}
      #endregion

      #region interface
      #region public
      #region IList<Layer>
      public int IndexOf(
        Layer item
        )
      {
        PdfDataObject baseDataObject = BaseDataObject;
        if(baseDataObject == null) // No layer.
          return -1;
        else if(baseDataObject is PdfDictionary) // Single layer.
          return item.BaseObject.Equals(BaseObject) ? 0 : -1;
        else // Multiple layers.
          return ((PdfArray)baseDataObject).IndexOf(item.BaseObject);
      }

      public void Insert(
        int index,
        Layer item
        )
      {EnsureArray().Insert(index, item.BaseObject);}

      public void RemoveAt(
        int index
        )
      {EnsureArray().RemoveAt(index);}

      public Layer this[
        int index
        ]
      {
        get
        {
          PdfDataObject baseDataObject = BaseDataObject;
          if(baseDataObject == null) // No layer.
            return null;
          else if(baseDataObject is PdfDictionary) // Single layer.
          {
            if(index != 0)
              throw new IndexOutOfRangeException();

            return Layer.Wrap(BaseObject);
          }
          else // Multiple layers.
            return Layer.Wrap(((PdfArray)baseDataObject)[index]);
        }
        set
        {EnsureArray()[index] = value.BaseObject;}
      }

      #region ICollection<Page>
      public void Add(
        Layer item
        )
      {EnsureArray().Add(item.BaseObject);}

      public void Clear(
        )
      {EnsureArray().Clear();}

      public bool Contains(
        Layer item
        )
      {
        PdfDataObject baseDataObject = BaseDataObject;
        if(baseDataObject == null) // No layer.
          return false;
        else if(baseDataObject is PdfDictionary) // Single layer.
          return item.BaseObject.Equals(BaseObject);
        else // Multiple layers.
          return ((PdfArray)baseDataObject).Contains(item.BaseObject);
      }

      public void CopyTo(
        Layer[] items,
        int index
        )
      {throw new NotImplementedException();}

      public int Count
      {
        get
        {
          PdfDataObject baseDataObject = BaseDataObject;
          if(baseDataObject == null) // No layer.
            return 0;
          else if(baseDataObject is PdfDictionary) // Single layer.
            return 1;
          else // Multiple layers.
            return ((PdfArray)baseDataObject).Count;
        }
      }

      public bool IsReadOnly
      {
        get
        {return false;}
      }

      public bool Remove(
        Layer item
        )
      {return EnsureArray().Remove(item.BaseObject);}

      #region IEnumerable<Layer>
      public IEnumerator<Layer> GetEnumerator(
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

      #region private
      private PdfArray EnsureArray(
        )
      {
        PdfDirectObject baseDataObject = BaseDataObject;
        if(!(baseDataObject is PdfArray))
        {
          PdfArray array = new PdfArray();
          if(baseDataObject != null)
          {array.Add(baseDataObject);}
          BaseObject = baseDataObject = array;
          membership.BaseDataObject[PdfName.OCGs] = BaseObject;
        }
        return (PdfArray)baseDataObject;
      }
      #endregion
      #endregion
    }
    #endregion

    #region static
    #region fields
    public static PdfName TypeName = PdfName.OCMD;
    #endregion

    #region interface
    #region public
    public static new LayerMembership Wrap(
      PdfDirectObject baseObject
      )
    {return baseObject != null ? new LayerMembership(baseObject) : null;}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public LayerMembership(
      Document context
      ) : base(context, TypeName)
    {}

    private LayerMembership(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    public override LayerMembership Membership
    {
      get
      {return this;}
    }

    public override IList<Layer> VisibilityLayers
    {
      get
      {return new VisibilityLayersImpl(this);}
    }

    public override VisibilityPolicyEnum VisibilityPolicy
    {
      get
      {return VisibilityPolicyEnumExtension.Get((PdfName)BaseDataObject[PdfName.P]);}
      set
      {BaseDataObject[PdfName.P] = value.GetName();}
    }
    #endregion
    #endregion
    #endregion
  }
}