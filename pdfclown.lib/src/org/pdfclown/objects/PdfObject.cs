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

using org.pdfclown;
using org.pdfclown.bytes;
using org.pdfclown.files;

using System;

namespace org.pdfclown.objects
{
  /**
    <summary>Abstract PDF object.</summary>
  */
  public abstract class PdfObject
    : IVisitable
  {
    #region static
    #region interface
    #region public
    /**
      <summary>Gets the clone of the specified object, registered inside the specified file context.</summary>
      <param name="object">Object to clone into the specified file context.</param>
      <param name="context">File context of the cloning.</param>
    */
    public static PdfObject Clone(
      PdfObject @object,
      File context
      )
    {return @object == null ? null : @object.Clone(context);}

    /**
      <summary>Ensures an indirect reference to be resolved into its corresponding data object.</summary>
      <param name="object">Object to resolve.</param>
    */
    public static PdfDataObject Resolve(
      PdfObject @object
      )
    {return @object == null ? null : @object.Resolve();}

    /**
      <summary>Ensures a data object to be unresolved into its corresponding indirect reference, if
      available.</summary>
      <param name="object">Object to unresolve.</param>
      <returns><see cref="PdfReference"/>, if available; <code>object</code>, otherwise.</returns>
    */
    public static PdfDirectObject Unresolve(
      PdfDataObject @object
      )
    {return @object == null ? null : @object.Unresolve();}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    protected PdfObject(
      )
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Creates a shallow copy of this object.</summary>
    */
    public object Clone(
      )
    {
      PdfObject clone = (PdfObject)MemberwiseClone();
      clone.Parent = null;
      return clone;
    }

    /**
      <summary>Creates a deep copy of this object within the specified file context.</summary>
    */
    public virtual PdfObject Clone(
      File context
      )
    {return Accept(context.Cloner, null);}

    /**
      <summary>Gets the indirect object containing this object.</summary>
      <seealso cref="DataContainer"/>
      <seealso cref="IndirectObject"/>
    */
    public virtual PdfIndirectObject Container
    {
      get
      {
        PdfObject parent = Parent;
        return parent != null ? parent.Container : null;
      }
    }

    /**
      <summary>Gets the indirect object containing the data associated to this object.</summary>
      <seealso cref="Container"/>
      <seealso cref="IndirectObject"/>
    */
    public PdfIndirectObject DataContainer
    {
      get
      {
        PdfIndirectObject indirectObject = IndirectObject;
        return indirectObject != null ? indirectObject : Container;
      }
    }

    /**
      <summary>Gets the file containing this object.</summary>
    */
    public virtual File File
    {
      get
      {
        PdfIndirectObject dataContainer = DataContainer;
        return dataContainer != null ? dataContainer.File : null;
      }
    }

    /**
      <summary>Gets the indirect object corresponding to this object.</summary>
      <seealso cref="Container"/>
      <seealso cref="DataContainer"/>
    */
    public virtual PdfIndirectObject IndirectObject
    {
      get
      {return Parent as PdfIndirectObject;}
    }

    /**
      <summary>Gets/Sets the parent of this object.</summary>
      <seealso cref="Container"/>
    */
    public abstract PdfObject Parent
    {
      get;
      internal set;
    }

    /**
      <summary>Gets the indirect reference of this object.</summary>
    */
    public virtual PdfReference Reference
    {
      get
      {
        PdfIndirectObject indirectObject = IndirectObject;
        return indirectObject != null ? indirectObject.Reference : null;
      }
    }

    /**
      <summary>Ensures this object to be resolved into its corresponding data object.</summary>
      <seealso cref="Unresolve()"/>
    */
    public PdfDataObject Resolve(
      )
    {return this is IPdfIndirectObject ? ((IPdfIndirectObject)this).DataObject : (PdfDataObject)this;}

    /**
      <summary>Swaps contents between this object and the other one.</summary>
      <param name="other">Object whose contents have to be swapped with this one's.</param>
      <returns>This object.</returns>
    */
    public abstract PdfObject Swap(
      PdfObject other
      );

    /**
      <summary>Ensures this object to be unresolved into its corresponding indirect reference, if
      available.</summary>
      <returns><see cref="PdfReference"/>, if available; <code>this</code>, otherwise.</returns>
      <seealso cref="Resolve()"/>
    */
    public PdfDirectObject Unresolve(
      )
    {
      PdfReference reference = Reference;
      return reference != null ? reference : (PdfDirectObject)this;
    }
  
    /**
      <summary>Gets/Sets whether the detection of object state changes is enabled.</summary>
    */
    public abstract bool Updateable
    {
      get;
      set;
    }

    /**
      <summary>Gets/Sets whether the initial state of this object has been modified.</summary>
    */
    public abstract bool Updated
    {
      get;
      protected internal set;
    }

    /**
      <summary>Serializes this object to the specified stream.</summary>
      <param name="stream">Target stream.</param>
      <param name="context">File context.</param>
    */
    public abstract void WriteTo(
      IOutputStream stream,
      File context
      );

    #region IVisitable
    public abstract PdfObject Accept(
      IVisitor visitor,
      object data
      );
    #endregion
    #endregion

    #region protected
    /**
      <summary>Updates the state of this object.</summary>
    */
    protected internal void Update(
      )
    {
      if(!Updateable || Updated)
        return;

      Updated = true;
      Virtual = false;

      // Propagate the update to the ascendants!
      if(Parent != null)
      {Parent.Update();}
    }

    /**
      <summary>Gets/Sets whether this object acts like a null-object placeholder.</summary>
    */
    protected internal abstract bool Virtual
    {
      get;
      set;
    }
    #endregion

    #region internal
    /**
      <summary>Ensures that the specified object is decontextualized from this object.</summary>
      <param name="obj">Object to decontextualize from this object.</param>
      <seealso cref="Include(PdfDataObject)"/>
    */
    internal void Exclude(
      PdfDataObject obj
      )
    {
      if(obj != null)
      {obj.Parent = null;}
    }

    /**
      <summary>Ensures that the specified object is contextualized into this object.</summary>
      <param name="obj">Object to contextualize into this object; if it is already contextualized
        into another object, it will be cloned to preserve its previous association.</param>
      <returns>Contextualized object.</returns>
      <seealso cref="Exclude(PdfDataObject)"/>
    */
    internal PdfDataObject Include(
      PdfDataObject obj
      )
    {
      if(obj != null)
      {
        if(obj.Parent != null)
        {obj = (PdfDataObject)obj.Clone();}
        obj.Parent = this;
      }
      return obj;
    }
    #endregion
    #endregion
    #endregion
  }
}