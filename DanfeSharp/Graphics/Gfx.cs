using org.pdfclown.documents.contents.composition;
using System.Drawing;
using System;
using org.pdfclown.documents.contents.xObjects;

namespace DanfeSharp.Graphics
{
    internal class Gfx 
    {
        public PrimitiveComposer PrimitiveComposer { get; private set; }

        public Gfx(PrimitiveComposer xg)
        {
            PrimitiveComposer = xg;          
        }

        public void DrawRectangle(float x, float y, float w, float h) => DrawRectangle(new RectangleF(x, y, w, h));
        public void DrawRectangle(RectangleF rect) => PrimitiveComposer.DrawRectangle(rect.ToPointMeasure());
        public void Fill() => PrimitiveComposer.Fill();
            

        internal void DrawString(string str, RectangleF rect, Fonte fonte, AlinhamentoHorizontal ah = AlinhamentoHorizontal.Esquerda, AlinhamentoVertical av = AlinhamentoVertical.Topo)
        {
            if (fonte.Tamanho <= 0) throw new ArgumentOutOfRangeException(nameof(fonte));

            var p = rect.Location;

            if (av == AlinhamentoVertical.Base)
                p.Y = rect.Bottom - fonte.AlturaLinha;
            else if (av == AlinhamentoVertical.Centro)
                p.Y += (rect.Height - fonte.AlturaLinha) / 2F;


            if (ah == AlinhamentoHorizontal.Direita)
                p.X = rect.Right - fonte.MedirLarguraTexto(str);
            if (ah == AlinhamentoHorizontal.Centro)
                p.X += (rect.Width - fonte.MedirLarguraTexto(str)) / 2F;

            PrimitiveComposer.SetFont(fonte.FonteInterna, fonte.Tamanho);
            PrimitiveComposer.ShowText(str, p.ToPointMeasure());
        }

        public void ShowXObject(XObject xobj, RectangleF r)
        {
            PointF p = new PointF();
            SizeF s = new SizeF();
            SizeF xs = xobj.Size.ToMm();

            if(r.Height >= r.Width)
            {
                if(xs.Height >= xs.Width)
                {
                    s.Height = r.Height;
                    s.Width = (s.Height * xs.Width) / xs.Height; 
                }
                else
                {
                    s.Width = r.Width;
                    s.Height = (s.Width * xs.Height) / xs.Width;
                }
            }
            else
            {
                if (xs.Height >= xs.Width)
                {
                    s.Width = r.Width;
                    s.Height = (s.Width * xs.Height) / xs.Width;
                }
                else
                {
                    s.Height = r.Height;
                    s.Width = (s.Height * xs.Width) / xs.Height;
                }
            }

            p.X = r.X + (r.Width - s.Width) / 2F;
            p.Y = r.Y + (r.Height - s.Height) / 2F;

            PrimitiveComposer.ShowXObject(xobj, p.ToPointMeasure(), s.ToPointMeasure());
        }

        public void StrokeRectangle(RectangleF rect, float width)
        {
            SetLineWidth(width);
            PrimitiveComposer.DrawRectangle(rect.ToPointMeasure());
            Stroke();
        }

        public void Stroke() => PrimitiveComposer.Stroke();
        public void SetLineWidth(float w) => PrimitiveComposer.SetLineWidth(w);      
        public void Flush() => PrimitiveComposer.Flush();
    }
}
