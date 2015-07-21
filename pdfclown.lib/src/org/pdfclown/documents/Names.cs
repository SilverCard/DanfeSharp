/*
  Copyright 2007-2012 Stefano Chizzolini. http://www.pdfclown.org

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

using org.pdfclown.documents.files;
using org.pdfclown.documents.interaction.actions;
using org.pdfclown.documents.interaction.navigation.document;
using org.pdfclown.documents.multimedia;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace org.pdfclown.documents
{
  /**
    <summary>Name dictionary [PDF:1.6:3.6.3].</summary>
  */
  [PDF(VersionEnum.PDF12)]
  public sealed class Names
    : PdfObjectWrapper<PdfDictionary>,
      ICompositeDictionary<PdfString>
  {
    #region dynamic
    #region constructors
    public Names(
      Document context
      ) : base(context, new PdfDictionary())
    {}

    internal Names(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the named destinations.</summary>
    */
    [PDF(VersionEnum.PDF12)]
    public NamedDestinations Destinations
    {
      get
      {return new NamedDestinations(BaseDataObject.Get<PdfDictionary>(PdfName.Dests, false));}
      set
      {BaseDataObject[PdfName.Dests] = value.BaseObject;}
    }

    /**
      <summary>Gets/Sets the named embedded files.</summary>
    */
    [PDF(VersionEnum.PDF14)]
    public NamedEmbeddedFiles EmbeddedFiles
    {
      get
      {return new NamedEmbeddedFiles(BaseDataObject.Get<PdfDictionary>(PdfName.EmbeddedFiles, false));}
      set
      {BaseDataObject[PdfName.EmbeddedFiles] = value.BaseObject;}
    }

    /**
      <summary>Gets/Sets the named JavaScript actions.</summary>
    */
    [PDF(VersionEnum.PDF13)]
    public NamedJavaScripts JavaScripts
    {
      get
      {return new NamedJavaScripts(BaseDataObject.Get<PdfDictionary>(PdfName.JavaScript, false));}
      set
      {BaseDataObject[PdfName.JavaScript] = value.BaseObject;}
    }

    /**
      <summary>Gets/Sets the named renditions.</summary>
    */
    [PDF(VersionEnum.PDF15)]
    public NamedRenditions Renditions
    {
      get
      {return new NamedRenditions(BaseDataObject.Get<PdfDictionary>(PdfName.Renditions, false));}
      set
      {BaseDataObject[PdfName.Renditions] = value.BaseObject;}
    }

    #region ICompositeDictionary
    public PdfObjectWrapper Get(
      Type type
      )
    {
      if(typeof(Destination).IsAssignableFrom(type))
        return Destinations;
      else if(typeof(FileSpecification).IsAssignableFrom(type))
        return EmbeddedFiles;
      else if(typeof(JavaScript).IsAssignableFrom(type))
        return JavaScripts;
      else if(typeof(Rendition).IsAssignableFrom(type))
        return Renditions;
      else
        return null;
    }

    public PdfObjectWrapper Get(
      Type type,
      PdfString key
      )
    {return (PdfObjectWrapper)type.GetMethod("get_Item", BindingFlags.Public | BindingFlags.Instance).Invoke(Get(type), new object[]{ key });}
    #endregion
    #endregion
    #endregion
    #endregion
  }
}