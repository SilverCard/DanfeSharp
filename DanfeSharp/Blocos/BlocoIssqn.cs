﻿using System.Drawing;

namespace DanfeSharp
{
    public class BlocoIssqn : BlocoDanfe
    {
        public DanfeCampo InscricaoMunicipal { get; set; }
        public DanfeCampo ValorTotalServicos { get; set; }
        public DanfeCampo BaseCalculoIssqn { get; set; }
        public DanfeCampo ValorIssqn { get; set; }

        public BlocoIssqn(DanfeDocumento danfeMaker)
            : base(danfeMaker)
        {
            Size = new SizeF(Danfe.InnerRect.Width, danfeMaker.CabecalhoBlocoAltura +  danfeMaker.CampoAltura +  DanfeDocumento.LineWidth);
            Initialize();
        }

        protected override void PosicionarCampos()
        {
            PosicionarLadoLado(InternalRectangle, InscricaoMunicipal, ValorTotalServicos, BaseCalculoIssqn, ValorIssqn);
        }

        protected override void CriarCampos()
        {
            InscricaoMunicipal = CriarCampo("Inscrição Municipal", Danfe.Model.Emitente.IM, org.pdfclown.documents.contents.composition.XAlignmentEnum.Center);
            ValorTotalServicos = CriarCampo("Valor Total dos Serviços", Danfe.Model.ValorTotalServicos.Formatar(), org.pdfclown.documents.contents.composition.XAlignmentEnum.Right);
            BaseCalculoIssqn = CriarCampo("Base de Cálculo do ISSQN", Danfe.Model.BaseIssqn.Formatar(), org.pdfclown.documents.contents.composition.XAlignmentEnum.Right);
            ValorIssqn = CriarCampo("Valor do ISSQN", Danfe.Model.ValorIssqn.Formatar(), org.pdfclown.documents.contents.composition.XAlignmentEnum.Right);
        }

        public override string Cabecalho
        {
            get
            {
                return "Cálculo do ISSQN";
            }
        }
    }
}