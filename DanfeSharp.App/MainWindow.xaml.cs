using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DanfeSharp.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Title = String.Format("{0} v{1}", Strings.AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
        }


        internal void ProcurarArquivo(TextBox tb, String filter, String title )
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = filter;
            dlg.Title = title;

            if (dlg.ShowDialog() == true)
            {
                tb.Text = dlg.FileName;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProcurarArquivo(PathXml, "NFe Processada (*.xml)|*.xml", "Abrir NFe");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ProcurarArquivo(PathLogo, "Logo (*.jpg, *.pdf)|*.jpg;*.pdf", "Abrir Logo");
        }

        private void BGerarDanfe_Click(object sender, RoutedEventArgs e)
        {
            GeradorDanfe.GerarDanfe(PathXml.Text, PathLogo.Text);
        }

        private void SetPathFromFiles(string[] files)
        {
            string pathXml = files.FirstOrDefault(x => x.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase));
            string pathLogo = files.FirstOrDefault(x => x.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) || x.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase));

            if (!String.IsNullOrWhiteSpace(pathXml))
                PathXml.Text = pathXml;

            if (!String.IsNullOrWhiteSpace(pathLogo))
                PathLogo.Text = pathLogo;
        }

        private void Event_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {              
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                SetPathFromFiles(files);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (e != null)
            {
                System.Diagnostics.Process.Start(e.Uri.AbsoluteUri.ToString());
            }
        }
    }
}
