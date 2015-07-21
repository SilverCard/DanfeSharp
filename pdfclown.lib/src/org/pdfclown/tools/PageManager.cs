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
using org.pdfclown.documents.contents;
using org.pdfclown.documents.contents.composition;
using org.pdfclown.documents.contents.objects;
using org.pdfclown.files;
using org.pdfclown.objects;

using System.Collections.Generic;

namespace org.pdfclown.tools
{
  /**
    <summary>Tool for page management.</summary>
  */
  public sealed class PageManager
  {
    /*
      NOTE: As you can read on the PDF Clown's User Guide, referential operations on high-level object such as pages
      can be done at two levels:
        1. shallow, involving page references but NOT their data within the document;
        2. deep, involving page data within the document.
      This means that, for example, if you remove a page reference (shallow level) from the pages collection,
      the data of that page (deep level) are still within the document!
    */

    #region static
    #region interface
    #region public
    /**
      <summary>Gets the data size of the specified page expressed in bytes.</summary>
      <param name="page">Page whose data size has to be calculated.</param>
    */
    public static long GetSize(
      Page page
      )
    {return GetSize(page, new HashSet<PdfReference>());}

    /**
      <summary>Gets the data size of the specified page expressed in bytes.</summary>
      <param name="page">Page whose data size has to be calculated.</param>
      <param name="visitedReferences">References to data objects excluded from calculation.
        This set is useful, for example, to avoid recalculating the data size of shared resources.
        During the operation, this set is populated with references to visited data objects.</param>
    */
    public static long GetSize(
      Page page,
      HashSet<PdfReference> visitedReferences
      )
    {return GetSize(page.BaseObject, visitedReferences, true);}
    #endregion

