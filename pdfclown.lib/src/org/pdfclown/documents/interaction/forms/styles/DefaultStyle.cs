/*
  Copyright 2008-2012 Stefano Chizzolini. http://www.pdfclown.org

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
using org.pdfclown.documents.contents.composition;
using org.pdfclown.documents.contents.fonts;
using org.pdfclown.documents.contents.xObjects;
using org.pdfclown.documents.interaction.annotations;
using org.pdfclown.objects;

using System;
using System.Drawing;

namespace org.pdfclown.documents.interaction.forms.styles
{
  /**
    <summary>Default field appearance style.</summary>
  */
  public sealed class DefaultStyle
    : FieldStyle
  {
    #region dynamic
    #region constructors
    public DefaultStyle(
      )
    {BackColor = new DeviceRGBColor(.9,.9,.9);}
    #endregion

    #region interface
    #region public
    public override void Apply(
      Field field
      )
    {
      if(field is PushButton)
      {Apply((PushButton)field);}
      else if(field is CheckBox)
      {Apply((CheckBox)field);}
      else if(field is TextField)
      {Apply((TextField)field);}
      else if(field is ComboBox)
      {Apply((ComboBox)field);}
      else if(field is ListBox)
      {Apply((ListBox)field);}
      else if(field is RadioButton)
      {Apply((RadioButton)field);}
    }

    private void Apply(
      CheckBox field
      )
    {
      Document document = field.Document;
      foreach(Widget widget in field.Widgets)
      {
        {
          PdfDictionary widgetDataObject = widget.BaseDataObject;
          widgetDataObject[PdfName.DA] = new PdfString("/ZaDb 0 Tf 0 0 0 rg");
          widgetDataObject[PdfName.MK] = new PdfDictionary(
            new PdfName[]
            {
              PdfName.BG,
              PdfName.BC,
              PdfName.CA
            },
            new PdfDirectObject[]
            {
              new PdfArray(new PdfDirectObject[]{PdfReal.Get(0.9412), PdfReal.Get(0.9412), PdfReal.Get(0.9412)}),
              new PdfArray(new PdfDirectObject[]{PdfInteger.Default, PdfInteger.Default, PdfInteger.Default}),
              new PdfString("4")
            }
            );
          widgetDataObject[PdfName.BS] = new PdfDictionary(
            new PdfName[]
            {
              PdfName.W,
              PdfName.S
            },
            new PdfDirectObject[]
            {
              PdfReal.Get(0.8),
              PdfName.S
            }
            );
          widgetDataObject[PdfName.H] = PdfName.P;
        }

        Appearance appearance = widget.Appearance;
        if(appearance == null)
        {widget.Appearance = appearance = new Appearance(document);}

        SizeF size = widget.Box.Size;

        AppearanceStates normalAppearance = appearance.Normal;
        FormXObject onState = new FormXObject(document, size);
        normalAppearance[PdfName.Yes] = onState;

  //TODO:verify!!!
  //   appearance.getRollover().put(PdfName.Yes,onState);
  //   appearance.getDown().put(PdfName.Yes,onState);
  //   appearance.getRollover().put(PdfName.Off,offState);
  //   appearance.getDown().put(PdfName.Off,offState);

        float lineWidth = 1;
        RectangleF frame = new RectangleF(lineWidth / 2, lineWidth / 2, size.Width - lineWidth, size.Height - lineWidth);
        {
          PrimitiveComposer composer = new PrimitiveComposer(onState);

          if(GraphicsVisibile)
          {
            composer.BeginLocalState();
            composer.SetLineWidth(lineWidth);
            composer.SetFillColor(BackColor);
            composer.SetStrokeColor(ForeColor);
            composer.DrawRectangle(frame, 5);
            composer.FillStroke();
            composer.End();
          }

          BlockComposer blockComposer = new BlockComposer(composer);
          blockComposer.Begin(frame,XAlignmentEnum.Center,YAlignmentEnum.Middle);
          composer.SetFillColor(ForeColor);
          composer.SetFont(
            new StandardType1Font(
              document,
              StandardType1Font.FamilyEnum.ZapfDingbats,
              true,
              false
              ),
            size.Height * 0.8
            );
          blockComposer.ShowText(new String(new char[]{CheckSymbol}));
          blockComposer.End();

          composer.Flush();
        }

        FormXObject offState = new FormXObject(document, size);
        normalAppearance[PdfName.Off] = offState;
        {
          if(GraphicsVisibile)
          {
            PrimitiveComposer composer = new PrimitiveComposer(offState);
  
            composer.BeginLocalState();
            composer.SetLineWidth(lineWidth);
            composer.SetFillColor(BackColor);
            composer.SetStrokeColor(ForeColor);
            composer.DrawRectangle(frame, 5);
            composer.FillStroke();
            composer.End();

            composer.Flush();
          }
        }
      }
    }

    private void Apply(
      RadioButton field
      )
    {
      Document document = field.Document;
      foreach(Widget widget in field.Widgets)
      {
        {
          PdfDictionary widgetDataObject = widget.BaseDataObject;
          widgetDataObject[PdfName.DA] = new PdfString("/ZaDb 0 Tf 0 0 0 rg");
          widgetDataObject[PdfName.MK] = new PdfDictionary(
            new PdfName[]
            {
              PdfName.BG,
              PdfName.BC,
              PdfName.CA
            },
            new PdfDirectObject[]
            {
              new PdfArray(new PdfDirectObject[]{PdfReal.Get(0.9412), PdfReal.Get(0.9412), PdfReal.Get(0.9412)}),
              new PdfArray(new PdfDirectObject[]{PdfInteger.Default, PdfInteger.Default, PdfInteger.Default}),
              new PdfString("l")
            }
            );
          widgetDataObject[PdfName.BS] = new PdfDictionary(
            new PdfName[]
            {
              PdfName.W,
              PdfName.S
            },
            new PdfDirectObject[]
            {
              PdfReal.Get(0.8),
              PdfName.S
            }
            );
          widgetDataObject[PdfName.H] = PdfName.P;
        }

        Appearance appearance = widget.Appearance;
        if(appearance == null)
        {widget.Appearance = appearance = new Appearance(document);}

        AppearanceStates normalAppearance = appearance.Normal;
        FormXObject onState = normalAppearance[new PdfName(((DualWidget)widget).WidgetName)];

  //TODO:verify!!!
  //   appearance.getRollover().put(new PdfName(...),onState);
  //   appearance.getDown().put(new PdfName(...),onState);
  //   appearance.getRollover().put(PdfName.Off,offState);
  //   appearance.getDown().put(PdfName.Off,offState);

        SizeF size = widget.Box.Size;
        float lineWidth = 1;
        RectangleF frame = new RectangleF(lineWidth / 2, lineWidth / 2, size.Width - lineWidth, size.Height - lineWidth);
        {
          PrimitiveComposer composer = new PrimitiveComposer(onState);

          if(GraphicsVisibile)
          {
            composer.BeginLocalState();
            composer.SetLineWidth(lineWidth);
            composer.SetFillColor(BackColor);
            composer.SetStrokeColor(ForeColor);
            composer.DrawEllipse(frame);
            composer.FillStroke();
            composer.End();
          }

          BlockComposer blockComposer = new BlockComposer(composer);
          blockComposer.Begin(frame,XAlignmentEnum.Center,YAlignmentEnum.Middle);
          composer.SetFillColor(ForeColor);
          composer.SetFont(
            new StandardType1Font(
              document,
              StandardType1Font.FamilyEnum.ZapfDingbats,
              true,
              false
              ),
            size.Height * 0.8
            );
          blockComposer.ShowText(new String(new char[]{RadioSymbol}));
          blockComposer.End();

          composer.Flush();
        }

        FormXObject offState = new FormXObject(document, size);
        normalAppearance[PdfName.Off] = offState;
        {
          if(GraphicsVisibile)
          {
            PrimitiveComposer composer = new PrimitiveComposer(offState);
  
            composer.BeginLocalState();
            composer.SetLineWidth(lineWidth);
            composer.SetFillColor(BackColor);
            composer.SetStrokeColor(ForeColor);
            composer.DrawEllipse(frame);
            composer.FillStroke();
            composer.End();
  
            composer.Flush();
          }
        }
      }
    }

    private void Apply(
      PushButton field
      )
    {
      Document document = field.Document;
      Widget widget = field.Widgets[0];

      Appearance appearance = widget.Appearance;
      if(appearance == null)
      {widget.Appearance = appearance = new Appearance(document);}

      FormXObject normalAppearanceState;
      {
        SizeF size = widget.Box.Size;
        normalAppearanceState = new FormXObject(document, size);
        PrimitiveComposer composer = new PrimitiveComposer(normalAppearanceState);

        float lineWidth = 1;
        RectangleF frame = new RectangleF(lineWidth / 2, lineWidth / 2, size.Width - lineWidth, size.Height - lineWidth);
        if(GraphicsVisibile)
        {
          composer.BeginLocalState();
          composer.SetLineWidth(lineWidth);
          composer.SetFillColor(BackColor);
          composer.SetStrokeColor(ForeColor);
          composer.DrawRectangle(frame, 5);
          composer.FillStroke();
          composer.End();
        }

        string title = (string)field.Value;
        if(title != null)
        {
          BlockComposer blockComposer = new BlockComposer(composer);
          blockComposer.Begin(frame,XAlignmentEnum.Center,YAlignmentEnum.Middle);
          composer.SetFillColor(ForeColor);
          composer.SetFont(
            new StandardType1Font(
              document,
              StandardType1Font.FamilyEnum.Helvetica,
              true,
              false
              ),
            size.Height * 0.5
            );
          blockComposer.ShowText(title);
          blockComposer.End();
        }

        composer.Flush();
      }
      appearance.Normal[null] = normalAppearanceState;
    }

    private void Apply(
      TextField field
      )
    {
      Document document = field.Document;
      Widget widget = field.Widgets[0];

      Appearance appearance = widget.Appearance;
      if(appearance == null)
      {widget.Appearance = appearance = new Appearance(document);}

      widget.BaseDataObject[PdfName.DA] = new PdfString("/Helv " + FontSize + " Tf 0 0 0 rg");

      FormXObject normalAppearanceState;
      {
        SizeF size = widget.Box.Size;
        normalAppearanceState = new FormXObject(document, size);
        PrimitiveComposer composer = new PrimitiveComposer(normalAppearanceState);

        float lineWidth = 1;
        RectangleF frame = new RectangleF(lineWidth / 2, lineWidth / 2, size.Width - lineWidth, size.Height - lineWidth);
        if(GraphicsVisibile)
        {
          composer.BeginLocalState();
          composer.SetLineWidth(lineWidth);
          composer.SetFillColor(BackColor);
          composer.SetStrokeColor(ForeColor);
          composer.DrawRectangle(frame, 5);
          composer.FillStroke();
          composer.End();
        }

        composer.BeginMarkedContent(PdfName.Tx);
        composer.SetFont(
          new StandardType1Font(
            document,
            StandardType1Font.FamilyEnum.Helvetica,
            false,
            false
            ),
          FontSize
          );
        composer.ShowText(
          (string)field.Value,
          new PointF(0,size.Height/2),
          XAlignmentEnum.Left,
          YAlignmentEnum.Middle,
          0
          );
        composer.End();

        composer.Flush();
      }
      appearance.Normal[null] = normalAppearanceState;
    }

    private void Apply(
      ComboBox field
      )
    {
      Document document = field.Document;
      Widget widget = field.Widgets[0];

      Appearance appearance = widget.Appearance;
      if(appearance == null)
      {widget.Appearance = appearance = new Appearance(document);}

      widget.BaseDataObject[PdfName.DA] = new PdfString("/Helv " + FontSize + " Tf 0 0 0 rg");

      FormXObject normalAppearanceState;
      {
        SizeF size = widget.Box.Size;
        normalAppearanceState = new FormXObject(document, size);
        PrimitiveComposer composer = new PrimitiveComposer(normalAppearanceState);

        float lineWidth = 1;
        RectangleF frame = new RectangleF(lineWidth / 2, lineWidth / 2, size.Width - lineWidth, size.Height - lineWidth);
        if(GraphicsVisibile)
        {
          composer.BeginLocalState();
          composer.SetLineWidth(lineWidth);
          composer.SetFillColor(BackColor);
          composer.SetStrokeColor(ForeColor);
          composer.DrawRectangle(frame, 5);
          composer.FillStroke();
          composer.End();
        }

        composer.BeginMarkedContent(PdfName.Tx);
        composer.SetFont(
          new StandardType1Font(
            document,
            StandardType1Font.FamilyEnum.Helvetica,
            false,
            false
            ),
          FontSize
          );
        composer.ShowText(
          (string)field.Value,
          new PointF(0,size.Height/2),
          XAlignmentEnum.Left,
          YAlignmentEnum.Middle,
          0
          );
        composer.End();

        composer.Flush();
      }
      appearance.Normal[null] = normalAppearanceState;
    }

    private void Apply(
      ListBox field
      )
    {
      Document document = field.Document;
      Widget widget = field.Widgets[0];

      Appearance appearance = widget.Appearance;
      if(appearance == null)
      {widget.Appearance = appearance = new Appearance(document);}

      {
        PdfDictionary widgetDataObject = widget.BaseDataObject;
        widgetDataObject[PdfName.DA] = new PdfString("/Helv " + FontSize + " Tf 0 0 0 rg");
        widgetDataObject[PdfName.MK] = new PdfDictionary(
          new PdfName[]
          {
            PdfName.BG,
            PdfName.BC
          },
          new PdfDirectObject[]
          {
            new PdfArray(new PdfDirectObject[]{PdfReal.Get(.9), PdfReal.Get(.9), PdfReal.Get(.9)}),
            new PdfArray(new PdfDirectObject[]{PdfInteger.Default, PdfInteger.Default, PdfInteger.Default})
          }
          );
      }

      FormXObject normalAppearanceState;
      {
        SizeF size = widget.Box.Size;
        normalAppearanceState = new FormXObject(document, size);
        PrimitiveComposer composer = new PrimitiveComposer(normalAppearanceState);

        float lineWidth = 1;
        RectangleF frame = new RectangleF(lineWidth / 2, lineWidth / 2, size.Width - lineWidth, size.Height - lineWidth);
        if(GraphicsVisibile)
        {
          composer.BeginLocalState();
          composer.SetLineWidth(lineWidth);
          composer.SetFillColor(BackColor);
          composer.SetStrokeColor(ForeColor);
          composer.DrawRectangle(frame, 5);
          composer.FillStroke();
          composer.End();
        }

        composer.BeginLocalState();
        if(GraphicsVisibile)
        {
          composer.DrawRectangle(frame, 5);
          composer.Clip(); // Ensures that the visible content is clipped within the rounded frame.
        }
        composer.BeginMarkedContent(PdfName.Tx);
        composer.SetFont(
          new StandardType1Font(
            document,
            StandardType1Font.FamilyEnum.Helvetica,
            false,
            false
            ),
          FontSize
          );
        double y = 3;
        foreach(ChoiceItem item in field.Items)
        {
          composer.ShowText(
            item.Text,
            new PointF(0, (float)y)
            );
          y += FontSize * 1.175;
          if(y > size.Height)
            break;
        }
        composer.End();
        composer.End();

        composer.Flush();
      }
      appearance.Normal[null] = normalAppearanceState;
    }
    #endregion
    #endregion
    #endregion
  }
}