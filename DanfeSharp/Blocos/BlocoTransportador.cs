using System.Drawing;

namespace DanfeSharp
{
    public class BlocoTransportador : BlocoDanfe
    {
        #region CamposLinhas1

        public DanfeCampo RazaoSocial { get; private set; }
        public DanfeCampo FreteConta { get; private set; }
        public DanfeCampo CodigoAntt { get; private set; }
        public DanfeCampo PlacaVeiculo { get; private set; }
        public DanfeCampo UfVeiculo { get; private set; }
        public DanfeCampo CnpjCpf { get; private set; }

        #endregion

        #region CamposLinhas2

        public DanfeCampo Endereco { get; private set; }
        public DanfeCampo Municipio { get; private set; }
        public DanfeCampo EnderecoUf { get; private set; }
        public DanfeCampo IE { get; private set; }

        #endregion

        #region CamposLinhas3

        public DanfeCampo Quantidade { get; private set; }
        public DanfeCampo Especie { get; private set; }
        public DanfeCampo Marca { get; private set; }
        public DanfeCampo Numeracao { get; private set; }
        public DanfeCampo PesoBruto { get; private set; }
        public DanfeCampo PesoLiquido { get; private set; }

        #endregion

        public BlocoTransportador(DanfeDocumento danfeMaker)
            : base(danfeMaker)
        {
            Size = new SizeF(Danfe.InnerRect.Width, danfeMaker.CabecalhoBlocoAltura + 3 * danfeMaker.CampoAltura + DanfeDocumento.LineWidth);
            Initialize();
        }

        protected override void CriarCampos()
        {
            var transportadora = Danfe.Model.Transportadora;

            //Campos Linha 1
            RazaoSocial = CriarCampo(Strings.RazaoSocial, transportadora.Nome);
            FreteConta = CriarCampo("Frete por Conta", transportadora.ModalidadeFreteString, org.pdfclown.documents.contents.composition.XAlignmentEnum.Center);
            CodigoAntt = CriarCampo("Código ANTT", transportadora.CodigoAntt, org.pdfclown.documents.contents.composition.XAlignmentEnum.Center);
            PlacaVeiculo = CriarCampo("Placa do Veículo", Formatador.FormatarPlacaVeiculo(transportadora.Placa), org.pdfclown.documents.contents.composition.XAlignmentEnum.Center);
            UfVeiculo = CriarCampo(Strings.UF, transportadora.VeiculoUf, org.pdfclown.documents.contents.composition.XAlignmentEnum.Center);
            CnpjCpf = CriarCampo(Strings.CnpjCpf, Formatador.FormatarCnpj(transportadora.CnpjCpf), org.pdfclown.documents.contents.composition.XAlignmentEnum.Center);

            //Campos Linha 2
            Endereco = CriarCampo(Strings.Endereco, transportadora.EnderecoLogadrouro);
            Municipio = CriarCampo(Strings.Municipio, transportadora.Municipio);
            EnderecoUf = CriarCampo(Strings.UF, transportadora.EnderecoUf, org.pdfclown.documents.contents.composition.XAlignmentEnum.Center);
            IE = CriarCampo(Strings.InscricaoEstadual, transportadora.Ie, org.pdfclown.documents.contents.composition.XAlignmentEnum.Center);

            //Campos Linha 3
            Quantidade = CriarCampo(Strings.Quantidade, transportadora.QuantidadeVolumes.Formatar(Formatador.FormatoNumero), org.pdfclown.documents.contents.composition.XAlignmentEnum.Right);
            Especie = CriarCampo("Espécie", transportadora.Especie);
            Marca = CriarCampo("Marca", transportadora.Marca);
            Numeracao = CriarCampo("Numeração", transportadora.Numeracao);
            PesoBruto = CriarCampo("Peso Bruto", transportadora.PesoBruto.Formatar(), org.pdfclown.documents.contents.composition.XAlignmentEnum.Right);
            PesoLiquido = CriarCampo("Peso Líquido", transportadora.PesoLiquido.Formatar(), org.pdfclown.documents.contents.composition.XAlignmentEnum.Right);
        }

        protected override void PosicionarCampos()
        {
            RectangleF linha = new RectangleF(InternalRectangle.Left, InternalRectangle.Top, InternalRectangle.Width, Danfe.CampoAltura);
            PosicionarLadoLadoMm(linha, new float[] { 0, 25, 30, 24, 10, 35 }, RazaoSocial, FreteConta, CodigoAntt, PlacaVeiculo, UfVeiculo, CnpjCpf);

            linha.Y = linha.Bottom;
            PosicionarLadoLado(linha, new float[] { 0, CodigoAntt.Retangulo.Width + PlacaVeiculo.Retangulo.Width, UfVeiculo.Retangulo.Width, CnpjCpf.Retangulo.Width }, Endereco, Municipio, EnderecoUf, IE);

            linha.Y = linha.Bottom;
            float l = (Endereco.Retangulo.Width - Utils.Mm2Pu(20)) / 2F;

            PosicionarLadoLado(linha, new float[] { Utils.Mm2Pu(20), l, l, 0, Utils.Mm2Pu(35), Utils.Mm2Pu(35) }, Quantidade, Especie, Marca, Numeracao, PesoBruto, PesoLiquido);

        }

        public override string Cabecalho
        {
            get
            {
                return "TRANSPORTADOR / VOLUMES TRANSPORTADOS";
            }
        }
    }
}