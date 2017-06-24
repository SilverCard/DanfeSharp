using System.Diagnostics;

namespace DanfeSharp.Test
{
    public static class Extentions
    {
        public static void SalvarTestPdf(this DanfeSharp.Danfe d)
        {
            d.Salvar(new StackTrace().GetFrame(1).GetMethod().Name + ".pdf");
        }
    }
}
