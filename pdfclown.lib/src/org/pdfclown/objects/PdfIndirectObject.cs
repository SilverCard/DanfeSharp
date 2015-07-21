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
using System.Text;

namespace org.pdfclown.objects
{
  /**
    <summary>PDF indirect object [PDF:1.6:3.2.9].</summary>
  */
  public class PdfIndirectObject
    : PdfObject,
      IPdfIndirectObject
  {
    #region static
    #region fields
    private static readonly byte[] BeginIndirectObjectChunk = tokens.Encoding.Pdf.Encode(Symbol.Space + Keyword.BeginIndirectObject + Symbol.LineFeed);
    private static readonly byte[] EndIndirectObjectChunk = tokens.Encoding.Pdf.Encode(Symbol.LineFeed + Keyword.EndIndirectObject + Symbol.LineFeed);
    #endregion
    #endregion

    #region dynamic
    #region fields
    private PdfDataObject dataObject;
    private File file;
    private bool original;
    private PdfReference reference;
    private XRefEntry xrefEntry;

    private bool updateable = true;
    private bool updated;
    private bool virtual_;
    #endregion

    #region constructors
    /**
      <param name="file">Associated file.</param>
      <param name="dataObject">
        <para>Data object associated to the indirect object. It MUST be</para>
        <list type="bullet">
          <item><code>null</code>, if the indirect object is original or free.</item>
          <item>NOT <code>null</code>, if the indirect object is new and in-use.</item>
        </list>
      </param>
      <param name="xrefEntry">Cross-reference entry associated to the indirect object. If the
        indirect object is new, its offset field MUST be set to 0.</param>
    */
    internal PdfIndirectObject(
      File file,
      PdfDataObject dataObject,
      XRefEntry xrefEntry
      )
    {
      this.file = file;
      this.dataObject = Include(dataObject);
      this.xrefEntry = xrefEntry;

      this.original = (xrefEntry.Offset >= 0);
      this.reference = new PdfReference(this);
    }
    #endregion

    #region interface
    #region public
    public override PdfObject Accept(
      IVisitor visitor,
      object data
      )
    {return visitor.Visit(this, data);}

    /**
      <summary>Adds the <see cref="DataObject">data object</see> to the specified object stream
      [PDF:1.6:3.4.6].</summary>
      <param name="objectStream">Target object stream.</param>
     */
    public void Compress(
      ObjectStream objectStream
      )
    {
      // Remove from previous object stream!
      Uncompress();

      if(objectStream != null)
      {
        // Add to the object stream!
        objectStream[xrefEntry.Number] = DataObject;
        // Update its xref entry!
        xrefEntry.Usage = XRefEntry.UsageEnum.InUseCompressed;
        xrefEntry.StreamNumber = objectStream.Reference.ObjectNumber;
        xrefEntry.Offset = XRefEntry.UndefinedOffset; // Internal object index unknown (to set on object stream serialization -- see ObjectStream).
      }
    }

    public override PdfIndirectObject Container
    {
      get
      {return this;}
    }

    public override File File
    {
      get
      {return file;}
    }

    public override int GetHashCode(
      )
    {
      /*
        NOTE: Uniqueness should be achieved XORring the (local) reference hashcode with the (global)
        file hashcode.
        NOTE: Do NOT directly invoke reference.GetHashCode() method here as, conversely relying on
        this method, it would trigger an infinite loop.
      */
      return reference.Id.GetHashCode() ^ file.GetHashCode();
    }

    /**
      <summary>Gets whether this object is compressed within an object stream [PDF:1.6:3.4.6].
      </summary>
    */
    public bool IsCompressed(
      )
    {return xrefEntry.Usage == XRefEntry.UsageEnum.InUseCompressed;}

    /**
      <summary>Gets whether this object contains a data object.</summary>
    */
    public bool IsInUse(
      )
    {return (xrefEntry.Usage == XRefEntry.UsageEnum.InUse);}

    /**
      <summary>Gets whether this object comes intact from an existing file.</summary>
    */
    public bool IsOriginal(
      )
    {return original;}

    public override PdfObject Parent
    {
      get
      {return null;} // NOTE: As indirect objects are root objects, no parent can be associated.
      internal set
      {/* NOOP: As indirect objects are root objects, no parent can be associated. */}
    }

    public override PdfObject Swap(
      PdfObject other
      )
    {
      PdfIndirectObject otherObject = (PdfIndirectObject)other;
      PdfDataObject otherDataObject = otherObject.dataObject;
      // Update the other!
      otherObject.DataObject = dataObject;
      // Update this one!
      this.DataObject = otherDataObject;
      return this;
    }

    /**
      <summary>Removes the <see cref="DataObject">data object</see> from its object stream [PDF:1.6:3.4.6].</summary>
    */
    public void Uncompress(
      )
    {
      if(!IsCompressed())
        return;

      // Remove from its object stream!
      ObjectStream oldObjectStream = (ObjectStream)file.IndirectObjects[xrefEntry.StreamNumber].DataObject;
      oldObjectStream.Remove(xrefEntry.Number);
      // Update its xref entry!
      xrefEntry.Usage = XRefEntry.UsageEnum.InUse;
      xrefEntry.StreamNumber = -1; // No object stream.
      xrefEntry.Offset = XRefEntry.UndefinedOffset; // Offset unknown (to set on file serialization -- see CompressedWriter).
    }

    public override bool Updateable
    {
      get
      {return updateable;}
      set
      {updateable = value;}
    }

    public override bool Updated
    {
      get
      {return updated;}
      protected internal set
      {
        if(value && original)
        {
          /*
            NOTE: It's expected that DropOriginal() is invoked by IndirectObjects indexer;
            such an action is delegated because clients may invoke directly the indexer skipping
            this method.
          */
          file.IndirectObjects.Update(this);
        }
        updated = value;
      }
    }

    public override void WriteTo(
      IOutputStream stream,
      File context
      )
    {
      // Header.
      stream.Write(reference.Id); stream.Write(BeginIndirectObjectChunk);
      // Body.
      DataObject.WriteTo(stream, context);
      // Tail.
      stream.Write(EndIndirectObjectChunk);
    }

    public XRefEntry XrefEntry
    {
      get
      {return xrefEntry;}
    }

    #region IPdfIndirectObject
    public PdfDataObject DataObject
    {
      get
      {
        if(dataObject == null)
        {
          switch(xrefEntry.Usage)
          {
            // Free entry (no data object at all).
            case XRefEntry.UsageEnum.Free:
              break;
            // In-use entry (late-bound data object).
            case XRefEntry.UsageEnum.InUse:
            {
              FileParser parser = file.Reader.Parser;
              // Retrieve the associated data object among the original objects!
              parser.Seek(xrefEntry.Offset);
              // Get the indirect data object!
              dataObject = Include(parser.ParsePdfObject(4)); // NOTE: Skips the indirect-object header.
              break;
            }
            case XRefEntry.UsageEnum.InUseCompressed:
            {
              // Get the object stream where its data object is stored!
              ObjectStream objectStream = (ObjectStream)file.IndirectObjects[xrefEntry.StreamNumber].DataObject;
              // Get the indirect data object!
              dataObject = Include(objectStream[xrefEntry.Number]);
              break;
            }
          }
        }
        return dataObject;
      }
      set
      {
        if(xrefEntry.Generation == XRefEntry.GenerationUnreusable)
          throw new Exception("Unreusable entry.");

        Exclude(dataObject);
        dataObject = Include(value);
        xrefEntry.Usage = XRefEntry.UsageEnum.InUse;
        Update();
      }
    }

    public void Delete(
      )
    {
      if(file == null)
        return;

      /*
        NOTE: It's expected that DropFile() is invoked by IndirectObjects.Remove() method;
        such an action is delegated because clients may invoke directly Remove() method,
        skipping this method.
      */
      file.IndirectObjects.RemoveAt(xrefEntry.Number);
    }

    public override PdfIndirectObject IndirectObject
    {
      get
      {return this;}
    }

    public override PdfReference Reference
    {
      get
      {return reference;}
    }

    public override string ToString(
      )
    {
      StringBuilder buffer = new StringBuilder();
      {
        // Header.
        buffer.Append(reference.Id).Append(" obj").Append(Symbol.LineFeed);
        // Body.
        buffer.Append(DataObject);
      }
      return buffer.ToString();
    }
    #endregion
    #endregion

    #region protected
    protected internal override bool Virtual
    {
      get
      {return virtual_;}
      set
      {
        if(virtual_ && !value)
        {
          /*
            NOTE: When a virtual indirect object becomes concrete it must be registered.
          */
          file.IndirectObjects.AddVirtual(this);
          virtual_ = false;
          Reference.Update();
        }
        else
        {virtual_ = value;}
        dataObject.Virtual = virtual_;
      }
    }
    #endregion

    #region internal
    internal void DropFile(
      )
    {
      Uncompress();
      file = null;
    }

    internal void DropOriginal(
      )
    {original = false;}
    #endregion
    #endregion
    #endregion
  }
}