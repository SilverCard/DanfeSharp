using DanfeSharp.Graphics;
using System;

namespace DanfeSharp
{
    class NumeroNfSerie : ElementoBase
    {
        public String NfNumero { get; private set; }
        public String NfSerie { get; private set; }

        public NumeroNfSerie(Estilo estilo, String nfNumero, String nfSerie) : base(estilo)
        {
            NfNumero = nfNumero;
            NfSerie = nfSerie;
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            var r = BoundingBox.InflatedRetangle(1);

            var f1 = Estilo.CriarFonteNegrito(14);
            var f2 = Estilo.CriarFonteNegrito(11F);

            gfx.DrawString("NF-e", r, f1, AlinhamentoHorizontal.Centro);

            r = r.CutTop(f1.AlturaLinha);

            TextStack ts = new TextStack(r)
            {
                AlinhamentoHorizontal = AlinhamentoHorizontal.Centro,
                AlinhamentoVertical = AlinhamentoVertical.Centro,
                LineHeightScale = 1F
            }
            .AddLine($"Nº.: {NfNumero}", f2)
            .AddLine($"Série: {NfSerie}", f2);

            ts.Draw(gfx);

        }
    }
}