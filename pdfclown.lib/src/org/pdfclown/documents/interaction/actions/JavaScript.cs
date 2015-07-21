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

using bytes = org.pdfclown.bytes;
using org.pdfclown.documents;
using org.pdfclown.objects;

using System;

namespace org.pdfclown.documents.interaction.actions
{
  /**
    <summary>'Cause a script to be compiled and executed by the JavaScript interpreter'
    action [PDF:1.6:8.6.4].</summary>
  */
  [PDF(VersionEnum.PDF13)]
  public sealed class JavaScript
    : Action
  {
    #region static
    #region interface
    #region internal
    /**
      <summary>Gets the Javascript script from the specified base data object.</summary>
    */
    internal static string GetScript(
      PdfDictionary baseDataObject,
      PdfName key
      )
    {
      PdfDataObject scriptObject = baseDataObject.Resolve(key);
      if(scriptObject == null)
        return null;
      else if(scriptObject is PdfTextString)
        return ((PdfTextString)scriptObject).StringValue;
      else
      {
        bytes::IBuffer scriptBuffer = ((PdfStream)scriptObject).Body;
        return scriptBuffer.GetString(0,(int)scriptBuffer.Length);
      }
    }

    /**
      <summary>Sets the Javascript script into the specified base data object.</summary>
    */
    internal static void SetScript(
      PdfDictionary baseDataObject,
      PdfName key,
      string value
      )
    {
      PdfDataObject scriptObject = baseDataObject.Resolve(key);
      if(!(scriptObject is PdfStream) && value.Length > 256)
      {baseDataObject[key] = baseDataObject.File.Register(scriptObject = new PdfStream());}
      // Insert the script!
      if(scriptObject is PdfStream)
      {
        bytes::IBuffer scriptBuffer = ((PdfStream)scriptObject).Body;
        scriptBuffer.SetLength(0);
        scriptBuffer.Append(value);
      }
      else
      {baseDataObject[key] = new PdfTextString(value);}
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    /**
      <summary>Creates a new action within the given document context.</summary>
    */
    public JavaScript(
      Document context,
      string script
      ) : base(context, PdfName.JavaScript)
    {Script = script;}

    internal JavaScript(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the JavaScript script to be executed.</summary>
    */
    public string Script
    {
      get
      {return GetScript(BaseDataObject, PdfName.JS);}
      set
      {SetScript(BaseDataObject, PdfName.JS, value);}
    }

    #region IPdfNamedObjectWrapper
    public PdfString Name
    {
      get
      {return RetrieveName();}
    }

    public PdfDirectObject NamedBaseObject
    {
      get
      {return RetrieveNamedBaseObject();}
    }
    #endregion
    #endregion
    #endregion
    #endregion
  }
}