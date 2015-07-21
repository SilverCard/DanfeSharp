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

using org.pdfclown.documents;
using org.pdfclown.documents.interchange.metadata;
using org.pdfclown.files;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace org.pdfclown.objects
{
  /**
    <summary>Base high-level representation of a weakly-typed PDF object.</summary>
  */
  public abstract class PdfObjectWrapper
    : IPdfObjectWrapper
  {
    #region static
    #region interface
    #region public
    /**
      <summary>Gets the PDF object backing the specified wrapper.</summary>
      <param name="wrapper">Object to extract the base from.</param>
    */
    public static PdfDirectObject GetBaseObject(
      PdfObjectWrapper wrapper
      )
    {return (wrapper != null ? wrapper.BaseObject : null);}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region fields
    private PdfDirectObject baseObject;
    #endregion

    #region constructors
    /**
      <summary>Instantiates an empty wrapper.</summary>
    */
    protected PdfObjectWrapper(
      )
    {}

    /**
      <summary>Instantiates a wrapper from the specified base object.</summary>
      <param name="baseObject">PDF object backing this wrapper. MUST be a <see cref="PdfReference"/>
      every time available.</param>
    */
    protected PdfObjectWrapper(
      PdfDirectObject baseObject
      )
    {BaseObject = baseObject;}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets a clone of the object, registered inside the given document context.</summary>
      <param name="context">Which document the clone has to be registered in.</param>
    */
    public virtual object Clone(
      Document context
      )
    {
      PdfObjectWrapper clone = (PdfObjectWrapper)base.MemberwiseClone();
      clone.BaseObject = (PdfDirectObject)BaseObject.Clone(context.File);
      return clone;
    }

    /**
      <summary>Gets the indirect object containing the base object.</summary>
    */
    public PdfIndirectObject Container
    {
      get
      {return baseObject.Container;}
    }

    /**
      <summary>Gets the indirect object containing the base data object.</summary>
    */
    public PdfIndirectObject DataContainer
    {
      get
      {return baseObject.DataContainer;}
    }

    /**
      <summary>Removes the object from its document context.</summary>
      <remarks>The object is no more usable after this method returns.</remarks>
      <returns>Whether the object was actually decontextualized (only indirect objects can be
      decontextualize).</returns>
    */
    public virtual bool Delete(
      )
    {
      // Is the object indirect?
      if(baseObject is PdfReference) // Indirect object.
      {
        ((PdfReference)baseObject).Delete();
        return true;
      }
      else // Direct object.
      {return false;}
    }

    /**
      <summary>Gets the document context.</summary>
    */
    public Document Document
    {
      get
      {
        File file = File;
        return file != null ? file.Document : null;
      }
    }

    public override bool Equals(
      object obj
      )
    {
      return obj != null
        && obj.GetType().Equals(GetType())
        && ((PdfObjectWrapper)obj).baseObject.Equals(baseObject);
    }

    /**
      <summary>Gets the file context.</summary>
    */
    public File File
    {
      get
      {return baseObject.File;}
    }

    public override int GetHashCode(
      )
    {return baseObject.GetHashCode();}

    #region IPdfObjectWrapper
    public virtual PdfDirectObject BaseObject
    {
      get
      {return baseObject;}
      protected set
      {baseObject = value;}
    }
    #endregion
    #endregion

    #region protected
    /**
      <summary>Checks whether the specified feature is compatible with the
        <see cref="Document.Version">document's conformance version</see>.</summary>
      <param name="feature">Entity whose compatibility has to be checked. Supported types:
        <list type="bullet">
          <item><see cref="VersionEnum"/></item>
          <item><see cref="string">Property name</see> resolvable to an <see cref="MemberInfo">annotated getter method</see></item>
          <item><see cref="MemberInfo"/></item>
        </list>
      </param>
    */
    protected void CheckCompatibility(
      object feature
      )
    {
      /*
        TODO: Caching!
      */
      Document.ConfigurationImpl.CompatibilityModeEnum compatibilityMode = Document.Configuration.CompatibilityMode;
      if(compatibilityMode == Document.ConfigurationImpl.CompatibilityModeEnum.Passthrough) // No check required.
        return;

      if(feature is Enum)
      {
        Type enumType = feature.GetType();
        if(enumType.GetCustomAttributes(typeof(FlagsAttribute),true).Length > 0)
        {
          int featureEnumValues = Convert.ToInt32(feature);
          List<Enum> featureEnumItems = new List<Enum>();
          foreach (int enumValue in Enum.GetValues(enumType))
          {
            if((featureEnumValues & enumValue) == enumValue)
            {featureEnumItems.Add((Enum)Enum.ToObject(enumType, enumValue));}
          }
          if(featureEnumItems.Count > 1)
          {feature = featureEnumItems;}
        }
      }
      if(feature is ICollection)
      {
        foreach(Object featureItem in (ICollection)feature)
        {CheckCompatibility(featureItem);}
        return;
      }

      Version featureVersion;
      if(feature is VersionEnum) // Explicit version.
      {featureVersion = ((VersionEnum)feature).GetVersion();}
      else // Implicit version (element annotation).
      {
        PDF annotation;
        {
          if(feature is string) // Property name.
          {feature = GetType().GetProperty((string)feature);}
          else if(feature is Enum) // Enum constant.
          {feature = feature.GetType().GetField(feature.ToString());}
          if(!(feature is MemberInfo))
            throw new ArgumentException("Feature type '" + feature.GetType().Name + "' not supported.");

          while(true)
          {
            object[] annotations = ((MemberInfo)feature).GetCustomAttributes(typeof(PDF),true);
            if(annotations.Length > 0)
            {
              annotation = (PDF)annotations[0];
              break;
            }

            feature = ((MemberInfo)feature).DeclaringType;
            if(feature == null) // Element hierarchy walk complete.
              return; // NOTE: As no annotation is available, we assume the feature has no specific compatibility requirements.
          }
        }
        featureVersion = annotation.Value.GetVersion();
      }
      // Is the feature version compatible?
      if(Document.Version.CompareTo(featureVersion) >= 0)
        return;

      // The feature version is NOT compatible: how to solve the conflict?
      switch(compatibilityMode)
      {
        case Document.ConfigurationImpl.CompatibilityModeEnum.Loose: // Accepts the feature version.
          // Synchronize the document version!
          Document.Version = featureVersion;
          break;
        case  Document.ConfigurationImpl.CompatibilityModeEnum.Strict: // Refuses the feature version.
          // Throw a violation to the document version!
          throw new Exception("Incompatible feature (version " + featureVersion + " was required against document version " + Document.Version);
        default:
          throw new NotImplementedException("Unhandled compatibility mode: " + compatibilityMode);
      }
    }

    /**
      <summary>Retrieves the name possibly associated to this object, walking through the document's
      name dictionary.</summary>
    */
    protected virtual PdfString RetrieveName(
      )
    {
      object names = Document.Names.Get(GetType());
      if(names == null)
        return null;

      /*
        NOTE: Due to variance issues, we have to go the reflection way (gosh!).
      */
      return (PdfString)names.GetType().GetMethod("GetKey").Invoke(names, new object[]{ this });
    }

    /**
      <summary>Retrieves the object name, if available; otherwise, behaves like
      <see cref="PdfObjectWrapper.BaseObject"/>.</summary>
    */
    protected PdfDirectObject RetrieveNamedBaseObject(
      )
    {
      PdfString name = RetrieveName();
      return name != null ? name : BaseObject;
    }
    #endregion
    #endregion
    #endregion
  }

  /**
    <summary>High-level representation of a strongly-typed PDF object.</summary>
    <remarks>
      <para>Specialized objects don't inherit directly from their low-level counterparts (e.g.
        <see cref="org.pdfclown.documents.contents.Contents">Contents</see> extends <see
        cref="org.pdfclown.objects.PdfStream">PdfStream</see>, <see
        cref="org.pdfclown.documents.Pages">Pages</see> extends <see
        cref="org.pdfclown.objects.PdfArray">PdfArray</see> and so on) because there's no plain
        one-to-one mapping between primitive PDF types and specialized instances: the
        <code>Content</code> entry of <code>Page</code> dictionaries may be a simple reference to a
        <code>PdfStream</code> or a <code>PdfArray</code> of references to <code>PdfStream</code>s,
        <code>Pages</code> collections may be spread across a B-tree instead of a flat
        <code>PdfArray</code> and so on.
      </para>
      <para>So, in order to hide all these annoying inner workings, I chose to adopt a composition
        pattern instead of the apparently-reasonable (but actually awkward!) inheritance pattern.
        Nonetheless, users can navigate through the low-level structure getting the <see
        cref="BaseDataObject">BaseDataObject</see> backing this object.
      </para>
    </remarks>
  */
  public abstract class PdfObjectWrapper<TDataObject>
    : PdfObjectWrapper
    where TDataObject : PdfDataObject
  {
    #region dynamic
    #region constructors
    /**
      <summary>Instantiates an empty wrapper.</summary>
    */
    protected PdfObjectWrapper(
      )
    {}

    /**
      <summary>Instantiates a wrapper from the specified base object.</summary>
      <param name="baseObject">PDF object backing this wrapper. MUST be a <see cref="PdfReference"/>
      every time available.</param>
    */
    protected PdfObjectWrapper(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}

    /**
      <summary>Instantiates a wrapper registering the specified base data object into the specified
      document context.</summary>
      <param name="context">Document context into which the specified data object has to be
      registered.</param>
      <param name="baseDataObject">PDF data object backing this wrapper.</param>
      <seealso cref="PdfObjectWrapper(File, PdfDataObject)"/>
    */
    protected PdfObjectWrapper(
      Document context,
      TDataObject baseDataObject
      ) : this(context.File, baseDataObject)
    {}

    /**
      <summary>Instantiates a wrapper registering the specified base data object into the specified
      file context.</summary>
      <param name="context">File context into which the specified data object has to be registered.
      </param>
      <param name="baseDataObject">PDF data object backing this wrapper.</param>
      <seealso cref="PdfObjectWrapper(Document, PdfDataObject)"/>
    */
    protected PdfObjectWrapper(
      File context,
      TDataObject baseDataObject
      ) : this(context.Register(baseDataObject))
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the underlying data object.</summary>
    */
    public TDataObject BaseDataObject
    {
      get
      {return (TDataObject)PdfObject.Resolve(BaseObject);}
    }

    /**
      <summary>Gets whether the underlying data object is concrete.</summary>
    */
    public bool Exists(
      )
    {return !BaseDataObject.Virtual;}

    /**
      <summary>Gets/Sets the metadata associated to this object.</summary>
      <returns><code>null</code>, if base data object's type isn't suitable (only
      <see cref="PdfDictionary"/> and <see cref="PdfStream"/> objects are allowed).</returns>
      <throws>NotSupportedException If base data object's type isn't suitable (only
      <see cref="PdfDictionary"/> and <see cref="PdfStream"/> objects are allowed).</throws>
    */
    public Metadata Metadata
    {
      get
      {
        PdfDictionary dictionary = Dictionary;
        if(dictionary == null)
          return null;

        return new Metadata(dictionary.Get<PdfStream>(PdfName.Metadata, false));
      }
      set
      {
        PdfDictionary dictionary = Dictionary;
        if(dictionary == null)
          throw new NotSupportedException("Metadata can be attached only to PdfDictionary/PdfStream base data objects.");

        dictionary[PdfName.Metadata] = PdfObjectWrapper.GetBaseObject(value);
      }
    }
    #endregion

    #region private
    private PdfDictionary Dictionary
    {
      get
      {
        TDataObject baseDataObject = BaseDataObject;
        if(baseDataObject is PdfDictionary)
          return baseDataObject as PdfDictionary;
        else if(baseDataObject is PdfStream)
          return (baseDataObject as PdfStream).Header;
        else
          return null;
      }
    }
    #endregion
    #endregion
    #endregion
  }
}