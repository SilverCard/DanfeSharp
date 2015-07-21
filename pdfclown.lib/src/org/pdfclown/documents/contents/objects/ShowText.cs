/*
  Copyright 2007-2011 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.documents.contents.fonts;
using org.pdfclown.objects;

using System;
using System.Collections.Generic;
using drawing = System.Drawing;
using System.Drawing.Drawing2D;

namespace org.pdfclown.documents.contents.objects
{
  /**
    <summary>Abstract 'show a text string' operation [PDF:1.6:5.3.2].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public abstract class ShowText
    : Operation
  {
    #region types
    public interface IScanner
    {
      /**
        <summary>Notifies the scanner about a text character.</summary>
        <param name="textChar">Scanned character.</param>
        <param name="textCharBox">Bounding box of the scanned character.</param>
      */
      void ScanChar(
        char textChar,
        drawing::RectangleF textCharBox
        );
    }
    #endregion

    #region dynamic
    #region constructors
    protected ShowText(
      string operator_
      ) : base(operator_)
    {}

    protected ShowText(
      string operator_,
      params PdfDirectObject[] operands
      ) : base(operator_, operands)
    {}

    protected ShowText(
      string operator_,
      IList<PdfDirectObject> operands
      ) : base(operator_, operands)
    {}
    #endregion

    #region interface
    #region public
    public override void Scan(
      ContentScanner.GraphicsState state
      )
    {Scan(state, null);}

    /**
      <summary>Executes scanning on this operation.</summary>
      <param name="state">Graphics state context.</param>
      <param name="textScanner">Scanner to be notified about text contents.
      In case it's null, the operation is applied to the graphics state context.</param>
    */
    public void Scan(
      ContentScanner.GraphicsState state,
      IScanner textScanner
      )
    {
      /*
        TODO: I really dislike this solution -- it's a temporary hack until the event-driven
        parsing mechanism is implemented...
      */
      /*
        TODO: support to vertical writing mode.
      */

      IContentContext context = state.Scanner.ContentContext;
      double contextHeight = context.Box.Height;
      Font font = state.Font;
      double fontSize = state.FontSize;
      double scale = state.Scale / 100;
      double scaledFactor = Font.GetScalingFactor(fontSize) * scale;
      double wordSpace = state.WordSpace * scale;
      double charSpace = state.CharSpace * scale;
      Matrix ctm = state.Ctm.Clone();
      Matrix tm = state.Tm;
      if(this is ShowTextToNextLine)
      {
        ShowTextToNextLine showTextToNextLine = (ShowTextToNextLine)this;
        double? newWordSpace = showTextToNextLine.WordSpace;
        if(newWordSpace != null)
        {
          if(textScanner == null)
          {state.WordSpace = newWordSpace.Value;}
          wordSpace = newWordSpace.Value * scale;
        }
        double? newCharSpace = showTextToNextLine.CharSpace;
        if(newCharSpace != null)
        {
          if(textScanner == null)
          {state.CharSpace = newCharSpace.Value;}
          charSpace = newCharSpace.Value * scale;
        }
        tm = state.Tlm.Clone();
        tm.Translate(0, (float)state.Lead);
      }
      else
      {tm = state.Tm.Clone();}

      foreach(object textElement in Value)
      {
        if(textElement is byte[]) // Text string.
        {
          string textString = font.Decode((byte[])textElement);
          foreach(char textChar in textString)
          {
            double charWidth = font.GetWidth(textChar) * scaledFactor;

            if(textScanner != null)
            {
              /*
                NOTE: The text rendering matrix is recomputed before each glyph is painted
                during a text-showing operation.
              */
              Matrix trm = ctm.Clone(); trm.Multiply(tm);
              double charHeight = font.GetHeight(textChar,fontSize);
              drawing::RectangleF charBox = new drawing::RectangleF(
                trm.Elements[4],
                (float)(contextHeight - trm.Elements[5] - font.GetAscent(fontSize) * trm.Elements[3]),
                (float)charWidth * trm.Elements[0],
                (float)charHeight * trm.Elements[3]
                );
              textScanner.ScanChar(textChar,charBox);
            }

            /*
              NOTE: After the glyph is painted, the text matrix is updated
              according to the glyph displacement and any applicable spacing parameter.
            */
            tm.Translate((float)(charWidth + charSpace + (textChar == ' ' ? wordSpace : 0)), 0);
          }
        }
        else // Text position adjustment.
        {tm.Translate((float)(-Convert.ToSingle(textElement) * scaledFactor), 0);}
      }

      if(textScanner == null)
      {
        state.Tm = tm;

        if(this is ShowTextToNextLine)
        {state.Tlm = tm.Clone();}
      }
    }

    /**
      <summary>Gets/Sets the encoded text.</summary>
      <remarks>Text is expressed in native encoding: to resolve it to Unicode, pass it
      to the decode method of the corresponding font.</remarks>
    */
    public abstract byte[] Text
    {
      get;
      set;
    }

    /**
      <summary>Gets/Sets the encoded text elements along with their adjustments.</summary>
      <remarks>Text is expressed in native encoding: to resolve it to Unicode, pass it
      to the decode method of the corresponding font.</remarks>
      <returns>Each element can be either a byte array or a number:
        <list type="bullet">
          <item>if it's a byte array (encoded text), the operator shows text glyphs;</item>
          <item>if it's a number (glyph adjustment), the operator inversely adjusts the next glyph position
          by that amount (that is: a positive value reduces the distance between consecutive glyphs).</item>
        </list>
      </returns>
    */
    public virtual IList<object> Value
    {
      get
      {return new List<object>(){Text};}
      set
      {Text = (byte[])value[0];}
    }
    #endregion
    #endregion
    #endregion
  }
}