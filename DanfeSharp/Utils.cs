using System;
using System.Text.RegularExpressions;

namespace DanfeSharp
{
    internal static class Utils
    {
        /// <summary>
        /// Verifica se uma string contém outra string no formato chave: valor.
        /// </summary>
        public static Boolean StringContemChaveValor(String str, String chave, String valor)
        {
            if (String.IsNullOrWhiteSpace(chave)) throw new ArgumentException(nameof(chave));
            if (String.IsNullOrWhiteSpace(str)) return false;

            return Regex.IsMatch(str, $@"({chave}):?\s*{valor}", RegexOptions.IgnoreCase);
        }        
    }
}
