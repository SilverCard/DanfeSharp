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
using org.pdfclown.documents.interaction.annotations;
using org.pdfclown.files;
using org.pdfclown.objects;

using System;
using System.Collections;
using System.Collections.Generic;

namespace org.pdfclown.documents.interaction.forms
{
  /**
    <summary>Field widget annotations [PDF:1.6:8.6].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class FieldWidgets
    : PdfObjectWrapper<PdfDataObject>,
    IList<Widget>
  {
    /*
      NOTE: Widget annotations may be singular (either merged to their field or within an array)
      or multiple (within an array).
      This implementation hides such a complexity to the user, smoothly exposing just the most
      general case (array) yet preserving its internal state.
    */
    #region dynamic
    #region fields
    private Field field;
    #endregion

    #region constructors
    internal FieldWidgets(
      PdfDirectObject baseObject,
      Field field
      ) : base(baseObject)
    {this.field = field;}
    #endregion

    #region interface
    #region public
    public override object Clone(
      Document context
      )
    {throw new NotImplementedException();} // TODO:verify field reference.

    /**
      <summary>Gets the field associated to these widgets.</summary>
    */
    public Field Field
    {
      get
      {return field;}
    }

    #region IList
    public int IndexOf(
      Widget value
      )
    {
      PdfDataObject baseDataObject = BaseDataObject;
      if(baseDataObject is PdfDictionary) // Single annotation.
      {
        if(value.BaseObject.Equals(BaseObject))
          return 0;
        else
          return -1;
      }

      return ((PdfArray)baseDataObject).IndexOf(value.BaseObject);
    }

    public void Insert(
      int index,
      Widget value
      )
    {EnsureArray().Insert(index,value.BaseObject);}

    public void RemoveAt(
      int index
      )
    {EnsureArray().RemoveAt(index);}

    public Widget this[
      int index
      ]
    {
      get
      {
        PdfDataObject baseDataObject = BaseDataObject;
        if(baseDataObject is PdfDictionary) // Single annotation.
        {
          if(index != 0)
            throw new ArgumentException("Index: " + index + ", Size: 1");

          return NewWidget(BaseObject);
        }

        return NewWidget(((PdfArray)baseDataObject)[index]);
      }
      set
      {EnsureArray()[index] = value.BaseObject;}
    }

    #region ICollection
    public void Add(
      Widget value
      )
    {
      value.BaseDataObject[PdfName.Parent] = field.BaseObject;

      EnsureArray().Add(value.BaseObject);
    }

    public void Clear(
      )
    {EnsureArray().Clear();}

    public bool Contains(
      Widget value
      )
    {
      PdfDataObject baseDataObject = BaseDataObject;
      if(baseDataObject is PdfDictionary) // Single annotation.
        return value.BaseObject.Equals(BaseObject);

      return ((PdfArray)baseDataObject).Contains(value.BaseObject);
    }

    public void CopyTo(
      Widget[] values,
      int index
      )
    {throw new NotImplementedException();}

    public int Count
    {
      get
      {
        PdfDataObject baseDataObject = BaseDataObject;
        if(baseDataObject is PdfDictionary) // Single annotation.
          return 1;

        return ((PdfArray)baseDataObject).Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {return false;}
    }

    public bool Remove(
      Widget value
      )
    {return EnsureArray().Remove(value.BaseObject);}

    #region IEnumerable<Widget>
    IEnumerator<Widget> IEnumerable<Widget>.GetEnumerator(
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
    {return ((IEnumerable<Widget>)this).GetEnumerator();}
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
      if(baseDataObject is PdfDictionary) // Merged annotation.
      {
        PdfArray widgetsArray = new PdfArray();
        {
          PdfDictionary fieldDictionary = (PdfDictionary)baseDataObject;
          PdfDictionary widgetDictionary = null;
          // Extracting widget entries from the field...
          foreach(PdfName key in new List<PdfName>(fieldDictionary.Keys))
          {
            // Is it a widget entry?
            if(key.Equals(PdfName.Type)
              || key.Equals(PdfName.Subtype)
              || key.Equals(PdfName.Rect)
              || key.Equals(PdfName.Contents)
              || key.Equals(PdfName.P)
              || key.Equals(PdfName.NM)
              || key.Equals(PdfName.M)
              || key.Equals(PdfName.F)
              || key.Equals(PdfName.BS)
              || key.Equals(PdfName.AP)
              || key.Equals(PdfName.AS)
              || key.Equals(PdfName.Border)
              || key.Equals(PdfName.C)
              || key.Equals(PdfName.A)
              || key.Equals(PdfName.AA)
              || key.Equals(PdfName.StructParent)
              || key.Equals(PdfName.OC)
              || key.Equals(PdfName.H)
              || key.Equals(PdfName.MK))
            {
              if(widgetDictionary == null)
              {
                widgetDictionary = new PdfDictionary();
                PdfReference widgetReference = File.Register(widgetDictionary);

                // Remove the field from the page annotations (as the widget annotation is decoupled from it)!
                PdfArray pageAnnotationsArray = (PdfArray)((PdfDictionary)fieldDictionary.Resolve(PdfName.P)).Resolve(PdfName.Annots);
                pageAnnotationsArray.Remove(field.BaseObject);

                // Add the widget to the page annotations!
                pageAnnotationsArray.Add(widgetReference);
                // Add the widget to the field widgets!
                widgetsArray.Add(widgetReference);
                // Associate the field to the widget!
                widgetDictionary[PdfName.Parent] = field.BaseObject;
              }

              // Transfer the entry from the field to the widget!
              widgetDictionary[key] = fieldDictionary[key];
              fieldDictionary.Remove(key);
            }
          }
        }
        BaseObject = widgetsArray;
        field.BaseDataObject[PdfName.Kids] = widgetsArray;

        baseDataObject = widgetsArray;
      }

      return (PdfArray)baseDataObject;
    }

    private Widget NewWidget(
      PdfDirectObject baseObject
      )
    {return Widget.Wrap(baseObject, field);}
    #endregion
    #endregion
    #endregion
  }
}