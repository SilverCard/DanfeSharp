using System;
using DanfeSharp.Graphics;

namespace DanfeSharp.Blocos
{
    internal abstract class BlocoBase : ElementoBase
    {
        public DanfeViewModel ViewModel { get; private set; }

        public abstract PosicaoBloco Posicao { get; }
        public VerticalStack MainVerticalStack { get; private set; }

        public virtual Boolean VisivelSomentePrimeiraPagina => true;
        public virtual String Cabecalho => null;

        public BlocoBase(DanfeViewModel viewModel, Estilo estilo) : base(estilo)
        {
            MainVerticalStack = new VerticalStack();
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            if (!String.IsNullOrWhiteSpace(Cabecalho))
            {
                MainVerticalStack.Add(new CabecalhoBloco(estilo, Cabecalho));
            }
        }

        public LinhaCampos AdicionarLinhaCampos()
        {
            var l = new LinhaCampos(Estilo, Width);
            l.Width = Width;
            l.Height = Constantes.CampoAltura;
            MainVerticalStack.Add(l);
            return l;
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);
            MainVerticalStack.SetPosition(X, Y);
            MainVerticalStack.Width = Width;
            MainVerticalStack.Draw(gfx);
        }

        public override float Height { get => MainVerticalStack.Height; set => throw new NotSupportedException(); }
        public override bool PossuiContono => false;            
    }
}
