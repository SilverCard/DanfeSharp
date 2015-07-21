/*
  Copyright 2007-2010 Stefano Chizzolini. http://www.pdfclown.org

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

using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace org.pdfclown.documents
{
  /**
    <summary>Page format.</summary>
    <remarks>This utility provides an easy access to the dimension of common page formats.</remarks>
  */
  public sealed class PageFormat
  {
    #region types
    /**
      <summary>Paper size.</summary>
      <remarks>
        References:
        <list type="bullet">
          <item>{ 'A' digit+ }: [ISO 216] "A" series: Paper and boards, trimmed sizes.</item>
          <item>{ 'B' digit+ }: [ISO 216] "B" series: Posters, wall charts and similar items.</item>
          <item>{ 'C' digit+ }: [ISO 269] "C" series: Envelopes or folders suitable for A-size
          stationery.</item>
          <item>{ 'Ansi' letter }: [ANSI/ASME Y14.1] ANSI series: US engineering drawing series.</item>
          <item>{ 'Arch' letter }: Architectural series.</item>
          <item>{ "Letter", "Legal", "Executive", "Statement", "Tabloid" }: Traditional north-american
          sizes.</item>
        </list>
      </remarks>
    */
    public enum SizeEnum
    {
      A0,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,
      B0,B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,
      C0,C1,C2,C3,C4,C5,C6,C7,C8,C9,C10,
      Letter,
      Legal,
      Executive,
      Statement,
      Tabloid,
      ArchA, ArchB, ArchC, ArchD, ArchE,
      AnsiA, AnsiB, AnsiC, AnsiD, AnsiE
    };

    /**
      <summary>Page orientation.</summary>
    */
    public enum OrientationEnum
    {
      Portrait,
      Landscape
    }
    #endregion

    #region static
    #region fields
    private static readonly string IsoSeriesSize_A = "A";
    private static readonly string IsoSeriesSize_B = "B";
    private static readonly string IsoSeriesSize_C = "C";

    private static readonly Regex IsoSeriesSizePattern = new Regex(
      "(["
        + IsoSeriesSize_A
        + IsoSeriesSize_B
        + IsoSeriesSize_C
        + "])([\\d]+)"
      );
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the default page size.</summary>
      <remarks>The returned dimension corresponds to the widely-established ISO A4 standard paper
      format, portrait orientation.</remarks>
    */
    public static Size GetSize(
      )
    {return GetSize(SizeEnum.A4);}

    /**
      <summary>Gets the page size of the given format, portrait orientation.</summary>
      <param name="size">Page size.</param>
    */
    public static Size GetSize(
      SizeEnum size
      )
    {return GetSize(size,OrientationEnum.Portrait);}

    /**
      <summary>Gets the page size of the given format and orientation.</summary>
      <param name="size">Page size.</param>
      <param name="orientation">Page orientation.</param>
    */
    public static Size GetSize(
      SizeEnum size,
      OrientationEnum orientation
      )
    {
      int width, height = 0;

      // Size.
      {
        string sizeName = size.ToString();
        Match match = IsoSeriesSizePattern.Match(sizeName);
        // Is it an ISO standard size?
        if(match.Success)
        {
          int baseWidth, baseHeight = 0;
          string isoSeriesSize = match.Groups[1].Value;
          if(isoSeriesSize.Equals(IsoSeriesSize_A))
          {baseWidth = 2384; baseHeight = 3370;}
          else if(isoSeriesSize.Equals(IsoSeriesSize_B))
          {baseWidth = 2834; baseHeight = 4008;}
          else if(isoSeriesSize.Equals(IsoSeriesSize_C))
          {baseWidth = 2599; baseHeight = 3676;}
          else
          {throw new NotImplementedException("Paper format " + size + " not supported yet.");}

          int isoSeriesSizeIndex = Int32.Parse(match.Groups[2].Value);
          double isoSeriesSizeFactor = 1 / Math.Pow(2,isoSeriesSizeIndex/2d);

          width = (int)Math.Floor(baseWidth * isoSeriesSizeFactor);
          height = (int)Math.Floor(baseHeight * isoSeriesSizeFactor);
        }
        else // Non-ISO size.
        {
          switch(size)
          {
            case SizeEnum.ArchA: width = 648; height = 864; break;
            case SizeEnum.ArchB: width = 864; height = 1296; break;
            case SizeEnum.ArchC: width = 1296; height = 1728; break;
            case SizeEnum.ArchD: width = 1728; height = 2592; break;
            case SizeEnum.ArchE: width = 2592; height = 3456; break;
            case SizeEnum.AnsiA: case SizeEnum.Letter: width = 612; height = 792; break;
            case SizeEnum.AnsiB: case SizeEnum.Tabloid: width = 792; height = 1224; break;
            case SizeEnum.AnsiC: width = 1224; height = 1584; break;
            case SizeEnum.AnsiD: width = 1584; height = 2448; break;
            case SizeEnum.AnsiE: width = 2448; height = 3168; break;
            case SizeEnum.Legal: width = 612; height = 1008; break;
            case SizeEnum.Executive: width = 522; height = 756; break;
            case SizeEnum.Statement: width = 396; height = 612; break;
            default: throw new NotImplementedException("Paper format " + size + " not supported yet.");
          }
        }
      }

      // Orientation.
      switch(orientation)
      {
        case OrientationEnum.Portrait:
          return new Size(width,height);
        case OrientationEnum.Landscape:
          return new Size(height,width);
        default:
          throw new NotImplementedException("Orientation " + orientation + " not supported yet.");
      }
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    private PageFormat(
      )
    {}
    #endregion
    #endregion
  }
}