using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanfeSharp.Model
{
    public class EmpresaViewModel
    {
        /// <summary>
        /// <para>Razão Social ou Nome</para>
        /// <para>Tag nNF</para>
        /// </summary>
        public String Nome { get; set; }

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
        /// Linha 1 do Endereço
        /// </summary>
        public String EnderecoLinha1
        {
            get
            {
                return Formatador.FormatarEnderecoLinha1(EnderecoLogadrouro, EnderecoNumero, EnderecoComplemento);
            }
        }

        /// <summary>
        /// Linha 3 do Endereço
        /// </summary>
        public String EnderecoLinha3
        {
            get
            {
                return String.Format("{0} {1} - {2}", Formatador.FormatarCEP(EnderecoCep), Municipio, EnderecoUf);
            }
        }

        

    }
}
