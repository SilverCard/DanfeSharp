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

namespace org.pdfclown.documents.contents.layers
{
  /**
    <summary>Optional content properties [PDF:1.7:4.10.3].</summary>
  */
  [PDF(VersionEnum.PDF15)]
  public class LayerDefinition
    : PdfObjectWrapper<PdfDictionary>,
      ILayerConfiguration
  {
    #region static
    #region interface
    #region public
    public static LayerDefinition Wrap(
      PdfDirectObject baseObject
      )
    {return baseObject != null ? new LayerDefinition(baseObject) : null;}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public LayerDefinition(
      Document context
      ) : base(context, new PdfDictionary())
    {Initialize();}

    private LayerDefinition(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {Initialize();}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the layer configurations used under particular circumstances.</summary>
    */
    public Array<LayerConfiguration> AlternateConfigurations
    {
      get
      {return Array<LayerConfiguration>.Wrap<LayerConfiguration>(BaseDataObject.Get<PdfArray>(PdfName.Configs));}
      set
      {BaseDataObject[PdfName.Configs] = value.BaseObject;}
    }

    /**
      <summary>Gets the default layer configuration, that is the initial state of the optional
      content groups when a document is first opened.</summary>
    */
    public LayerConfiguration DefaultConfiguration
    {
      get
      {return new LayerConfiguration(BaseDataObject[PdfName.D]);}
      set
      {BaseDataObject[PdfName.D] = value.BaseObject;}
    }

    #region ILayerConfiguration
    public string Creator
    {
      get
      {return DefaultConfiguration.Creator;}
      set
      {DefaultConfiguration.Creator = value;}
    }

    public Layers Layers
    {
      get
      {return DefaultConfiguration.Layers;}
      set
      {DefaultConfiguration.Layers = value;}
    }

    public ListModeEnum ListMode
    {
      get
      {return DefaultConfiguration.ListMode;}
      set
      {DefaultConfiguration.ListMode = value;}
    }

    public Array<LayerGroup> OptionGroups
    {
      get
      {return DefaultConfiguration.OptionGroups;}
    }

    public string Title
    {
      get
      {return DefaultConfiguration.Title;}
      set
      {DefaultConfiguration.Title = value;}
    }

    public bool? Visible
    {
      get
      {return DefaultConfiguration.Visible;}
      set
      {DefaultConfiguration.Visible = value;}
    }
    #endregion
    #endregion

    #region internal
    /**
      <summary>Gets the collection of all the layer objects in the document.</summary>
    */
    /*
     * TODO: manage layer removal from file (unregistration) -- attach a removal listener
     * to the IndirectObjects collection: anytime a PdfDictionary with Type==PdfName.OCG is removed,
     * that listener MUST update this collection.
     * Listener MUST be instantiated when LayerDefinition is associated to the document.
     */
    internal PdfArray AllLayersObject
    {
      get
      {return (PdfArray)BaseDataObject.Resolve(PdfName.OCGs);}
    }
    #endregion

    #region private
    private void Initialize(
      )
    {
      PdfDictionary baseDataObject = BaseDataObject;
      if(baseDataObject.Count == 0)
      {
        baseDataObject.Updateable = false;
        baseDataObject[PdfName.OCGs] = new PdfArray();
        baseDataObject[PdfName.D] = new LayerConfiguration(Document).BaseObject;
        //TODO: as this is optional, verify whether it can be lazily instantiated later.
        DefaultConfiguration.Layers = new Layers(Document);
        baseDataObject.Updateable = true;
      }
    }
    #endregion
    #endregion
    #endregion
  }
}

