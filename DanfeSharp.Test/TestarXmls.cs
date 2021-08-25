using DanfeSharp.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace DanfeSharp.Test
{
    [TestClass]
    public class TestarXmls
    {
        [TestMethod]
        public void XmlPasta()
        {           
            var arquivos = Directory.EnumerateFiles("../../XmlTestes", "*.xml");

            foreach (var arquivo in arquivos)
            {
                try
                {
                    DanfeViewModel model = DanfeViewModel.CreateFromXmlFile(arquivo);
                    using (DanfeDocumento danfe = new DanfeDocumento(model))
                    {
                        danfe.Gerar();

                        using (MemoryStream ms = new MemoryStream())
                        {
                            danfe.Salvar(ms);
                        }                        
                    }
                }
                catch(Exception e)
                {
                    Debugger.Break();
                }
                
            }
        }
    }
}
