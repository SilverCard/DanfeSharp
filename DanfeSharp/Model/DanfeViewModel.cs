using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        public String ChaveAcesso { get; set; }
                    

        /// <summary>
        /// <para>Descrição da Natureza da Operação</para>
        /// <para>Tag natOp</para>
        /// </summary>
        public String NaturezaOperacao { get; set; }

        /// <summary>
        /// <para>Informações Complementares de interesse do Contribuinte</para>
        /// <para>Tag infCpl</para>
        /// </summary>
        public String InformacoesComplementares { get; set; }

        /// <summary>
        /// <para>Informações adicionais de interesse do Fisco</para>
        /// <para>Tag infAdFisco</para>
        /// </summary>
        public String InformacoesAdicionaisFisco { get; set; }

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
        public String ProtocoloAutorizacao { get; set; }

        /// <summary>
        /// Faturas da Nota Fiscal
        /// </summary>
        public List<DuplicataViewModel> Duplicatas { get; set; }

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
        public Double ValorTotalProdutos { get; set; }

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
        /// <para>Valor Total do IPI</para>
        /// <para>Tag vIPI</para>
        /// </summary>
        public Double ValorIpi { get; set; }
        
        /// <summary>
        /// <para>Valor Total da NF-e </para>
        /// <para>Tag vNF</para>
        /// </summary>
        public Double ValorTotalNota { get; set; }

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

        /// <summary>
        /// Caminho do Logotipo para impressão no DANFE
        /// </summary>
        public String LogoPath { get; set; }

        #region BlocoCalculoIssqn

        /// <summary>
        /// <para>Valor Total dos Serviços sob não-incidência ou não tributados pelo ICMS</para>
        /// <para>Tag vServ</para>
        /// </summary> 
        public Double? ValorTotalServicos { get; set; }


        /// <summary>
        /// <para>Base de Cálculo do ISS</para>
        /// <para>Tag vBC</para>
        /// </summary>
        public Double? BaseIssqn { get; set; }

        /// <summary>
        /// <para>Valor Total do ISS</para>
        /// <para>Tag vISS</para>
        /// </summary>
        public Double? ValorIssqn { get; set; }
        #endregion

        public DanfeViewModel ()
	    {
            Emitente = new EmpresaViewModel();
            Destinatario = new EmpresaViewModel();
            Duplicatas = new List<DuplicataViewModel>();
            Produtos = new List<ProdutoViewModel>();
            Transportadora = new TransportadoraViewModel();            
	    }

        
        public Boolean MostrarCalculoIssqn
        {
            get
            {
                return !String.IsNullOrWhiteSpace(Emitente.IM) ||
                    ValorTotalServicos.HasValue ||
                    BaseIssqn.HasValue ||
                    ValorIssqn.HasValue;
            }
        }

        public Boolean PossuiLogo
        {
            get
            {
                return !String.IsNullOrWhiteSpace(LogoPath);
            }
        }
        
        /// <summary>
        /// Substitui o ponto e vírgula (;) por uma quebra de linha.
        /// </summary>
        private String BreakLines(String str)
        {
            return str == null ? String.Empty : str.Replace(';', '\n');
        }
        
        public String InformacoesComplementaresCompleta
        {
            get
            {
                StringBuilder sb = new StringBuilder("");

                if(!String.IsNullOrWhiteSpace(InformacoesComplementares))
                {
                    sb.Append("Inf. Contribuinte: ");
                    sb.AppendLine(BreakLines(InformacoesComplementares));
                }

                if (!String.IsNullOrWhiteSpace(InformacoesAdicionaisFisco))
                {
                    sb.Append("Inf. Fisco: ");
                    sb.AppendLine(BreakLines(InformacoesAdicionaisFisco));
                }

                // Verifica se o valor aproximado dos tributos já consta nas informações complementares
                String rgValorAproximadoTributos = @"(vlr|val(or)?)(\s*)(aproximado|aprox(\.?))(\s*)(de|dos)?(\s*)(tributos)";

                Boolean isValorImpresso = String.IsNullOrWhiteSpace(InformacoesComplementares) ? false : Regex.IsMatch(InformacoesComplementares, rgValorAproximadoTributos, RegexOptions.IgnoreCase);

                if (ValorAproximadoTributos.HasValue && !isValorImpresso)
                {
                    sb.AppendLine("Valor Aproximado dos Tributos: " + ValorAproximadoTributos.Formatar());
                }

                if (!String.IsNullOrWhiteSpace(Destinatario.Email))
                {
                    sb.Append("Email destinatário: ");
                    sb.AppendLine(Destinatario.Email);
                }          

                return sb.ToString().TrimEnd();
            }
        }

        public static DanfeViewModel CreateFromXmlFile(String path)
        {
            return DanfeViewModelCreator.CreateFromXmlFile(path);
        }

        public static DanfeViewModel CreateFromXmlString(String xml)
        {
            return DanfeViewModelCreator.CreateFromXmlString(xml);
        }

    }
}
