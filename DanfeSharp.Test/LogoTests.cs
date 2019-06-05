using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace DanfeSharp.Test
{
    [TestClass]
    public class LogoTests
    {
        public static readonly string OutputDirectoryName = Path.Combine("Output", "ComLogo");

        static LogoTests()
        {
            if (!Directory.Exists(OutputDirectoryName)) Directory.CreateDirectory(OutputDirectoryName);
        }

        public void TestLogo(String logoPath, [CallerMemberName] string pdfName = null)
        {
            var model = FabricaFake.DanfeViewModel_1();
            model.Orientacao = Orientacao.Retrato;
            using (DanfeSharp.Danfe d = new DanfeSharp.Danfe(model))
            {
                if (logoPath.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
                {
                    d.AdicionarLogoPdf(logoPath);
                    model.Emitente.NomeFantasia = "Logo Vetor Ltda.";
                }
                else
                {
                    d.AdicionarLogoImagem(logoPath);
                    model.Emitente.NomeFantasia = "Logo Raster Ltda.";
                }

                d.Gerar();
                d.Salvar(Path.Combine(OutputDirectoryName, pdfName + ".pdf"));
            }
        }
        

        [TestMethod]
        public void LogoQuadradoJPG() => TestLogo("Logos/JPG/Quadrado.jpg");

        [TestMethod]
        public void LogoHorizontalJPG() => TestLogo("Logos/JPG/Horizontal.jpg");  

        [TestMethod]
        public void LogoVerticalJPG() => TestLogo("Logos/JPG/Vertical.jpg");

        [TestMethod]
        public void LogoQuadradoPDF() => TestLogo("Logos/PDF/Quadrado.pdf");

        [TestMethod]
        public void LogoHorizontalPDF() => TestLogo("Logos/PDF/Horizontal.pdf");

        [TestMethod]
        public void LogoVerticalPDF() => TestLogo("Logos/PDF/Vertical.pdf");

    }
}
