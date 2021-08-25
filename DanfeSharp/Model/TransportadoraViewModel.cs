﻿using System;
using System.Collections.Generic;

namespace DanfeSharp.Model
{
    public class TransportadoraViewModel : EmpresaViewModel
    {
        public static readonly Dictionary<int, String> ModalidadesFrete = new Dictionary<int,string>()
        {
            {0, "Emitente"},
            {1, "Dest/Rem"},
            {2, "Terceiros"},
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
        public String CodigoAntt { get; set; }

        /// <summary>
        /// <para>Placa do Veículo.</para>
        /// <para>Tag placa</para>
        /// </summary>
        public String Placa { get; set; }

        /// <summary>
        /// <para>Sigla da UF do Veículo</para>
        /// <para>Tag UF</para>
        /// </summary>
        public String VeiculoUf { get; set; }

        /// <summary>
        /// <para>Quantidade de volumes transportados.</para>
        /// <para>Tag qVol</para>
        /// </summary>
        public Double? QuantidadeVolumes { get; set; }

        /// <summary>
        /// <para>Espécie dos volumes transportados.</para>
        /// <para>Tag esp</para>
        /// </summary>
        public String Especie { get; set; }

        /// <summary>
        /// <para>Marca dos volumes transportados.</para>
        /// <para>Tag marca</para>
        /// </summary>
        public String Marca { get; set; }

        /// <summary>
        /// <para>Numeração dos volumes transportados.</para>
        /// <para>Tag nVol</para>
        /// </summary>
        public String Numeracao { get; set; }

        /// <summary>
        /// <para>Peso Líquido (em kg).</para>
        /// <para>Tag pesoL</para>
        /// </summary>
        public Double? PesoLiquido { get; set; }

        /// <summary>
        /// <para>Peso Bruto (em kg).</para>
        /// <para>Tag pesoB</para>
        /// </summary>
        public Double? PesoBruto { get; set; }

        public String ModalidadeFreteString
        {
            get
            {
                String result = "";

                if (ModalidadesFrete.ContainsKey(ModalidadeFrete))
                {
                    result = String.Format("({0}) {1}", ModalidadeFrete, ModalidadesFrete[ModalidadeFrete]);
                }

                return result;
            }
        }
    }
}
