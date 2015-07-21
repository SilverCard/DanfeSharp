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

using org.pdfclown.bytes;
using org.pdfclown.files;
using org.pdfclown.objects;
using org.pdfclown.tokens;

using System;
using System.Collections.Generic;
using System.Text;

namespace org.pdfclown.documents.contents.objects
{
  /**
    <summary>Content stream instruction [PDF:1.6:3.7.1].</summary>
  */
  [PDF(VersionEnum.PDF10)]
  public abstract class Operation
    : ContentObject
  {
    #region static
    #region interface
    #region public
    /**
      <summary>Gets an operation.</summary>
      <param name="operator_">Operator.</param>
      <param name="operands">List of operands.</param>
    */
    public static Operation Get(
      string operator_,
      IList<PdfDirectObject> operands
      )
    {
      if(operator_ == null)
        return null;

      if(operator_.Equals(SaveGraphicsState.OperatorKeyword))
        return SaveGraphicsState.Value;
      else if(operator_.Equals(SetFont.OperatorKeyword))
        return new SetFont(operands);
      else if(operator_.Equals(SetStrokeColor.OperatorKeyword)
        || operator_.Equals(SetStrokeColor.ExtendedOperatorKeyword))
        return new SetStrokeColor(operator_, operands);
      else if(operator_.Equals(SetStrokeColorSpace.OperatorKeyword))
        return new SetStrokeColorSpace(operands);
      else if(operator_.Equals(SetFillColor.OperatorKeyword)
        || operator_.Equals(SetFillColor.ExtendedOperatorKeyword))
        return new SetFillColor(operator_, operands);
      else if(operator_.Equals(SetFillColorSpace.OperatorKeyword))
        return new SetFillColorSpace(operands);
      else if(operator_.Equals(SetDeviceGrayStrokeColor.OperatorKeyword))
        return new SetDeviceGrayStrokeColor(operands);
      else if(operator_.Equals(SetDeviceGrayFillColor.OperatorKeyword))
        return new SetDeviceGrayFillColor(operands);
      else if(operator_.Equals(SetDeviceRGBStrokeColor.OperatorKeyword))
        return new SetDeviceRGBStrokeColor(operands);
      else if(operator_.Equals(SetDeviceRGBFillColor.OperatorKeyword))
        return new SetDeviceRGBFillColor(operands);
      else if(operator_.Equals(SetDeviceCMYKStrokeColor.OperatorKeyword))
        return new SetDeviceCMYKStrokeColor(operands);
      else if(operator_.Equals(SetDeviceCMYKFillColor.OperatorKeyword))
        return new SetDeviceCMYKFillColor(operands);
      else if(operator_.Equals(RestoreGraphicsState.OperatorKeyword))
        return RestoreGraphicsState.Value;
      else if(operator_.Equals(BeginSubpath.OperatorKeyword))
        return new BeginSubpath(operands);
      else if(operator_.Equals(CloseSubpath.OperatorKeyword))
        return CloseSubpath.Value;
      else if(operator_.Equals(PaintPath.CloseStrokeOperatorKeyword))
        return PaintPath.CloseStroke;
      else if(operator_.Equals(PaintPath.FillOperatorKeyword)
        || operator_.Equals(PaintPath.FillObsoleteOperatorKeyword))
        return PaintPath.Fill;
      else if(operator_.Equals(PaintPath.FillEvenOddOperatorKeyword))
        return PaintPath.FillEvenOdd;
      else if(operator_.Equals(PaintPath.StrokeOperatorKeyword))
        return PaintPath.Stroke;
      else if(operator_.Equals(PaintPath.FillStrokeOperatorKeyword))
        return PaintPath.FillStroke;
      else if(operator_.Equals(PaintPath.FillStrokeEvenOddOperatorKeyword))
        return PaintPath.FillStrokeEvenOdd;
      else if(operator_.Equals(PaintPath.CloseFillStrokeOperatorKeyword))
        return PaintPath.CloseFillStroke;
      else if(operator_.Equals(PaintPath.CloseFillStrokeEvenOddOperatorKeyword))
        return PaintPath.CloseFillStrokeEvenOdd;
      else if(operator_.Equals(PaintPath.EndPathNoOpOperatorKeyword))
        return PaintPath.EndPathNoOp;
      else if(operator_.Equals(ModifyClipPath.NonZeroOperatorKeyword))
        return ModifyClipPath.NonZero;
      else if(operator_.Equals(ModifyClipPath.EvenOddOperatorKeyword))
        return ModifyClipPath.EvenOdd;
      else if(operator_.Equals(TranslateTextToNextLine.OperatorKeyword))
        return TranslateTextToNextLine.Value;
      else if(operator_.Equals(ShowSimpleText.OperatorKeyword))
        return new ShowSimpleText(operands);
      else if(operator_.Equals(ShowTextToNextLine.SimpleOperatorKeyword)
        || operator_.Equals(ShowTextToNextLine.SpaceOperatorKeyword))
        return new ShowTextToNextLine(operator_, operands);
      else if(operator_.Equals(ShowAdjustedText.OperatorKeyword))
        return new ShowAdjustedText(operands);
      else if(operator_.Equals(TranslateTextRelative.SimpleOperatorKeyword)
        || operator_.Equals(TranslateTextRelative.LeadOperatorKeyword))
        return new TranslateTextRelative(operator_, operands);
      else if(operator_.Equals(SetTextMatrix.OperatorKeyword))
        return new SetTextMatrix(operands);
      else if(operator_.Equals(ModifyCTM.OperatorKeyword))
        return new ModifyCTM(operands);
      else if(operator_.Equals(PaintXObject.OperatorKeyword))
        return new PaintXObject(operands);
      else if(operator_.Equals(PaintShading.OperatorKeyword))
        return new PaintShading(operands);
      else if(operator_.Equals(SetCharSpace.OperatorKeyword))
        return new SetCharSpace(operands);
      else if(operator_.Equals(SetLineCap.OperatorKeyword))
        return new SetLineCap(operands);
      else if(operator_.Equals(SetLineDash.OperatorKeyword))
        return new SetLineDash(operands);
      else if(operator_.Equals(SetLineJoin.OperatorKeyword))
        return new SetLineJoin(operands);
      else if(operator_.Equals(SetLineWidth.OperatorKeyword))
        return new SetLineWidth(operands);
      else if(operator_.Equals(SetMiterLimit.OperatorKeyword))
        return new SetMiterLimit(operands);
      else if(operator_.Equals(SetTextLead.OperatorKeyword))
        return new SetTextLead(operands);
      else if(operator_.Equals(SetTextRise.OperatorKeyword))
        return new SetTextRise(operands);
      else if(operator_.Equals(SetTextScale.OperatorKeyword))
        return new SetTextScale(operands);
      else if(operator_.Equals(SetTextRenderMode.OperatorKeyword))
        return new SetTextRenderMode(operands);
      else if(operator_.Equals(SetWordSpace.OperatorKeyword))
        return new SetWordSpace(operands);
      else if(operator_.Equals(DrawLine.OperatorKeyword))
        return new DrawLine(operands);
      else if(operator_.Equals(DrawRectangle.OperatorKeyword))
        return new DrawRectangle(operands);
      else if(operator_.Equals(DrawCurve.FinalOperatorKeyword)
        || operator_.Equals(DrawCurve.FullOperatorKeyword)
        || operator_.Equals(DrawCurve.InitialOperatorKeyword))
        return new DrawCurve(operator_, operands);
      else if(operator_.Equals(EndInlineImage.OperatorKeyword))
        return EndInlineImage.Value;
      else if(operator_.Equals(BeginText.OperatorKeyword))
        return BeginText.Value;
      else if(operator_.Equals(EndText.OperatorKeyword))
        return EndText.Value;
      else if(operator_.Equals(BeginMarkedContent.SimpleOperatorKeyword)
        || operator_.Equals(BeginMarkedContent.PropertyListOperatorKeyword))
        return new BeginMarkedContent(operator_, operands);
      else if(operator_.Equals(EndMarkedContent.OperatorKeyword))
        return EndMarkedContent.Value;
      else if(operator_.Equals(MarkedContentPoint.SimpleOperatorKeyword)
        || operator_.Equals(MarkedContentPoint.PropertyListOperatorKeyword))
        return new MarkedContentPoint(operator_, operands);
      else if(operator_.Equals(BeginInlineImage.OperatorKeyword))
        return BeginInlineImage.Value;
      else if(operator_.Equals(EndInlineImage.OperatorKeyword))
        return EndInlineImage.Value;
      else if(operator_.Equals(ApplyExtGState.OperatorKeyword))
        return new ApplyExtGState(operands);
      else // No explicit operation implementation available.
        return new GenericOperation(operator_, operands);
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region fields
    protected string operator_;
    protected IList<PdfDirectObject> operands;
    #endregion

    #region constructors
    protected Operation(
      string operator_
      )
    {this.operator_ = operator_;}

    protected Operation(
      string operator_,
      PdfDirectObject operand
      )
    {
      this.operator_ = operator_;

      this.operands = new List<PdfDirectObject>();
      this.operands.Add(operand);
    }

    protected Operation(
      string operator_,
      params PdfDirectObject[] operands
      )
    {
      this.operator_ = operator_;
      this.operands = new List<PdfDirectObject>(operands);
    }

    protected Operation(
      string operator_,
      IList<PdfDirectObject> operands
      )
    {
      this.operator_ = operator_;
      this.operands = operands;
    }
    #endregion

    #region interface
    #region public
    public string Operator
    {get{return operator_;}}

    public IList<PdfDirectObject> Operands
    {get{return operands;}}

    public override string ToString(
      )
    {
      StringBuilder buffer = new StringBuilder();

      // Begin.
      buffer.Append("{");

      // Operator.
      buffer.Append(operator_);

      // Operands.
      if(operands != null)
      {
        buffer.Append(" [");
        for(
          int i = 0, count = operands.Count;
          i < count;
          i++
          )
        {
          if(i > 0)
          {buffer.Append(", ");}

          buffer.Append(operands[i].ToString());
        }
        buffer.Append("]");
      }

      // End.
      buffer.Append("}");

      return buffer.ToString();
    }

    public override void WriteTo(
      IOutputStream stream,
      Document context
      )
    {
      if(operands != null)
      {
        File fileContext = context.File;
        foreach(PdfDirectObject operand in operands)
        {operand.WriteTo(stream, fileContext); stream.Write(Chunk.Space);}
      }
      stream.Write(operator_); stream.Write(Chunk.LineFeed);
    }
    #endregion
    #endregion
    #endregion
  }
}