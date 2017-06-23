namespace DanfeSharp.Blocos
{
    class BlocoCalculoImposto : BlocoBase
    {
        public BlocoCalculoImposto(DanfeViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var a = AlinhamentoHorizontal.Direita;
            var m = ViewModel.CalculoImposto;

            AdicionarLinhaCampos()
            .ComCampo("BASE DE CÁLC. DO ICMS", m.BaseCalculoIcms.Formatar(), a)
            .ComCampo("VALOR DO ICMS", m.ValorIcms.Formatar(), a)
            .ComCampo("BASE DE CÁLC. ICMS S.T.", m.BaseCalculoIcmsSt.Formatar(), a)
            .ComCampo("VALOR DO ICMS SUBST.", m.ValorIcmsSt.Formatar(), a)
            .ComCampo("V. IMP. IMPORTAÇÃO", m.ValorII.Formatar(), a)
            .ComCampo("V. ICMS UF REMET.", m.vICMSUFRemet.Formatar())
            .ComCampo("VALOR DO FCP", m.vFCP.Formatar())
            .ComCampo("VALOR DO PIS", m.ValorPis.Formatar(), a)
            .ComCampo("V. TOTAL PRODUTOS", m.ValorTotalProdutos.Formatar(), a)
            .ComLargurasIguais();

            AdicionarLinhaCampos()
            .ComCampo("Valor do Frete", m.ValorFrete.Formatar(), a)
            .ComCampo("Valor do Seguro", m.ValorSeguro.Formatar(), a)
            .ComCampo("Desconto", m.Desconto.Formatar(), a)
            .ComCampo("Outras Despesas", m.OutrasDespesas.Formatar(), a)
            .ComCampo("Valor Ipi", m.ValorIpi.Formatar(), a)
            .ComCampo("V. ICMS UF DEST.", m.vICMSUFDest.Formatar())
            .ComCampo("V. TOT. TRIB.", m.ValorAproximadoTributos.Formatar())
            .ComCampo("VALOR DO COFINS", m.ValorCofins.Formatar(), a)
            .ComCampo("Valor Total da Nota", m.ValorTotalNota.Formatar(), a)
            .ComLargurasIguais();
        }

        public override PosicaoBloco Posicao => PosicaoBloco.Topo;
        public override string Cabecalho => "Cálculo do Imposto";
    }
}
