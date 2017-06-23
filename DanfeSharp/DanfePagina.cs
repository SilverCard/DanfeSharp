using System.Drawing;
using System.Linq;
using DanfeSharp.Blocos;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;
using DanfeSharp.Graphics;

namespace DanfeSharp
{
    internal class DanfePagina
    {
        public Danfe Danfe { get; private set; }
        public Page PdfPage { get; private set; }
        public PrimitiveComposer PrimitiveComposer { get; private set; }
        public Gfx Gfx { get; private set; }
        public RectangleF RetanguloNumeroFolhas { get;  set; }
        public RectangleF RetanguloCorpo { get; private set; }
        public RectangleF RetanguloDesenhavel { get; private set; }
        public RectangleF Retangulo { get; private set; }

        public DanfePagina(Danfe danfe)
        {
            Danfe = danfe;
            PdfPage = new Page(Danfe.PdfDocument);
            Danfe.PdfDocument.Pages.Add(PdfPage);
         
            PrimitiveComposer = new PrimitiveComposer(PdfPage);
            Gfx = new Gfx(PrimitiveComposer);

            if (Danfe.ViewModel.Orientacao == Orientacao.Retrato)            
                Retangulo = new RectangleF(0, 0, Constantes.A4Largura, Constantes.A4Altura);            
            else            
                Retangulo = new RectangleF(0, 0, Constantes.A4Altura, Constantes.A4Largura);
            
            RetanguloDesenhavel = Retangulo.InflatedRetangle(Constantes.Margem);
            PdfPage.Size = new SizeF(Retangulo.Width.ToPoint(), Retangulo.Height.ToPoint());       

           

        }

        private void DesenharCanhoto()
        {
            var c = Danfe.Canhoto;
            c.SetPosition(RetanguloDesenhavel.Location);

            if (Danfe.ViewModel.Orientacao == Orientacao.Retrato)
            {
                c.Width = RetanguloDesenhavel.Width;
                c.Draw(Gfx);
                RetanguloDesenhavel = RetanguloDesenhavel.CutTop(c.Height);
            }
            else
            {
                c.Width = RetanguloDesenhavel.Height;

                Gfx.PrimitiveComposer.BeginLocalState();
                Gfx.PrimitiveComposer.Rotate(90, new PointF(c.Y - c.X, c.Width + c.X + c.Y).ToPointMeasure());
                c.Draw(Gfx);
                Gfx.PrimitiveComposer.End();

                RetanguloDesenhavel = RetanguloDesenhavel.CutLeft(c.Height);
            }

        }

        public void DesenhaNumeroPaginas(int n, int total)
        {
            Gfx.DrawString($"Folha {n}/{total}", RetanguloNumeroFolhas, Danfe.EstiloPadrao.FonteNumeroFolhas, AlinhamentoHorizontal.Centro);
            Gfx.Flush();
        }

        public void DesenharBlocos(bool isPrimeirapagina = false)
        {
            if (isPrimeirapagina) DesenharCanhoto();

            var blocos = isPrimeirapagina ? Danfe._Blocos : Danfe._Blocos.Where(x => x.VisivelSomentePrimeiraPagina == false);

            foreach (var bloco in blocos)
            {
                bloco.Width = RetanguloDesenhavel.Width;

                if (bloco.Posicao == PosicaoBloco.Topo)
                {
                    bloco.SetPosition(RetanguloDesenhavel.Location);
                    RetanguloDesenhavel = RetanguloDesenhavel.CutTop(bloco.Height);
                }
                else
                {
                    bloco.SetPosition(RetanguloDesenhavel.X, RetanguloDesenhavel.Bottom - bloco.Height);
                    RetanguloDesenhavel = RetanguloDesenhavel.CutBottom(bloco.Height);
                }

                bloco.Draw(Gfx);

                if (bloco is BlocoIdentificacaoEmitente)
                {
                    var rf = (bloco as BlocoIdentificacaoEmitente).RetanguloNumeroFolhas;
                    RetanguloNumeroFolhas = rf;
                }
            }

            RetanguloCorpo = RetanguloDesenhavel;
            Gfx.Flush();
        }
    }
}
