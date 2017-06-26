using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DanfeSharp.Test
{
    [TestClass]
    public class DanfeTest
    {
        [TestMethod]
        public void GerarDanfeXml()
        {
            string pasta = @"D:\NFe";
            string caminhoOut = "DanfesDeXml";

            if (!Directory.Exists(caminhoOut)) Directory.CreateDirectory(caminhoOut);

            foreach (var pathXml in Directory.EnumerateFiles(pasta, "*.xml"))
            {
                var model = DanfeViewModelCreator.CreateFromXmlFile(pathXml);
                using (Danfe danfe = new Danfe(model))
                {
                    danfe.Gerar();
                    danfe.Salvar($"{caminhoOut}/{model.ChaveAcesso}.pdf");
                }
            }
        }

        [TestMethod]
        public void RetratoSemIcmsInterestadual()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Retrato;
            model.ExibirIcmsInterestadual = false;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void PaisagemSemIcmsInterestadual()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Paisagem;
            model.ExibirIcmsInterestadual = false;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void Retrato()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Retrato;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);       
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void Paisagem()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Paisagem;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void RetratoHomologacao()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.TipoAmbiente = 2;
            model.Orientacao = Orientacao.Retrato;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

        [TestMethod]
        public void PaisagemHomologacao()
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.TipoAmbiente = 2;
            model.Orientacao = Orientacao.Paisagem;
            DanfeSharp.Danfe d = new DanfeSharp.Danfe(model);
            d.Gerar();
            d.SalvarTestPdf();
        }

    }
}
