using org.pdfclown.documents.contents.composition;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

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
        public String Cabecalho { get; set; }

        /// <summary>
        /// Texto do corpo.
        /// </summary>
        public String Corpo { get; set; }

        /// <summary>
        /// Retângulo do campo, contendo o tamanho e as coordenadas.
        /// </summary>
        public RectangleF Retangulo { get; set; }

        /// <summary>
        /// Alinhamento horizontal do corpo.
        /// </summary>
        public XAlignmentEnum CorpoAlinhamentoX { get; set; }

        /// <summary>
        /// Alinhamento vertical do corpo.
        /// </summary>
        public YAlignmentEnum CorpoAlinhamentoY { get; set; }

        /// <summary>
        /// Tamanho da fonte do corpo.
        /// </summary>
        public Double CorpoTamanhoFonte { get; set; }

        /// <summary>
        /// Indica se a fonte do corpo está em negrito.
        /// </summary>
        public Boolean IsCorpoNegrito { get; set; }

        /// <summary>
        /// Fonte normal.
        /// </summary>
        public org.pdfclown.documents.contents.fonts.Font Fonte { get; set; }

        /// <summary>
        /// Fonte em Negrito
        /// </summary>
        public org.pdfclown.documents.contents.fonts.Font FonteBold { get; set; }

        /// <summary>
        /// Verdadeiro caso o campo deverá ser impresso.
        /// </summary>
        public Boolean WillPrint { get; set; }

        /// <summary>
        /// Se o texto for multilinha, o seu tamanho será ajustado para caber na largura caso haja overflow.
        /// </summary>
        public Boolean MultiLinha { get; set; }

        /// <summary>
        /// Tamanho da fonte do cabeçalho.
        /// </summary>
        public const double TamanhoFonteCabecalho = 6;

        public static readonly float PaddingInferior = Unit.Mm2Pu(0.3F);
        public static readonly float PaddingSuperior = Unit.Mm2Pu(0.7F);
        public static readonly float PaddingHorizontal = Unit.Mm2Pu(0.75F);

        /// <summary>
        /// Espaço extra entre as linhas de texto
        /// </summary>
        public const float LineSpace = 1;

        
        public DanfeCampo(String cabecalho, String corpo):
            this(cabecalho, corpo, RectangleF.Empty)
        {
        }

        public DanfeCampo(String cabecalho, String corpo, RectangleF retangulo , XAlignmentEnum corpoAlinhamentoX = XAlignmentEnum.Left, double corpoTamanhoFonte = 10, Boolean isCorpoNegrito = false, YAlignmentEnum corpoAlinhamentoY = YAlignmentEnum.Bottom)
        {
            Cabecalho = cabecalho;
            Corpo = corpo;
            Retangulo = retangulo;
            CorpoAlinhamentoX = corpoAlinhamentoX;
            CorpoTamanhoFonte = corpoTamanhoFonte;
            CorpoAlinhamentoY = corpoAlinhamentoY;
            IsCorpoNegrito = isCorpoNegrito;
            WillPrint = true;
            MultiLinha = false;
        }

        /// <summary>
        /// Valida o retângulo.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="composer"></param>
        private void ValidadeRectangle(RectangleF rect, PrimitiveComposer composer)
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
        public static double AjustarFonte(String text, org.pdfclown.documents.contents.fonts.Font fonte, double width, double max, double min = 6)
        {
            Double size = max;
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
        public void Print(PrimitiveComposer comp)
        {
            if (!WillPrint)
                return;

            BlockComposer bComp = new BlockComposer(comp);
            RectangleF pRect = Retangulo.GetPaddedRectangle(PaddingHorizontal, PaddingHorizontal, PaddingSuperior, PaddingInferior);

            comp.SetFont(FonteBold, TamanhoFonteCabecalho);
            ValidadeRectangle(pRect, comp);
            bComp.SafeBegin(pRect, XAlignmentEnum.Left, YAlignmentEnum.Top);
            bComp.ShowText(Cabecalho.ToUpper());
            bComp.End();

            bComp.LineSpace = new Length(LineSpace, Length.UnitModeEnum.Absolute);

            if (!String.IsNullOrWhiteSpace(Corpo))
            {
                org.pdfclown.documents.contents.fonts.Font fonteCorpo = IsCorpoNegrito ? FonteBold : Fonte;                

                double largura = fonteCorpo.GetWidth(Corpo, CorpoTamanhoFonte);
                double novoTamanho = CorpoTamanhoFonte;

                if (!MultiLinha && largura > pRect.Width)
                {
                    novoTamanho = (CorpoTamanhoFonte * pRect.Width) / largura - Unit.Mm2Pu(0.005F);
                    comp.SetFont(fonteCorpo, novoTamanho);
                }

                comp.SetFont(fonteCorpo, novoTamanho);

                if (CorpoAlinhamentoY == YAlignmentEnum.Top)
                {
                    float yOffSet = (float)FonteBold.GetLineHeight(TamanhoFonteCabecalho) + LineSpace;
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
