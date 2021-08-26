using System;
using System.Drawing;

namespace DanfeSharp
{
    class BlocoDadosAdicionais : BlocoDanfe
    {
        public static readonly float AlturaMinima = Utils.Mm2Pu(20);

        public DanfeCampo ReservadoFisco { get; set; }
        public DanfeCampo InformacoesComplementares { get; set; }
        public const double TamanhoFonteCorpo = 6;

        public BlocoDadosAdicionais(DanfeDocumento danfe)
            : base(danfe)
        {
            float altura = CalcularAltura();
            Size = new SizeF(danfe.InnerRect.Width, Danfe.CabecalhoBlocoAltura + DanfeDocumento.LineWidth + altura);
            Initialize();
        }

        private float CalcularAltura()
        {
            double larguraTexto = Danfe.InnerRect.Width - Utils.Mm2Pu(78F);
            double alturaTexto = Utils.CountTextLines(Danfe.Font, TamanhoFonteCorpo, larguraTexto, Danfe.Model.InformacoesComplementaresCompleta) * (Danfe.Font.GetLineHeight(TamanhoFonteCorpo) + DanfeCampo.LineSpace);
            alturaTexto += Danfe.FontBold.GetLineHeight(DanfeCampo.TamanhoFonteCabecalho) + DanfeCampo.LineSpace;
            alturaTexto += DanfeCampo.PaddingSuperior + DanfeCampo.PaddingInferior;
            return (float)Math.Max(alturaTexto, AlturaMinima);
        }
        
        protected override void CriarCampos()
        {

            ReservadoFisco = CriarCampo("Reservado ao Fisco", null);
            InformacoesComplementares = CriarCampo("Informações Complementares", Danfe.Model.InformacoesComplementaresCompleta, RectangleF.Empty, org.pdfclown.documents.contents.composition.XAlignmentEnum.Left, TamanhoFonteCorpo, false, org.pdfclown.documents.contents.composition.YAlignmentEnum.Top);
            InformacoesComplementares.MultiLinha = true;
        }

        protected override void PosicionarCampos()
        {
            PosicionarLadoLadoMm(InternalRectangle, new float[] { 0, 76.2F }, InformacoesComplementares, ReservadoFisco);
        }

        public override string Cabecalho
        {
            get
            {
                return "Dados Adicionais";
            }
        }
    }
}
