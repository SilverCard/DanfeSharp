using DanfeSharp.Model;
using org.pdfclown.documents.contents.composition;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanfeSharp
{
    class BlocoDestinatario : BlocoDanfe
    {

        public DanfeCampo Nome { get; set; }
        public DanfeCampo CnpjCpf { get; set; }
        public DanfeCampo DataEmissao { get; set; }


        public DanfeCampo Endereco { get; set; }
        public DanfeCampo Bairro { get; set; }
        public DanfeCampo Cep { get; set; }
        public DanfeCampo DataEntradaSaida { get; set; }

        public DanfeCampo Municipio { get; set; }
        public DanfeCampo Fone { get; set; }
        public DanfeCampo Uf { get; set; }
        public DanfeCampo Ie { get; set; }
        public DanfeCampo Hora { get; set; }

        public BlocoDestinatario(DanfeDocumento danfeMaker) : base(danfeMaker)
        {
            Size = new System.Drawing.SizeF(Danfe.InnerRect.Width, Danfe.CabecalhoBlocoAltura + 3 * Danfe.CampoAltura + DanfeDocumento.LineWidth);
            Initialize();
        }


        protected override void CriarCampos()
        {
            var destinatario = Danfe.Model.Destinatario;
            Nome = CriarCampo("Nome / Razão Social", destinatario.Nome);
            CnpjCpf = CriarCampo(Strings.CnpjCpf, Formatador.FormatarCpfCnpj(destinatario.CnpjCpf), XAlignmentEnum.Center);
            DataEmissao = CriarCampo("Data de emissão", Danfe.Model.DataHoraEmissao.Formatar(), XAlignmentEnum.Center);

            Endereco = CriarCampo(Strings.Endereco, destinatario.EnderecoLinha1);
            Bairro = CriarCampo("Bairro", destinatario.EnderecoBairro);
            Cep = CriarCampo("CEP", Formatador.FormatarCEP(destinatario.EnderecoCep), XAlignmentEnum.Center);
            DataEntradaSaida = CriarCampo("Data entrada/saída", Danfe.Model.DataSaidaEntrada.Formatar() , XAlignmentEnum.Center );

            Municipio = CriarCampo(Strings.Municipio, destinatario.Municipio);
            Fone = CriarCampo("Fone/Fax", Formatador.FormatarTelefone(destinatario.Telefone), XAlignmentEnum.Center);
            Uf = CriarCampo(Strings.UF, destinatario.EnderecoUf , XAlignmentEnum.Center);
            Ie = CriarCampo(Strings.InscricaoEstadual, destinatario.Ie, XAlignmentEnum.Center);
            Hora = CriarCampo("Hora Entrada/Saída", Danfe.Model.HoraSaidaEntrada.Formatar(), XAlignmentEnum.Center);
        }

        protected override void PosicionarCampos()
        {
            RectangleF linha = InternalRectangle;

            linha.Height = Danfe.CampoAltura;
            PosicionarLadoLadoMm(linha, new float[] { 0, 40, 30 }, Nome, CnpjCpf, DataEmissao);

            linha.Y += Danfe.CampoAltura;
            PosicionarLadoLadoMm(linha, new float[] { 0, 50, 25, 30 }, Endereco, Bairro, Cep, DataEntradaSaida);

            linha.Y += Danfe.CampoAltura;
            PosicionarLadoLadoMm(linha, new float[] { 0, 35, 11, 40, 30 }, Municipio, Fone, Uf, Ie, Hora);
        }

        public override string Cabecalho
        {
            get
            {
                return "Destinatário/Remetente";
            }
        }
    }
}
