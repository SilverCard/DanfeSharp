namespace DanfeSharp.Blocos
{
    internal class BlocoTransportador : BlocoBase
    {
        public const float LarguraCampoPlacaVeiculo = 24F/200F * 100;
        public const float LarguraCampoCodigoAntt = 30F / 200F * 100;
        public const float LarguraCampoCnpj = 35F / 200F * 100;
        public const float LarguraCampoUf = 10F / 200F * 100;

        public BlocoTransportador(DanfeViewModel viewModel, Estilo campoEstilo) : base(viewModel, campoEstilo)
        {
            var transportadora = viewModel.Transportadora;

            AdicionarLinhaCampos()
                .ComCampo(Strings.RazaoSocial, transportadora.Nome)
                .ComCampo("Frete por conta", transportadora.ModalidadeFreteString, AlinhamentoHorizontal.Centro)
                .ComCampo("Código ANTT", transportadora.CodigoAntt, AlinhamentoHorizontal.Centro)
                .ComCampo("Placa do Veículo", Formatador.FormatarPlacaVeiculo(transportadora.Placa), AlinhamentoHorizontal.Centro)
                .ComCampo(Strings.UF, transportadora.VeiculoUf, AlinhamentoHorizontal.Centro)
                .ComCampo(Strings.CnpjCpf, Formatador.FormatarCnpj(transportadora.CnpjCpf), AlinhamentoHorizontal.Centro)
                .ComLarguras(0, 25F / 200F * 100, LarguraCampoCodigoAntt, LarguraCampoPlacaVeiculo, LarguraCampoUf, LarguraCampoCnpj);

            AdicionarLinhaCampos()
                .ComCampo(Strings.Endereco, transportadora.EnderecoLogadrouro)
                .ComCampo(Strings.Municipio, transportadora.Municipio)
                .ComCampo(Strings.UF, transportadora.EnderecoUf, AlinhamentoHorizontal.Centro)
                .ComCampo(Strings.InscricaoEstadual, transportadora.Ie, AlinhamentoHorizontal.Centro)
                .ComLarguras(0, LarguraCampoPlacaVeiculo + LarguraCampoCodigoAntt, LarguraCampoUf, LarguraCampoCnpj);

            var l = (float)(LarguraCampoCodigoAntt + LarguraCampoPlacaVeiculo + LarguraCampoUf + LarguraCampoCnpj) / 3F;

            AdicionarLinhaCampos()
                .ComCampo(Strings.Quantidade, transportadora.QuantidadeVolumes.Formatar(Formatador.FormatoNumero), AlinhamentoHorizontal.Direita)
                .ComCampo("Espécie", transportadora.Especie)
                .ComCampo("Marca", transportadora.Marca)
                .ComCampo("Numeração", transportadora.Numeracao)
                .ComCampo("Peso Bruto", transportadora.PesoBruto.Formatar("N3"), AlinhamentoHorizontal.Direita)
                .ComCampo("Peso Líquido", transportadora.PesoLiquido.Formatar("N3"), AlinhamentoHorizontal.Direita)
                .ComLarguras(20F / 200F * 100, 0, 0, l, l, l);

        }

        public override PosicaoBloco Posicao => PosicaoBloco.Topo;
        public override string Cabecalho => "Transportador / Volumes Transportados";
    }
}
