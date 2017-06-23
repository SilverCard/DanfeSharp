using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.pdfclown.files;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;
using org.pdfclown.documents.contents.fonts;
using System.Drawing;
using System.Linq;
using DanfeSharp.Graphics;
using System.IO;

namespace DanfeSharp.Test
{
    [TestClass]
    public class CampoTest
    {
        [TestMethod]
        public void DanfePaisagemKabum()
        {

            var model = (DanfeViewModel.CreateFromXmlFile(@""));
            model.Orientacao = Orientacao.Retrato;


            for (int i = 0; i < 50; i++)
            {               
                model.Produtos.Add(model.Produtos[0]);
                model.Produtos.Add(model.Produtos[1]);
            }

          //  model.InformacoesComplementares = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed placerat mollis nulla nec consequat. Ut tincidunt turpis in lacus egestas, eget dignissim dolor pulvinar. Fusce cursus ante urna, id varius purus volutpat vitae. Pellentesque augue nisl, porta non mi at, commodo gravida enim. Sed posuere consequat luctus. Praesent lacinia nulla at eros tempus, vitae tincidunt eros laoreet. Nunc odio elit, gravida nec venenatis ac, posuere at magna. Nam nec convallis massa, at blandit nibh. Vestibulum semper bibendum finibus. Vestibulum tempus dolor quis velit facilisis, eget pulvinar augue convallis. Aenean et lacus molestie orci vehicula v\r\nenenatis in et urna. Vestibulum eget mauris rhoncus, vehicula libero id, ultrices est.\r\nMorbi sagittis pretium massa, in luctus enim volutpat et. Sed et lacus condimentum, suscipit magna vitae, finibus ante. Suspendisse potenti. Sed non libero sit amet libero porttitor bibendum. Etiam et mauris vestibulum, blandit odio quis, accumsan est. Proin et nulla nec erat malesuada auctor quis sit amet risus. Morbi at ante bibendum, vestibulum arcu sit amet, feugiat lacus. Vestibulum commodo, arcu vel tempus mollis, enim velit sagittis orci, et pellentesque est massa et risus. Fusce nec pharetra arcu. Aliquam neque orci, mattis nec nunc ut, aliquet imperdiet arcu. Maecenas congue odio vitae erat tristique, vel mollis lacus semper.Sed volutpat quis leo ac pulvinar. Vestibulum vel dui velit. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Aliquam vehicula velit eget lectus consequat, in maximus sapien maximus. Maecenas commodo libero at ullamcorper rhoncus. Nulla facilisi. Aenean diam ligula, facilisis vel sollicitudin id, facilisis nec ligula. efwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwefefwef";

            //for (int i = 0; i < 8; i++)
            //{
            //    model.Duplicatas.Add(model.Duplicatas[0]);
            //}

            Danfe d = new Danfe(model);

          

            d.AdicionarLogoPdf(@"C: \Users\ricar\Desktop\logo.pdf");

            d.Gerar();

            d.Salvar("DanfePaisagem.pdf");
        }

        [TestMethod]
        public void Tabela()
        {
            SimplePdfPageTester t = new SimplePdfPageTester();
            Gfx gfx = t.Gfx;
            var e = t.CriarEstilo();

            Tabela tb = new Tabela(e);

            tb.ComColuna(7.5f, AlinhamentoHorizontal.Esquerda, "CÓDIGO PRODUTO")
                .ComColuna(0, AlinhamentoHorizontal.Centro, "DESCRIÇÃO DO PRODUTO / SERVIÇO")
                .ComColuna(6.5f, AlinhamentoHorizontal.Direita, "NCM/SH")
                .ComColuna(3.1F, AlinhamentoHorizontal.Direita, "O/CST")
                .ComColuna(3F, AlinhamentoHorizontal.Direita, "CFOP")
                .ComColuna(3.1F, AlinhamentoHorizontal.Direita, "UN")
                .ComColuna(5.3F, AlinhamentoHorizontal.Direita, "QUANTI.")
                .ComColuna(5.3F, AlinhamentoHorizontal.Direita, "VALOR UNIT.")
                .ComColuna(5, AlinhamentoHorizontal.Direita, "VALOR TOTAL.")
                .ComColuna(5, AlinhamentoHorizontal.Direita, "B CÁLC ICMS")
                .ComColuna(5, AlinhamentoHorizontal.Direita, "VALOR ICMS")
                .ComColuna(5, AlinhamentoHorizontal.Direita, "VALOR IPI")
                .ComColuna(5, AlinhamentoHorizontal.Direita, "ALIQ.", "ICMS")
                .ComColuna(5, AlinhamentoHorizontal.Direita, "ALIQ.", "IPI");

            tb.AjustarLarguraColunas();

            var r = new RectangleF(10, 10, 200, 100);

            tb.SetPosition(10, 10);
            tb.Width = r.Width;
            tb.Height = r.Height;



            for (int i = 0; i < 10; i++)
            {
                tb.AdicionarLinha(new System.Collections.Generic.List<string>()
                {
                    "",
                    "FONE DE OUVIDO PHILIPS INTRA-AURICULAR SHE800010 - TOT TRIB. 29.43STX\r\nGARANTIA 3 MESES",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                });

            }



            tb.Draw(gfx);

            gfx.Flush();

            t.Save();
        }

        [TestMethod]
        public void Campo()
        {
            SimplePdfPageTester t = new SimplePdfPageTester();
            Gfx gfx = t.Gfx;
            var e = t.CriarEstilo();

            var l = "Lorem ipsum\r\ndolor sit amet, consectetur adipiscing elit. Quisque ultrices imperdiet purus, sed placerat sapien interdum id. Integer bibendum ac ligula pulvinar iaculis. Suspendisse vulputate ligula nec interdum tempus. Integer elementum faucibus tellus. Morbi pulvinar ultricies sodales. Integer sem urna, mollis vestibulum nisl nec, pharetra malesuada purus. Nam vitae blandit sapien. Donec id nunc sapien. Praesent felis libero, dapibus scelerisque dolor dignissim, ullamcorper mollis urna. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Praesent venenatis viverra nulla ut viverra. Vestibulum rutrum interdum mi, et laoreet arcu gravida a. Suspendisse ac hendrerit dolor. abcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabca bcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabc.";

            var tb = new TextBlock(l, e.FonteCampoConteudo);
            tb.X = 130;
            tb.Y = 10;
            tb.Width = 70;

     
            gfx.StrokeRectangle(tb.BoundingBox, 0.5F);

            tb.Draw(gfx);

            var f1 = e.FonteCampoConteudo;

            var s1 = f1.MedirLarguraTexto("ab c");
            var s2 = f1.MedirLarguraTexto(" ");
            var s3 = f1.MedirLarguraTexto("c");
            var s4 = f1.MedirLarguraTexto("ab");

            Campo c = new Campo("Cabecalho", "Conteudo", e)
            {
                Width = 50,
                X = 10,
                Y = 10
            };

            c.Draw(gfx);

            var barcode = new Barcode128C("1258745", e)
            {
                X = 50,
                Y = 10,
                Width = 40,
                Height = 20
            };

            barcode.Draw(gfx);

            for (int i = 0; i < 20; i++)
            {
                c.Y += c.Height;
                c.Draw(gfx);
                c.Conteudo += "abc";               
            }

            gfx.Flush();

            t.Save();


        }
    }
}
