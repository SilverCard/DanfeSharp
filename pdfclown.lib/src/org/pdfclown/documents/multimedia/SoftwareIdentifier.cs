/*
  Copyright 2012 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.documents.contents.colorSpaces;
using org.pdfclown.documents.interaction;
using actions = org.pdfclown.documents.interaction.actions;
using org.pdfclown.files;
using org.pdfclown.objects;
using org.pdfclown.util.math;
using org.pdfclown.util.metadata;

using System;
using System.Collections.Generic;

namespace org.pdfclown.documents.multimedia
{
  /**
    <summary>Software identifier [PDF:1.7:9.1.6].</summary>
  */
  [PDF(VersionEnum.PDF15)]
  public sealed class SoftwareIdentifier
    : PdfObjectWrapper<PdfDictionary>
  {
    #region types
    /**
      <summary>Software version number [PDF:1.7:9.1.6].</summary>
    */
    public sealed class VersionObject
      : PdfObjectWrapper<PdfArray>,
        IVersion
    {
      public VersionObject(
        params int[] numbers
        ) : base(new PdfArray())
      {
        PdfArray baseDataObject = BaseDataObject;
        foreach(int number in numbers)
        {baseDataObject.Add(new PdfInteger(number));}
      }

      internal VersionObject(
        PdfDirectObject baseObject
        ) : base(baseObject)
      {}

      public int CompareTo(
        IVersion value
        )
      {return VersionUtils.CompareTo(this, value);}

      public IList<int> Numbers
      {
        get
        {
          IList<int> numbers = new List<int>();
          foreach(PdfDirectObject numberObject in BaseDataObject)
          {numbers.Add(((PdfInteger)numberObject).IntValue);}
          return numbers;
        }
      }

      public override string ToString(
        )
      {return VersionUtils.ToString(this);}
    }
    #endregion

    #region static
    #region interface
    #region public
    public static SoftwareIdentifier Wrap(
      PdfDirectObject baseObject
      )
    {return baseObject != null ? new SoftwareIdentifier(baseObject) : null;}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public SoftwareIdentifier(
      Document context
      ) : base(context, new PdfDictionary())
    {}

    private SoftwareIdentifier(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the operating system identifiers that indicate which operating systems this
      object applies to.</summary>
      <remarks>The defined values are the same as those defined for SMIL 2.0's systemOperatingSystem
      attribute. An empty list is considered to represent all operating systems.</remarks>
    */
    public IList<string> OSes
    {
      get
      {
        IList<string> oses = new List<string>();
        {
          PdfArray osesObject = (PdfArray)BaseDataObject[PdfName.OS];
          if(osesObject != null)
          {
            foreach(PdfDirectObject osObject in osesObject)
            {oses.Add(((PdfString)osObject).StringValue);}
          }
        }
        return oses;
      }
    }

    /**
      <summary>Gets the URI that identifies a piece of software.</summary>
      <remarks>It is interpreted according to its scheme; the only presently defined scheme is
      vnd.adobe.swname. The scheme name is case-insensitive; if is not recognized by the viewer
      application, the software must be considered a non-match. The syntax of URIs of this scheme is
      "vnd.adobe.swname:" software_name where software_name is equivalent to reg_name as defined in
      Internet RFC 2396, Uniform Resource Identifiers (URI): Generic Syntax.</remarks>
    */
    public Uri URI
    {
      get
      {
        PdfString uriObject = (PdfString)BaseDataObject[PdfName.U];
        return uriObject != null ? new Uri(uriObject.StringValue) : null;
      }
    }

    /**
      <summary>Gets the software version bounds.</summary>
    */
    public Interval<VersionObject> Version
    {
      get
      {
        PdfDictionary baseDataObject = BaseDataObject;
        return new Interval<VersionObject>(
          new VersionObject((PdfArray)baseDataObject[PdfName.L]),
          new VersionObject((PdfArray)baseDataObject[PdfName.H]),
          (bool)PdfBoolean.GetValue(baseDataObject[PdfName.LI], true),
          (bool)PdfBoolean.GetValue(baseDataObject[PdfName.HI], true)
          );
      }
    }

    //TODO:setters!!!
    #endregion
    #endregion
    #endregion
  }
}