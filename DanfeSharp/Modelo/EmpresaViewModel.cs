using System;
using System.Text;

namespace DanfeSharp.Modelo
{
    public class EmpresaViewModel
    {
        /// <summary>
        /// <para>Razão Social ou Nome</para>
        /// <para>Tag xNome</para>
        /// </summary>
        public String RazaoSocial { get; set; }

        /// <summary>
        /// <para>Nome fantasia</para>
        /// <para>Tag xFant</para>
        /// </summary>
        public String NomeFantasia { get; set; }

        /// <summary>
        /// <para>Logradouro</para>
        /// <para>Tag xLgr</para>
        /// </summary>
        public String EnderecoLogadrouro { get; set; }

        /// <summary>
        /// <para>Complemento</para>
        /// <para>Tag xCpl</para>
        /// </summary>
        public String EnderecoComplemento { get; set; }

        /// <summary>
        /// <para>Número</para>
        /// <para>Tag nro</para>
        /// </summary>
        public String EnderecoNumero { get; set; }

        /// <summary>
        /// <para>Código do CEP</para>
        /// <para>Tag CEP</para>
        /// </summary>
        public String EnderecoCep { get; set; }

        /// <summary>
        /// <para>Bairro</para>
        /// <para>Tag xBairro</para>
        /// </summary>
        public String EnderecoBairro { get; set; }

        /// <summary>
        /// <para>Sigla da UF</para>
        /// <para>Tag UF</para>
        /// </summary>
        public String EnderecoUf { get; set; }

        /// <summary>
        /// <para>Nome do município</para>
        /// <para>Tag xMun</para>
        /// </summary>
        public String Municipio { get; set; }

        /// <summary>
        /// <para>Telefone</para>
        /// <para>Tag fone</para>
        /// </summary>
        public String Telefone { get; set; }
        
        /// <summary>
        /// <para>CNPJ ou CPF</para>
        /// <para>Tag CNPJ ou CPF</para>
        /// </summary>
        public String CnpjCpf { get; set; }

        /// <summary>
        /// <para>Inscrição Estadual</para>
        /// <para>Tag IE</para>
        /// </summary>
        public String Ie { get; set; }

        /// <summary>
        /// <para>IE do Substituto Tributário</para>
        /// <para>Tag IEST</para>
        /// </summary>
        public String IeSt { get; set; }

        /// <summary>
        /// <para>Inscrição Municipal</para>
        /// <para>Tag IM</para>
        /// </summary>
        public String IM { get; set; }

        /// <summary>
        /// <para>Email</para>
        /// <para>Tag email</para>
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// Código de Regime Tributário
        /// </summary>
        public String CRT { get; set; }

        /// <summary>
        /// Linha 1 do Endereço
        /// </summary>
        public String EnderecoLinha1
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(EnderecoLogadrouro);
                if (!String.IsNullOrWhiteSpace(EnderecoNumero)) sb.Append(", ").Append(EnderecoNumero);
                if (!String.IsNullOrWhiteSpace(EnderecoComplemento)) sb.Append(" - ").Append(EnderecoComplemento);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Linha 1 do Endereço
        /// </summary>
        public String EnderecoLinha2 => $"{EnderecoBairro} - CEP: {Formatador.FormatarCEP(EnderecoCep)}";


        /// <summary>
        /// Linha 3 do Endereço
        /// </summary>
        public String EnderecoLinha3
        {
            get
            {
                StringBuilder sb = new StringBuilder()
                    .Append(Municipio).Append(" - ").Append(EnderecoUf);

                if (!String.IsNullOrWhiteSpace(Telefone))
                    sb.Append(" Fone: ").Append(Formatador.FormatarTelefone(Telefone));

                return sb.ToString();               
            }
        }
        
    }
}
