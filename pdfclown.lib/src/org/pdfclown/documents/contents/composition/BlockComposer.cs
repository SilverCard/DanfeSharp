/*
  Copyright 2007-2012 Stefano Chizzolini. http://www.pdfclown.org

  Contributors:
    * Stefano Chizzolini (original code developer, http://www.stefanochizzolini.it):
      - enhancement of [MG]'s line alignment implementation.
    * Manuel Guilbault (code contributor, manuel.guilbault@gmail.com):
      - line alignment.

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
using fonts = org.pdfclown.documents.contents.fonts;
using org.pdfclown.documents.contents.objects;
using xObjects = org.pdfclown.documents.contents.xObjects;
using org.pdfclown.objects;
using org.pdfclown.util.math;

using System;
using System.Collections.Generic;
using System.Drawing;

namespace org.pdfclown.documents.contents.composition
{
  /**
    <summary>
      <para>Content block composer.</para>
      <para>It provides content positioning functionalities for page typesetting.</para>
    </summary>
  */
  /*
    TODO: Manage all the graphics parameters (especially
    those text-related, like horizontal scaling etc.) using ContentScanner -- see PDF:1.6:5.2-3!!!
  */
  public sealed class BlockComposer
  {
    #region types
    private sealed class ContentPlaceholder
      : Operation
    {
      public List<ContentObject> objects = new List<ContentObject>();

      public ContentPlaceholder(
        ) : base(null)
      {}

      public List<ContentObject> Objects
      {
        get
        {return objects;}
      }

      public override void WriteTo(
        IOutputStream stream,
        Document context
        )
      {
        foreach(ContentObject obj in objects)
        {obj.WriteTo(stream, context);}
      }
    }

    private sealed class Row
    {
      /**
        <summary>Row base line.</summary>
      */
      public double BaseLine;
      /**
        <summary>Row's graphics objects container.</summary>
      */
      public ContentPlaceholder Container;
      public double Height;
      /**
        <summary>Row's objects.</summary>
      */
      public List<RowObject> Objects = new List<RowObject>();
      /**
        <summary>Number of space characters.</summary>
      */
      public int SpaceCount = 0;
      public double Width;
      /**
        <summary>Vertical location relative to the block frame.</summary>
      */
      public double Y;

      internal Row(
        ContentPlaceholder container,
        double y
        )
      {
        this.Container = container;
        this.Y = y;
      }
    }

    private sealed class RowObject
    {
      public enum TypeEnum
      {
        Text,
        XObject
      }

      /**
        <summary>Base line.</summary>
      */
      public double BaseLine;
      /**
        <summary>Graphics objects container associated to this object.</summary>
      */
      public ContainerObject Container;
      public double Height;
      /**
        <summary>Line alignment (can be either LineAlignmentEnum or Double).</summary>
      */
      public object LineAlignment;
      public int SpaceCount;
      public TypeEnum Type;
      public double Width;

      internal RowObject(
        TypeEnum type,
        ContainerObject container,
        double height,
        double width,
        int spaceCount,
        object lineAlignment,
        double baseLine
        )
      {
        Type = type;
        Container = container;
        Height = height;
        Width = width;
        SpaceCount = spaceCount;
        LineAlignment = lineAlignment;
        BaseLine = baseLine;
      }
    }
    #endregion

    #region dynamic
    /*
      NOTE: In order to provide fine-grained alignment,
      there are 2 postproduction state levels:
        1- row level (see EndRow());
        2- block level (see End()).

      NOTE: Graphics instructions' layout follows this scheme (XS-BNF syntax):
        block = { beginLocalState translation parameters rows endLocalState }
        beginLocalState { "q\r" }
        translation = { "1 0 0 1 " number ' ' number "cm\r" }
        parameters = { ... } // Graphics state parameters.
        rows = { row* }
        row = { object* }
        object = { parameters beginLocalState translation content endLocalState }
        content = { ... } // Text, image (and so on) showing operators.
        endLocalState = { "Q\r" }
      NOTE: all the graphics state parameters within a block are block-level ones,
      i.e. they can't be represented inside row's or row object's local state, in order to
      facilitate parameter reuse within the same block.
    */
    #region fields
    private readonly PrimitiveComposer baseComposer;
    private readonly ContentScanner scanner;

    private bool hyphenation;
    private char hyphenationCharacter = '-';
    private LineAlignmentEnum lineAlignment = LineAlignmentEnum.BaseLine;
    private Length lineSpace = new Length(0, Length.UnitModeEnum.Relative);
    private XAlignmentEnum xAlignment;
    private YAlignmentEnum yAlignment;

    /** <summary>Area available for the block contents.</summary> */
    private RectangleF frame;
    /** <summary>Actual area occupied by the block contents.</summary> */
    private RectangleF boundBox;

    private Row currentRow;
    private bool rowEnded;

    private LocalGraphicsState container;

    private double lastFontSize;
    #endregion

    #region constructors
    public BlockComposer(
      PrimitiveComposer baseComposer
      )
    {
      this.baseComposer = baseComposer;
      this.scanner = baseComposer.Scanner;
    }
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the base composer.</summary>
    */
    public PrimitiveComposer BaseComposer
    {
      get
      {return baseComposer;}
    }

    /**
      <summary>Begins a content block.</summary>
      <param name="frame">Block boundaries.</param>
      <param name="xAlignment">Horizontal alignment.</param>
      <param name="yAlignment">Vertical alignment.</param>
    */
    public void Begin(
      RectangleF frame,
      XAlignmentEnum xAlignment,
      YAlignmentEnum yAlignment
      )
    {
      this.frame = frame;
      this.xAlignment = xAlignment;
      this.yAlignment = yAlignment;
      lastFontSize = 0;

      // Open the block local state!
      /*
        NOTE: This device allows a fine-grained control over the block representation.
        It MUST be coupled with a closing statement on block end.
      */
      container = baseComposer.BeginLocalState();

      boundBox = new RectangleF(
        frame.X,
        frame.Y,
        frame.Width,
        0
        );

      BeginRow();
    }

    /**
      <summary>Gets the area occupied by the already-placed block contents.</summary>
    */
    public RectangleF BoundBox
    {
      get
      {return boundBox;}
    }

    /**
      <summary>Ends the content block.</summary>
    */
    public void End(
      )
    {
      // End last row!
      EndRow(true);

      // Block translation.
      container.Objects.Insert(
        0,
        new ModifyCTM(
          1, 0, 0, 1,
          boundBox.X, // Horizontal translation.
          -boundBox.Y // Vertical translation.
          )
        );

      // Close the block local state!
      baseComposer.End();
    }

    /**
      <summary>Gets the area where to place the block contents.</summary>
    */
    public RectangleF Frame
    {
      get
      {return frame;}
    }

    /**
      <summary>Gets/Sets whether the hyphenation algorithm has to be applied.</summary>
      <remarks>Initial value: <code>false</code>.</remarks>
    */
    public bool Hyphenation
    {
      get
      {return hyphenation;}
      set
      {hyphenation = value;}
    }

    /**
      <summary>Gets/Sets the character shown at the end of the line before a hyphenation break.
      </summary>
      <remarks>Initial value: hyphen symbol (U+002D, i.e. '-').</remarks>
    */
    public char HyphenationCharacter
    {
      get
      {return hyphenationCharacter;}
      set
      {hyphenationCharacter = value;}
    }

    /**
      <summary>Gets/Sets the default line alignment.</summary>
      <remarks>Initial value: <see cref="LineAlignmentEnum.BaseLine"/>.</remarks>
    */
    public LineAlignmentEnum LineAlignment
    {
      get
      {return lineAlignment;}
      set
      {lineAlignment = value;}
    }

    /**
      <summary>Gets/Sets the text interline spacing.</summary>
      <remarks>Initial value: 0.</remarks>
    */
    public Length LineSpace
    {
      get
      {return lineSpace;}
      set
      {lineSpace = value;}
    }

    /**
      <summary>Gets the content scanner.</summary>
    */
    public ContentScanner Scanner
    {
      get
      {return scanner;}
    }

    /**
      <summary>Ends current paragraph.</summary>
    */
    public void ShowBreak(
      )
    {
      EndRow(true);
      BeginRow();
    }

    /**
      <summary>Ends current paragraph, specifying the offset of the next one.</summary>
      <remarks>This functionality allows higher-level features such as paragraph indentation
      and margin.</remarks>
      <param name="offset">Relative location of the next paragraph.</param>
    */
    public void ShowBreak(
      SizeF offset
      )
    {
      ShowBreak();

      currentRow.Y += offset.Height;
      currentRow.Width = offset.Width;
    }

    /**
      <summary>Ends current paragraph, specifying the alignment of the next one.</summary>
      <remarks>This functionality allows higher-level features such as paragraph indentation and margin.</remarks>
      <param name="xAlignment">Horizontal alignment.</param>
    */
    public void ShowBreak(
      XAlignmentEnum xAlignment
      )
    {
      ShowBreak();

      this.xAlignment = xAlignment;
    }

    /**
      <summary>Ends current paragraph, specifying the offset and alignment of the next one.</summary>
      <remarks>This functionality allows higher-level features such as paragraph indentation and margin.</remarks>
      <param name="offset">Relative location of the next paragraph.</param>
      <param name="xAlignment">Horizontal alignment.</param>
    */
    public void ShowBreak(
      SizeF offset,
      XAlignmentEnum xAlignment
      )
    {
      ShowBreak(offset);

      this.xAlignment = xAlignment;
    }

    /**
      <summary>Shows text.</summary>
      <remarks>Default line alignment is applied.</remarks>
      <param name="text">Text to show.</param>
      <returns>Last shown character index.</returns>
    */
    public int ShowText(
      string text
      )
    {return ShowText(text, lineAlignment);}

    /**
      <summary>Shows text.</summary>
      <param name="text">Text to show.</param>
      <param name="lineAlignment">Line alignment. It can be:
        <list type="bullet">
          <item><see cref="LineAlignmentEnum"/></item>
          <item><see cref="Length">: arbitrary super-/sub-script, depending on whether the value is
          positive or not.</item>
        </list>
      </param>
      <returns>Last shown character index.</returns>
    */
    public int ShowText(
      string text,
      object lineAlignment
      )
    {
      if(currentRow == null
        || text == null)
        return 0;

      ContentScanner.GraphicsState state = baseComposer.State;
      fonts::Font font = state.Font;
      double fontSize = state.FontSize;
      double lineHeight = font.GetLineHeight(fontSize);
      double baseLine = font.GetAscent(fontSize);
      lineAlignment = ResolveLineAlignment(lineAlignment);

      TextFitter textFitter = new TextFitter(
        text,
        0,
        font,
        fontSize,
        hyphenation,
        hyphenationCharacter
        );
      int textLength = text.Length;
      int index = 0;

      while(true)
      {
        if(currentRow.Width == 0) // Current row has just begun.
        {
          // Removing leading space...
          while(true)
          {
            if(index == textLength) // Text end reached.
              goto endTextShowing;
            else if(text[index] != ' ') // No more leading spaces.
              break;

            index++;
          }
        }

        if(OperationUtils.Compare(currentRow.Y + lineHeight, frame.Height) == 1) // Text's height exceeds block's remaining vertical space.
        {
          // Terminate the current row and exit!
          EndRow(false);
          goto endTextShowing;
        }

        // Does the text fit?
        if(textFitter.Fit(
          index,
          frame.Width - currentRow.Width, // Remaining row width.
          currentRow.SpaceCount == 0
          ))
        {
          // Get the fitting text!
          string textChunk = textFitter.FittedText;
          double textChunkWidth = textFitter.FittedWidth;
          PointF textChunkLocation = new PointF(
            (float)currentRow.Width,
            (float)currentRow.Y
            );

          // Insert the fitting text!
          RowObject obj;
          {
            obj = new RowObject(
              RowObject.TypeEnum.Text,
              baseComposer.BeginLocalState(), // Opens the row object's local state.
              lineHeight,
              textChunkWidth,
              CountOccurrence(' ',textChunk),
              lineAlignment,
              baseLine
              );
            baseComposer.ShowText(textChunk, textChunkLocation);
            baseComposer.End();  // Closes the row object's local state.
          }
          AddRowObject(obj, lineAlignment);

          index = textFitter.EndIndex;
        }

        // Evaluating trailing text...
        while(true)
        {
          if(index == textLength) // Text end reached.
            goto endTextShowing;

          switch(text[index])
          {
            case '\r':
              break;
            case '\n':
              // New paragraph!
              index++;
              ShowBreak();
              goto endTrailParsing;
            default:
              // New row (within the same paragraph)!
              EndRow(false);
              BeginRow();
              goto endTrailParsing;
          }

          index++;
        } endTrailParsing:;
      } endTextShowing:;
      if(index >= 0
        && lineAlignment.Equals(LineAlignmentEnum.BaseLine))
      {lastFontSize = fontSize;}

      return index;
    }

    /**
      <summary>Shows the specified external object.</summary>
      <remarks>Default line alignment is applied.</remarks>
      <param name="xObject">External object.</param>
      <param name="size">Size of the external object.</param>
      <returns>Whether the external object was successfully shown.</returns>
    */
    public bool ShowXObject(
      xObjects::XObject xObject,
      SizeF? size
      )
    {return ShowXObject(xObject, size, lineAlignment);}

    /**
      <summary>Shows the specified external object.</summary>
      <param name="xObject">External object.</param>
      <param name="size">Size of the external object.</param>
      <param name="lineAlignment">Line alignment. It can be:
        <list type="bullet">
          <item><see cref="LineAlignmentEnum"/></item>
          <item><see cref="Length">: arbitrary super-/sub-script, depending on whether the value is
          positive or not.</item>
        </list>
      </param>
      <returns>Whether the external object was successfully shown.</returns>
    */
    public bool ShowXObject(
      xObjects::XObject xObject,
      SizeF? size,
      object lineAlignment
      )
    {
      if(currentRow == null
        || xObject == null)
        return false;

      if(!size.HasValue)
      {size = xObject.Size;}
      lineAlignment = ResolveLineAlignment(lineAlignment);

      while(true)
      {
        if(OperationUtils.Compare(currentRow.Y + size.Value.Height, frame.Height) == 1) // Object's height exceeds block's remaining vertical space.
        {
          // Terminate current row and exit!
          EndRow(false);
          return false;
        }
        else if(OperationUtils.Compare(currentRow.Width + size.Value.Width, frame.Width) < 1) // There's room for the object in the current row.
        {
          PointF location = new PointF(
            (float)currentRow.Width,
            (float)currentRow.Y
            );
          RowObject obj;
          {
            obj = new RowObject(
              RowObject.TypeEnum.XObject,
              baseComposer.BeginLocalState(), // Opens the row object's local state.
              size.Value.Height,
              size.Value.Width,
              0,
              lineAlignment,
              size.Value.Height
              );
            baseComposer.ShowXObject(xObject, location, size);
            baseComposer.End(); // Closes the row object's local state.
          }
          AddRowObject(obj, lineAlignment);

          return true;
        }
        else // There's NOT enough room for the object in the current row.
        {
          // Go to next row!
          EndRow(false);
          BeginRow();
        }
      }
    }

    /**
      <summary>Gets the horizontal alignment applied to the current content block.</summary>
    */
    public XAlignmentEnum XAlignment
    {
      get
      {return xAlignment;}
    }

    /**
      <summary>Gets the vertical alignment applied to the current content block.</summary>
    */
    public YAlignmentEnum YAlignment
    {
      get
      {return yAlignment;}
    }
    #endregion

    #region private
    /**
      <summary>Adds an object to the current row.</summary>
      <param name="obj">Object to add.</param>
      <param name="lineAlignment">Object's line alignment.</param>
    */
    private void AddRowObject(
      RowObject obj,
      object lineAlignment
      )
    {
      currentRow.Objects.Add(obj);
      currentRow.SpaceCount += obj.SpaceCount;
      currentRow.Width += obj.Width;

      if(lineAlignment is double || lineAlignment.Equals(LineAlignmentEnum.BaseLine))
      {
        double gap = (lineAlignment is double ? (Double)lineAlignment : 0);
        double superGap = obj.BaseLine + gap - currentRow.BaseLine;
        if(superGap > 0)
        {
          currentRow.Height += superGap;
          currentRow.BaseLine += superGap;
        }
        double subGap = currentRow.BaseLine + (obj.Height - obj.BaseLine) - gap - currentRow.Height;
        if(subGap > 0)
        {currentRow.Height += subGap;}
      }
      else if(obj.Height > currentRow.Height)
      {currentRow.Height = obj.Height;}
    }

    /**
      <summary>Begins a content row.</summary>
    */
    private void BeginRow(
      )
    {
      rowEnded = false;

      double rowY = boundBox.Height;
      if(rowY > 0)
      {
        ContentScanner.GraphicsState state = baseComposer.State;
        rowY += lineSpace.GetValue(state.Font.GetLineHeight(state.FontSize));
      }
      currentRow = new Row(
        (ContentPlaceholder)baseComposer.Add(new ContentPlaceholder()),
        rowY
        );
    }

    private int CountOccurrence(
      char value,
      string text
      )
    {
      int count = 0;
      int fromIndex = 0;
      do
      {
        int foundIndex = text.IndexOf(value,fromIndex);
        if(foundIndex == -1)
          return count;

        count++;

        fromIndex = foundIndex + 1;
      }
      while(true);
    }

    /**
      <summary>Ends the content row.</summary>
      <param name="broken">Indicates whether this is the end of a paragraph.</param>
    */
    private void EndRow(
      bool broken
      )
    {
      if(rowEnded)
        return;

      rowEnded = true;

      double[] objectXOffsets = new double[currentRow.Objects.Count]; // Horizontal object displacements.
      double wordSpace = 0; // Exceeding space among words.
      double rowXOffset = 0; // Horizontal row offset.

      List<RowObject> objects = currentRow.Objects;

      // Horizontal alignment.
      XAlignmentEnum xAlignment = this.xAlignment;
      switch(xAlignment)
      {
        case XAlignmentEnum.Left:
          break;
        case XAlignmentEnum.Right:
          rowXOffset = frame.Width - currentRow.Width;
          break;
        case XAlignmentEnum.Center:
          rowXOffset = (frame.Width - currentRow.Width) / 2;
          break;
        case XAlignmentEnum.Justify:
          // Are there NO spaces?
          if(currentRow.SpaceCount == 0
            || broken) // NO spaces.
          {
            /* NOTE: This situation equals a simple left alignment. */
            xAlignment = XAlignmentEnum.Left;
          }
          else // Spaces exist.
          {
            // Calculate the exceeding spacing among the words!
            wordSpace = (frame.Width - currentRow.Width) / currentRow.SpaceCount;

            // Define the horizontal offsets for justified alignment.
            for(
              int index = 1,
                count = objects.Count;
              index < count;
              index++
              )
            {
              /*
                NOTE: The offset represents the horizontal justification gap inserted
                at the left side of each object.
              */
              objectXOffsets[index] = objectXOffsets[index - 1] + objects[index - 1].SpaceCount * wordSpace;
            }
          }
          break;
      }

      SetWordSpace wordSpaceOperation = new SetWordSpace(wordSpace);

      // Vertical alignment and translation.
      for(
        int index = objects.Count - 1;
        index >= 0;
        index--
        )
      {
        RowObject obj = objects[index];

        // Vertical alignment.
        double objectYOffset = 0;
        {
          LineAlignmentEnum lineAlignment;
          double lineRise;
          {
            object objectLineAlignment = obj.LineAlignment;
            if(objectLineAlignment is Double)
            {
              lineAlignment = LineAlignmentEnum.BaseLine;
              lineRise = (double)objectLineAlignment;
            }
            else
            {
              lineAlignment = (LineAlignmentEnum)objectLineAlignment;
              lineRise = 0;
            }
          }
          switch (lineAlignment)
          {
              case LineAlignmentEnum.Top:
                  /* NOOP */
                  break;
              case LineAlignmentEnum.Middle:
                  objectYOffset = -(currentRow.Height - obj.Height) / 2;
                  break;
              case LineAlignmentEnum.BaseLine:
                  objectYOffset = -(currentRow.BaseLine - obj.BaseLine - lineRise);
                  break;
              case LineAlignmentEnum.Bottom:
                  objectYOffset = -(currentRow.Height - obj.Height);
                  break;
              default:
                  throw new NotImplementedException("Line alignment " + lineAlignment + " unknown.");
          }
        }

        IList<ContentObject> containedGraphics = obj.Container.Objects;
        // Word spacing.
        containedGraphics.Insert(0,wordSpaceOperation);
        // Translation.
        containedGraphics.Insert(
          0,
          new ModifyCTM(
            1, 0, 0, 1,
            objectXOffsets[index] + rowXOffset, // Horizontal alignment.
            objectYOffset // Vertical alignment.
            )
          );
      }

      // Update the actual block height!
      boundBox.Height = (float)(currentRow.Y + currentRow.Height);

      // Update the actual block vertical location!
      double yOffset;
      switch(yAlignment)
      {
        case YAlignmentEnum.Bottom:
          yOffset = frame.Height - boundBox.Height;
          break;
        case YAlignmentEnum.Middle:
          yOffset = (frame.Height - boundBox.Height) / 2;
          break;
        case YAlignmentEnum.Top:
        default:
          yOffset = 0;
          break;
      }
      boundBox.Y = (float)(frame.Y + yOffset);

      // Discard the current row!
      currentRow = null;
    }

    private object ResolveLineAlignment(
      object lineAlignment
      )
    {
      if(!(lineAlignment is LineAlignmentEnum
        || lineAlignment is Length))
        throw new ArgumentException("MUST be either LineAlignmentEnum or Length.", "lineAlignment");

      if(lineAlignment.Equals(LineAlignmentEnum.Super))
      {lineAlignment = new Length(0.33, Length.UnitModeEnum.Relative);}
      else if(lineAlignment.Equals(LineAlignmentEnum.Sub))
      {lineAlignment = new Length(-0.33, Length.UnitModeEnum.Relative);}
      if(lineAlignment is Length)
      {
        if(lastFontSize == 0)
        {lastFontSize = baseComposer.State.FontSize;}
        lineAlignment = ((Length)lineAlignment).GetValue(lastFontSize);
      }

      return lineAlignment;
    }
    #endregion
    #endregion
    #endregion
  }
}