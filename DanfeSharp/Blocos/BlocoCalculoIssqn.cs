namespace DanfeSharp.Blocos
{
    internal class BlocoCalculoIssqn : BlocoBase
    {
        public BlocoCalculoIssqn(DanfeViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var m = viewModel.CalculoIssqn;

            AdicionarLinhaCampos()
                .ComCampo("INSCRIÇÃO MUNICIPAL", m.InscricaoMunicipal, AlinhamentoHorizontal.Centro)
                .ComCampo("VALOR TOTAL DOS SERVIÇOS", m.ValorTotalServicos.Formatar(), AlinhamentoHorizontal.Direita)
                .ComCampo("BASE DE CÁLCULO DO ISSQN", m.BaseIssqn.Formatar(), AlinhamentoHorizontal.Direita)
                .ComCampo("VALOR TOTAL DO ISSQN", m.ValorIssqn.Formatar(), AlinhamentoHorizontal.Direita)
                .ComLargurasIguais();
        }

        public override PosicaoBloco Posicao => PosicaoBloco.Base;
        public override string Cabecalho => "CÁLCULO DO ISSQN";
    }
}
