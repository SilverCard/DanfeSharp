using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanfeSharp.Modelo
{
    public class CalculoIssqnViewModel
    {
        public String InscricaoMunicipal { get; set; }

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

        /// <summary>
        /// Mostrar ou não o Bloco.
        /// </summary>
        public Boolean Mostrar { get; set; }

        public CalculoIssqnViewModel()
        {
            Mostrar = false;
        }
    }
}
