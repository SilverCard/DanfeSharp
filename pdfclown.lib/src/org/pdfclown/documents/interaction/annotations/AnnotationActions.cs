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

using org.pdfclown.documents.interaction.actions;
using org.pdfclown.files;
using org.pdfclown.objects;

using system = System;
using System.Collections;
using System.Collections.Generic;

namespace org.pdfclown.documents.interaction.annotations
{
  /**
    <summary>Annotation actions [PDF:1.6:8.5.2].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public class AnnotationActions
    : PdfObjectWrapper<PdfDictionary>,
      IDictionary<PdfName,Action>
  {
    #region dynamic
    #region fields
    private Annotation parent;
    #endregion

    #region constructors
    public AnnotationActions(
      Annotation parent
      ) : base(parent.Document, new PdfDictionary())
    {this.parent = parent;}

    internal AnnotationActions(
      Annotation parent,
      PdfDirectObject baseObject
      ) : base(baseObject)
    {this.parent = parent;}
    #endregion

    #region interface
    #region public
    public override object Clone(
      Document context
      )
    {throw new system::NotImplementedException();} // TODO: verify parent reference.

    /**
      <summary>Gets/Sets the action to be performed when the annotation is activated.</summary>
    */
    public Action OnActivate
    {
      get
      {return parent.Action;}
      set
      {parent.Action = value;}
    }

    /**
      <summary>Gets/Sets the action to be performed when the cursor enters the annotation's active area.</summary>
    */
    public Action OnEnter
    {
      get
      {return this[PdfName.E];}
      set
      {this[PdfName.E] = value;}
    }

    /**
      <summary>Gets/Sets the action to be performed when the cursor exits the annotation's active area.</summary>
    */
    public Action OnExit
    {
      get
      {return this[PdfName.X];}
      set
      {this[PdfName.X] = value;}
    }

    /**
      <summary>Gets/Sets the action to be performed when the mouse button is pressed
      inside the annotation's active area.</summary>
    */
    public Action OnMouseDown
    {
      get
      {return this[PdfName.D];}
      set
      {this[PdfName.D] = value;}
    }

    /**
      <summary>Gets/Sets the action to be performed when the mouse button is released
      inside the annotation's active area.</summary>
    */
    public Action OnMouseUp
    {
      get
      {return this[PdfName.U];}
      set
      {this[PdfName.U] = value;}
    }

    /**
      <summary>Gets/Sets the action to be performed when the page containing the annotation is closed.</summary>
    */
    public Action OnPageClose
    {
      get
      {return this[PdfName.PC];}
      set
      {this[PdfName.PC] = value;}
    }

    /**
      <summary>Gets/Sets the action to be performed when the page containing the annotation
      is no longer visible in the viewer application's user interface.</summary>
    */
    public Action OnPageInvisible
    {
      get
      {return this[PdfName.PI];}
      set
      {this[PdfName.PI] = value;}
    }

    /**
      <summary>Gets/Sets the action to be performed when the page containing the annotation is opened.</summary>
    */
    public Action OnPageOpen
    {
      get
      {return this[PdfName.PO];}
      set
      {this[PdfName.PO] = value;}
    }

    /**
      <summary>Gets/Sets the action to be performed when the page containing the annotation
      becomes visible in the viewer application's user interface.</summary>
    */
    public Action OnPageVisible
    {
      get
      {return this[PdfName.PV];}
      set
      {this[PdfName.PV] = value;}
    }

    #region IDictionary
    public void Add(
      PdfName key,
      Action value
      )
    {BaseDataObject.Add(key,value.BaseObject);}

    public bool ContainsKey(
      PdfName key
      )
    {
      return BaseDataObject.ContainsKey(key)
        || (PdfName.A.Equals(key) && parent.BaseDataObject.ContainsKey(key));
    }

    public ICollection<PdfName> Keys
    {
      get
      {return BaseDataObject.Keys;}
    }

    public bool Remove(
      PdfName key
      )
    {
      if(PdfName.A.Equals(key) && parent.BaseDataObject.ContainsKey(key))
      {
        OnActivate = null;
        return true;
      }
      else
        return BaseDataObject.Remove(key);
    }

    public Action this[
      PdfName key
      ]
    {
      get
      {return Action.Wrap(BaseDataObject[key]);}
      set
      {BaseDataObject[key] = (value != null ? value.BaseObject : null);}
    }

    public bool TryGetValue(
      PdfName key,
      out Action value
      )
    {
      value = this[key];
      if(value == null)
        return ContainsKey(key);
      else
        return true;
    }

    public ICollection<Action> Values
    {
      get
      {
        List<Action> values;
        {
          ICollection<PdfDirectObject> objs = BaseDataObject.Values;
          values = new List<Action>(objs.Count);
          foreach(PdfDirectObject obj in objs)
          {values.Add(Action.Wrap(obj));}
          Action action = OnActivate;
          if(action != null)
          {values.Add(action);}
        }
        return values;
      }
    }

    #region ICollection
    void ICollection<KeyValuePair<PdfName,Action>>.Add(
      KeyValuePair<PdfName,Action> entry
      )
    {Add(entry.Key,entry.Value);}

    public void Clear(
      )
    {
      BaseDataObject.Clear();
      OnActivate = null;
    }

    bool ICollection<KeyValuePair<PdfName,Action>>.Contains(
      KeyValuePair<PdfName,Action> entry
      )
    {return entry.Value.BaseObject.Equals(BaseDataObject[entry.Key]);}

    public void CopyTo(
      KeyValuePair<PdfName,Action>[] entries,
      int index
      )
    {throw new system::NotImplementedException();}

    public int Count
    {
      get
      {return BaseDataObject.Count + (parent.BaseDataObject.ContainsKey(PdfName.A) ? 1 : 0);}
    }

    public bool IsReadOnly
    {
      get
      {return false;}
    }

    public bool Remove(
      KeyValuePair<PdfName,Action> entry
      )
    {
      return BaseDataObject.Remove(
        new KeyValuePair<PdfName,PdfDirectObject>(
          entry.Key,
          entry.Value.BaseObject
          )
        );
    }

    #region IEnumerable<KeyValuePair<PdfName,Action>>
    IEnumerator<KeyValuePair<PdfName,Action>> IEnumerable<KeyValuePair<PdfName,Action>>.GetEnumerator(
      )
    {
      foreach(PdfName key in Keys)
      {yield return new KeyValuePair<PdfName,Action>(key,this[key]);}
    }

    #region IEnumerable
    IEnumerator IEnumerable.GetEnumerator(
      )
    {return ((IEnumerable<KeyValuePair<PdfName,Action>>)this).GetEnumerator();}
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion
  }
}