using System;
using System.Collections.Generic;

namespace DanfeSharp.Model
{
    public class TransportadoraViewModel : EmpresaViewModel
    {
        public static readonly Dictionary<int, string> ModalidadesFrete = new Dictionary<int,string>()
        {
            {0, "Por Conta do Rem."},
            {1, "Por Conta do Dest."},
            {2, "Terceiros"},
            {3, "Proprio por Conta Rem."},
            {4, "Proprio por Conta Dest."},
            {9, "Sem frete"}
        };

        /// <summary>
        /// <para>Modalidade do frete.</para>
        /// <para>Tag modFrete</para>
        /// </summary>
        public int ModalidadeFrete { get; set; }
        
        /// <summary>
        /// <para>Registro Nacional de Transportador de Carga (ANTT).</para>
        /// <para>Tag RNTC</para>
        /// </summary>
        public string CodigoAntt { get; set; }

        /// <summary>
        /// <para>Placa do Veículo.</para>
        /// <para>Tag placa</para>
        /// </summary>
        public string Placa { get; set; }

        /// <summary>
        /// <para>Sigla da UF do Veículo</para>
        /// <para>Tag UF</para>
        /// </summary>
        public string VeiculoUf { get; set; }

        /// <summary>
        /// <para>Quantidade de volumes transportados.</para>
        /// <para>Tag qVol</para>
        /// </summary>
        public double? QuantidadeVolumes { get; set; }

        /// <summary>
        /// <para>Espécie dos volumes transportados.</para>
        /// <para>Tag esp</para>
        /// </summary>
        public string Especie { get; set; }

        /// <summary>
        /// <para>Marca dos volumes transportados.</para>
        /// <para>Tag marca</para>
        /// </summary>
        public string Marca { get; set; }

        /// <summary>
        /// <para>Numeração dos volumes transportados.</para>
        /// <para>Tag nVol</para>
        /// </summary>
        public string Numeracao { get; set; }

        /// <summary>
        /// <para>Peso Líquido (em kg).</para>
        /// <para>Tag pesoL</para>
        /// </summary>
        public double? PesoLiquido { get; set; }

        /// <summary>
        /// <para>Peso Bruto (em kg).</para>
        /// <para>Tag pesoB</para>
        /// </summary>
        public double? PesoBruto { get; set; }

        public string ModalidadeFreteString
        {
            get
            {
                string result = "";

                if (ModalidadesFrete.ContainsKey(ModalidadeFrete))
                {
                    result = string.Format("({0}) {1}", ModalidadeFrete, ModalidadesFrete[ModalidadeFrete]);
                }

                return result;
            }
        }
    }
}
