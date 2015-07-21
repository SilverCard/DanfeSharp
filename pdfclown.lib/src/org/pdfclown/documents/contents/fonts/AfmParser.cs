/*
  Copyright 2009-2011 Stefano Chizzolini. http://www.pdfclown.org

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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace org.pdfclown.documents.contents.fonts
{
  /**
    <summary>AFM file format parser [AFM:4.1].</summary>
  */
  public sealed class AfmParser
  {
    #region types
    /**
      <summary>Font header (Global font information).</summary>
    */
    public sealed class FontMetrics
    {
      public bool IsCustomEncoding;
      public string FontName;
      public string Weight;
      public float ItalicAngle;
      public bool IsFixedPitch;
      public short XMin;
      public short YMin;
      public short XMax;
      public short YMax;
      public short UnderlinePosition;
      public short UnderlineThickness;
      public short CapHeight;
      public short XHeight;
      public short Ascender;
      public short Descender;
      public short StemH;
      public short StemV;
    }
    #endregion

    #region dynamic
    #region fields
    public FontMetrics Metrics;

    public Dictionary<int,int> GlyphIndexes;
    public Dictionary<int,int> GlyphKernings;
    public Dictionary<int,int> GlyphWidths;

    public IInputStream FontData;
    #endregion

    #region constructors
    internal AfmParser(
      IInputStream fontData
      )
    {
      FontData = fontData;

      Load();
    }
    #endregion

    #region interface
    #region private
    private void Load(
      )
    {
      Metrics = new FontMetrics();
      LoadFontHeader();
      LoadCharMetrics();
      LoadKerningData();
    }

    /**
      <summary>Loads the font header [AFM:4.1:3,4,4.1,4.2].</summary>
    */
    private void LoadFontHeader(
      )
    {
      string line;
      Regex linePattern = new Regex("(\\S+)\\s+(.+)");
      while((line = FontData.ReadLine()) != null)
      {
        MatchCollection lineMatches = linePattern.Matches(line);
        if(lineMatches.Count < 1)
          continue;

        Match lineMatch = lineMatches[0];

        string key = lineMatch.Groups[1].Value;
        switch(key)
        {
          case "FontName":
            Metrics.FontName = lineMatch.Groups[2].Value; break;
          case "Weight":
            Metrics.Weight = lineMatch.Groups[2].Value; break;
          case "ItalicAngle":
            Metrics.ItalicAngle = Single.Parse(lineMatch.Groups[2].Value); break;
          case "IsFixedPitch":
            Metrics.IsFixedPitch = lineMatch.Groups[2].Value.Equals("true"); break;
          case "FontBBox":
            {
              string[] coordinates = Regex.Split(lineMatch.Groups[2].Value,"\\s+");
              Metrics.XMin = Int16.Parse(coordinates[0]);
              Metrics.YMin = Int16.Parse(coordinates[1]);
              Metrics.XMax = Int16.Parse(coordinates[2]);
              Metrics.YMax = Int16.Parse(coordinates[3]);
            } break;
          case "UnderlinePosition":
            Metrics.UnderlinePosition = Int16.Parse(lineMatch.Groups[2].Value); break;
          case "UnderlineThickness":
            Metrics.UnderlineThickness = Int16.Parse(lineMatch.Groups[2].Value); break;
          case "EncodingScheme":
            Metrics.IsCustomEncoding = lineMatch.Groups[2].Value.Equals("FontSpecific"); break;
          case "CapHeight":
            Metrics.CapHeight = Int16.Parse(lineMatch.Groups[2].Value); break;
          case "XHeight":
            Metrics.XHeight = Int16.Parse(lineMatch.Groups[2].Value); break;
          case "Ascender":
            Metrics.Ascender = Int16.Parse(lineMatch.Groups[2].Value); break;
          case "Descender":
            Metrics.Descender = Int16.Parse(lineMatch.Groups[2].Value); break;
          case "StdHW":
            Metrics.StemH = Int16.Parse(lineMatch.Groups[2].Value); break;
          case "StdVW":
            Metrics.StemV = Int16.Parse(lineMatch.Groups[2].Value); break;
          case "StartCharMetrics":
            goto endParsing;
        }
      }
endParsing:
      if(Metrics.Ascender == 0)
      {Metrics.Ascender = Metrics.YMax;}
      if(Metrics.Descender == 0)
      {Metrics.Descender = Metrics.YMin;}
    }

    /**
      <summary>Loads individual character metrics [AFM:4.1:3,4,4.4,8].</summary>
    */
    private void LoadCharMetrics(
      )
    {
      GlyphIndexes = new Dictionary<int,int>();
      GlyphWidths = new Dictionary<int,int>();

      string line;
      Regex linePattern = new Regex("C (\\S+) ; WX (\\S+) ; N (\\S+)");
      int implicitCharCode = short.MaxValue;
      while((line = FontData.ReadLine()) != null)
      {
        MatchCollection lineMatches = linePattern.Matches(line);
        if(lineMatches.Count < 1)
        {
          if(line.Equals("EndCharMetrics"))
            break;

          continue;
        }

        Match lineMatch = lineMatches[0];

        int charCode = Int32.Parse(lineMatch.Groups[1].Value);
        int width = Int32.Parse(lineMatch.Groups[2].Value);
        string charName = lineMatch.Groups[3].Value;
        if(charCode < 0)
        {
          if(charName == null)
            continue;

          charCode = ++implicitCharCode;
        }
        int code = (
          charName == null
              || Metrics.IsCustomEncoding
            ? charCode
            : GlyphMapping.NameToCode(charName).Value
          );

        GlyphIndexes[code] = charCode;
        GlyphWidths[charCode] = width;
      }
    }

    /**
      <summary>Loads kerning data [AFM:4.1:3,4,4.5,9].</summary>
    */
    private void LoadKerningData(
      )
    {
      GlyphKernings = new Dictionary<int,int>();

      string line;
      while((line = FontData.ReadLine()) != null)
      {
        if(line.StartsWith("StartKernPairs"))
          break;
      }

      Regex linePattern = new Regex("KPX (\\S+) (\\S+) (\\S+)");
      while((line = FontData.ReadLine()) != null)
      {
        MatchCollection lineMatches = linePattern.Matches(line);
        if(lineMatches.Count < 1)
        {
          if(line.Equals("EndKernPairs"))
            break;

          continue;
        }

        Match lineMatch = lineMatches[0];

        int code1 = GlyphMapping.NameToCode(lineMatch.Groups[1].Value).Value;
        int code2 = GlyphMapping.NameToCode(lineMatch.Groups[2].Value).Value;
        int pair = code1 << 16 + code2;
        int value = Int32.Parse(lineMatch.Groups[3].Value);

        GlyphKernings[pair] = value;
      }
    }
    #endregion
    #endregion
    #endregion
  }
}