using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DanfeSharp
{
    public class DanfePagina
    {
        /// <summary>
        /// Blocos superiores
        /// </summary>
        public List<BlocoDanfe> BlocosSuperiores { get; private set; }

        /// <summary>
        /// Blocos inferiores
        /// </summary>
        public List<BlocoDanfe> BlocosInferiores { get; private set; }

        private DanfeDocumento _Danfe;
        private org.pdfclown.documents.contents.composition.PrimitiveComposer _Composer;
        internal org.pdfclown.documents.Page _Page;

        public DanfePagina(DanfeDocumento danfe)
        {       
            _Danfe = danfe;
            _Page = new org.pdfclown.documents. Page(_Danfe.Document, _Danfe.Size);
            _Composer = new org.pdfclown.documents.contents.composition.PrimitiveComposer(_Page);
            BlocosInferiores = new List<BlocoDanfe>();
            BlocosSuperiores = new List<BlocoDanfe>();
        }

        public void AdicionarBlocoSuperior(BlocoDanfe bloco)
        {
            BlocosSuperiores.Add(bloco);
        }

        public void AdicionarBlocoInferior(BlocoDanfe bloco)
        {
            BlocosInferiores.Add(bloco);
        }

        private RectangleF GetFolhaRect()
        {
            BlocoDadosNFe bloco = BlocosSuperiores.FirstOrDefault(x => x is BlocoDadosNFe) as BlocoDadosNFe;

            if(bloco == null)
            {
                throw new Exception("O bloco BlocoDadosNFe não foi encontrado.");
            }

            RectangleF r = bloco.RetanguloFolha;
            r.X += bloco.Posicao.X;
            r.Y += bloco.Posicao.Y;

            return r;
        }

        private void PosicionarBlocos()
        {
            float y = _Danfe.InnerRect.Top;

            foreach (var bloco in BlocosSuperiores)
            {
                bloco.Posicao = new PointF(_Danfe.InnerRect.Left, y);
                y += bloco.Size.Height;
            }

            y = _Danfe.InnerRect.Bottom;

            foreach (var bloco in Enumerable.Reverse(BlocosInferiores))
            {
                y -= bloco.Size.Height;
                bloco.Posicao = new PointF(_Danfe.InnerRect.Left, y);
            }
        }

        private void PrintMarcaDAgua(string text)
        {
            PointF p = PointF.Empty;

            var blocoProdutos = BlocosSuperiores.FirstOrDefault(x => x is BlocoProdutos);

            if (blocoProdutos == null)
            {
                p = new PointF(_Danfe.Size.Width / 2f, _Danfe.Size.Height / 2f);
            }
            else
            {
                p.X = blocoProdutos.Posicao.X + blocoProdutos.Size.Width / 2F;
                p.Y = blocoProdutos.Posicao.Y + blocoProdutos.Size.Height / 2F;
            }

            _Composer.BeginLocalState();
            _Composer.SetFont(_Danfe.FontBold, 50);
            org.pdfclown.documents.contents.ExtGState state = new org.pdfclown.documents.contents.ExtGState(_Danfe.Document);
            state.FillAlpha = 0.3F;
            _Composer.ApplyState(state);
            _Composer.ShowText(text, p, org.pdfclown.documents.contents.composition.XAlignmentEnum.Center, org.pdfclown.documents.contents.composition.YAlignmentEnum.Middle, 0);
            _Composer.End();
        }

        public void PrintCreditos()
        {
            org.pdfclown.documents.contents.composition.BlockComposer bComp = new org.pdfclown.documents.contents.composition.BlockComposer(_Composer);
            RectangleF rect = _Danfe.InnerRect;

            rect.Y = rect.Bottom + Utils.Mm2Pu(0.5F);
            rect.Height = _Danfe.Size.Height - rect.Y;

            _Composer.SetFont(_Danfe.Font, 6);
            bComp.SafeBegin(rect, org.pdfclown.documents.contents.composition.XAlignmentEnum.Right, org.pdfclown.documents.contents.composition.YAlignmentEnum.Top);
            bComp.ShowText("Gerado com DanfeSharp");
            bComp.End(); 
        }

        private void PrintNumeroFolhas(int pagina, int nPaginas)
        {
            // Número de folhas
            var rFolhas = GetFolhaRect();
            org.pdfclown.documents.contents.composition.BlockComposer bComp = new org.pdfclown.documents.contents.composition.BlockComposer(_Composer);
            bComp.SafeBegin(rFolhas, org.pdfclown.documents.contents.composition.XAlignmentEnum.Left, org.pdfclown.documents.contents.composition.YAlignmentEnum.Bottom);
            _Composer.SetFont(_Danfe.FontBold, BlocoDadosNFe.TamanhoFonteNumeracao);
            bComp.ShowText(string.Format("{0}/{1}", pagina, nPaginas));
            bComp.End();
        }

        public void Renderizar(int pagina, int nPaginas)
        {
            PosicionarBlocos();

            foreach (var bloco in BlocosSuperiores.Union(BlocosInferiores))
            {
                _Composer.ShowXObject(bloco.ToXObject(), bloco.Posicao);
            }

            PrintNumeroFolhas(pagina, nPaginas);           
         
            if(_Danfe.Model.TipoAmbiente != 1)
            {
                PrintMarcaDAgua("SEM VALOR FISCAL");
            }

            PrintCreditos();

            _Composer.Flush();
        }

        public float GetAlturaCorpo(RectangleF innerRectangle)
        {
            return innerRectangle.Height - BlocosSuperiores.Sum(x => x.Size.Height) - BlocosInferiores.Sum(x => x.Size.Height);
        }
    }
}