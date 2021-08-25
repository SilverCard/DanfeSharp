using org.pdfclown.documents.contents.composition;
using System;
using System.Drawing;

namespace DanfeSharp
{
    public class BlocoDadosNFe : BlocoDanfe
    {

        public const String DescricaoDanfe = "DOCUMENTO AUXILIAR DA NOTA FISCAL ELETRÔNICA";
        public const String MensagemConsulta = "Consulta de autenticidade no portal nacional da NF-e www.nfe.fazenda.gov.br/portal ou no site da Sefaz Autorizadora.";


        public const float TamanhoFonteNumeracao = 9;

        /// <summary>
        /// Largura do retangulo com a numeração da NFe
        /// </summary>
        public static readonly float RectNumeracaoW = Utils.Mm2Pu(30);

        public static readonly SizeF BarcodeTamanhoRetangulo = new SizeF(Utils.Mm2Pu(85), Utils.Mm2Pu(10));

        private RectangleF RetEmitente;

        /// <summary>
        /// Retângulo da descrição do DANFE
        /// </summary>
        public RectangleF RetDescDanfe { get; private set; }

        public RectangleF RetCodigoBarras { get; set; }
        private RectangleF RetCampoVariavel;

        public DanfeCampo NaturezaOperacao { get; set; }
        public DanfeCampo CampoVariavel2 { get; set; }

        public DanfeCampo Ie { get; set; }
        public DanfeCampo IeSt { get; set; }
        public DanfeCampo Cnpj { get; set; }

        public DanfeCampo ChaveAcesso { get; set; }

        /// <summary>
        /// Retângulo da descrição do DANFE
        /// </summary>
        public RectangleF RetanguloFolha { get; private set; }



        public BlocoDadosNFe(DanfeDocumento danfeMaker)
            : base(danfeMaker)
        {
            Size = new SizeF(Danfe.InnerRect.Width, danfeMaker.CampoAltura + 2 * BarcodeTamanhoRetangulo.Height + 2 * Danfe.CampoAltura + DanfeDocumento.LineWidth);

            Initialize();
        }

        protected override void CriarCampos()
        {
            NaturezaOperacao = CriarCampo("Natureza da operação", Danfe.Model.NaturezaOperacao);
            CampoVariavel2 = CriarCampo("PROTOCOLO DE AUTORIZAÇÃO DE USO", Danfe.Model.ProtocoloAutorizacao, XAlignmentEnum.Center);
            ChaveAcesso = CriarCampo("Chave de Acesso", Formatador.FormatarChaveAcesso(Danfe.Model.ChaveAcesso), RectangleF.Empty, XAlignmentEnum.Center, 10, true);

            Ie = CriarCampo(Strings.InscricaoEstadual, Danfe.Model.Emitente.Ie, XAlignmentEnum.Center);
            IeSt = CriarCampo("INSC. EST. DO SUBST. TRIBUTÁRIO", Danfe.Model.Emitente.IeSt, XAlignmentEnum.Center);
            Cnpj = CriarCampo(Strings.CnpjCpf, Formatador.FormatarCnpj(Danfe.Model.Emitente.CnpjCpf), XAlignmentEnum.Center);
        }

