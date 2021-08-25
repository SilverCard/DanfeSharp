using org.pdfclown.documents.contents.composition;
using System.Drawing;

namespace DanfeSharp
{
    public class BlocoCalculoImposto : BlocoDanfe
    {
        #region CamposLinhas1

        public DanfeCampo BaseCalculoIcms { get; private set; }
        public DanfeCampo ValorIcms { get; private set; }
        public DanfeCampo BaseCalculoIcmsSt { get; private set; }
        public DanfeCampo ValorCalculoIcmsSt { get; private set; }
        public DanfeCampo ValorPis { get; private set; }
        public DanfeCampo ValorProdutos { get; private set; }

        #endregion


        #region CamposLinhas2

        public DanfeCampo ValorFrete { get; private set; }
        public DanfeCampo ValorSeguro { get; private set; }
        public DanfeCampo Desconto { get; private set; }
        public DanfeCampo OutrasDespesas { get; private set; }
        public DanfeCampo ValorIpi { get; private set; }
        public DanfeCampo ValorCofins { get; private set; }
        public DanfeCampo ValorNota { get; private set; }

        #endregion


        public BlocoCalculoImposto(DanfeDocumento danfeMaker)
            : base(danfeMaker)
        {
            Size = new SizeF(Danfe.InnerRect.Width, danfeMaker.CabecalhoBlocoAltura + 2 * danfeMaker.CampoAltura + DanfeDocumento.LineWidth);
            Initialize();

        }

        protected override DanfeCampo CriarCampo(string cabecalho, string corpo, XAlignmentEnum corpoAlinhamentoX = XAlignmentEnum.Right)
        {
            return base.CriarCampo(cabecalho, corpo, corpoAlinhamentoX);
        }

        protected override void CriarCampos()
        {
            BaseCalculoIcms = CriarCampo("BASE DE CÁLC. ICMS", Danfe.Model.BaseCalculoIcms.Formatar());
            ValorIcms = CriarCampo("VALOR DO ICMS", Danfe.Model.ValorIcms.Formatar());
            BaseCalculoIcmsSt = CriarCampo("BASE DE CÁLC. ICMS S.T.", Danfe.Model.BaseCalculoIcmsSt.Formatar());
            ValorCalculoIcmsSt = CriarCampo("VALOR DO ICMS S.T.", Danfe.Model.ValorIcmsSt.Formatar());
            ValorPis = CriarCampo("VALOR DO PIS", Danfe.Model.ValorPis.Formatar());
            ValorProdutos = CriarCampo("VALOR TOTAL PRODUTOS", Danfe.Model.ValorTotalProdutos.Formatar());

            ValorFrete = CriarCampo("VALOR DO FRETE", Danfe.Model.ValorFrete.Formatar());
            ValorSeguro = CriarCampo("VALOR DO SEGURO", Danfe.Model.ValorSeguro.Formatar());
            Desconto = CriarCampo("DESCONTO", Danfe.Model.Desconto.Formatar());
            OutrasDespesas = CriarCampo("OUTRAS DESPESAS", Danfe.Model.OutrasDespesas.Formatar());
            ValorIpi = CriarCampo("VALOR DO IPI", Danfe.Model.ValorIpi.Formatar());
            ValorCofins = CriarCampo("VALOR DO COFINS", Danfe.Model.ValorCofins.Formatar());
            ValorNota = CriarCampo("VALOR TOTAL DA NOTA", Danfe.Model.ValorTotalNota.Formatar(), RectangleF.Empty, XAlignmentEnum.Right, 10, true);
        }

        protected override void PosicionarCampos()
        {
            RectangleF linha = new RectangleF(InternalRectangle.Left, InternalRectangle.Top, InternalRectangle.Width, Danfe.CampoAltura);
            PosicionarLadoLado(linha, BaseCalculoIcms, ValorIcms, BaseCalculoIcmsSt, ValorCalculoIcmsSt, ValorPis, ValorProdutos);

            linha.Y = linha.Bottom;
            PosicionarLadoLado(linha, new float[] { 0, 0, 0, 0, 0, 0, ValorProdutos.Retangulo.Width }, ValorFrete, ValorSeguro, Desconto, OutrasDespesas, ValorIpi, ValorCofins, ValorNota);
        }


        public override string Cabecalho
        {
            get
            {
                return "CÁLCULO DO IMPOSTO";
            }
        }
    }
}