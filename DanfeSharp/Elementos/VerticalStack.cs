using DanfeSharp.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanfeSharp
{
    internal class VerticalStack : DrawableBase
    {
        public List<DrawableBase> Drawables { get; private set; }

        public VerticalStack()
        {
            Drawables = new List<DrawableBase>();
        }

        public VerticalStack(float width) : this()
        {
            Width = width;
        }


        public void Add(params DrawableBase[] db)
        {
            foreach (var item in db)
            {
                if (item == this) throw new InvalidOperationException();

                Drawables.Add(item);
            }
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            float x = X, y = Y;

            for (int i = 0; i < Drawables.Count; i++)
            {
                var db = Drawables[i];
                db.Width = Width;
                db.SetPosition(x, y);
                db.Draw(gfx);
                y += db.Height;             
            }
        }

        public override float Height { get => Drawables.Sum(x => x.Height); set => throw new NotSupportedException(); }
        public override void SetSize(float w, float h) => throw new NotSupportedException();


    }
}