        protected override void PosicionarCampos()
        {
            RectangleF linha = new RectangleF(InternalRectangle.X, InternalRectangle.Top + 2 * BarcodeTamanhoRetangulo.Height + Danfe.CampoAltura, InternalRectangle.Width, Danfe.CampoAltura);

            PosicionarLadoLado(linha, new float[] { 0, BarcodeTamanhoRetangulo.Width }, NaturezaOperacao, CampoVariavel2);

            linha.Y += Danfe.CampoAltura;
            PosicionarLadoLado(linha, Ie, IeSt, Cnpj);

            RetEmitente = InternalRectangle;
            RetEmitente.Width = InternalRectangle.Width - BarcodeTamanhoRetangulo.Width - RectNumeracaoW;
            RetCodigoBarras = new RectangleF(InternalRectangle.Right - BarcodeTamanhoRetangulo.Width, InternalRectangle.Top, BarcodeTamanhoRetangulo.Width, BarcodeTamanhoRetangulo.Height);
            ChaveAcesso.Retangulo = new RectangleF(RetCodigoBarras.Left, RetCodigoBarras.Bottom, RetCodigoBarras.Width, Danfe.CampoAltura);

            RetCampoVariavel = RetCodigoBarras;
            RetCampoVariavel.Y = ChaveAcesso.Retangulo.Bottom;

            RetEmitente.Height = RetCodigoBarras.Height + ChaveAcesso.Retangulo.Height + RetCampoVariavel.Height;
            RetDescDanfe = new RectangleF(RetEmitente.Right, InternalRectangle.Top, RectNumeracaoW, RetEmitente.Height);

        }

        private void PrintDescricaoDanfe(RectangleF area, BlockComposer bComp, PrimitiveComposer comp)
        {

            comp.SafeDrawRectangle(area);

            // Retangulo com padding
            RectangleF pRet = area.GetPaddedRectangleMm(1);

            // DANFE e descrição
            bComp.SafeBegin(pRet, XAlignmentEnum.Center, YAlignmentEnum.Top);
            comp.SetFont(Danfe.FontBold, 12);
            bComp.ShowText("DANFE");
            comp.SetFont(Danfe.Font, 6);
            bComp.ShowBreak(new SizeF(0, 1));
            bComp.ShowText(DescricaoDanfe);
            bComp.End();

            // Entrada, Saída
            RectangleF rEntrasaSaida = new RectangleF(pRet.X, bComp.BoundBox.Bottom + Utils.Mm2Pu(1.5F), pRet.Width, pRet.Bottom - bComp.BoundBox.Bottom);
            rEntrasaSaida = rEntrasaSaida.GetPaddedRectangleMm(0, 2.5F);

            comp.SetFont(Danfe.Font, 8);
            bComp.SafeBegin(rEntrasaSaida, XAlignmentEnum.Left, YAlignmentEnum.Top);
            bComp.ShowText("0 - Entrada\n1 - Saída");
            bComp.End();

            RectangleF rEntrasaSaida2 = bComp.BoundBox;
            rEntrasaSaida2 = new RectangleF(rEntrasaSaida.Right - bComp.BoundBox.Height, bComp.BoundBox.Y, bComp.BoundBox.Height, bComp.BoundBox.Height);
            comp.SafeDrawRectangle(rEntrasaSaida2);

            bComp.SafeBegin(rEntrasaSaida2, XAlignmentEnum.Center, YAlignmentEnum.Middle);
            bComp.ShowText(Danfe.Model.TipoNF.ToString());
            bComp.End();



            // Número Série e Folha  
            RectangleF retEsquerdo = pRet;
            retEsquerdo.Width = Utils.Mm2Pu(8);

            RectangleF retDireito = pRet;
            retDireito.X = retEsquerdo.Right + Utils.Mm2Pu(1);
            retDireito.Width = pRet.Right - retDireito.Left;

            RetanguloFolha = retDireito;
            retDireito.Height -= (float)Danfe.FontBold.GetLineHeight(TamanhoFonteNumeracao);

            comp.SetFont(Danfe.FontBold, TamanhoFonteNumeracao);
            bComp.SafeBegin(retEsquerdo, XAlignmentEnum.Right, YAlignmentEnum.Bottom);
            bComp.ShowText("Nº\nSérie\nFolha");
            bComp.End();

            bComp.SafeBegin(retDireito, XAlignmentEnum.Left, YAlignmentEnum.Bottom);
            bComp.ShowText(String.Format("{0}\n{1}", Danfe.Model.NumeroNF.ToString(Formatador.FormatoNumeroNF), Danfe.Model.Serie));
            bComp.End();

        }

