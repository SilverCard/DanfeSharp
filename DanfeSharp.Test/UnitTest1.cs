using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Drawing;

namespace DanfeSharp.Test
{
    [TestClass]
    public class UnitTest1
    {
        public static DanfeViewModel Modelo1()
        {
            DanfeViewModel model = new DanfeViewModel()
            {
                Orientacao = Orientacao.Retrato,
                NfNumero = 999999999,
                NfSerie = 999,
                NaturezaOperacao = "Venda",
                ChaveAcesso = "32161205570714000825550010017054141078477682",
                InformacoesComplementares = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec et ligula congue, consequat sem eget, sagittis nunc.Proin metus nibh, varius nec metus ac, volutpat eleifend nunc. Vivamus accumsan, mauris at aliquam varius, nulla leo pellentesque sem, tristique consequat enim dui eget neque.Nam facilisis tortor in velit tempor ullamcorper.Nunc varius turpis sit amet urna mollis faucibus. Nunc eget velit vel nunc dictum tempus.Quisque nec viverra risus.\r\nQuisque dictum elementum tincidunt. Quisque sit amet neque neque.Etiam massa orci, aliquam interdum enim non, dignissim viverra erat. Sed dignissim magna vitae velit mollis tincidunt.Quisque luctus, sem ut viverra mollis, tellus lectus posuere ipsum, pretium hendrerit justo risus vitae metus.Vivamus fermentum, sem nec egestas vehicula, nulla libero placerat ex, at volutpat eros neque in metus.Curabitur nec pellentesque nunc, non fringilla sem. Aenean enim ante, auctor ut porttitor vitae, semper et velit. Integer mollis tortor sit amet efficitur fermentum.Maecenas nulla justo, tempor porttitor lacinia a, mollis eu quam. Proin molestie eget turpis non sagittis. Donec at tellus id erat faucibus efficitur.Proin non rutrum justo. Curabitur commodo ultricies purus, ac ornare lacus molestie ac. Nunc eu dictum lorem."
            };

            return model;
        }
        [TestMethod]
        public void DanfePaisagemKabum()
        {

            var model = (DanfeViewModel.CreateFromXmlFile(@"C:\Users\ricar\Downloads\32161205570714000825550010017054141078477682.xml"));
            model.Orientacao = Orientacao.Paisagem;

            Danfe d = new Danfe(model);

            d.AdicionarPagina();
            d.AdicionarPagina();
            d.AdicionarPagina();
            d.PreencherNumeroFolhas();

            d.Salvar("DanfePaisagem.pdf");
        }


        [TestMethod]
        public void DanfeRetratoKabum()
        {

            var model = (DanfeViewModel.CreateFromXmlFile(@"C:\Users\ricar\Downloads\32161205570714000825550010017054141078477682.xml"));
            model.Orientacao = Orientacao.Retrato;

            Danfe d = new Danfe(model);

            d.AdicionarPagina();
            d.AdicionarPagina();
            d.AdicionarPagina();
            d.PreencherNumeroFolhas();

            d.Salvar("DanfeRetrato.pdf");
         

        }


        [TestMethod]
        public void Stack()
        {
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();

            XGraphics gfx = XGraphics.FromPdfPage(page);
            Gfx gfx2 = new Gfx(gfx);
            Estilo estilo = new Estilo();

            LinhaCampos stack = new LinhaCampos(estilo, 100);
            stack.Height = Constantes.CampoAltura;



            stack.ComCampo("Data", "Data")
                .ComCampo("Recebedor", "Recebedor")
                .ComLarguras(30, 0);


            VerticalStack pilha2 = new VerticalStack();
            pilha2.Width = 100;
         //   pilha2.Height = 2 * Constantes.CampoAltura;

            pilha2.SetPosition(10, 10);

            pilha2.Add(new Campo("Topo", "w", estilo));
            pilha2.Add(stack);

            pilha2.Draw(gfx2);

            pilha2.Y = pilha2.BoundingBox.Bottom;
            pilha2.Width = 150;
            pilha2.Draw(gfx2);

            pilha2.Y = pilha2.BoundingBox.Bottom;
            pilha2.Width = 100;
            pilha2.Draw(gfx2);

            document.Save("Pilha.pdf");

        }


        [TestMethod]
        public void TestMethod1()
        {
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();

            XGraphics gfx = XGraphics.FromPdfPage(page);
            Gfx gfx2 = new Gfx(gfx);

            Estilo estilo = new Estilo();

            Campo campo1 = new Campo("Cabeçalho", "vulputate", estilo);



            campo1.X = 10;
            campo1.Y = 10;
            campo1.Width = 30;




            campo1.Draw(gfx2);


            CampoMultilinha campo2 = new CampoMultilinha("Cabecalho", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur ultrices dignissim est ac eleifend. Curabitur sed sem viverra, viverra nibh maximus, consectetur libero. Sed elementum urna in consequat scelerisque. Vestibulum eu pellentesque odio. Maecenas velit nunc, semper a felis ac, auctor consequat justo. Maecenas tempus turpis eget metus volutpat efficitur. Donec quis lacus eget sapien tristique mattis. Cras commodo blandit vulputate.", estilo);
            campo2.Y = campo1.BoundingBox.Bottom;
            campo2.X = 10;
            campo2.Width = 50;
            campo2.Draw(gfx2);

            campo1.Y = campo2.BoundingBox.Bottom;

            for (int i = 0; i < 15; i++)
            {
                campo1.Draw(gfx2);
                campo1.Conteudo += "abc";
                campo1.Y += campo1.Height;
            }


            document.Save("TextCampo.pdf");

        }
    }
}
