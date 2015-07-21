/*
  Copyright 2010-2012 Stefano Chizzolini. http://www.pdfclown.org

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

using System.Collections.Generic;

namespace org.pdfclown.documents.contents.colorSpaces
{
  /**
    <summary>Color value defined by numeric-level components.</summary>
  */
  public abstract class LeveledColor
    : Color
  {
    #region dynamic
    #region constructors
    protected LeveledColor(
      ColorSpace colorSpace,
      PdfDirectObject baseObject
      ) : base(colorSpace, baseObject)
    {}
    #endregion

    #region interface
    #region public
    public sealed override IList<PdfDirectObject> Components
    {
      get
      {return BaseArray;}
    }

    public override bool Equals(
      object obj
      )
    {
      if(obj == null
        || !obj.GetType().Equals(GetType()))
        return false;

      IEnumerator<PdfDirectObject> objectIterator = ((LeveledColor)obj).BaseArray.GetEnumerator();
      IEnumerator<PdfDirectObject> thisIterator = BaseArray.GetEnumerator();
      while(thisIterator.MoveNext())
      {
        objectIterator.MoveNext();
        if(!thisIterator.Current.Equals(objectIterator.Current))
          return false;
      }
      return true;
    }

    public sealed override int GetHashCode(
      )
    {
      int hashCode = 0;
      foreach(PdfDirectObject component in BaseArray)
      {hashCode ^= component.GetHashCode();}
      return hashCode;
    }
    #endregion

    #region protected
    /*
      NOTE: This is a workaround to the horrible lack of covariance support in C# 3 which forced me
      to flatten type parameters at top hierarchy level (see Java implementation). Anyway, suggestions
      to overcome this issue are welcome!
    */
    protected PdfArray BaseArray
    {
      get
      {return (PdfArray)BaseDataObject;}
    }

    /**
      <summary>Gets the specified color component.</summary>
      <param name="index">Component index.</param>
    */
    protected double GetComponentValue(
      int index
      )
    {return ((IPdfNumber)Components[index]).RawValue;}

    protected void SetComponentValue(
      int index,
      double value
      )
    {Components[index] = PdfReal.Get(NormalizeComponent(value));}
    #endregion
    #endregion
    #endregion
  }
}