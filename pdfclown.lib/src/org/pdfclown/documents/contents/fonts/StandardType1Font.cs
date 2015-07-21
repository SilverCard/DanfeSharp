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

using bytes = org.pdfclown.bytes;
using org.pdfclown.documents;
using org.pdfclown.files;
using org.pdfclown.objects;
using org.pdfclown.util;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace org.pdfclown.documents.contents.fonts
{
  /**
    <summary>Standard Type 1 font [PDF:1.6:5.5.1].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public sealed class StandardType1Font
    : Type1Font
  {
    #region types
    /**
      <summary>Standard Type 1 font families [PDF:1.6:5.5.1].</summary>
    */
    public enum FamilyEnum
    {
      Courier,
      Helvetica,
      Times,
      Symbol,
      ZapfDingbats
    };
    #endregion

    #region static
    #region interface
    #region private
    private static bool IsSymbolic(
      FamilyEnum value
      )
    {
      switch(value)
      {
        case FamilyEnum.Courier:
        case FamilyEnum.Helvetica:
        case FamilyEnum.Times:
          return false;
        case FamilyEnum.Symbol:
        case FamilyEnum.ZapfDingbats:
          return true;
        default:
          throw new NotImplementedException();
      }
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public StandardType1Font(
      Document context,
      FamilyEnum family,
      bool bold,
      bool italic
      ) : base(context)
    {
      string fontName = family.ToString();
      switch(family)
      {
        case(FamilyEnum.Symbol):
        case(FamilyEnum.ZapfDingbats):
          break;
        case(FamilyEnum.Times):
          if(bold)
          {
            fontName += "-Bold";
            if(italic)
            {fontName += "Italic";}
          }
          else if(italic)
          {fontName += "-Italic";}
          else
          {fontName += "-Roman";}
          break;
        default:
          if(bold)
          {
            fontName += "-Bold";
            if(italic)
            {fontName += "Oblique";}
          }
          else if(italic)
          {fontName += "-Oblique";}
          break;
      }
      PdfName encodingName = (IsSymbolic(family) ? null : PdfName.WinAnsiEncoding);

      Create(fontName,encodingName);
    }

    internal StandardType1Font(
      PdfDirectObject baseObject
      ) : base(baseObject)
    {}
    #endregion

    #region interface
    #region public
    public override double Ascent
    {
      get
      {return metrics.Ascender;}
    }

    public override double Descent
    {
      get
      {return metrics.Descender;}
    }

    public override FlagsEnum Flags
    {
      //TODO:IMPL!!!
      get
      {return 0;}
    }
    #endregion

    #region protected
    protected override IDictionary<ByteArray,int> GetNativeEncoding(
      )
    {
      if(symbolic) // Symbolic font.
      {
        Dictionary<ByteArray,int> codes = new Dictionary<ByteArray,int>();
        foreach(KeyValuePair<int,int> glyphIndexEntry in glyphIndexes)
        {
          codes[
            new ByteArray(new byte[]{ConvertUtils.IntToByteArray(glyphIndexEntry.Value)[3]})
            ] = glyphIndexEntry.Key;
        }
        return codes;
      }
      else // Nonsymbolic font.
        return Encoding.Get(PdfName.StandardEncoding).GetCodes();
    }

    protected override void OnLoad(
      )
    {
      /*
        NOTE: Standard Type 1 fonts ordinarily omit their descriptor;
        otherwise, when overridden they degrade to a common Type 1 font.
        Metrics of non-overridden Standard Type 1 fonts MUST be loaded from resources.
      */
      Load(((PdfName)BaseDataObject[PdfName.BaseFont]).StringValue);

      base.OnLoad();
    }
    #endregion

    #region private
    /**
      <summary>Creates the font structures.</summary>
    */
    private void Create(
      string fontName,
      PdfName encodingName
      )
    {
      /*
        NOTE: Standard Type 1 fonts SHOULD omit extended font descriptions [PDF:1.6:5.5.1].
      */
      // Subtype.
      BaseDataObject[PdfName.Subtype] = PdfName.Type1;
      // BaseFont.
      BaseDataObject[PdfName.BaseFont] = new PdfName(fontName);
      // Encoding.
      if(encodingName != null)
      {BaseDataObject[PdfName.Encoding] = encodingName;}

      Load();
    }

    /**
      <summary>Loads the font metrics.</summary>
    */
    private void Load(
      string fontName
      )
    {
      Stream fontMetricsStream = null;
      try
      {
        fontMetricsStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("fonts.afm." + fontName);

        AfmParser parser = new AfmParser(new bytes::Stream(fontMetricsStream));
        metrics = parser.Metrics;
        symbolic = metrics.IsCustomEncoding;
        glyphIndexes = parser.GlyphIndexes;
        glyphKernings = parser.GlyphKernings;
        glyphWidths = parser.GlyphWidths;
      }
      catch(Exception e)
      {throw new Exception("Failed to load '" + fontName + "'.",e);}
      finally
      {
        if(fontMetricsStream != null)
        {fontMetricsStream.Close();}
      }
    }
    #endregion
    #endregion
    #endregion
  }
}