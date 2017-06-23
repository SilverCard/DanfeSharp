using System;

namespace DanfeSharp.Modelo
{
    public class DuplicataViewModel
    {
        /// <summary>
        /// <para>Número da Duplicata</para>
        /// <para>Tag nDup</para>
        /// </summary>
        public String Numero { get; set; }

        /// <summary>
        /// <para>Data de vencimento</para>
        /// <para>Tag dVenc</para>
        /// </summary>
        public DateTime? Vecimento { get; set; }

        /// <summary>
        /// <para>Valor da duplicata</para>
        /// <para>Tag vDup</para>
        /// </summary>
        public Double? Valor { get; set; }
    }
}
