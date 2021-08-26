using System;
using System.Drawing;

namespace DanfeSharp
{
    /// <summary>
    /// Campo do DANFE.
    /// </summary>
    public class DanfeCampo
    {
        /// <summary>
        /// Texto do cabeçalho.
        /// </summary>
        public string Cabecalho { get; set; }

        /// <summary>
        /// Texto do corpo.
        /// </summary>
        public string Corpo { get; set; }

        /// <summary>
        /// Retângulo do campo, contendo o tamanho e as coordenadas.
        /// </summary>
        public RectangleF Retangulo { get; set; }

        /// <summary>
        /// Alinhamento horizontal do corpo.
        /// </summary>
        public org.pdfclown.documents.contents.composition.XAlignmentEnum CorpoAlinhamentoX { get; set; }

        /// <summary>
        /// Alinhamento vertical do corpo.
        /// </summary>
        public org.pdfclown.documents.contents.composition.YAlignmentEnum CorpoAlinhamentoY { get; set; }

        /// <summary>
        /// Tamanho da fonte do corpo.
        /// </summary>
        public double CorpoTamanhoFonte { get; set; }

        /// <summary>
        /// Indica se a fonte do corpo está em negrito.
        /// </summary>
        public bool IsCorpoNegrito { get; set; }
        
        /// <summary>
        /// Se o texto for multilinha, o seu tamanho será ajustado para caber na largura caso haja overflow.
        /// </summary>
        public bool MultiLinha { get; set; }

        /// <summary>
        /// Tamanho da fonte do cabeçalho.
        /// </summary>
        public const double TamanhoFonteCabecalho = 6;

        public static readonly float PaddingInferior = Utils.Mm2Pu(0.3F);
        public static readonly float PaddingSuperior = Utils.Mm2Pu(0.7F);
        public static readonly float PaddingHorizontal = Utils.Mm2Pu(0.75F);

        /// <summary>
        /// Espaço extra entre as linhas de texto
        /// </summary>
        public const float LineSpace = 1;
        
        public DanfeCampo(string cabecalho, string corpo):
            this(cabecalho, corpo, RectangleF.Empty)
        {
        }

        public DanfeCampo(string cabecalho, string corpo, RectangleF retangulo , org.pdfclown.documents.contents.composition.XAlignmentEnum corpoAlinhamentoX = org.pdfclown.documents.contents.composition.XAlignmentEnum.Left, double corpoTamanhoFonte = 10, bool isCorpoNegrito = false, org.pdfclown.documents.contents.composition.YAlignmentEnum corpoAlinhamentoY = org.pdfclown.documents.contents.composition.YAlignmentEnum.Bottom)
        {
            Cabecalho = cabecalho;
            Corpo = corpo;
            Retangulo = retangulo;
            CorpoAlinhamentoX = corpoAlinhamentoX;
            CorpoTamanhoFonte = corpoTamanhoFonte;
            CorpoAlinhamentoY = corpoAlinhamentoY;
            IsCorpoNegrito = isCorpoNegrito;
            MultiLinha = false;
        }

        /// <summary>
        /// Valida o retângulo.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="composer"></param>
        private void ValidadeRectangle(RectangleF rect, org.pdfclown.documents.contents.composition.PrimitiveComposer composer)
        {
            if(rect.X < 0 || rect.Y < 0 || rect.Width <= 0 || rect.Height <= 0 ||
               rect.Height > composer.Scanner.CanvasSize.Height || rect.Width > composer.Scanner.CanvasSize.Width)
            {
                throw new Exception("O Retângulo não está dentro do área visível.");
            }
        }

        /// <summary>
        /// Calcula o melhor tamanho da fonte para o texto caber na largura.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fonte"></param>
        /// <param name="width"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static double AjustarFonte(string text, org.pdfclown.documents.contents.fonts.Font fonte, double width, double max, double min = 6)
        {
            double size = max;
            double w = fonte.GetWidth(text, max);

            if(w > width)
            {
                size = (max * width) / w - 0.005F;
                size = Math.Max(min, size);
            }

            return size;
        }

        /// <summary>
        /// Imprime o campo no composer.
        /// </summary>
        /// <param name="comp"></param>
        public void Print(org.pdfclown.documents.contents.composition.PrimitiveComposer comp, org.pdfclown.documents.contents.fonts.Font fonte, org.pdfclown.documents.contents.fonts.Font fonteBold)
        {

            org.pdfclown.documents.contents.composition.BlockComposer bComp = new org.pdfclown.documents.contents.composition.BlockComposer(comp);
            RectangleF pRect = Retangulo.GetPaddedRectangle(PaddingHorizontal, PaddingHorizontal, PaddingSuperior, PaddingInferior);

            comp.SetFont(fonteBold, TamanhoFonteCabecalho);
            ValidadeRectangle(pRect, comp);
            bComp.SafeBegin(pRect, org.pdfclown.documents.contents.composition.XAlignmentEnum.Left, org.pdfclown.documents.contents.composition.YAlignmentEnum.Top);
            bComp.ShowText(Cabecalho.ToUpper());
            bComp.End();

            bComp.LineSpace = new org.pdfclown.documents.contents.composition.Length(LineSpace, org.pdfclown.documents.contents.composition.Length.UnitModeEnum.Absolute);

            if (!string.IsNullOrWhiteSpace(Corpo))
            {
                org.pdfclown.documents.contents.fonts.Font fonteCorpo = IsCorpoNegrito ? fonteBold : fonte;                

                double largura = fonteCorpo.GetWidth(Corpo, CorpoTamanhoFonte);
                double novoTamanho = CorpoTamanhoFonte;

                if (!MultiLinha && largura > pRect.Width)
                {
                    novoTamanho = (CorpoTamanhoFonte * pRect.Width) / largura - Utils.Mm2Pu(0.005F);
                    comp.SetFont(fonteCorpo, novoTamanho);
                }

                comp.SetFont(fonteCorpo, novoTamanho);

                if (CorpoAlinhamentoY == org.pdfclown.documents.contents.composition.YAlignmentEnum.Top)
                {
                    float yOffSet = (float)fonteBold.GetLineHeight(TamanhoFonteCabecalho) + LineSpace;
                    pRect.Y += yOffSet;
                    pRect.Height -= yOffSet;
                }

                ValidadeRectangle(pRect, comp);
                bComp.SafeBegin(pRect, CorpoAlinhamentoX, CorpoAlinhamentoY);
                bComp.ShowText(Corpo);
                bComp.End();
            }

            comp.SafeDrawRectangle(Retangulo);
        }

        public override string ToString()
        {
            return Cabecalho;
        }
    }
}