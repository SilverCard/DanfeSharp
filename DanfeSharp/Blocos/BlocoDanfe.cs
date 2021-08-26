﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DanfeSharp
{
    public abstract class BlocoDanfe
    {
        public DanfeDocumento Danfe { get; protected set; }
        public SizeF Size { get; protected set; }
        public virtual string Cabecalho { get; private set; }
        protected RectangleF InternalRectangle { get; set; }

        protected org.pdfclown.documents.contents.xObjects.FormXObject _RenderedObject;

        public HashSet<DanfeCampo> Campos { get; set; }
        public PointF Posicao { get; set; }

        //protected bool _PossuiBordaTopo = true;
        //protected bool _PossuiBordaBaixo = true;
        //protected bool _PossuiBordaEsquerda = true;
        //protected bool _PossuiBordaDireita = true;

        public BlocoDanfe(DanfeDocumento danfeMaker)
        {
            Danfe = danfeMaker;
            _RenderedObject = null;
            Cabecalho = null;
            Campos = new HashSet<DanfeCampo>();
        }

        protected void Initialize()
        {
            InternalRectangle = GetInnerRectangle();
            CriarCampos();
            PosicionarCampos();
        }

        protected virtual DanfeCampo CriarCampo(string cabecalho, string corpo, org.pdfclown.documents.contents.composition.XAlignmentEnum corpoAlinhamentoX = org.pdfclown.documents.contents.composition.XAlignmentEnum.Left)
        {
            DanfeCampo campo = new DanfeCampo(cabecalho, corpo);
            Campos.Add(campo);
            campo.CorpoAlinhamentoX = corpoAlinhamentoX;
            return campo;
        }

        protected virtual DanfeCampo CriarCampo(string cabecalho, string corpo, RectangleF retangulo, org.pdfclown.documents.contents.composition.XAlignmentEnum corpoAlinhamentoX = org.pdfclown.documents.contents.composition.XAlignmentEnum.Left, double corpoTamanhoFonte = 10, bool isCorpoNegrito = false, org.pdfclown.documents.contents.composition.YAlignmentEnum corpoAlinhamentoY = org.pdfclown.documents.contents.composition.YAlignmentEnum.Bottom)
        {
            DanfeCampo campo = new DanfeCampo(cabecalho, corpo, retangulo, corpoAlinhamentoX, corpoTamanhoFonte, isCorpoNegrito, corpoAlinhamentoY);
            Campos.Add(campo);
            return campo;
        }

        protected virtual void ToXObjectInternal(org.pdfclown.documents.contents.composition.PrimitiveComposer composer)
        {
        }

        protected abstract void CriarCampos();
        protected abstract void PosicionarCampos();

        /// <summary>
        /// Posiciona os campos lado a lado em ordem com a mesma largura
        /// </summary>
        /// <param name="area">Retângulo contendo a área na qual os campos irão serem posicionados.</param>
        /// <param name="campos">Campos a serem posicionados.</param>
        public static void PosicionarLadoLado(RectangleF area, params DanfeCampo[] campos)
        {
            float w = area.Width / (float)campos.Length;

            area.Width = w;
            campos[0].Retangulo = area;

            for (int i = 1; i < campos.Length; i++)
            {
                area.X = area.Right;
                campos[i].Retangulo = area;
            }
        }

        /// <summary>
        /// Posiciona os campos lado a lado em ordem utilizando a largura contida no array
        /// </summary>
        /// <param name="area">Retângulo contendo a área na qual os campos irão serem posicionados.</param>
        /// <param name="larguras">Larguras dos campos em milímetros</param>
        /// <param name="campos">Campos a serem posicionados.</param>
        public static void PosicionarLadoLadoMm(RectangleF area, float[] larguras, params DanfeCampo[] campos)
        {
            for (int i = 0; i < larguras.Length; i++)
            {
                if (larguras[i] != 0)
                {
                    larguras[i] = Utils.Mm2Pu(larguras[i]);
                }
            }

            PosicionarLadoLado(area, larguras, campos);
        }

        /// <summary>
        /// Posiciona os campos lado a lado em ordem utilizando a largura contida no array
        /// </summary>
        /// <param name="area">Retângulo contendo a área na qual os campos irão serem posicionados.</param>
        /// <param name="larguras">Larguras dos campos.</param>
        /// <param name="campos">Campos a serem posicionados.</param>
        public static void PosicionarLadoLado(RectangleF area, float[] larguras, params DanfeCampo[] campos)
        {
            if (larguras.Length != campos.Length)
            {
                throw new ArgumentException("O tamanho do array das larguras deve ser igual a quantidade de campos.");
            }

            float sum = larguras.Sum();

            if (sum > area.Width)
            {
                throw new ArgumentException("A soma das larguras não deve ultrapassar a largura do retângulo.");
            }

            // Distribui a largura que faltou
            float w = area.Width - sum;
            w /= (float)larguras.Count(x => x == 0);

            for (int i = 0; i < larguras.Length; i++)
            {
                if (larguras[i] == 0)
                {
                    larguras[i] = w;
                }
            }

            // Posiciona os campos
            area.Width = larguras[0];
            campos[0].Retangulo = area;

            for (int i = 1; i < campos.Length; i++)
            {
                area.X = area.Right;
                area.Width = larguras[i];
                campos[i].Retangulo = area;
            }
        }

        /// <summary>
        /// Imprime o cabeçalho do bloco.
        /// </summary>
        private void PrintCabecalho(org.pdfclown.documents.contents.composition.PrimitiveComposer comp)
        {
            org.pdfclown.documents.contents.composition.BlockComposer bComp = new org.pdfclown.documents.contents.composition.BlockComposer(comp);

            RectangleF rect = new RectangleF(0, 0, Size.Width, Danfe.CabecalhoBlocoAltura);
            rect = rect.GetPaddedRectangleMm(0, 0, 1, 0.3F);

            comp.SetFont(Danfe.FontBold, 5);
            bComp.SafeBegin(rect, org.pdfclown.documents.contents.composition.XAlignmentEnum.Left, org.pdfclown.documents.contents.composition.YAlignmentEnum.Bottom);
            bComp.ShowText(Cabecalho.ToUpper());
            bComp.End();

            comp.Flush();
        }

        /// <summary>
        /// Renderiza o bloco para um XObject.
        /// </summary>
        public virtual org.pdfclown.documents.contents.xObjects.XObject ToXObject()
        {
            if (_RenderedObject == null)
            {
                _RenderedObject = new org.pdfclown.documents.contents.xObjects.FormXObject(Danfe.Document, Size);

                org.pdfclown.documents.contents.composition.PrimitiveComposer composer = new org.pdfclown.documents.contents.composition.PrimitiveComposer(_RenderedObject);
                var obj = composer.BeginLocalState();
                composer.SetLineWidth(DanfeDocumento.LineWidth);

                if (PossuiCabecalho)
                {
                    RectangleF rect = GetHeaderInnerRectangle();
                    //Danfe.PrintCabecalhoBloco2(composer, rect.Top, rect.Left, rect.Width, Cabecalho);
                    PrintCabecalho(composer);
                }

                ToXObjectInternal(composer);

                foreach (var campo in Campos)
                {
                    campo.Print(composer, Danfe.Font, Danfe.FontBold);
                }

                composer.Stroke();

                composer.End();
                composer.Flush();
            }

            return _RenderedObject;
        }

        private RectangleF GetHeaderInnerRectangle()
        {
            return new RectangleF(DanfeDocumento.LineWidth / 2F, DanfeDocumento.LineWidth / 2F, Size.Width - DanfeDocumento.LineWidth, Size.Height - DanfeDocumento.LineWidth);
        }

        private RectangleF GetInnerRectangle()
        {
            float y = 0;

            if (PossuiCabecalho)
            {
                y += Danfe.CabecalhoBlocoAltura;
            }


            return new RectangleF(DanfeDocumento.LineWidth / 2F, DanfeDocumento.LineWidth / 2F + y, Size.Width - DanfeDocumento.LineWidth, Size.Height - DanfeDocumento.LineWidth - y);
        }

        public bool PossuiCabecalho
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Cabecalho);
            }
        }

        public RectangleF Retangulo
        {
            get
            {
                return new RectangleF(Posicao, Size);
            }
        }
    }
}