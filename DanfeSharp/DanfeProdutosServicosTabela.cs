using org.pdfclown.documents.contents;
using org.pdfclown.documents.contents.composition;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DanfeSharp
{
    public class DanfeProdutosServicosTabela
    {
        /// <summary>
        /// Valores da tabela, [Colunas, Linhas]
        /// </summary>
        public String[,] Valores { get; set; }

        public readonly float Margem = Utils.Mm2Pu(0.75F);
        public readonly float AlturaCabecalhoTabela = Utils.Mm2Pu(5);

        private float CurrentY = 0;

        private Coluna[] _Colunas = 
        {
		    new Coluna("CÓDIGO", 15, XAlignmentEnum.Center),
            new Coluna("DESCRIÇÃO DO PRODUTO / SERVIÇO", 0,  XAlignmentEnum.Left),
            new Coluna("NCM/SH", 13, XAlignmentEnum.Center),
            new Coluna("O/CST", 8, XAlignmentEnum.Center),
            new Coluna("CFOP", 8, XAlignmentEnum.Center),
            new Coluna("UN", 8, XAlignmentEnum.Center),
            new Coluna(Strings.Quantidade, 15),
            new Coluna("VALOR UNIT", 15),
            new Coluna("VALOR TOTAL", 15),
            new Coluna("B.CÁLC ICMS", 15),
            new Coluna("VALOR ICMS", 10),
            new Coluna("VALOR IPI", 10),
            new Coluna("ALÍQ. ICMS", 9),
            new Coluna("ALÍQ. IPI", 9)
        };

        private DanfeDocumento _Documento;

        public DanfeProdutosServicosTabela(DanfeDocumento documento)
        {

           // _Rectangles = new RectangleF[NColunas];
            _Documento = documento;

            // Converte as larguras para PDF Unit
            for (int i = 0; i < _Colunas.Length; i++)
            {
                _Colunas[i].Largura = Utils.Mm2Pu(_Colunas[i].Largura);
            }
        }

        public void PrintCabecalhos(PrimitiveComposer composer)
        {
            BlockComposer bComp = new BlockComposer(composer);
            composer.SetFont(_Documento.FontBold, 5);

            for (int i = 0; i < _Colunas.Length; i++)
            {
                RectangleF r = _Colunas[i].Retangulo;
                r.Height = AlturaCabecalhoTabela;
                bComp.SafeBegin(r.GetPaddedRectangle(Margem), _Colunas[i].AlinhamentoHorizontal, YAlignmentEnum.Middle);
                bComp.ShowText(_Colunas[i].Cabecalho.ToUpper());
                bComp.End();

                composer.SafeDrawRectangle(r);
            }

        }

        private void PrintTextCell(PrimitiveComposer composer, String text, RectangleF rect, XAlignmentEnum align = XAlignmentEnum.Center)
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.Replace(Char.ConvertFromUtf32(160), Char.ConvertFromUtf32(32));
            }

            BlockComposer bComp = new BlockComposer(composer);
            bComp.SafeBegin(rect, align, YAlignmentEnum.Top);
            bComp.ShowText(text);
            bComp.End();
            CurrentY = Math.Max(CurrentY, bComp.BoundBox.Bottom);

        }

        private Boolean RowFit(int i, RectangleF[] rects)
        {
            for (int c = 0; c < 14; c++)
            {
                if (Utils.GetTextHeight(_Documento.Font, 6, rects[c].Width, Valores[i, c]) + Margem > rects[c].Height)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Print as linhas tracejadas na tabela, ignorando a última
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="y">Lista com as posições y</param>
        /// <param name="xBegin"></param>
        /// <param name="xEnd"></param>
        private void PrintLinhasTracejadas(PrimitiveComposer composer, List<float> y, float xBegin, float xEnd)
        {
            if (xBegin < 0)
            {
                throw new ArgumentOutOfRangeException("xBegin");
            }

            if (xEnd > composer.Scanner.CanvasSize.Width)
            {
                throw new ArgumentOutOfRangeException("xEnd");
            }

            composer.BeginLocalState();
            composer.SetLineDash(new LineDash(new Double[] { 3, 2 }));
            for (int i = 0; i < y.Count - 1; i++)
            {
                composer.DrawLine(new PointF(xBegin, y[i]), new PointF(xEnd, y[i]));
            }

            composer.Stroke();
            composer.End();
        }

        internal int PrintTable(PrimitiveComposer composer, RectangleF tableArea, int lInit = 0)
        {
            _Colunas[1].Largura = tableArea.Width - _Colunas.Sum(x => x.Largura);
            _Colunas[0].Retangulo = new RectangleF(tableArea.Left, tableArea.Top, _Colunas[0].Largura, tableArea.Height - AlturaCabecalhoTabela);


            for (int i = 1; i < _Colunas.Length; i++)
            {
                RectangleF r = _Colunas[i - 1].Retangulo;
                r.Width = _Colunas[i].Largura;
                r.X = _Colunas[i - 1].Retangulo.Right;
                _Colunas[i].Retangulo = r;
            }

            PrintCabecalhos(composer);

            composer.SetFont(_Documento.Font, 6);

            // Retângulos com padding
            var pr = _Colunas.Select(x => x.Retangulo).ToArray();

            for (int i = 0; i < _Colunas.Length; i++)
            {

                pr[i].Y += AlturaCabecalhoTabela + Margem;
                pr[i].Height -= AlturaCabecalhoTabela + 2 * Margem;
                pr[i].Width -= 2 * Margem;
                pr[i].X += Margem;
            }

            composer.Stroke();

            List<float> dashedLinesY = new List<float>();

            int l = lInit;
            for (; l < Valores.GetLength(0); l++)
            {

                if (!RowFit(l, pr) || CurrentY >= tableArea.Bottom || pr[0].Height <= 0 || ((_Documento.Font.GetLineHeight(6) + Margem) > pr[0].Height))
                {
                    break;
                }


                for (int c = 0; c < _Colunas.Length; c++)
                {
                    PrintTextCell(composer, Valores[l, c], pr[c], _Colunas[c].AlinhamentoHorizontal);
                }

                for (int c = 0; c < _Colunas.Length; c++)
                {
                    pr[c].Y = CurrentY + 2 * Margem;
                    pr[c].Height = tableArea.Bottom - pr[c].Y;
                }

                dashedLinesY.Add(CurrentY + Margem);

            }

            PrintLinhasTracejadas(composer, dashedLinesY, tableArea.Left, tableArea.Right);

            for (int i = 0; i < _Colunas.Length; i++)
            {
                RectangleF rct = _Colunas[i].Retangulo;
                rct.Height = CurrentY + Margem - _Colunas[i].Retangulo.Top;
                _Colunas[i].Retangulo = rct;
                composer.SafeDrawRectangle(_Colunas[i].Retangulo);
            }

            return l;
        }

        private class Coluna
        {
            public String Cabecalho { get; private set; }
            public float Largura { get; set; }
            public XAlignmentEnum AlinhamentoHorizontal { get; set; }
            public RectangleF Retangulo { get; set; }

            public Coluna(String cabecalho, float largura, XAlignmentEnum alinhamentoHorizontal = XAlignmentEnum.Right)
            {
                Cabecalho = cabecalho;
                Largura = largura;
                AlinhamentoHorizontal = alinhamentoHorizontal;
            }

            public override string ToString()
            {
                return Cabecalho;
            }
        }
    }
}
