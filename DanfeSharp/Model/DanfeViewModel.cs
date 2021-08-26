using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DanfeSharp.Model
{
    public class DanfeViewModel
    {
        /// <summary>
        /// <para>Número do Documento Fiscal</para>
        /// <para>Tag nNF</para>
        /// </summary>
        public int NumeroNF { get; set; }

        /// <summary>
        /// <para>Série do Documento Fiscal</para>
        /// <para>Tag serie</para>
        /// </summary>
        public int Serie { get; set; }

        /// <summary>
        /// Chave de Acesso
        /// </summary>
        public string ChaveAcesso { get; set; }                    

        /// <summary>
        /// <para>Descrição da Natureza da Operação</para>
        /// <para>Tag natOp</para>
        /// </summary>
        public string NaturezaOperacao { get; set; }

        /// <summary>
        /// <para>Informações Complementares de interesse do Contribuinte</para>
        /// <para>Tag infCpl</para>
        /// </summary>
        public string InformacoesComplementares { get; set; }

        /// <summary>
        /// <para>Informações adicionais de interesse do Fisco</para>
        /// <para>Tag infAdFisco</para>
        /// </summary>
        public string InformacoesAdicionaisFisco { get; set; }

        /// <summary>
        /// <para>Data e Hora de emissão do Documento Fiscal</para>
        /// <para>Tag dhEmi ou dEmi</para>
        /// </summary>
        public DateTime? DataHoraEmissao { get; set; }

        /// <summary>
        /// <para>Data de Saída ou da Entrada da Mercadoria/Produto</para>
        /// <para>Tag dSaiEnt e dhSaiEnt</para>
        /// </summary>
        public DateTime? DataSaidaEntrada { get; set; }

        /// <summary>
        /// <para>Hora de Saída ou da Entrada da Mercadoria/Produto</para>
        /// <para>Tag dSaiEnt e hSaiEnt</para>
        /// </summary>
        public TimeSpan? HoraSaidaEntrada { get; set; }

        /// <summary>
        /// Dados do Emitente
        /// </summary>
        public EmpresaViewModel Emitente { get; set; }

        /// <summary>
        /// Dados do Destinatário
        /// </summary>
        public EmpresaViewModel Destinatario { get; set; }
        
        /// <summary>
        /// <para>Tipo de Operação - 0-entrada / 1-saída</para>
        /// <para>Tag tpNF</para>
        /// </summary>
        public int TipoNF { get; set; }

        /// <summary>
        /// Numero do protocolo com sua data e hora
        /// </summary>
        public string ProtocoloAutorizacao { get; set; }

        /// <summary>
        /// Faturas da Nota Fiscal
        /// </summary>
        public List<DuplicataViewModel> Duplicatas { get; set; }

        /// <summary>
        /// <para>Base de Cálculo do ICMS</para>
        /// <para>Tag vBC</para>
        /// </summary>
        public double BaseCalculoIcms { get; set; }
        
        /// <summary>
        /// <para>Valor Total do ICMS</para>
        /// <para>Tag vICMS</para>
        /// </summary>
        public double ValorIcms { get; set; }

        /// <summary>
        /// <para>Base de Cálculo do ICMS ST</para>
        /// <para>Tag vBCST</para>
        /// </summary>
        public double BaseCalculoIcmsSt { get; set; }

        /// <summary>
        /// <para>Valor Total do ICMS ST</para>
        /// <para>Tag vST</para>
        /// </summary>
        public double ValorIcmsSt { get; set; }

        /// <summary>
        /// <para>Valor Total do PIS</para>
        /// <para>Tag vPIS</para>
        /// </summary>
        public double ValorPis { get; set; }

        /// <summary>
        /// <para>Valor Total dos produtos e serviços</para>
        /// <para>Tag vProd</para>
        /// </summary>
        public double ValorTotalProdutos { get; set; }

        /// <summary>
        /// <para>Valor Total do Frete</para>
        /// <para>Tag vFrete</para>
        /// </summary>
        public double ValorFrete { get; set; }

        /// <summary>
        /// <para>Valor Total do Seguro</para>
        /// <para>Tag vSeg</para>
        /// </summary>
        public double ValorSeguro { get; set; }

        /// <summary>
        /// <para>Valor Total do Desconto </para>
        /// <para>Tag vDesc</para>
        /// </summary>
        public double Desconto { get; set; }

        /// <summary>
        /// <para>Outras Despesas acessórias</para>
        /// <para>Tag vOutro</para>
        /// </summary>
        public double OutrasDespesas { get; set; }

        /// <summary>
        /// <para>Valor Total do IPI</para>
        /// <para>Tag vIPI</para>
        /// </summary>
        public double ValorIpi { get; set; }

        /// <summary>
        /// <para>Valor Total do COFINS</para>
        /// <para>Tag vCOFINS</para>
        /// </summary>
        public double ValorCofins { get; set; }

        /// <summary>
        /// <para>Valor Total da NF-e </para>
        /// <para>Tag vNF</para>
        /// </summary>
        public double ValorTotalNota { get; set; }

        /// <summary>
        /// Tipo de Ambiente
        /// </summary>
        public int TipoAmbiente { get; set; }

        /// <summary>
        /// <para>Valor aproximado total de tributos federais, estaduais e municipais (NT 2013.003)</para>
        /// <para>Tag vTotTrib</para>
        /// </summary>
        public double? ValorAproximadoTributos { get; set; }

        /// <summary>
        /// Dados da Transportadora
        /// </summary>
        public TransportadoraViewModel Transportadora { get; set; }

        /// <summary>
        /// Produtos da Nota Fiscal
        /// </summary>
        public List<ProdutoViewModel> Produtos { get; set; }

        #region BlocoCalculoIssqn

        /// <summary>
        /// <para>Valor Total dos Serviços sob não-incidência ou não tributados pelo ICMS</para>
        /// <para>Tag vServ</para>
        /// </summary> 
        public double? ValorTotalServicos { get; set; }

        /// <summary>
        /// <para>Base de Cálculo do ISS</para>
        /// <para>Tag vBC</para>
        /// </summary>
        public double? BaseIssqn { get; set; }

        /// <summary>
        /// <para>Valor Total do ISS</para>
        /// <para>Tag vISS</para>
        /// </summary>
        public double? ValorIssqn { get; set; }
        #endregion

        public DanfeViewModel ()
	    {
            Emitente = new EmpresaViewModel();
            Destinatario = new EmpresaViewModel();
            Duplicatas = new List<DuplicataViewModel>();
            Produtos = new List<ProdutoViewModel>();
            Transportadora = new TransportadoraViewModel();            
	    }
        
        public bool MostrarCalculoIssqn
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Emitente.IM) ||
                    ValorTotalServicos.HasValue ||
                    BaseIssqn.HasValue ||
                    ValorIssqn.HasValue;
            }
        }
                
        /// <summary>
        /// Substitui o ponto e vírgula (;) por uma quebra de linha.
        /// </summary>
        private string BreakLines(string str)
        {
            return str == null ? string.Empty : str.Replace(';', '\n');
        }
        
        public string InformacoesComplementaresCompleta
        {
            get
            {
                StringBuilder sb = new StringBuilder("");

                if(!string.IsNullOrWhiteSpace(InformacoesComplementares))
                {
                    sb.Append("Inf. Contribuinte: ");
                    sb.AppendLine(BreakLines(InformacoesComplementares));
                }

                if (!string.IsNullOrWhiteSpace(InformacoesAdicionaisFisco))
                {
                    sb.Append("Inf. Fisco: ");
                    sb.AppendLine(BreakLines(InformacoesAdicionaisFisco));
                }

                // Verifica se o valor aproximado dos tributos já consta nas informações complementares
                string rgValorAproximadoTributos = @"(vlr|val(or)?)(\s*)(aproximado|aprox(\.?))(\s*)(de|dos)?(\s*)(tributos)";

                bool isValorImpresso = string.IsNullOrWhiteSpace(InformacoesComplementares) ? false : Regex.IsMatch(InformacoesComplementares, rgValorAproximadoTributos, RegexOptions.IgnoreCase);

                if (ValorAproximadoTributos.HasValue && !isValorImpresso)
                {
                    sb.AppendLine("Valor Aproximado dos Tributos: " + ValorAproximadoTributos.Formatar());
                }

                if (!string.IsNullOrWhiteSpace(Destinatario.Email))
                {
                    sb.Append("Email destinatário: ");
                    sb.AppendLine(Destinatario.Email);
                }          

                return sb.ToString().TrimEnd();
            }
        }

        public static DanfeViewModel CreateFromXmlFile(string path)
        {
            return DanfeViewModelCreator.CreateFromXmlFile(path);
        }

        public static DanfeViewModel CreateFromXmlString(string xml)
        {
            return DanfeViewModelCreator.CreateFromXmlString(xml);
        }
    }
}