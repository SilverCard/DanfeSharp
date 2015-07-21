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

using org.pdfclown.documents.contents;
using org.pdfclown.documents.contents.objects;
using org.pdfclown.util.math;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace org.pdfclown.tools
{
  /**
    <summary>Tool for extracting text from <see cref="IContentContext">content contexts</see>.</summary>
  */
  public sealed class TextExtractor
  {
    #region types
    /**
      <summary>Text-to-area matching mode.</summary>
    */
    public enum AreaModeEnum
    {
      /**
        <summary>Text string must be contained by the area.</summary>
      */
      Containment,
      /**
        <summary>Text string must intersect the area.</summary>
      */
      Intersection
    }

    /**
      <summary>Text filter by interval.</summary>
      <remarks>Iterated intervals MUST be ordered.</remarks>
    */
    public interface IIntervalFilter
      : IEnumerator<Interval<int>>
    {
      /**
        <summary>Notifies current matching.</summary>
        <param name="interval">Current interval.</param>
        <param name="match">Text string matching the current interval.</param>
      */
      void Process(
        Interval<int> interval,
        ITextString match
        );
    }

    private class IntervalFilter
      : IIntervalFilter
    {
      private IList<Interval<int>> intervals;

      private IList<ITextString> textStrings = new List<ITextString>();
      private int index = 0;

      public IntervalFilter(
        IList<Interval<int>> intervals
        )
      {this.intervals = intervals;}

      public Interval<int> Current
      {
        get
        {return intervals[index];}
      }

      object IEnumerator.Current
      {
        get
        {return this.Current;}
      }

      public void Dispose(
        )
      {/* NOOP */}

      public bool MoveNext(
        )
      {return (++index < intervals.Count);}

      public void Process(
        Interval<int> interval,
        ITextString match
        )
      {textStrings.Add(match);}

      public void Reset(
        )
      {throw new NotSupportedException();}

      public IList<ITextString> TextStrings
      {
        get
        {return textStrings;}
      }
    }

    /**
      <summary>Text string.</summary>
      <remarks>This is typically used to assemble contiguous raw text strings
      laying on the same line.</remarks>
    */
    private class TextString
      : ITextString
    {
      private List<TextChar> textChars = new List<TextChar>();

      public RectangleF? Box
      {
        get
        {
          RectangleF? box = null;
          foreach(TextChar textChar in textChars)
          {
            if(!box.HasValue)
            {box = (RectangleF?)textChar.Box;}
            else
            {box = RectangleF.Union(box.Value,textChar.Box);}
          }
          return box;
        }
      }

      public string Text
      {
        get
        {
          StringBuilder textBuilder = new StringBuilder();
          foreach(TextChar textChar in textChars)
          {textBuilder.Append(textChar);}
          return textBuilder.ToString();
        }
      }

      public List<TextChar> TextChars
      {
        get
        {return textChars;}
      }
    }

    /**
      <summary>Text string position comparer.</summary>
    */
    private class TextStringPositionComparer<T>
      : IComparer<T>
      where T : ITextString
    {
      #region static
      /**
        <summary>Gets whether the specified boxes lay on the same text line.</summary>
      */
      public static bool IsOnTheSameLine(
        RectangleF box1,
        RectangleF box2
        )
      {
        /*
          NOTE: In order to consider the two boxes being on the same line,
          we apply a simple rule of thumb: at least 25% of a box's height MUST
          lay on the horizontal projection of the other one.
        */
        double minHeight = Math.Min(box1.Height, box2.Height);
        double yThreshold = minHeight * .75;
        return ((box1.Y > box2.Y - yThreshold
            && box1.Y < box2.Bottom + yThreshold - minHeight)
          || (box2.Y > box1.Y - yThreshold
            && box2.Y < box1.Bottom + yThreshold - minHeight));
      }
      #endregion

      #region dynamic
      #region IComparer
      public int Compare(
        T textString1,
        T textString2
        )
      {
        RectangleF box1 = textString1.Box.Value;
        RectangleF box2 = textString2.Box.Value;
        if(IsOnTheSameLine(box1,box2))
        {
          if(box1.X < box2.X)
            return -1;
          else if(box1.X > box2.X)
            return 1;
          else
            return 0;
        }
        else if(box1.Y < box2.Y)
          return -1;
        else
          return 1;
      }
      #endregion
      #endregion
    }
    #endregion

    #region static
    #region fields
    public static readonly RectangleF DefaultArea = new RectangleF(0,0,0,0);
    #endregion

    #region interface
    #region public
    /**
      <summary>Converts text information into plain text.</summary>
      <param name="textStrings">Text information to convert.</param>
      <returns>Plain text.</returns>
    */
    public static string ToString(
      IDictionary<RectangleF?,IList<ITextString>> textStrings
      )
    {return ToString(textStrings, "", "");}

    /**
      <summary>Converts text information into plain text.</summary>
      <param name="textStrings">Text information to convert.</param>
      <param name="lineSeparator">Separator to apply on line break.</param>
      <param name="areaSeparator">Separator to apply on area break.</param>
      <returns>Plain text.</returns>
    */
    public static string ToString(
      IDictionary<RectangleF?,IList<ITextString>> textStrings,
      string lineSeparator,
      string areaSeparator
      )
    {
      StringBuilder textBuilder = new StringBuilder();
      foreach(IList<ITextString> areaTextStrings in textStrings.Values)
      {
        if(textBuilder.Length > 0)
        {textBuilder.Append(areaSeparator);}

        foreach(ITextString textString in areaTextStrings)
        {textBuilder.Append(textString.Text).Append(lineSeparator);}
      }
      return textBuilder.ToString();
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region fields
    private AreaModeEnum areaMode = AreaModeEnum.Containment;
    private List<RectangleF> areas;
    private float areaTolerance = 0;
    private bool dehyphenated;
    private bool sorted;
    #endregion

    #region constructors
    public TextExtractor(
      ) : this(true, false)
    {}

    public TextExtractor(
      bool sorted,
      bool dehyphenated
      ) : this(null, sorted, dehyphenated)
    {}

    public TextExtractor(
      IList<RectangleF> areas,
      bool sorted,
      bool dehyphenated
      )
    {
      Areas = areas;
      Dehyphenated = dehyphenated;
      Sorted = sorted;
    }
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the text-to-area matching mode.</summary>
    */
    public AreaModeEnum AreaMode
    {
      get{return areaMode;}
      set{areaMode = value;}
    }

    /**
      <summary>Gets the graphic areas whose text has to be extracted.</summary>
    */
    public IList<RectangleF> Areas
    {
      get
      {return areas;}
      set
      {areas = (value == null ? new List<RectangleF>() : new List<RectangleF>(value));}
    }

    /**
      <summary>Gets the admitted outer area (in points) for containment matching purposes.</summary>
      <remarks>This measure is useful to ensure that text whose boxes overlap with the area bounds
      is not excluded from the match.</remarks>
    */
    public float AreaTolerance
    {
      get
      {return areaTolerance;}
      set
      {areaTolerance = value;}
    }

    /**
      <summary>Gets/Sets whether the text strings have to be dehyphenated.</summary>
    */
    public bool Dehyphenated
    {
      get
      {return dehyphenated;}
      set
      {
        dehyphenated = value;
        if(dehyphenated)
        {Sorted = true;}
      }
    }

    /**
      <summary>Extracts text strings from the specified content context.</summary>
      <param name="contentContext">Source content context.</param>
    */
    public IDictionary<RectangleF?,IList<ITextString>> Extract(
      IContentContext contentContext
      )
    {
      IDictionary<RectangleF?,IList<ITextString>> extractedTextStrings;
      {
        List<ITextString> textStrings = new List<ITextString>();
        {
          // 1. Extract the source text strings!
          List<ContentScanner.TextStringWrapper> rawTextStrings = new List<ContentScanner.TextStringWrapper>();
          Extract(
            new ContentScanner(contentContext),
            rawTextStrings
            );

          // 2. Sort the target text strings!
          if(sorted)
          {Sort(rawTextStrings,textStrings);}
          else
          {
            foreach(ContentScanner.TextStringWrapper rawTextString in rawTextStrings)
            {textStrings.Add(rawTextString);}
          }
        }

        // 3. Filter the target text strings!
        if(areas.Count == 0)
        {
          extractedTextStrings = new Dictionary<RectangleF?,IList<ITextString>>();
          extractedTextStrings[DefaultArea] = textStrings;
        }
        else
        {extractedTextStrings = Filter(textStrings,areas.ToArray());}
      }
      return extractedTextStrings;
    }

    /**
      <summary>Extracts text strings from the specified contents.</summary>
      <param name="contents">Source contents.</param>
    */
    public IDictionary<RectangleF?,IList<ITextString>> Extract(
      Contents contents
      )
    {return Extract(contents.ContentContext);}

    /**
      <summary>Gets the text strings matching the specified intervals.</summary>
      <param name="textStrings">Text strings to filter.</param>
      <param name="intervals">Text intervals to match. They MUST be ordered and not overlapping.</param>
      <returns>A list of text strings corresponding to the specified intervals.</returns>
    */
    public IList<ITextString> Filter(
      IDictionary<RectangleF?,IList<ITextString>> textStrings,
      IList<Interval<int>> intervals
      )
    {
      IntervalFilter filter = new IntervalFilter(intervals);
      Filter(textStrings, filter);
      return filter.TextStrings;
    }
  
    /**
      <summary>Processes the text strings matching the specified filter.</summary>
      <param name="textStrings">Text strings to filter.</param>
      <param name="filter">Matching processor.</param>
    */
    public void Filter(
      IDictionary<RectangleF?,IList<ITextString>> textStrings,
      IIntervalFilter filter
      )
    {
      IEnumerator<IList<ITextString>> textStringsIterator = textStrings.Values.GetEnumerator();
      if(!textStringsIterator.MoveNext())
        return;

      IEnumerator<ITextString> areaTextStringsIterator = textStringsIterator.Current.GetEnumerator();
      if(!areaTextStringsIterator.MoveNext())
        return;

      IList<TextChar> textChars = areaTextStringsIterator.Current.TextChars;
      int baseTextCharIndex = 0;
      int textCharIndex = 0;
      while(filter.MoveNext())
      {
        Interval<int> interval = filter.Current;
        TextString match = new TextString();
        {
          int matchStartIndex = interval.Low;
          int matchEndIndex = interval.High;
          while(matchStartIndex > baseTextCharIndex + textChars.Count)
          {
            baseTextCharIndex += textChars.Count;
            if(!areaTextStringsIterator.MoveNext())
            {areaTextStringsIterator = textStringsIterator.Current.GetEnumerator(); areaTextStringsIterator.MoveNext();}
            textChars = areaTextStringsIterator.Current.TextChars;
          }
          textCharIndex = matchStartIndex - baseTextCharIndex;
  
          while(baseTextCharIndex + textCharIndex < matchEndIndex)
          {
            if(textCharIndex == textChars.Count)
            {
              baseTextCharIndex += textChars.Count;
              if(!areaTextStringsIterator.MoveNext())
              {areaTextStringsIterator = textStringsIterator.Current.GetEnumerator(); areaTextStringsIterator.MoveNext();}
              textChars = areaTextStringsIterator.Current.TextChars;
              textCharIndex = 0;
            }
            match.TextChars.Add(textChars[textCharIndex++]);
          }
        }
        filter.Process(interval, match);
      }
    }

    /**
      <summary>Gets the text strings matching the specified area.</summary>
      <param name="textStrings">Text strings to filter, grouped by source area.</param>
      <param name="area">Graphic area which text strings have to be matched to.</param>
    */
    public IList<ITextString> Filter(
      IDictionary<RectangleF?,IList<ITextString>> textStrings,
      RectangleF area
      )
    {return Filter(textStrings,new RectangleF[]{area})[area];}

    /**
      <summary>Gets the text strings matching the specified areas.</summary>
      <param name="textStrings">Text strings to filter, grouped by source area.</param>
      <param name="areas">Graphic areas which text strings have to be matched to.</param>
    */
    public IDictionary<RectangleF?,IList<ITextString>> Filter(
      IDictionary<RectangleF?,IList<ITextString>> textStrings,
      params RectangleF[] areas
      )
    {
      IDictionary<RectangleF?,IList<ITextString>> filteredTextStrings = null;
      foreach(IList<ITextString> areaTextStrings in textStrings.Values)
      {
        IDictionary<RectangleF?,IList<ITextString>> filteredAreasTextStrings = Filter(areaTextStrings,areas);
        if(filteredTextStrings == null)
        {filteredTextStrings = filteredAreasTextStrings;}
        else
        {
          foreach(KeyValuePair<RectangleF?,IList<ITextString>> filteredAreaTextStringsEntry in filteredAreasTextStrings)
          {
            IList<ITextString> filteredTextStringsList = filteredTextStrings[filteredAreaTextStringsEntry.Key];
            foreach(ITextString filteredAreaTextString in filteredAreaTextStringsEntry.Value)
            {filteredTextStringsList.Add(filteredAreaTextString);}
          }
        }
      }
      return filteredTextStrings;
    }

    /**
      <summary>Gets the text strings matching the specified area.</summary>
      <param name="textStrings">Text strings to filter.</param>
      <param name="area">Graphic area which text strings have to be matched to.</param>
    */
    public IList<ITextString> Filter(
      IList<ITextString> textStrings,
      RectangleF area
      )
    {return Filter(textStrings,new RectangleF[]{area})[area];}

    /**
      <summary>Gets the text strings matching the specified areas.</summary>
      <param name="textStrings">Text strings to filter.</param>
      <param name="areas">Graphic areas which text strings have to be matched to.</param>
    */
    public IDictionary<RectangleF?,IList<ITextString>> Filter(
      IList<ITextString> textStrings,
      params RectangleF[] areas
      )
    {
      IDictionary<RectangleF?,IList<ITextString>> filteredAreasTextStrings = new Dictionary<RectangleF?,IList<ITextString>>();
      foreach(RectangleF area in areas)
      {
        IList<ITextString> filteredAreaTextStrings = new List<ITextString>();
        filteredAreasTextStrings[area] = filteredAreaTextStrings;
        RectangleF toleratedArea = (areaTolerance != 0
          ? new RectangleF(
            area.X - areaTolerance,
            area.Y - areaTolerance,
            area.Width + areaTolerance * 2,
            area.Height + areaTolerance * 2
            )
          : area);
        foreach(ITextString textString in textStrings)
        {
          RectangleF? textStringBox = textString.Box;
          if(toleratedArea.IntersectsWith(textStringBox.Value))
          {
            TextString filteredTextString = new TextString();
            List<TextChar> filteredTextStringChars = filteredTextString.TextChars;
            foreach(TextChar textChar in textString.TextChars)
            {
              RectangleF textCharBox = textChar.Box;
              if((areaMode == AreaModeEnum.Containment && toleratedArea.Contains(textCharBox))
                || (areaMode == AreaModeEnum.Intersection && toleratedArea.IntersectsWith(textCharBox)))
              {filteredTextStringChars.Add(textChar);}
            }
            filteredAreaTextStrings.Add(filteredTextString);
          }
        }
      }
      return filteredAreasTextStrings;
    }

    /**
      <summary>Gets/Sets whether the text strings have to be sorted.</summary>
    */
    public bool Sorted
    {
      get
      {return sorted;}
      set
      {
        sorted = value;
        if(!sorted)
        {Dehyphenated = false;}
      }
    }
    #endregion

    #region private
    /**
      <summary>Scans a content level looking for text.</summary>
    */
    private void Extract(
      ContentScanner level,
      IList<ContentScanner.TextStringWrapper> extractedTextStrings
      )
    {
      if(level == null)
        return;

      while(level.MoveNext())
      {
        ContentObject content = level.Current;
        if(content is Text)
        {
          // Collect the text strings!
          foreach(ContentScanner.TextStringWrapper textString in ((ContentScanner.TextWrapper)level.CurrentWrapper).TextStrings)
          {extractedTextStrings.Add(textString);}
        }
        else if(content is XObject)
        {
          // Scan the external level!
          Extract(
            ((XObject)content).GetScanner(level),
            extractedTextStrings
            );
        }
        else if(content is ContainerObject)
        {
          // Scan the inner level!
          Extract(
            level.ChildLevel,
            extractedTextStrings
            );
        }
      }
    }

    /**
      <summary>Sorts the extracted text strings.</summary>
      <remarks>Sorting implies text position ordering, integration and aggregation.</remarks>
      <param name="rawTextStrings">Source (lower-level) text strings.</param>
      <param name="textStrings">Target (higher-level) text strings.</param>
    */
    private void Sort(
      List<ContentScanner.TextStringWrapper> rawTextStrings,
      List<ITextString> textStrings
      )
    {
      // Sorting the source text strings...
      {
        TextStringPositionComparer<ContentScanner.TextStringWrapper> positionComparator = new TextStringPositionComparer<ContentScanner.TextStringWrapper>();
        rawTextStrings.Sort(positionComparator);
      }

      // Aggregating and integrating the source text strings into the target ones...
      TextString textString = null;
      TextStyle textStyle = null;
      TextChar previousTextChar = null;
      bool dehyphenating = false;
      foreach(ContentScanner.TextStringWrapper rawTextString in rawTextStrings)
      {
        /*
          NOTE: Contents on the same line are grouped together within the same text string.
        */
        // Add a new text string in case of new line!
        if(textString != null
          && textString.TextChars.Count > 0
          && !TextStringPositionComparer<ITextString>.IsOnTheSameLine(
            textString.Box.Value,
            rawTextString.Box.Value
            ))
        {
          if(dehyphenated
            && previousTextChar.Value == '-') // Hyphened word.
          {
            textString.TextChars.Remove(previousTextChar);
            dehyphenating = true;
          }
          else // Full word.
          {
            // Add synthesized space character!
            textString.TextChars.Add(
              new TextChar(
                ' ',
                new RectangleF(
                  previousTextChar.Box.Right,
                  previousTextChar.Box.Top,
                  0,
                  previousTextChar.Box.Height
                  ),
                textStyle,
                true
                )
              );
            textString = null;
            dehyphenating = false;
          }
          previousTextChar = null;
        }
        if(textString == null)
        {textStrings.Add(textString = new TextString());}

        textStyle = rawTextString.Style;
        double spaceWidth = textStyle.Font.GetWidth(' ', textStyle.FontSize);
        if(spaceWidth == 0)
        {spaceWidth = textStyle.FontSize * .25f;} // NOTE: as a rule of thumb, space width is estimated according to the font size.
        foreach(TextChar textChar in rawTextString.TextChars)
        {
          if(previousTextChar != null)
          {
            /*
              NOTE: PDF files may have text contents omitting space characters,
              so they must be inferred and synthesized, marking them as virtual
              in order to allow the user to distinguish between original contents
              and augmented ones.
            */
            float characterSpace = textChar.Box.X - previousTextChar.Box.Right;
            if(characterSpace >= spaceWidth)
            {
              // Add synthesized space character!
              textString.TextChars.Add(
                previousTextChar = new TextChar(
                  ' ',
                  new RectangleF(
                    previousTextChar.Box.Right,
                    textChar.Box.Y,
                    characterSpace,
                    textChar.Box.Height
                    ),
                  textStyle,
                  true
                  )
                );
            }
            if(dehyphenating
              && previousTextChar.Value == ' ')
            {
              textStrings.Add(textString = new TextString());
              dehyphenating = false;
            }
          }
          textString.TextChars.Add(previousTextChar = textChar);
        }
      }
    }
    #endregion
    #endregion
    #endregion
  }
}
