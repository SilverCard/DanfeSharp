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
using org.pdfclown.bytes.filters;
using org.pdfclown.documents.files;
using org.pdfclown.files;
using org.pdfclown.tokens;

using System;
using System.Collections;
using System.Collections.Generic;

namespace org.pdfclown.objects
{
  /**
    <summary>PDF stream object [PDF:1.6:3.2.7].</summary>
  */
  public class PdfStream
    : PdfDataObject,
      IFileResource
  {
    #region static
    #region fields
    private static readonly byte[] BeginStreamBodyChunk = Encoding.Pdf.Encode(Symbol.LineFeed + Keyword.BeginStream + Symbol.LineFeed);
    private static readonly byte[] EndStreamBodyChunk = Encoding.Pdf.Encode(Symbol.LineFeed + Keyword.EndStream);
    #endregion
    #endregion

    #region dynamic
    #region fields
    internal IBuffer body;
    internal PdfDictionary header;

    private PdfObject parent;
    private bool updateable = true;
    private bool updated;
    private bool virtual_;

    /**
      <summary>Indicates whether {@link #body} has already been resolved and therefore contains the
      actual stream data.</summary>
    */
    private bool bodyResolved;
    #endregion

    #region constructors
    public PdfStream(
      ) : this(
        new PdfDictionary(),
        new bytes.Buffer()
        )
    {}

    public PdfStream(
      PdfDictionary header
      ) : this(
        header,
        new bytes.Buffer()
        )
    {}

    public PdfStream(
      IBuffer body
      ) : this(
        new PdfDictionary(),
        body
        )
    {}

    public PdfStream(
      PdfDictionary header,
      IBuffer body
      )
    {
      this.header = (PdfDictionary)Include(header);

      this.body = body;
      body.Dirty = false;
      body.OnChange += delegate(
        object sender,
        EventArgs args
        )
      {Update();};
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
      <summary>Gets the decoded stream body.</summary>
    */
    public IBuffer Body
    {
      get
      {
        /*
          NOTE: Encoding filters are removed by default because they belong to a lower layer (token
          layer), so that it's appropriate and consistent to transparently keep the object layer
          unaware of such a facility.
        */
        return GetBody(true);
      }
    }

    public PdfDirectObject Filter
    {
      get
      {
        return (PdfDirectObject)(header[PdfName.F] == null
          ? header.Resolve(PdfName.Filter)
          : header.Resolve(PdfName.FFilter));
      }
      protected set
      {
        header[
          header[PdfName.F] == null
            ? PdfName.Filter
            : PdfName.FFilter
          ] = value;
      }
    }

    /**
      <summary>Gets the stream body.</summary>
      <param name="decode">Defines whether the body has to be decoded.</param>
    */
    public IBuffer GetBody(
      bool decode
      )
    {
      if(!bodyResolved)
      {
        /*
          NOTE: In case of stream data from external file, a copy to the local buffer has to be done.
        */
        FileSpecification dataFile = DataFile;
        if(dataFile != null)
        {
          Updateable = false;
          body.SetLength(0);
          body.Write(dataFile.GetInputStream());
          body.Dirty = false;
          Updateable = true;
        }
        bodyResolved = true;
      }
      if(decode)
      {
        PdfDataObject filter = Filter;
        if(filter != null) // Stream encoded.
        {
          header.Updateable = false;
          PdfDataObject parameters = Parameters;
          if(filter is PdfName) // Single filter.
          {
            body.Decode(
              bytes.filters.Filter.Get((PdfName)filter),
              (PdfDictionary)parameters
              );
          }
          else // Multiple filters.
          {
            IEnumerator<PdfDirectObject> filterIterator = ((PdfArray)filter).GetEnumerator();
            IEnumerator<PdfDirectObject> parametersIterator = (parameters != null ? ((PdfArray)parameters).GetEnumerator() : null);
            while(filterIterator.MoveNext())
            {
              PdfDictionary filterParameters;
              if(parametersIterator == null)
              {filterParameters = null;}
              else
              {
                parametersIterator.MoveNext();
                filterParameters = (PdfDictionary)Resolve(parametersIterator.Current);
              }
              body.Decode(bytes.filters.Filter.Get((PdfName)Resolve(filterIterator.Current)), filterParameters);
            }
          }
          Filter = null; // The stream is free from encodings.
          header.Updateable = true;
        }
      }
      return body;
    }

    /**
      <summary>Gets the stream header.</summary>
    */
    public PdfDictionary Header
    {
      get
      {return header;}
    }

    public PdfDirectObject Parameters
    {
      get
      {
        return (PdfDirectObject)(header[PdfName.F] == null
          ? header.Resolve(PdfName.DecodeParms)
          : header.Resolve(PdfName.FDecodeParms));
      }
      protected set
      {
        header[
          header[PdfName.F] == null
            ? PdfName.DecodeParms
            : PdfName.FDecodeParms
          ] = value;
      }
    }

    public override PdfObject Parent
    {
      get
      {return parent;}
      internal set
      {parent = value;}
    }

    /**
      <param name="preserve">Indicates whether the data from the old data source substitutes the
      new one. This way data can be imported to/exported from local or preserved in case of external
      file location changed.</param>
      <seealso cref="DataFile"/>
    */
    public void SetDataFile(
      FileSpecification value,
      bool preserve
      )
    {
      /*
        NOTE: If preserve argument is set to true, body's dirtiness MUST be forced in order to ensure
        data serialization to the new external location.

        Old data source | New data source | preserve | Action
        ----------------------------------------------------------------------------------------------
        local           | not null        | false     | A. Substitute local with new file.
        local           | not null        | true      | B. Export local to new file.
        external        | not null        | false     | C. Substitute old file with new file.
        external        | not null        | true      | D. Copy old file data to new file.
        local           | null            | (any)     | E. No action.
        external        | null            | false     | F. Empty local.
        external        | null            | true      | G. Import old file to local.
        ----------------------------------------------------------------------------------------------
      */
      FileSpecification oldDataFile = DataFile;
      PdfDirectObject dataFileObject = (value != null ? value.BaseObject : null);
      if(value != null)
      {
        if(preserve)
        {
          if(oldDataFile != null) // Case D (copy old file data to new file).
          {
            if(!bodyResolved)
            {
              // Transfer old file data to local!
              GetBody(false); // Ensures that external data is loaded as-is into the local buffer.
            }
          }
          else // Case B (export local to new file).
          {
            // Transfer local settings to file!
            header[PdfName.FFilter] = header[PdfName.Filter]; header.Remove(PdfName.Filter);
            header[PdfName.FDecodeParms] = header[PdfName.DecodeParms]; header.Remove(PdfName.DecodeParms);

            // Ensure local data represents actual data (otherwise it would be substituted by resolved file data)!
            bodyResolved = true;
          }
          // Ensure local data has to be serialized to new file!
          body.Dirty = true;
        }
        else // Case A/C (substitute local/old file with new file).
        {
          // Dismiss local/old file data!
          body.SetLength(0);
          // Dismiss local/old file settings!
          Filter = null;
          Parameters = null;
          // Ensure local data has to be loaded from new file!
          bodyResolved = false;
        }
      }
      else
      {
        if(oldDataFile != null)
        {
          if(preserve) // Case G (import old file to local).
          {
            // Transfer old file data to local!
            GetBody(false); // Ensures that external data is loaded as-is into the local buffer.
            // Transfer old file settings to local!
            header[PdfName.Filter] = header[PdfName.FFilter]; header.Remove(PdfName.FFilter);
            header[PdfName.DecodeParms] = header[PdfName.FDecodeParms]; header.Remove(PdfName.FDecodeParms);
          }
          else // Case F (empty local).
          {
            // Dismiss old file data!
            body.SetLength(0);
            // Dismiss old file settings!
            Filter = null;
            Parameters = null;
            // Ensure local data represents actual data (otherwise it would be substituted by resolved file data)!
            bodyResolved = true;
          }
        }
        else // E (no action).
        { /* NOOP */ }
      }
      header[PdfName.F] = dataFileObject;
    }

    public override PdfObject Swap(
      PdfObject other
      )
    {
      PdfStream otherStream = (PdfStream)other;
      PdfDictionary otherHeader = otherStream.header;
      IBuffer otherBody = otherStream.body;
      // Update the other!
      otherStream.header = this.header;
      otherStream.body = this.body;
      otherStream.Update();
      // Update this one!
      this.header = otherHeader;
      this.body = otherBody;
      this.Update();
      return this;
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
      {updated = value;}
    }

    public override void WriteTo(
      IOutputStream stream,
      File context
      )
    {
      /*
        NOTE: The header is temporarily tweaked to accommodate serialization settings.
      */
      header.Updateable = false;

      byte[] bodyData;
      {
        bool bodyUnencoded;
        {
          FileSpecification dataFile = DataFile;
          /*
            NOTE: In case of external file, the body buffer has to be saved back only if the file was
            actually resolved (that is brought into the body buffer) and modified.
          */
          bool encodeBody = (dataFile == null || (bodyResolved && body.Dirty));
          if(encodeBody)
          {
            PdfDirectObject filterObject = Filter;
            if(filterObject == null) // Unencoded body.
            {
              /*
                NOTE: Header entries related to stream body encoding are temporary, instrumental to
                the current serialization process only.
              */
              bodyUnencoded = true;

              // Set the filter to apply!
              filterObject = PdfName.FlateDecode; // zlib/deflate filter.
              // Get encoded body data applying the filter to the stream!
              bodyData = body.Encode(bytes.filters.Filter.Get((PdfName)filterObject), null);
              // Set 'Filter' entry!
              Filter = filterObject;
            }
            else // Encoded body.
            {
              bodyUnencoded = false;

              // Get encoded body data!
              bodyData = body.ToByteArray();
            }

            if(dataFile != null)
            {
              /*
                NOTE: In case of external file, body data has to be serialized there, leaving empty
                its representation within this stream.
              */
              try
              {
                IOutputStream dataFileOutputStream = dataFile.GetOutputStream();
                dataFileOutputStream.Write(bodyData);
                dataFileOutputStream.Dispose();
              }
              catch(Exception e)
              {throw new Exception("Data writing into " + dataFile.Path + " failed.", e);}
              // Local serialization is empty!
              bodyData = new byte[]{};
            }
          }
          else
          {
            bodyUnencoded = false;
            bodyData = new byte[]{};
          }
        }

        // Set the encoded data length!
        header[PdfName.Length] = PdfInteger.Get(bodyData.Length);

        // 1. Header.
        header.WriteTo(stream, context);

        if(bodyUnencoded)
        {
          // Restore actual header entries!
          header[PdfName.Length] = PdfInteger.Get((int)body.Length);
          Filter = null;
        }
      }

      // 2. Body.
      stream.Write(BeginStreamBodyChunk);
      stream.Write(bodyData);
      stream.Write(EndStreamBodyChunk);

      header.Updateable = true;
    }

    #region IFileResource
    [PDF(VersionEnum.PDF12)]
    public FileSpecification DataFile
    {
      get
      {return FileSpecification.Wrap(header[PdfName.F]);}
      set
      {SetDataFile(value, false);}
    }
    #endregion
    #endregion

    #region protected
    protected internal override bool Virtual
    {
      get
      {return virtual_;}
      set
      {virtual_ = value;}
    }
    #endregion
    #endregion
    #endregion
  }
}