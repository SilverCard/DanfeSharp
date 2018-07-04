using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DanfeSharp
{
    /// <summary>
    /// Classe que ajuda na formatação de dados.
    /// </summary>
    public static class Formatador
    {
        /// <summary>
        /// Cultura pt-BR
        /// </summary>
        public static readonly CultureInfo Cultura = new CultureInfo(1046);

        static Formatador()
        {
            Cultura.NumberFormat.CurrencyPositivePattern = 2;
            Cultura.NumberFormat.CurrencyNegativePattern = 9;
        }

        public const String FormatoNumeroNF = @"000\.000\.000";

        public const String CEP = @"^(\d{5})\-?(\d{3})$";
        public const String CNPJ = @"^(\d{2})\.?(\d{3})\.?(\d{3})\/?(\d{4})\-?(\d{2})$";
        public const String CPF = @"^(\d{3})\.?(\d{3})\.?(\d{3})\-?(\d{2})$";
        public const String Telefone = @"^\(?(\d{2})\)?\s*(\d{4,5})\s*\-?\s*(\d{4})$";

        public const String FormatoMoeda = "#,0.00##";
        public const String FormatoNumero = "#,0.####";

        private static String InternalRegexReplace(String input, String pattern, String replacement)
        {
            String result = input;

            if (!String.IsNullOrWhiteSpace(input))
            {
                input = input.Trim();

                Regex rgx = new Regex(pattern);

                if (rgx.IsMatch(input))
                {
                    result = rgx.Replace(input, replacement);
                }
            }

            return result;
        }

        /// <summary>
        /// Formata a linha 1 do endereço. Ex. Floriano Peixoto, 512
        /// </summary>
        /// <param name="endereco"></param>
        /// <param name="numero"></param>
        /// <returns></returns>
        public static String FormatarEnderecoLinha1(String endereco, int? numero, String complemento = null)
        {
            String sNumero = numero.HasValue ? numero.Value.ToString() : null;
            return FormatarEnderecoLinha1(endereco, numero, complemento);
        }

        /// <summary>
        /// Formata a linha 1 do endereço. Ex. Floriano Peixoto, 512
        /// </summary>
        /// <param name="endereco"></param>
        /// <param name="numero"></param>
        /// <returns></returns>
        public static String FormatarEnderecoLinha1(String endereco, String numero = null, String complemento = null)
        {
            String linha1 = String.Empty;

            if (!String.IsNullOrWhiteSpace(endereco))
            {
                linha1 = String.Format("{0}, {1}", endereco.Trim(), String.IsNullOrWhiteSpace(numero) ? "S/N" : numero.Trim());

                if (!String.IsNullOrWhiteSpace(complemento))
                {
                    linha1 += " - " + complemento.Trim();
                }
            }

            return linha1;
        }

        /// <summary>
        /// Formata um CEP
        /// </summary>
        /// <param name="cep">CEP</param>
        /// <returns>CEP Formatado ou vazio caso cep inválido</returns>
        public static String FormatarCEP(String cep)
        {
            return InternalRegexReplace(cep, CEP, "$1-$2");
        }

        public static String FormatarCEP(int cep)
        {
            if (cep < 0)
            {
                throw new ArgumentOutOfRangeException("cep", "o cep não pode ser negativo.");
            }

            return FormatarCEP(cep.ToString().PadLeft(8, '0'));
        }

        public static String FormatarCnpj(String cnpj)
        {
            return InternalRegexReplace(cnpj, CNPJ, "$1.$2.$3/$4-$5");
        }

        public static String FormatarCpf(String cpf)
        {
            return InternalRegexReplace(cpf, CPF, "$1.$2.$3-$4");
        }

        /// <summary>
        /// Formata um número de documento
        /// </summary>
        /// <param name="cpfCnpj"></param>
        /// <returns></returns>
        public static String FormatarCpfCnpj(String cpfCnpj)
        {
            String result;

            if (!String.IsNullOrWhiteSpace(cpfCnpj))
            {
                result = cpfCnpj.Trim();

                if (Regex.IsMatch(result, CPF))
                {
                    result = FormatarCpf(result);
                }
                else if (Regex.IsMatch(result, CNPJ))
                {
                    result = FormatarCnpj(result);
                }
            }
            else
            {
                result = String.Empty;
            }

            return result;
        }

        /// <summary>
        /// Formata uma string de município com a uf, ex Caçapava do Sul - RS
        /// </summary>
        /// <param name="municipio">Município</param>
        /// <param name="uf">UF</param>
        /// <param name="separador">Separador</param>
        /// <returns>String formatada.</returns>
        public static String FormatarMunicipioUf(String municipio, String uf, String separador = " - ")
        {
            String result = "";

            if (!String.IsNullOrWhiteSpace(municipio) && !String.IsNullOrWhiteSpace(uf))
            {
                result = String.Format("{0}{1}{2}", municipio.Trim(), separador, uf.Trim());
            }
            else if (!String.IsNullOrWhiteSpace(municipio))
            {
                result = municipio.Trim();
            }
            else if (!String.IsNullOrWhiteSpace(uf))
            {
                result = uf.Trim();
            }

            return result;
        }

        public static String FormatarTelefone(String telefone)
        {
            return InternalRegexReplace(telefone, Telefone, "($1) $2-$3");
        }

        public static String FormatarChaveAcesso(String chaveAcesso)
        {
            return Regex.Replace(chaveAcesso, ".{4}", "$0 ").TrimEnd();
        }

        public static String Formatar(this Double number, String formato = FormatoMoeda)
        {
            return number.ToString(formato, Cultura);
        }

        public static String Formatar(this int number, String formato = FormatoMoeda)
        {
            return number.ToString(formato, Cultura);
        }

        public static String Formatar(this int? number, String formato = FormatoMoeda)
        {
            return number.HasValue ? number.Value.Formatar(formato) : String.Empty;
        }

        public static String Formatar(this Double? number, String formato = FormatoMoeda)
        {
            return number.HasValue ? number.Value.Formatar(formato) : String.Empty;
        }

        public static String FormatarMoeda(this Double? number)
        {
            return number.HasValue ? number.Value.ToString("C", Cultura) : String.Empty;
        }

        public static String Formatar(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToShortDateString() : String.Empty;
        }

        public static String Formatar(this TimeSpan? timeSpan)
        {
            return timeSpan.HasValue ? timeSpan.Value.ToString() : String.Empty;
        }
    }
}
