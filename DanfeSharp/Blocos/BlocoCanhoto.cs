using DanfeSharp.Model;
using System;
using System.Drawing;

namespace DanfeSharp
{
    public class BlocoCanhoto : BlocoDanfe
    {
        /// <summary>
        /// Margem vertical da linha pontilhada
        /// </summary>
        public static readonly float MargemLinhaPontilhada = Utils.Mm2Pu(2F);

        /// <summary>
        /// Largura do campo contendo as numerações da NFe
        /// </summary>
        public static readonly float NumeracaoWidth = Utils.Mm2Pu(35);

        /// <summary>
        /// Altura do Campo Recebemos
        /// </summary>
        public static readonly float RecebemosHeight = Utils.Mm2Pu(8.5F);

        public DanfeCampo DataRecebimento { get; set; }
        public DanfeCampo AssinaturaRecebedor { get; set; }

        public RectangleF RetRecebemos { get; set; }
        public RectangleF RetNumeracao { get; set; }

        public BlocoCanhoto(DanfeDocumento danfeMaker)
            : base(danfeMaker)
        {
            Size = new SizeF(Danfe.InnerRect.Width, RecebemosHeight + danfeMaker.CampoAltura + 2 * MargemLinhaPontilhada);
            Initialize();
        }

        protected override void CriarCampos()
        {
            DataRecebimento = CriarCampo("Data de Recebimento", null);
            AssinaturaRecebedor = CriarCampo("Assinatura recebedor", null);
        }

        protected override void PosicionarCampos()
        {
            RetNumeracao = new RectangleF(InternalRectangle.Right - NumeracaoWidth, InternalRectangle.Top, NumeracaoWidth, Danfe.CampoAltura + RecebemosHeight);
            RetRecebemos = new RectangleF(InternalRectangle.Left, InternalRectangle.Top, InternalRectangle.Width - RetNumeracao.Width, RecebemosHeight);

            RectangleF ret = new RectangleF(InternalRectangle.Left, RetRecebemos.Bottom, InternalRectangle.Width - RetNumeracao.Width, Danfe.CampoAltura);
            PosicionarLadoLadoMm(ret, new float[] { 41, 0 }, DataRecebimento, AssinaturaRecebedor);
        }

        protected override void ToXObjectInternal(org.pdfclown.documents.contents.composition.PrimitiveComposer composer)
        {
            EmpresaViewModel empresa;
            if (Danfe.Model.TipoNF == 1)
            {
                empresa = Danfe.Model.Emitente;
            }
            else if(Danfe.Model.TipoNF == 0)
            {
                empresa = Danfe.Model.Destinatario;
            }
            else
            {
                throw new Exception("Tipo de NF não suportado.");
            }

            org.pdfclown.documents.contents.composition.BlockComposer bComp = new org.pdfclown.documents.contents.composition.BlockComposer(composer);

            composer.SafeDrawRectangle(RetNumeracao);
            composer.SafeDrawRectangle(RetRecebemos);   

            composer.SetFont(Danfe.Font, 6);
            bComp.SafeBegin(RetRecebemos.GetPaddedRectangleMm(1), org.pdfclown.documents.contents.composition.XAlignmentEnum.Left, org.pdfclown.documents.contents.composition.YAlignmentEnum.Middle);
            bComp.ShowText(string.Format("RECEBEMOS DE {0} OS PRODUTOS E/OU SERVIÇOS CONSTANTES DA NOTA FISCAL ELETRÔNICA INDICADA ABAIXO.", empresa.Nome));
            bComp.End();

            // Numeração da NFe
            composer.SafeDrawRectangle(RetNumeracao);
            composer.SetFont(Danfe.FontBold, 12);
            bComp.SafeBegin(RetNumeracao, org.pdfclown.documents.contents.composition.XAlignmentEnum.Center, org.pdfclown.documents.contents.composition.YAlignmentEnum.Middle);
            bComp.ShowText(string.Format("NF-e\nNº {0}\nSérie {1}", Danfe.Model.NumeroNF.ToString(Formatador.FormatoNumeroNF), Danfe.Model.Serie));
            bComp.End();

            composer.Stroke();

            // Linha pontilhada
            composer.BeginLocalState();
            composer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));
            composer.DrawLine(new PointF(InternalRectangle.Left, Size.Height - MargemLinhaPontilhada), new PointF(InternalRectangle.Right, Size.Height - MargemLinhaPontilhada));
            composer.Stroke();
            composer.End();
        }
    }
}