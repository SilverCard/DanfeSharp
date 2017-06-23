using DanfeSharp.Graphics;
using System;

namespace DanfeSharp
{
    internal class CabecalhoBloco : ElementoBase
    {
        public const float MargemSuperior = 0.8F;
        public String Cabecalho { get; set; }

        public CabecalhoBloco(Estilo estilo, String cabecalho) : base(estilo)
        {
            Cabecalho = cabecalho ?? throw new ArgumentNullException(cabecalho);
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);
            gfx.DrawString(Cabecalho.ToUpper(), BoundingBox, Estilo.FonteBlocoCabecalho, 
                AlinhamentoHorizontal.Esquerda, AlinhamentoVertical.Base );
        }

        public override float Height { get => MargemSuperior + Estilo.FonteBlocoCabecalho.AlturaLinha; set => throw new NotSupportedException(); }
        public override bool PossuiContono => false;
    }
}
