using DanfeSharp.Graphics;
using org.pdfclown.documents.contents.xObjects;
using System;
using System.Drawing;

namespace DanfeSharp
{
    internal class IdentificacaoEmitente : ElementoBase
    {
        public DanfeViewModel ViewModel { get; private set; }
        public XObject Logo { get;  set; }

        public IdentificacaoEmitente(Estilo estilo, DanfeViewModel viewModel) : base(estilo)
        {
            ViewModel = viewModel;
            Logo = null;
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);
            var rp = BoundingBox.InflatedRetangle(0.75F);
            float lh = 12.5F;

            Fonte f2, f3;
            
            if (Logo == null)
            {
                var f1 = Estilo.CriarFonteRegular(6);
                gfx.DrawString("IDENTIFICAÇÃO DO EMITENTE", rp, f1, AlinhamentoHorizontal.Centro);
                rp = rp.CutTop(f1.AlturaLinha);

                f2 = Estilo.CriarFonteNegrito(10);
                f3 = Estilo.CriarFonteRegular(9);
            }
            else
            {
                float lw = (lh * Logo.Size.Width) / Logo.Size.Height;
                var rl = new RectangleF(rp.X, rp.Y, rp.Width, lh);
                gfx.ShowXObject(Logo, rl);
                rp = rp.CutTop(12.5F);

                f2 = Estilo.CriarFonteNegrito(9);
                f3 = Estilo.CriarFonteRegular(8);
            }          
            
            var ts = new TextStack(rp) {  LineHeightScale = 1 }
                .AddLine(ViewModel.Emitente.Nome, f2)
                .AddLine(ViewModel.Emitente.EnderecoLinha1.Trim(), f3)
                .AddLine(ViewModel.Emitente.EnderecoLinha2.Trim(), f3)
                .AddLine(ViewModel.Emitente.EnderecoLinha3.Trim(), f3);
             
            ts.AlinhamentoHorizontal = AlinhamentoHorizontal.Centro;
            ts.AlinhamentoVertical = AlinhamentoVertical.Centro;
            ts.Draw(gfx);


        }
    }
}
