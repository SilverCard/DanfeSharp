using DanfeSharp.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanfeSharp
{
    internal class CutLine : DrawableBase
    {

        public float Margin { get; set; }
        public double[] DashPattern { get; set; }

        public CutLine(float margin)
        {
            Margin = margin;
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            gfx.PrimitiveComposer.BeginLocalState();
            gfx.PrimitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
            gfx.PrimitiveComposer.DrawLine(new PointF(BoundingBox.Left, Y + Margin).ToPointMeasure(), new PointF(BoundingBox.Right, Y + Margin).ToPointMeasure() );
            gfx.PrimitiveComposer.Stroke();
            gfx.PrimitiveComposer.End();

        }

        public override float Height { get => 2 * Margin; set => throw new NotSupportedException(); }
    }
}
