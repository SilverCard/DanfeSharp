using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanfeSharp
{
    internal class TabelaColuna
    {
        public String[] Cabecalho { get; private set; }
        public float PorcentagemLargura { get; set; }
        public AlinhamentoHorizontal AlinhamentoHorizontal { get; private set; }

        public TabelaColuna(String[] cabecalho, float porcentagemLargura, AlinhamentoHorizontal alinhamentoHorizontal = AlinhamentoHorizontal.Esquerda)
        {
            Cabecalho = cabecalho ?? throw new ArgumentNullException(nameof(cabecalho));
            PorcentagemLargura = porcentagemLargura;
            AlinhamentoHorizontal = alinhamentoHorizontal;
        }

        public override string ToString()
        {
            return String.Join(" ", Cabecalho);
        }
    }
}
