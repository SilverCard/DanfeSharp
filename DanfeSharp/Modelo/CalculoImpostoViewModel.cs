using System;

namespace DanfeSharp.Modelo
{
    public class CalculoImpostoViewModel
    {
        /// <summary>
        /// <para>Base de Cálculo do ICMS</para>
        /// <para>Tag vBC</para>
        /// </summary>
        public Double BaseCalculoIcms { get; set; }

        /// <summary>
        /// <para>Valor Total do ICMS</para>
        /// <para>Tag vICMS</para>
        /// </summary>
        public Double ValorIcms { get; set; }

        /// <remarks/>
        public double? vICMSUFDest;

        /// <remarks/>
        public double? vICMSUFRemet;

        /// <remarks/>
        public double? vFCP;

        /// <summary>
        /// <para>Base de Cálculo do ICMS ST</para>
        /// <para>Tag vBCST</para>
        /// </summary>
        public Double BaseCalculoIcmsSt { get; set; }

        /// <summary>
        /// <para>Valor Total do ICMS ST</para>
        /// <para>Tag vST</para>
        /// </summary>
        public Double ValorIcmsSt { get; set; }

        /// <summary>
        /// <para>Valor Total dos produtos e serviços</para>
        /// <para>Tag vProd</para>
        /// </summary>
        public Double? ValorTotalProdutos { get; set; }

        /// <summary>
        /// <para>Valor Total do Frete</para>
        /// <para>Tag vFrete</para>
        /// </summary>
        public Double ValorFrete { get; set; }

        /// <summary>
        /// <para>Valor Total do Seguro</para>
        /// <para>Tag vSeg</para>
        /// </summary>
        public Double ValorSeguro { get; set; }

        /// <summary>
        /// <para>Valor Total do Desconto </para>
        /// <para>Tag vDesc</para>
        /// </summary>
        public Double Desconto { get; set; }

        /// <summary>
        /// <para>Outras Despesas acessórias</para>
        /// <para>Tag vOutro</para>
        /// </summary>
        public Double OutrasDespesas { get; set; }

        /// <summary>
        /// Valor do imposto de importação.
        /// </summary>
        public double ValorII { get; set; }

        /// <summary>
        /// <para>Valor Total do IPI</para>
        /// <para>Tag vIPI</para>
        /// </summary>
        public Double ValorIpi { get; set; }

        /// <summary>
        /// Valor do PIS
        /// </summary>
        public Double ValorPis { get; set; }

        /// <summary>
        /// Valor do COFINS
        /// </summary>
        public Double ValorCofins { get; set; }

        /// <summary>
        /// <para>Valor Total da NF-e </para>
        /// <para>Tag vNF</para>
        /// </summary>
        public Double ValorTotalNota { get; set; }


        /// <summary>
        /// <para>Valor aproximado total de tributos federais, estaduais e municipais (NT 2013.003)</para>
        /// <para>Tag vTotTrib</para>
        /// </summary>
        public double? ValorAproximadoTributos { get; set; }
    }
}
