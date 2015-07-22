using DanfeSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DanfeSharp.App
{
    public static class GeradorDanfe
    {
        public static void GerarDanfe(String xmlPath, String logoPath)
        {
            try
            {
                DanfeViewModel model = DanfeViewModel.CreateFromXmlFile(xmlPath);
                DanfeDocumento danfe = new DanfeDocumento(model);

                if(!String.IsNullOrWhiteSpace(logoPath))
                {
                    danfe.AdicionarLogo(logoPath);
                }

                danfe.Gerar();

                String outFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(xmlPath), model.ChaveAcesso + ".pdf");
                danfe.Salvar(outFile);

                var process = System.Diagnostics.Process.Start(outFile);

                if(process == null)
                {
                    MessageBox.Show(String.Format("Não foi possível abrir o DANFE gerado.\nEle foi gravado em: {0}", outFile), Strings.AppName, MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Erro ao gerar o DANFE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
