using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DanfeSharp
{
    /// <summary>
    /// Classe que ajuda na formatação de dados.
    /// </summary>
    public static class Formatador
    {
        public static readonly CultureInfo Culture = new CultureInfo("pt-BR");

        public const string FormatoNumeroNF = @"000\.000\.000";

        public const string CEP = @"^(\d{5})\-?(\d{3})$";
        public const string CNPJ = @"^(\d{2})\.?(\d{3})\.?(\d{3})\/?(\d{4})\-?(\d{2})$";
        public const string CPF = @"^(\d{3})\.?(\d{3})\.?(\d{3})\-?(\d{2})$";
        public const string Telefone = @"^\(?(\d{2})\)?\s*(\d{4,5})\s*\-?\s*(\d{4})$";
        public const string Placa = @"^([A-Z]{3})\s*\-?\s*(\d{4})$";

        public const string FormatoMoeda = "#,0.00##";
        public const string FormatoNumero = "#,0.####";

        private static string InternalRegexReplace(string input, string pattern, string replacement)
        {
            string result = input;

            if (!string.IsNullOrWhiteSpace(input))
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
        public static string FormatarEnderecoLinha1(string endereco, int? numero, string complemento = null)
        {
            string sNumero = numero.HasValue ? numero.Value.ToString() : null;
            return FormatarEnderecoLinha1(endereco, numero, complemento);
        }

        /// <summary>
        /// Formata a linha 1 do endereço. Ex. Floriano Peixoto, 512
        /// </summary>
        /// <param name="endereco"></param>
        /// <param name="numero"></param>
        /// <returns></returns>
        public static string FormatarEnderecoLinha1(string endereco, string numero = null, string complemento = null)
        {
            string linha1 = string.Empty;

            if (!string.IsNullOrWhiteSpace(endereco))
            {
                linha1 = string.Format("{0}, {1}", endereco.Trim(), string.IsNullOrWhiteSpace(numero) ? "S/N" : numero.Trim());

                if (!string.IsNullOrWhiteSpace(complemento))
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
        public static string FormatarCEP(string cep)
        {
            return InternalRegexReplace(cep, CEP, "$1-$2");
        }

        public static string FormatarCEP(int cep)
        {
            if (cep < 0)
            {
                throw new ArgumentOutOfRangeException("cep", "o cep não pode ser negativo.");
            }

            return FormatarCEP(cep.ToString().PadLeft(8, '0'));
        }

        public static string FormatarCnpj(string cnpj)
        {
            return InternalRegexReplace(cnpj, CNPJ, "$1.$2.$3/$4-$5");
        }

        public static string FormatarCpf(string cpf)
        {
            return InternalRegexReplace(cpf, CPF, "$1.$2.$3-$4");
        }

        /// <summary>
        /// Formata um número de documento
        /// </summary>
        /// <param name="cpfCnpj"></param>
        /// <returns></returns>
        public static string FormatarCpfCnpj(string cpfCnpj)
        {
            string result;

            if (!string.IsNullOrWhiteSpace(cpfCnpj))
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
                result = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// Formata uma string de município com a uf, ex Caçapava do Sul - RS
        /// </summary>
        /// <param name="municipio">Município</param>
        /// <param name="uf">UF</param>
        /// <param name="separador">Separador</param>
        /// <returns>string formatada.</returns>
        public static string FormatarMunicipioUf(string municipio, string uf, string separador = " - ")
        {
            string result = "";

            if (!string.IsNullOrWhiteSpace(municipio) && !string.IsNullOrWhiteSpace(uf))
            {
                result = string.Format("{0}{1}{2}", municipio.Trim(), separador, uf.Trim());
            }
            else if (!string.IsNullOrWhiteSpace(municipio))
            {
                result = municipio.Trim();
            }
            else if (!string.IsNullOrWhiteSpace(uf))
            {
                result = uf.Trim();
            }

            return result;
        }

        public static string FormatarPlacaVeiculo(string placa)
        {
            return InternalRegexReplace(placa, Placa, "$1-$2");
        }

        public static string FormatarTelefone(string telefone)
        {
            return InternalRegexReplace(telefone, Telefone, "($1) $2-$3");
        }

        public static string FormatarChaveAcesso(string cnpj)
        {
            return Regex.Replace(cnpj, ".{4}", "$0 ").TrimEnd();
        }

        public static string Formatar(this double number, string formato = FormatoMoeda)
        {
            return number.ToString(formato, Culture);
        }

        public static string Formatar(this int number, string formato = FormatoMoeda)
        {
            return number.ToString(formato, Culture);
        }

        public static string Formatar(this int? number, string formato = FormatoMoeda)
        {
            return number.HasValue ? number.Value.Formatar(formato) : string.Empty;
        }

        public static string Formatar(this double? number, string formato = FormatoMoeda)
        {
            return number.HasValue ? number.Value.Formatar(formato) : string.Empty;
        }

        public static string Formatar(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToShortDateString() : string.Empty;
        }

        public static string Formatar(this TimeSpan? timeSpan)
        {
            return timeSpan.HasValue ? timeSpan.Value.ToString() : string.Empty;
        }
    }
}