/*
  Copyright 2007-2012 Stefano Chizzolini. http://www.pdfclown.org

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

using org.pdfclown.documents.contents.fonts;

using System;
using System.Text.RegularExpressions;

namespace org.pdfclown.documents.contents.composition
{
  /**
    <summary>Text fitter.</summary>
  */
  public sealed class TextFitter
  {
    #region dynamic
    #region fields
    private readonly Font font;
    private readonly double fontSize;
    private readonly bool hyphenation;
    private readonly char hyphenationCharacter;
    private readonly string text;
    private double width;

    private int beginIndex = 0;
    private int endIndex = -1;
    private string fittedText;
    private double fittedWidth;
    #endregion

    #region constructors
    public TextFitter(
      string text,
      double width,
      Font font,
      double fontSize,
      bool hyphenation,
      char hyphenationCharacter
      )
    {
      this.text = text;
      this.width = width;
      this.font = font;
      this.fontSize = fontSize;
      this.hyphenation = hyphenation;
      this.hyphenationCharacter = hyphenationCharacter;
    }
    #endregion

    #region interface
    #region public
    /**
      <summary>Fits the text inside the specified width.</summary>
      <param name="unspacedFitting">Whether fitting of unspaced text is allowed.</param>
      <returns>Whether the operation was successful.</returns>
    */
    public bool Fit(
      bool unspacedFitting
      )
    {
      return Fit(
        endIndex + 1,
        width,
        unspacedFitting
        );
    }

    /**
      <summary>Fits the text inside the specified width.</summary>
      <param name="index">Beginning index, inclusive.</param>
      <param name="width">Available width.</param>
      <param name="unspacedFitting">Whether fitting of unspaced text is allowed.</param>
      <returns>Whether the operation was successful.</returns>
    */
    public bool Fit(
      int index,
      double width,
      bool unspacedFitting
      )
    {
      beginIndex = index;
      this.width = width;

      fittedText = null;
      fittedWidth = 0;

      string hyphen = String.Empty;

      // Fitting the text within the available width...
      {
        Regex pattern = new Regex(@"(\s*)(\S*)");
        Match match = pattern.Match(text,beginIndex);
        while(match.Success)
        {
          // Scanning for the presence of a line break...
          {
            Group leadingWhitespaceGroup = match.Groups[1];
            /*
              NOTE: This text fitting algorithm returns everytime it finds a line break character,
              as it's intended to evaluate the width of just a single line of text at a time.
            */
            for(
              int spaceIndex = leadingWhitespaceGroup.Index,
                spaceEnd = leadingWhitespaceGroup.Index + leadingWhitespaceGroup.Length;
              spaceIndex < spaceEnd;
              spaceIndex++
              )
            {
              switch(text[spaceIndex])
              {
                case '\n':
                case '\r':
                  index = spaceIndex;
                  goto endFitting; // NOTE: I know GOTO is evil, but in this case using it sparingly avoids cumbersome boolean flag checks.
              }
            }
          }

          Group matchGroup = match.Groups[0];
          // Add the current word!
          int wordEndIndex = matchGroup.Index + matchGroup.Length; // Current word's limit.
          double wordWidth = font.GetWidth(matchGroup.Value, fontSize); // Current word's width.
          fittedWidth += wordWidth;
          // Does the fitted text's width exceed the available width?
          if(fittedWidth > width)
          {
            // Remove the current (unfitting) word!
            fittedWidth -= wordWidth;
            wordEndIndex = index;
            if(!hyphenation
              && (wordEndIndex > beginIndex // There's fitted content.
                || !unspacedFitting // There's no fitted content, but unspaced fitting isn't allowed.
                || text[beginIndex] == ' ') // Unspaced fitting is allowed, but text starts with a space.
              ) // Enough non-hyphenated text fitted.
              goto endFitting;

            /*
              NOTE: We need to hyphenate the current (unfitting) word.
            */
            Hyphenate(
              hyphenation,
              ref index,
              ref wordEndIndex,
              wordWidth,
              out hyphen
              );

            break;
          }
          index = wordEndIndex;

          match = match.NextMatch();
        }
      }
endFitting:
      fittedText = text.Substring(beginIndex, index - beginIndex) + hyphen;
      endIndex = index;

      return (fittedWidth > 0);
    }

    /**
      <summary>Gets the begin index of the fitted text inside the available text.</summary>
    */
    public int BeginIndex
    {
      get
      {return beginIndex;}
    }

    /**
      <summary>Gets the end index of the fitted text inside the available text.</summary>
    */
    public int EndIndex
    {
      get
      {return endIndex;}
    }

    /**
      <summary>Gets the fitted text.</summary>
    */
    public string FittedText
    {
      get
      {return fittedText;}
    }

    /**
      <summary>Gets the fitted text's width.</summary>
    */
    public double FittedWidth
    {
      get
      {return fittedWidth;}
    }

    /**
      <summary>Gets the font used to fit the text.</summary>
    */
    public Font Font
    {
      get
      {return font;}
    }

    /**
      <summary>Gets the size of the font used to fit the text.</summary>
    */
    public double FontSize
    {
      get
      {return fontSize;}
    }

    /**
      <summary>Gets whether the hyphenation algorithm has to be applied.</summary>
    */
    public bool Hyphenation
    {
      get
      {return hyphenation;}
    }

    /**
      <summary>Gets/Sets the character shown at the end of the line before a hyphenation break.
      </summary>
    */
    public char HyphenationCharacter
    {
      get
      {return hyphenationCharacter;}
    }

    /**
      <summary>Gets the available text.</summary>
    */
    public string Text
    {
      get
      {return text;}
    }

    /**
      <summary>Gets the available width.</summary>
    */
    public double Width
    {
      get
      {return width;}
    }
    #endregion

    #region private
    private  void Hyphenate(
      bool hyphenation,
      ref int index,
      ref int wordEndIndex,
      double wordWidth,
      out string hyphen
      )
    {
      /*
        TODO: This hyphenation algorithm is quite primitive (to improve!).
      */
      while(true)
      {
        // Add the current character!
        char textChar = text[wordEndIndex];
        wordWidth = font.GetWidth(textChar, fontSize);
        wordEndIndex++;
        fittedWidth += wordWidth;
        // Does the fitted text's width exceed the available width?
        if(fittedWidth > width)
        {
          // Remove the current character!
          fittedWidth -= wordWidth;
          wordEndIndex--;
          if(hyphenation)
          {
            // Is hyphenation to be applied?
            if(wordEndIndex > index + 4) // Long-enough word chunk.
            {
              // Make room for the hyphen character!
              wordEndIndex--;
              index = wordEndIndex;
              textChar = text[wordEndIndex];
              fittedWidth -= font.GetWidth(textChar, fontSize);

              // Add the hyphen character!
              textChar = hyphenationCharacter;
              fittedWidth += font.GetWidth(textChar, fontSize);

              hyphen = textChar.ToString();
            }
            else // No hyphenation.
            {
              // Removing the current word chunk...
              while(wordEndIndex > index)
              {
                wordEndIndex--;
                textChar = text[wordEndIndex];
                fittedWidth -= font.GetWidth(textChar, fontSize);
              }

              hyphen = String.Empty;
            }
          }
          else
          {
            index = wordEndIndex;
            
            hyphen = String.Empty;
          }
          break;
        }
      }
    }
    #endregion
    #endregion
    #endregion
  }
}