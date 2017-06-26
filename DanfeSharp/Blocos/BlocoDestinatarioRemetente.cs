using DanfeSharp.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanfeSharp.Blocos
{
    internal class BlocoDestinatarioRemetente : BlocoBase
    {
        public BlocoDestinatarioRemetente(DanfeViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var destinatario = viewModel.Destinatario;

            AdicionarLinhaCampos()
            .ComCampo(Strings.RazaoSocial, destinatario.Nome)
            .ComCampo(Strings.CnpjCpf, Formatador.FormatarCpfCnpj(destinatario.CnpjCpf), AlinhamentoHorizontal.Centro)
            .ComCampo("Data de Emissão", viewModel.DataHoraEmissao.Formatar(), AlinhamentoHorizontal.Centro)
            .ComLarguras(0, 45F/200F * 100, 30F/200F * 100);

            AdicionarLinhaCampos()
            .ComCampo(Strings.Endereco, destinatario.EnderecoLinha1)
            .ComCampo("Bairro / Distrito", destinatario.EnderecoBairro)
            .ComCampo("Cep", Formatador.FormatarCEP(destinatario.EnderecoCep), AlinhamentoHorizontal.Centro)
            .ComCampo("Data Entrada / Saída", ViewModel.DataSaidaEntrada.Formatar(), AlinhamentoHorizontal.Centro)
            .ComLarguras(0, 45F/200 * 100F, 25F / 200F * 100, 30F / 200F * 100);

            AdicionarLinhaCampos()
            .ComCampo(Strings.Municipio, destinatario.Municipio)
            .ComCampo("Fone / Fax", Formatador.FormatarTelefone(destinatario.Telefone), AlinhamentoHorizontal.Centro)
            .ComCampo(Strings.UF, destinatario.EnderecoUf, AlinhamentoHorizontal.Centro)
            .ComCampo(Strings.InscricaoEstadual, destinatario.Ie, AlinhamentoHorizontal.Centro)
            .ComCampo("Hora Entrada / Saída", ViewModel.HoraSaidaEntrada.Formatar(), AlinhamentoHorizontal.Centro)
            .ComLarguras(0, 35F / 200F * 100, 7F / 200F * 100, 40F / 200F * 100, 30F / 200F * 100);
        }

        public override string Cabecalho => "Destinatário / Remetente";
        public override PosicaoBloco Posicao => PosicaoBloco.Topo;
    }
}