        private SizeF BestFitSize(SizeF actual, SizeF maxSize)
        {
            SizeF bestSize = SizeF.Empty;

            float aspectRatio = actual.Width / actual.Height;
            float aspectRatio2 = maxSize.Width / maxSize.Height;

            if (aspectRatio >= aspectRatio2)
            {
                bestSize.Width = maxSize.Width;
                bestSize.Height = bestSize.Width / aspectRatio;
            }
            else
            {
                bestSize.Height = maxSize.Height;
                bestSize.Width = bestSize.Height * aspectRatio;
            }

            return bestSize;
        }



        private void PrintLogo(PrimitiveComposer comp, RectangleF area)
        {
           
            area = area.GetPaddedRectangleMm(1.5F, 1.5F, 1.5F, 1F);

            SizeF bestsize = BestFitSize(Danfe._Logo.Size, area.Size);
            PointF point = PointF.Empty;
            point.X = area.X + Math.Abs(area.Width - bestsize.Width) / 2F;
            point.Y = area.Y + Math.Abs(area.Height - bestsize.Height) / 2F;

            comp.ShowXObject(Danfe._Logo, point, bestsize);
        }

        private void PrintIdentificacaoEmitente(RectangleF area, BlockComposer bComp, PrimitiveComposer comp)
        {
            comp.SafeDrawRectangle(area);

            // Retângulo com padding
            RectangleF pRet = area.GetPaddedRectangleMm(1);

            var emitente = Danfe.Model.Emitente;

            var yAlign = Danfe.PossuiLogo ? YAlignmentEnum.Bottom : YAlignmentEnum.Middle;

            bComp.SafeBegin(pRet, XAlignmentEnum.Left, yAlign);

            double bestSize = DanfeCampo.AjustarFonte(emitente.Nome, Danfe.FontBold, pRet.Width, Danfe.PossuiLogo ? 10 : 12);
            comp.SetFont(Danfe.FontBold, bestSize);

            bComp.ShowText(emitente.Nome);
            bComp.ShowBreak();
            comp.SetFont(Danfe.Font, Danfe.PossuiLogo ? 7 : 8);
            bComp.ShowText(emitente.EnderecoLinha1);
            bComp.ShowBreak();
            bComp.ShowText(emitente.EnderecoBairro);
            bComp.ShowBreak();
            bComp.ShowText(emitente.EnderecoLinha3);

            if (!String.IsNullOrWhiteSpace(emitente.Telefone))
            {
                bComp.ShowBreak();
                bComp.ShowText(String.Format("Fone: {0}", Formatador.FormatarTelefone(emitente.Telefone)));
            }

            bComp.End();

            if (Danfe.PossuiLogo)
            {
                RectangleF logoRectangle = area;
                logoRectangle.Height = bComp.BoundBox.Top - logoRectangle.Y;
                PrintLogo(comp, logoRectangle);
            }
        }


        protected override void ToXObjectInternal(PrimitiveComposer comp)
        {
            BlockComposer bComp = new BlockComposer(comp);

            PrintIdentificacaoEmitente(RetEmitente, bComp, comp);
            PrintDescricaoDanfe(RetDescDanfe, bComp, comp);

            var emitente = Danfe.Model.Emitente;

            comp.SafeDrawRectangle(RetCodigoBarras);
            comp.SafeDrawRectangle(RetCampoVariavel);

            comp.SetFont(Danfe.Font, 8);
            bComp.SafeBegin(RetCampoVariavel, XAlignmentEnum.Center, YAlignmentEnum.Middle);
            bComp.ShowText(MensagemConsulta);
            bComp.End();

            comp.Stroke();

            var barcode = new Barcode128C(Danfe.Model.ChaveAcesso, RetCodigoBarras.Size).ToXObject(Danfe.Document);
            comp.ShowXObject(barcode, new PointF(RetCodigoBarras.X, RetCodigoBarras.Y));
        }

    }
}
