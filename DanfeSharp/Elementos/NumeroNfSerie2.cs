using DanfeSharp.Graphics;
using DanfeSharp.Modelo;
using System;
using System.Drawing;

namespace DanfeSharp
{
    class NumeroNfSerie2 : ElementoBase
    {
        public RectangleF RetanguloNumeroFolhas { get; private set; }
        public DanfeViewModel ViewModel { get; private set; }

        public NumeroNfSerie2(Estilo estilo, DanfeViewModel viewModel) : base(estilo)
        {
            ViewModel = viewModel;
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);
            float paddingHorizontal = ViewModel.Orientacao == Orientacao.Retrato ? 2.5F : 5F;

            var rp1 = BoundingBox.InflatedRetangle(1F, 0.5F, paddingHorizontal);
            var rp2 = rp1;

            var f1 = Estilo.CriarFonteNegrito(13);
            var f1h = f1.AlturaLinha;
            gfx.DrawString("DANFE", rp2, f1, AlinhamentoHorizontal.Centro);

            rp2 = rp2.CutTop(f1h + 1F);

            var f2 = Estilo.CriarFonteRegular(7F);
            var f2h = (float)f2.AlturaLinha;

            var ts = new TextStack(rp2)
            {
                AlinhamentoVertical = AlinhamentoVertical.Topo
            }
            .AddLine("Documento Auxiliar da", f2)
            .AddLine("Nota Fiscal Eletrônica", f2);

            ts.Draw(gfx);

            rp2 = rp2.CutTop(2F * f2h + 2.5F);

            var f3 = Estilo.FonteNumeroFolhas;
            var f3h = f3.AlturaLinha;

            ts = new TextStack(rp2)
            {
                AlinhamentoVertical = AlinhamentoVertical.Topo,
                AlinhamentoHorizontal = AlinhamentoHorizontal.Esquerda
            }
            .AddLine("0 - ENTRADA", f3)
            .AddLine("1 - SAÍDA", f3);
            ts.Draw(gfx);

            float rectEsSize = 1.75F * f3h;
            var rectEs = new RectangleF(rp2.Right - rectEsSize, rp2.Y + (2F * f3h - rectEsSize) / 2F, rectEsSize, rectEsSize);

            gfx.StrokeRectangle(rectEs, 0.25F );

            gfx.DrawString(ViewModel.TipoNF.ToString(), rectEs, f3, AlinhamentoHorizontal.Centro, AlinhamentoVertical.Centro);


            var f4 = Estilo.CriarFonteNegrito(8.5F);
            var f4h = f4.AlturaLinha;

            rp2.Height = 2F * f4h * TextStack.DefaultLineHeightScale + f3h;
            rp2.Y = rp1.Bottom - rp2.Height;

            ts = new TextStack(rp2)
            {
                AlinhamentoVertical = AlinhamentoVertical.Topo,
                AlinhamentoHorizontal = AlinhamentoHorizontal.Centro
            }
            .AddLine("N.°: " + ViewModel.NfNumero.ToString(Formatador.FormatoNumeroNF), f4)
            .AddLine($"Série: {ViewModel.NfSerie}", f4);

            ts.Draw(gfx);

            RetanguloNumeroFolhas = new RectangleF(rp1.Left, rp1.Bottom - Estilo.FonteNumeroFolhas.AlturaLinha, rp1.Width, Estilo.FonteNumeroFolhas.AlturaLinha);
        }
    }
}