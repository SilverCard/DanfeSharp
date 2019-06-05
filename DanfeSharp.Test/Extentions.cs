using System.Diagnostics;
using System.IO;

namespace DanfeSharp.Test
{
    public static class Extentions
    {
        public const string OutputDirectoryName = "Output";

        public static void SalvarTestPdf(this DanfeSharp.Danfe d, string outputName = null)
        {
            if (!Directory.Exists(OutputDirectoryName)) Directory.CreateDirectory(OutputDirectoryName);

            if (outputName == null) outputName = new StackTrace().GetFrame(1).GetMethod().Name;

            d.Salvar(Path.Combine(OutputDirectoryName, outputName + ".pdf"));
        }
    }
}
