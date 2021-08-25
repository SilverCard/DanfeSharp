using System;
using System.Windows;

namespace DanfeSharp.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if(e.Args.Length > 0)
            {
                String xmlPath = e.Args[0];
                String logoPath = e.Args.Length >= 2 ? e.Args[1] : null;

                GeradorDanfe.GerarDanfe(xmlPath, logoPath);
                Application.Current.Shutdown();
            }
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Erro Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
