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

using org.pdfclown.bytes;
using org.pdfclown.files;
using org.pdfclown.tokens;

using System;

namespace org.pdfclown.objects
{
  /**
    <summary>PDF indirect reference object [PDF:1.6:3.2.9].</summary>
  */
  public sealed class PdfReference
    : PdfDirectObject,
      IPdfIndirectObject
  {
    #region dynamic
    #region fields
    private PdfIndirectObject indirectObject;

    private int objectNumber;

    private File file;
    private PdfObject parent;
    private bool updated;
    #endregion

    #region constructors
    internal PdfReference(
      PdfIndirectObject indirectObject
      )
    {this.indirectObject = indirectObject;}

    /**
      <remarks>
        <para>This is a necessary hack because indirect objects are unreachable on parsing bootstrap
        (see File(IInputStream) constructor).</para>
      </remarks>
    */
    internal PdfReference(
      FileParser.Reference reference,
      File file
      )
    {
      this.objectNumber = reference.ObjectNumber;
      this.file = file;
    }
    #endregion

    #region interface
    #region public
    public override PdfObject Accept(
      IVisitor visitor,
      object data
      )
    {return visitor.Visit(this, data);}

    public override int CompareTo(
      PdfDirectObject obj
      )
    {throw new NotImplementedException();}

    public override bool Equals(
      object @object
      )
    {
      return base.Equals(@object)
        || (@object != null
          && @object.GetType().Equals(GetType())
          && ((PdfReference)@object).Id.Equals(Id));
    }

    /**
      <summary>Gets the generation number.</summary>
    */
    public int GenerationNumber
    {
      get
      {return IndirectObject.XrefEntry.Generation;}
    }

    public override int GetHashCode(
      )
    {return IndirectObject.GetHashCode();}

    /**
      <summary>Gets the object identifier.</summary>
      <remarks>This corresponds to the serialized representation of an object identifier within a PDF file.</remarks>
    */
    public string Id
    {
      get
      {return ("" + ObjectNumber + Symbol.Space + GenerationNumber);}
    }

    /**
      <summary>Gets the object reference.</summary>
      <remarks>This corresponds to the serialized representation of a reference within a PDF file.</remarks>
    */
    public string IndirectReference
    {
      get
      {return (Id + Symbol.Space + Symbol.CapitalR);}
    }

    /**
      <summary>Gets the object number.</summary>
    */
    public int ObjectNumber
    {
      get
      {return IndirectObject.XrefEntry.Number;}
    }

    public override PdfObject Parent
    {
      get
      {return parent;}
      internal set
      {parent = value;}
    }

    public override PdfObject Swap(
      PdfObject other
      )
    {return IndirectObject.Swap(((PdfReference)other).IndirectObject).Reference;}

    public override string ToString(
      )
    {return IndirectReference;}

    public override bool Updateable
    {
      get
      {return IndirectObject.Updateable;}
      set
      {IndirectObject.Updateable = value;}
    }

    public override bool Updated
    {
      get
      {return updated;}
      protected internal set
      {updated = value;}
    }

    public override void WriteTo(
      IOutputStream stream,
      File context
      )
    {stream.Write(IndirectReference);}

    #region IPdfIndirectObject
    public PdfDataObject DataObject
    {
      get
      {return IndirectObject.DataObject;}
      set
      {IndirectObject.DataObject = value;}
    }

    public void Delete(
      )
    {IndirectObject.Delete();}

    public override PdfIndirectObject IndirectObject
    {
      get
      {
        if(indirectObject == null)
        {indirectObject = file.IndirectObjects[objectNumber];}

        return indirectObject;
      }
    }

    public override PdfReference Reference
    {
      get
      {return this;}
    }
    #endregion
    #endregion

    #region protected
    protected internal override bool Virtual
    {
      get
      {return IndirectObject.Virtual;}
      set
      {IndirectObject.Virtual = value;}
    }
    #endregion
    #endregion
    #endregion
  }
}