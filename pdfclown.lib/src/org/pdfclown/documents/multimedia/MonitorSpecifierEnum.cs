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
using org.pdfclown.util;

using System;

namespace org.pdfclown.documents.multimedia
{
  /**
    <summary>Monitor specifier [PDF:1.7:9.1.6].</summary>
  */
  [PDF(VersionEnum.PDF15)]
  public enum MonitorSpecifierEnum
  {
    /**
      <summary>The monitor containing the largest section of the document window.</summary>
    */
    LargestDocumentWindowSection,
    /**
      <summary>The monitor containing the smallest section of the document window.</summary>
    */
    SmallestDocumentWindowSection,
    /**
      <summary>Primary monitor, otherwise the monitor containing the largest section of the document
      window.</summary>
    */
    Primary,
    /**
      <summary>The monitor with the greatest color depth (in bits).</summary>
    */
    GreatestColorDepth,
    /**
      <summary>The monitor with the greatest area (in pixels squared).</summary>
    */
    GreatestArea,
    /**
      <summary>The monitor with the greatest height (in pixels).</summary>
    */
    GreatestHeight,
    /**
      <summary>The monitor with the greatest width (in pixels).</summary>
    */
    GreatestWidth
  }

  internal static class MonitorSpecifierEnumExtension
  {
    private static readonly BiDictionary<MonitorSpecifierEnum,PdfInteger> codes;

    static MonitorSpecifierEnumExtension()
    {
      codes = new BiDictionary<MonitorSpecifierEnum,PdfInteger>();
      codes[MonitorSpecifierEnum.LargestDocumentWindowSection] = new PdfInteger(0);
      codes[MonitorSpecifierEnum.SmallestDocumentWindowSection] = new PdfInteger(1);
      codes[MonitorSpecifierEnum.Primary] = new PdfInteger(2);
      codes[MonitorSpecifierEnum.GreatestColorDepth] = new PdfInteger(3);
      codes[MonitorSpecifierEnum.GreatestArea] = new PdfInteger(4);
      codes[MonitorSpecifierEnum.GreatestHeight] = new PdfInteger(5);
      codes[MonitorSpecifierEnum.GreatestWidth] = new PdfInteger(6);
    }

    public static MonitorSpecifierEnum? Get(
      PdfInteger code
      )
    {
      if(code == null)
        return MonitorSpecifierEnum.LargestDocumentWindowSection;

      MonitorSpecifierEnum? monitorSpecifier = codes.GetKey(code);
      if(!monitorSpecifier.HasValue)
        throw new NotSupportedException("Monitor specifier unknown: " + code);

      return monitorSpecifier;
    }

    public static PdfInteger GetCode(
      this MonitorSpecifierEnum monitorSpecifier
      )
    {return codes[monitorSpecifier];}
  }
}