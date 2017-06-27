using DanfeSharp.Graphics;
using System;

namespace DanfeSharp
{
    /// <summary>
    /// Campo multilinha.
    /// </summary>
    internal class CampoMultilinha : Campo
    {   
        TextBlock _tbConteudo;

        public CampoMultilinha(String cabecalho, String conteudo, Estilo estilo, AlinhamentoHorizontal alinhamentoHorizontalConteudo = AlinhamentoHorizontal.Esquerda)
              : base(cabecalho, conteudo, estilo, alinhamentoHorizontalConteudo)
        {
            _tbConteudo = new TextBlock(conteudo, estilo.FonteCampoConteudo);
            IsConteudoNegrito = false;
        }

        protected override void DesenharConteudo(Gfx gfx)
        {
            if (!String.IsNullOrWhiteSpace(Conteudo))
            {
                _tbConteudo.SetPosition(RetanguloDesenhvael.X, RetanguloDesenhvael.Y + Estilo.FonteCampoCabecalho.AlturaLinha + Estilo.PaddingInferior);
                _tbConteudo.Draw(gfx);
            }
        }        

        public override float Height
        {
            get
            {
                return Math.Max(_tbConteudo.Height + Estilo.FonteCampoCabecalho.AlturaLinha + Estilo.PaddingSuperior + 2*Estilo.PaddingInferior , base.Height);
            }
            set
            {
                base.Height = value;
            }
        }

        public override string Conteudo { get => base.Conteudo; set { base.Conteudo = value; } }
        public override float Width { get => base.Width; set { base.Width = value; _tbConteudo.Width = value - 2*Estilo.PaddingHorizontal;  } }

    }
}
