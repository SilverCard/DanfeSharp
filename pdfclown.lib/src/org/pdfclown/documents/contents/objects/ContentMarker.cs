/*
  Copyright 2010-2011 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.objects;

using System;
using System.Collections.Generic;

namespace org.pdfclown.documents.contents.objects
{
  /**
    <summary>Abstract content marker [PDF:1.6:10.5].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public abstract class ContentMarker
    : Operation,
      IResourceReference<PropertyList>
  {
    #region dynamic
    #region constructors
    protected ContentMarker(
      PdfName tag
      ) : this(tag, null)
    {}

    protected ContentMarker(
      PdfName tag,
      PdfDirectObject properties
      ) : base(null, tag)
    {
      if(properties != null)
      {
        operands.Add(properties);
        operator_ = PropertyListOperator;
      }
      else
      {operator_ = SimpleOperator;}
    }

    protected ContentMarker(
      string operator_,
      IList<PdfDirectObject> operands
      ) : base(operator_, operands)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the private information meaningful to the program (application or plugin extension)
      creating the marked content.</summary>
      <param name="context">Content context.</param>
    */
    public PropertyList GetProperties(
      IContentContext context
      )
    {
      object properties = Properties;
      return properties is PdfName
        ? context.Resources.PropertyLists[(PdfName)properties]
        : (PropertyList)properties;
    }

    /**
      <summary>Gets/Sets the private information meaningful to the program (application or plugin
      extension) creating the marked content. It can be either an inline <see cref="PropertyList"/>
      or the <see cref="PdfName">name</see> of an external PropertyList resource.</summary>
    */
    public object Properties
    {
      get
      {
        PdfDirectObject propertiesObject = operands[1];
        if(propertiesObject == null)
          return null;
        else if(propertiesObject is PdfName)
          return propertiesObject;
        else if(propertiesObject is PdfDictionary)
          return PropertyList.Wrap(propertiesObject);
        else
          throw new NotSupportedException("Property list type unknown: " + propertiesObject.GetType().Name);
      }
      set
      {
        if(value == null)
        {
          operator_ = SimpleOperator;
          if(operands.Count > 1)
          {operands.RemoveAt(1);}
        }
        else
        {
          PdfDirectObject operand;
          if(value is PdfName)
          {operand = (PdfName)value;}
          else if(value is PropertyList)
          {operand = ((PropertyList)value).BaseDataObject;}
          else
            throw new ArgumentException("value MUST be a PdfName or a PropertyList.");

          operator_ = PropertyListOperator;
          if(operands.Count > 1)
          {operands[1] = operand;}
          else
          {operands.Add(operand);}
        }
      }
    }

    /**
      <summary>Gets/Sets the marker indicating the role or significance of the marked content.</summary>
    */
    public PdfName Tag
    {
      get
      {return (PdfName)operands[0];}
      set
      {operands[0] = value;}
    }

    #region IResourceReference
    public PropertyList GetResource(
      IContentContext context
      )
    {return GetProperties(context);}

    public PdfName Name
    {
      get
      {
        object properties = Properties;
        return (properties is PdfName ? (PdfName)properties : null);
      }
      set
      {Properties = value;}
    }
    #endregion
    #endregion

    #region protected
    protected abstract string PropertyListOperator
    {get;}

    protected abstract string SimpleOperator
    {get;}
    #endregion
    #endregion
    #endregion
  }
}