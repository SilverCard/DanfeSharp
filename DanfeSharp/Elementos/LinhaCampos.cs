using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanfeSharp
{
    internal class LinhaCampos : FlexibleLine
    {
        public Estilo Estilo { get; private set; }

        public LinhaCampos(Estilo estilo, float width, float height = Constantes.CampoAltura) : base()
        {
            Estilo = estilo;
            SetSize(width, height);
        }


        public LinhaCampos(Estilo estilo) : base()
        {
            Estilo = estilo;
        }

        public virtual LinhaCampos ComCampo(String cabecalho, String conteudo, AlinhamentoHorizontal alinhamentoHorizontalConteudo = AlinhamentoHorizontal.Esquerda)
        {
            var campo = new Campo(cabecalho, conteudo, Estilo, alinhamentoHorizontalConteudo);
            Elementos.Add(campo);
            return this;
        }
    }
}