    #region private
    /**
      <summary>Gets the data size of the specified object expressed in bytes.</summary>
      <param name="object">Data object whose size has to be calculated.</param>
      <param name="visitedReferences">References to data objects excluded from calculation.
        This set is useful, for example, to avoid recalculating the data size of shared resources.
        During the operation, this set is populated with references to visited data objects.</param>
      <param name="isRoot">Whether this data object represents the page root.</param>
    */
    private static long GetSize(
      PdfDirectObject obj,
      HashSet<PdfReference> visitedReferences,
      bool isRoot
      )
    {
      long dataSize = 0;
      {
        PdfDataObject dataObject = PdfObject.Resolve(obj);

        // 1. Evaluating the current object...
        if(obj is PdfReference)
        {
          PdfReference reference = (PdfReference)obj;
          if(visitedReferences.Contains(reference))
            return 0; // Avoids circular references.

          if(dataObject is PdfDictionary
            && PdfName.Page.Equals(((PdfDictionary)dataObject)[PdfName.Type])
            && !isRoot)
            return 0; // Avoids references to other pages.

          visitedReferences.Add(reference);

          // Calculate the data size of the current object!
          IOutputStream buffer = new Buffer();
          reference.IndirectObject.WriteTo(buffer, reference.File);
          dataSize += buffer.Length;
        }

        // 2. Evaluating the current object's children...
        ICollection<PdfDirectObject> values = null;
        {
          if(dataObject is PdfStream)
          {dataObject = ((PdfStream)dataObject).Header;}
          if(dataObject is PdfDictionary)
          {values = ((PdfDictionary)dataObject).Values;}
          else if(dataObject is PdfArray)
          {values = (PdfArray)dataObject;}
        }
        if(values != null)
        {
          // Calculate the data size of the current object's children!
          foreach(PdfDirectObject value in values)
          {dataSize += GetSize(value, visitedReferences, false);}
        }
      }
      return dataSize;
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region fields
    private Document document;
    private Pages pages;
    #endregion

    #region constructors
    public PageManager(
      ) : this(null)
    {}

    public PageManager(
      Document document
      )
    {Document = document;}
    #endregion

    #region interface
    #region public
    /**
      <summary>Appends a document to the end of the document.</summary>
      <param name="document">Document to be added.</param>
    */
    public void Add(
      Document document
      )
    {Add((ICollection<Page>)document.Pages);}

    /**
      <summary>Inserts a document at the specified position in the document.</summary>
      <param name="index">Position at which the document has to be inserted.</param>
      <param name="document">Document to be inserted.</param>
    */
    public void Add(
      int index,
      Document document
      )
    {Add(index,(ICollection<Page>)document.Pages);}

    /**
      <summary>Appends a collection of pages to the end of the document.</summary>
      <param name="pages">Pages to be added.</param>
    */
    public void Add(
      ICollection<Page> pages
      )
    {
      // Add the source pages to the document (deep level)!
      ICollection<Page> importedPages = (ICollection<Page>)document.Include(pages); // NOTE: Alien pages MUST be contextualized (i.e. imported).

      // Add the imported pages to the pages collection (shallow level)!
      this.pages.AddAll(importedPages);
    }

    /**
      <summary>Inserts a collection of pages at the specified position in the document.</summary>
      <param name="index">Position at which the pages have to be inserted.</param>
      <param name="pages">Pages to be inserted.</param>
    */
    public void Add(
      int index,
      ICollection<Page> pages
      )
    {
      // Add the source pages to the document (deep level)!
      ICollection<Page> importedPages = (ICollection<Page>)document.Include(pages); // NOTE: Alien pages MUST be contextualized (i.e. imported).

      // Add the imported pages to the pages collection (shallow level)!
      if(index >= this.pages.Count)
      {this.pages.AddAll(importedPages);}
      else
      {this.pages.InsertAll(index, importedPages);}
    }

    /**
      <summary>Gets/Sets the document being managed.</summary>
    */
    public Document Document
    {
      get
      {return document;}
      set
      {
        document = value;
        pages = document.Pages;
      }
    }

    /**
      <summary>Extracts a page range from the document.</summary>
      <param name="startIndex">The beginning index, inclusive.</param>
      <param name="endIndex">The ending index, exclusive.</param>
      <returns>Extracted page range.</returns>
    */
    public Document Extract(
      int startIndex,
      int endIndex
      )
    {
      Document extractedDocument = new File().Document;
      {
        // Add the pages to the target file!
        /*
          NOTE: To be added to an alien document,
          pages MUST be contextualized within it first,
          then added to the target pages collection.
        */
        extractedDocument.Pages.AddAll(
          (ICollection<Page>)extractedDocument.Include(
            pages.GetSlice(startIndex, endIndex)
            )
          );
      }
      return extractedDocument;
    }

    /**
      <summary>Moves a page range to a target position within the document.</summary>
      <param name="startIndex">The beginning index, inclusive.</param>
      <param name="endIndex">The ending index, exclusive.</param>
      <param name="targetIndex">The target index.</param>
    */
    public void Move(
      int startIndex,
      int endIndex,
      int targetIndex
      )
    {
      int pageCount = pages.Count;

      IList<Page> movingPages = pages.GetSlice(startIndex, endIndex);

      // Temporarily remove the pages from the pages collection!
      /*
        NOTE: Shallow removal (only page references are removed, as their data are kept in the document).
      */
      pages.RemoveAll(movingPages);

      // Adjust indexes!
      pageCount -= movingPages.Count;
      if(targetIndex > startIndex)
      {targetIndex -= movingPages.Count;} // Adjusts the target position due to shifting for temporary page removal.

      // Reinsert the pages at the target position!
      /*
        NOTE: Shallow addition (only page references are added, as their data are already in the document).
      */
      if(targetIndex >= pageCount)
      {pages.AddAll(movingPages);}
      else
      {pages.InsertAll(targetIndex, movingPages);}
    }

    /**
      <summary>Removes a page range from the document.</summary>
      <param name="startIndex">The beginning index, inclusive.</param>
      <param name="endIndex">The ending index, exclusive.</param>
    */
    public void Remove(
      int startIndex,
      int endIndex
      )
    {
      IList<Page> removingPages = pages.GetSlice(startIndex, endIndex);

      // Remove the pages from the pages collection!
      /* NOTE: Shallow removal. */
      pages.RemoveAll(removingPages);

      // Remove the pages from the document (decontextualize)!
      /* NOTE: Deep removal. */
      document.Exclude(removingPages);
    }

    /**
      <summary>Bursts the document into single-page documents.</summary>
      <returns>Split subdocuments.</returns>
    */
    public IList<Document> Split(
      )
    {
      IList<Document> documents = new List<Document>();
      foreach(Page page in pages)
      {
        Document pageDocument = new File().Document;
        pageDocument.Pages.Add((Page)page.Clone(pageDocument));
        documents.Add(pageDocument);
      }
      return documents;
    }

    /**
      <summary>Splits the document into multiple subdocuments delimited by the specified page indexes.</summary>
      <param name="indexes">Split page indexes.</param>
      <returns>Split subdocuments.</returns>
    */
    public IList<Document> Split(
      params int[] indexes
      )
    {
      IList<Document> documents = new List<Document>();
      {
        int startIndex = 0;
        foreach(int index in indexes)
        {
          documents.Add(Extract(startIndex, index));
          startIndex = index;
        }
        documents.Add(Extract(startIndex, pages.Count));
      }
      return documents;
    }

    /**
      <summary>Splits the document into multiple subdocuments on maximum file size.</summary>
      <param name="maxDataSize">Maximum data size (expressed in bytes) of target files.
        Note that resulting files may be a little bit larger than this value, as file data include (along with actual page data)
        some extra structures such as cross reference tables.</param>
      <returns>Split documents.</returns>
    */
    public IList<Document> Split(
      long maxDataSize
      )
    {
      IList<Document> documents = new List<Document>();
      {
        int startPageIndex = 0;
        long incrementalDataSize = 0;
        HashSet<PdfReference> visitedReferences = new HashSet<PdfReference>();
        foreach(Page page in pages)
        {
          long pageDifferentialDataSize = GetSize(page, visitedReferences);
          incrementalDataSize += pageDifferentialDataSize;
          if(incrementalDataSize > maxDataSize) // Data size limit reached.
          {
            int endPageIndex = page.Index;

            // Split the current document page range!
            documents.Add(Extract(startPageIndex, endPageIndex));

            startPageIndex = endPageIndex;
            incrementalDataSize = GetSize(page, visitedReferences = new HashSet<PdfReference>());
          }
        }
        // Split the last document page range!
        documents.Add(Extract(startPageIndex, pages.Count));
      }
      return documents;
    }
    #endregion
    #endregion
    #endregion
  }
}