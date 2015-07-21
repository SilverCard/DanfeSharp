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
using org.pdfclown.util;

using System;
using System.Collections.Generic;

namespace org.pdfclown.documents.contents.layers
{
  /**
    <summary>Layer entity.</summary>
  */
  [PDF(VersionEnum.PDF15)]
  public abstract class LayerEntity
    : PropertyList
  {
    #region types
    /**
      <summary>Membership visibility policy [PDF:1.7:4.10.1].</summary>
    */
    public enum VisibilityPolicyEnum
    {
      /**
        <summary>Visible only if all of the visibility layers are ON.</summary>
      */
      AllOn,
      /**
        <summary>Visible if any of the visibility layers are ON.</summary>
      */
      AnyOn,
      /**
        <summary>Visible if any of the visibility layers are OFF.</summary>
      */
      AnyOff,
      /**
        <summary>Visible only if all of the visibility layers are OFF.</summary>
      */
      AllOff
    }
    #endregion

    #region dynamic
    #region constructors
    protected LayerEntity(
      Document context,
      PdfName typeName
      ) : base(
        context,
        new PdfDictionary(
          new PdfName[]
          {PdfName.Type},
          new PdfDirectObject[]
          {typeName}
        ))
    {}

    protected LayerEntity(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the default membership.</summary>
      <remarks>This collection corresponds to the hierarchical relation between this layer entity
      and its ascendants.</remarks>
    */
    public virtual LayerMembership Membership
    {
      get
      {return null;}
    }

    /**
      <summary>Gets the layers whose states determine the visibility of content controlled by this
      entity.</summary>
    */
    public virtual IList<Layer> VisibilityLayers
    {
      get
      {return null;}
    }

    /**
      <summary>Gets/Sets the visibility policy of this entity.</summary>
    */
    public virtual VisibilityPolicyEnum VisibilityPolicy
    {
      get
      {return VisibilityPolicyEnum.AllOn;}
      set
      {}
    }
    #endregion
    #endregion
    #endregion
  }

  internal static class VisibilityPolicyEnumExtension
  {
    private static readonly BiDictionary<LayerMembership.VisibilityPolicyEnum,PdfName> codes;

    static VisibilityPolicyEnumExtension()
    {
      codes = new BiDictionary<LayerMembership.VisibilityPolicyEnum,PdfName>();
      codes[LayerMembership.VisibilityPolicyEnum.AllOn] = PdfName.AllOn;
      codes[LayerMembership.VisibilityPolicyEnum.AnyOn] = PdfName.AnyOn;
      codes[LayerMembership.VisibilityPolicyEnum.AnyOff] = PdfName.AnyOff;
      codes[LayerMembership.VisibilityPolicyEnum.AllOff] = PdfName.AllOff;
    }

    public static LayerMembership.VisibilityPolicyEnum Get(
      PdfName name
      )
    {
      if(name == null)
        return LayerMembership.VisibilityPolicyEnum.AnyOn;

      LayerMembership.VisibilityPolicyEnum? visibilityPolicy = codes.GetKey(name);
      if(!visibilityPolicy.HasValue)
        throw new NotSupportedException("Visibility policy unknown: " + name);

      return visibilityPolicy.Value;
    }

    public static PdfName GetName(
      this LayerMembership.VisibilityPolicyEnum visibilityPolicy
      )
    {return codes[visibilityPolicy];}
  }
}