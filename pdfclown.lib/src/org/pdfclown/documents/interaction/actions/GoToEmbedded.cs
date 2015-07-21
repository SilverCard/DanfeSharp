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
using org.pdfclown.documents.files;
using org.pdfclown.documents.interaction.navigation.document;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;

namespace org.pdfclown.documents.interaction.actions
{
  /**
    <summary>'Change the view to a specified destination in a PDF file embedded in another PDF file'
    action [PDF:1.6:8.5.3].</summary>
  */
  [PDF(VersionEnum.PDF11)]
  public sealed class GoToEmbedded
    : GotoNonLocal<Destination>
  {
    #region types
    /**
      <summary>Path information to the target document [PDF:1.6:8.5.3].</summary>
    */
    public class PathElement
      : PdfObjectWrapper<PdfDictionary>
    {
      #region types
      /**
        <summary>Relationship between the target and the current document [PDF:1.6:8.5.3].</summary>
      */
      public enum RelationEnum
      {
        /**
          <summary>Parent.</summary>
        */
        Parent,
        /**
          <summary>Child.</summary>
        */
        Child
      };
      #endregion

      #region static
      #region fields
      private static readonly Dictionary<RelationEnum,PdfName> RelationEnumCodes;
      #endregion

      #region constructors
      static PathElement()
      {
        RelationEnumCodes = new Dictionary<RelationEnum,PdfName>();
        RelationEnumCodes[RelationEnum.Parent] = PdfName.P;
        RelationEnumCodes[RelationEnum.Child] = PdfName.C;
      }
      #endregion

      #region interface
      #region private
      /**
        <summary>Gets the code corresponding to the given value.</summary>
      */
      private static PdfName ToCode(
        RelationEnum value
        )
      {return RelationEnumCodes[value];}

      /**
        <summary>Gets the relation corresponding to the given value.</summary>
      */
      private static RelationEnum ToRelationEnum(
        PdfName value
        )
      {
        foreach(KeyValuePair<RelationEnum,PdfName> relation in RelationEnumCodes)
        {
          if(relation.Value.Equals(value))
            return relation.Key;
        }
        throw new Exception("'" + value.Value + "' doesn't represent a valid relation.");
      }
      #endregion
      #endregion
      #endregion

      #region dynamic
      #region constructors
      /**
        <summary>Creates a new path element representing the parent of the document.</summary>
      */
      public PathElement(
        Document context,
        PathElement next
        ) : this(
          context,
          RelationEnum.Parent,
          null,
          null,
          null,
          next
          )
      {}

      /**
        <summary>Creates a new path element located in the embedded files collection of the document.</summary>
      */
      public PathElement(
        Document context,
        string embeddedFileName,
        PathElement next
        ) : this(
          context,
          RelationEnum.Child,
          embeddedFileName,
          null,
          null,
          next
          )
      {}

      /**
        <summary>Creates a new path element associated with a file attachment annotation.</summary>
      */
      public PathElement(
        Document context,
        object annotationPageRef,
        object annotationRef,
        PathElement next
        ) : this(
          context,
          RelationEnum.Child,
          null,
          annotationPageRef,
          annotationRef,
          next
          )
      {}

      /**
        <summary>Creates a new path element.</summary>
      */
      private PathElement(
        Document context,
        RelationEnum relation,
        string embeddedFileName,
        object annotationPageRef,
        object annotationRef,
        PathElement next
        ) : base(context, new PdfDictionary())
      {
        Relation = relation;
        EmbeddedFileName = embeddedFileName;
        AnnotationPageRef = annotationPageRef;
        AnnotationRef = annotationRef;
        Next = next;
      }

      /**
        <summary>Instantiates an existing path element.</summary>
      */
      internal PathElement(
        PdfDirectObject baseObject
        ) : base(baseObject)
      {}
      #endregion

      #region interface
      #region public
      public override object Clone(
        Document context
        )
      {throw new NotImplementedException();}

      /**
        <summary>Gets/Sets the page reference to the file attachment annotation.</summary>
        <returns>Either the (zero-based) number of the page in the current document containing the file attachment annotation,
        or the name of a destination in the current document that provides the page number of the file attachment annotation.</returns>
      */
      public object AnnotationPageRef
      {
        get
        {
          PdfDirectObject pageRefObject = BaseDataObject[PdfName.P];
          if(pageRefObject == null)
            return null;

          if(pageRefObject is PdfInteger)
            return ((PdfInteger)pageRefObject).Value;
          else
            return ((PdfString)pageRefObject).Value;
        }
        set
        {
          if(value == null)
          {BaseDataObject.Remove(PdfName.P);}
          else
          {
            PdfDirectObject pageRefObject;
            if(value is int)
            {pageRefObject = PdfInteger.Get((int)value);}
            else if(value is string)
            {pageRefObject = new PdfString((string)value);}
            else
              throw new ArgumentException("Wrong argument type: it MUST be either a page number Integer or a named destination String.");

            BaseDataObject[PdfName.P] = pageRefObject;
          }
        }
      }

      /**
        <summary>Gets/Sets the reference to the file attachment annotation.</summary>
        <returns>Either the (zero-based) index of the annotation in the list of annotations
        associated to the page specified by the annotationPageRef property, or the name of the annotation.</returns>
      */
      public object AnnotationRef
      {
        get
        {
          PdfDirectObject annotationRefObject = BaseDataObject[PdfName.A];
          if(annotationRefObject == null)
            return null;

          if(annotationRefObject is PdfInteger)
            return ((PdfInteger)annotationRefObject).Value;
          else
            return ((PdfTextString)annotationRefObject).Value;
        }
        set
        {
          if(value == null)
          {BaseDataObject.Remove(PdfName.A);}
          else
          {
            PdfDirectObject annotationRefObject;
            if(value is int)
            {annotationRefObject = PdfInteger.Get((int)value);}
            else if(value is string)
            {annotationRefObject = new PdfTextString((string)value);}
            else
              throw new ArgumentException("Wrong argument type: it MUST be either an annotation index Integer or an annotation name String.");

            BaseDataObject[PdfName.A] = annotationRefObject;
          }
        }
      }

      /**
        <summary>Gets/Sets the embedded file name.</summary>
      */
      public string EmbeddedFileName
      {
        get
        {
          PdfString fileNameObject = (PdfString)BaseDataObject[PdfName.N];
          return fileNameObject != null ? (string)fileNameObject.Value : null;
        }
        set
        {
          if(value == null)
          {BaseDataObject.Remove(PdfName.N);}
          else
          {BaseDataObject[PdfName.N] = new PdfString(value);}
        }
      }

      /**
        <summary>Gets/Sets the relationship between the target and the current document.</summary>
      */
      public RelationEnum Relation
      {
        get
        {return ToRelationEnum((PdfName)BaseDataObject[PdfName.R]);}
        set
        {BaseDataObject[PdfName.R] = ToCode(value);}
      }

      /**
        <summary>Gets/Sets a further path information to the target document.</summary>
      */
      public PathElement Next
      {
        get
        {
          PdfDirectObject targetObject = BaseDataObject[PdfName.T];
          return targetObject != null ? new PathElement(targetObject) : null;
        }
        set
        {
          if(value == null)
          {BaseDataObject.Remove(PdfName.T);}
          else
          {BaseDataObject[PdfName.T] = value.BaseObject;}
        }
      }
      #endregion
      #endregion
      #endregion
    }
    #endregion

    #region dynamic
    #region constructors
    /**
      <summary>Creates a new instance within the specified document context, pointing to a
      destination within an embedded document.</summary>
      <param name="context">Document context.</param>
      <param name="destinationPath">Path information to the target document within the destination
      file.</param>
      <param name="destination">Destination within the target document.</param>
    */
    public GoToEmbedded(
      Document context,
      PathElement destinationPath,
      Destination destination
      ) : this(
        context,
        null,
        destinationPath,
        destination
        )
    {}

    /**
      <summary>Creates a new instance within the specified document context, pointing to a
      destination within another document.</summary>
      <param name="context">Document context.</param>
      <param name="destinationFile">File in which the destination is located.</param>
      <param name="destination">Destination within the target document.</param>
    */
    public GoToEmbedded(
      Document context,
      FileSpecification destinationFile,
      Destination destination
      ) : this(
        context,
        destinationFile,
        null,
        destination
        )
    {}

    /**
      <summary>Creates a new instance within the specified document context.</summary>
      <param name="context">Document context.</param>
      <param name="destinationFile">File in which the destination is located.</param>
      <param name="destinationPath">Path information to the target document within the destination
      file.</param>
      <param name="destination">Destination within the target document.</param>
    */
    public GoToEmbedded(
      Document context,
      FileSpecification destinationFile,
      PathElement destinationPath,
      Destination destination
      ) : base(
        context,
        PdfName.GoToE,
        destinationFile,
        destination
        )
    {DestinationPath = destinationPath;}

    internal GoToEmbedded(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets/Sets the path information to the target document.</summary>
    */
    public PathElement DestinationPath
    {
      get
      {
        PdfDirectObject targetObject = BaseDataObject[PdfName.T];
        return targetObject != null ? new PathElement(targetObject) : null;
      }
      set
      {
        if(value == null)
        {BaseDataObject.Remove(PdfName.T);}
        else
        {BaseDataObject[PdfName.T] = value.BaseObject;}
      }
    }
    #endregion
    #endregion
    #endregion
  }
}