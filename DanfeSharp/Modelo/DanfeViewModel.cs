using DanfeSharp.Modelo;
using System;
using System.Collections.Generic;
using System.Text;

namespace DanfeSharp
{
    public class DanfeViewModel
    {
        /// <summary>
        /// <para>Número do Documento Fiscal</para>
        /// <para>Tag nNF</para>
        /// </summary>
        public int NfNumero { get; set; }

        /// <summary>
        /// <para>Série do Documento Fiscal</para>
        /// <para>Tag serie</para>
        /// </summary>
        public int NfSerie { get; set; }

        public Orientacao Orientacao { get; set; }

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
        /// Dados da Transportadora
        /// </summary>
        public TransportadoraViewModel Transportadora { get; set; }

        public CalculoImpostoViewModel CalculoImposto { get; set; }

        /// <summary>
        /// Produtos da Nota Fiscal
        /// </summary>
        public List<ProdutoViewModel> Produtos { get; set; }

        /// <summary>
        /// Tipo de Ambiente
        /// </summary>
        public int TipoAmbiente { get; set; }

        /// <summary>
        /// Informação adicional de compra formatada, Nota de Empenho, Pedido e Contrato
        /// </summary>
        public String InformacaoCompra { get; set; }

        public CalculoIssqnViewModel CalculoIssqn { get; set; }

        public DanfeViewModel ()
	    {
            Orientacao = Orientacao.Paisagem;
            CalculoImposto = new CalculoImpostoViewModel();
            Emitente = new EmpresaViewModel();
            Destinatario = new EmpresaViewModel();
            Duplicatas = new List<DuplicataViewModel>();
            Produtos = new List<ProdutoViewModel>();
            Transportadora = new TransportadoraViewModel();
            CalculoIssqn = new CalculoIssqnViewModel();
        }

        
        public Boolean MostrarCalculoIssqn { get; set; }
    
                
        /// <summary>
        /// Substitui o ponto e vírgula (;) por uma quebra de linha.
        /// </summary>
        private String BreakLines(String str)
        {
            return str == null ? String.Empty : str.Replace(';', '\n');
        }   
       
        public static DanfeViewModel CreateFromXmlFile(String path)
        {
            return DanfeViewModelCreator.CreateFromXmlFile(path);
        }

        public static DanfeViewModel CreateFromXmlString(String xml)
        {
            return DanfeViewModelCreator.CreateFromXmlString(xml);
        }

        public virtual String TextoRecebimento
        {
            get
            {
                return $"Recebemos de {Emitente.Nome} os produtos e/ou serviços constantes na Nota Fiscal Eletrônica indicada {(Orientacao == Orientacao.Retrato ? "abaixo" : "ao lado" )}. Emissão: {DataHoraEmissao.Formatar()} Valor Total: R$ {CalculoImposto.ValorTotalNota.Formatar()} Destinatário: {Destinatario.Nome}";
            }
        }

        public virtual String TextoAdicional()
        {
            StringBuilder sb = new StringBuilder();

            //Todo NF-e Referenciadas?

            if (!String.IsNullOrEmpty(InformacoesComplementares))
                sb.Append("Inf. Contribuinte: ").Append(InformacoesComplementares.Trim()).Replace(";", "\r\n");

            if (!String.IsNullOrWhiteSpace(InformacaoCompra))
                sb.Append(" ").Append(InformacaoCompra);

            if (!String.IsNullOrEmpty(Destinatario.Email))
                sb.Append(" Email do Destinatário: ").Append(Destinatario.Email);

            if (!String.IsNullOrEmpty(InformacoesAdicionaisFisco))
                sb.Append(" Inf. fisco: ").Append(InformacoesAdicionaisFisco);

            //Todo NT 2013.003 Lei da Transparência


            return sb.ToString();
        }

        public Boolean IsRetrato => Orientacao == Orientacao.Retrato;
        public Boolean IsPaisagem => Orientacao == Orientacao.Paisagem;

    }
}
