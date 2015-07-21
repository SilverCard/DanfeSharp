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
using org.pdfclown.documents;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace org.pdfclown.documents.contents.fonts
{
  /**
    <summary>Adobe standard glyph mapping (unicode-encoding against glyph-naming)
    [PDF:1.6:D;AGL:2.0].</summary>
  */
  internal class GlyphMapping
  {
    private static Dictionary<string,int> codes = new Dictionary<string,int>();

    static GlyphMapping(
      )
    {Load();}

    public static int? NameToCode(
      string name
      )
    {int code; return codes.TryGetValue(name, out code) ? code : (int?)null;}

    /**
      <summary>Loads the glyph list mapping character names to character codes (unicode
      encoding).</summary>
    */
    private static void Load(
      )
    {
      StreamReader glyphListStream = null;
      try
      {
        // Open the glyph list!
        /*
          NOTE: The Adobe Glyph List [AGL:2.0] represents the reference name-to-unicode map
          for consumer applications.
        */
        glyphListStream = new StreamReader(
          Assembly.GetExecutingAssembly().GetManifestResourceStream("fonts.AGL20")
          );

        // Parsing the glyph list...
        string line;
        Regex linePattern = new Regex("^(\\w+);([A-F0-9]+)$");
        while((line = glyphListStream.ReadLine()) != null)
        {
          MatchCollection lineMatches = linePattern.Matches(line);
          if(lineMatches.Count < 1)
            continue;

          Match lineMatch = lineMatches[0];

          string name = lineMatch.Groups[1].Value;
          int code = Int32.Parse(
            lineMatch.Groups[2].Value,
            NumberStyles.HexNumber
            );

          // Associate the character name with its corresponding character code!
          codes[name] = code;
        }
      }
      finally
      {
        if(glyphListStream != null)
        {glyphListStream.Close();}
      }
    }
  }
}