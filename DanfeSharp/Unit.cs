using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanfeSharp
{
    /// <summary>
    /// Essa classe contém metodos para converter entre unidades
    /// </summary>
    public static class Unit
    {
        /// <summary>
        /// 1 Pdf Unit equivale a essa constante
        /// </summary>
        private const float Mm2PuConst = 127F / 360F;

        /// <summary>
        /// Converte milímetro para Pdf Unit
        /// </summary>
        /// <param name="mm">milímetro</param>
        /// <returns>Valor convertido</returns>
        public static float Mm2Pu(float mm)
        {
            return mm / Mm2PuConst;
        }
    }
}
