using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;
using org.pdfclown.documents.contents.fonts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanfeSharp
{   

    public class BlocoProdutos : BlocoDanfe
    {
        public int ProdutoIndex { get; private set; }
        public int ProdutoIndexEnd { get; private set; }
        
        String[,] CellValues;


        public BlocoProdutos(DanfeDocumento danfeMaker, float height, int produtoIndex)
            : base(danfeMaker)
        {
            Size = new SizeF(Danfe.InnerRect.Width, height);
            ProdutoIndex = produtoIndex;
            Initialize();
           


            CellValues = new string[Danfe.Model.Produtos.Count, 14];

            for (int i = 0; i < Danfe.Model.Produtos.Count; i++)
            {
                for (int y = 0; y < 14; y++)
                {
                    CellValues[i, y] = "";
                } 
            }

            for (int i = 0; i < Danfe.Model.Produtos.Count; i++)
            {
                var produto = Danfe.Model.Produtos[i];
                CellValues[i, 0] = produto.Codigo;
                CellValues[i, 1] = produto.DescricaoCompleta;
                CellValues[i, 2] = produto.Ncm;
                CellValues[i, 3] = produto.OCst;
                CellValues[i, 4] = produto.Cfop.ToString();
                CellValues[i, 5] = produto.Unidade;
                CellValues[i, 6] = produto.Quantidade.Formatar(Formatador.FormatoNumero);
                CellValues[i, 7] = produto.ValorUnitario.Formatar();
                CellValues[i, 8] = produto.ValorTotal.Formatar();
                CellValues[i, 9] = produto.BaseIcms.Formatar();
                CellValues[i, 10] = produto.ValorIcms.Formatar();
                CellValues[i, 11] = produto.ValorIpi.Formatar();
                CellValues[i, 12] = produto.AliquotaIcms.Formatar();
                CellValues[i, 13] = produto.AliquotaIpi.Formatar();
            }
        }


        protected override void CriarCampos()
        {
        }

        protected override void PosicionarCampos()
        {
        }

        protected override void ToXObjectInternal(PrimitiveComposer composer)
        {
            DanfeProdutosServicosTabela table = new DanfeProdutosServicosTabela(Danfe);
            table.Valores = CellValues;

            int end = table.PrintTable(composer, InternalRectangle, ProdutoIndex);

            if (ProdutoIndex == end)
            {
                throw new Exception("O índice do produto não foi incrementado ao gerar a tabela.");
            }

            ProdutoIndexEnd = end;
       
            composer.Stroke();
        }

        public override string Cabecalho
        {
            get
            {
                return "DADOS DOS PRODUTOS / SERVIÇOS";
            }
        }
    }
}
