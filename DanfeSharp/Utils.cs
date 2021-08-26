using DanfeSharp.Schemas;
using System;
using System.Xml.Linq;

namespace DanfeSharp
{
    public static class Utils
    {
        /// <summary>
        /// 1 Pdf Unit equivale a essa constante
        /// </summary>
        private const float Mm2PuConst = 127F / 360F;

        /// <summary>
        /// Converte milímetro para Pdf Unit
        /// </summary>
        /// <param name="mm">milímetro</param>
        /// <returns>Valor convertido</returns>
        public static float Mm2Pu(float mm)
        {
            return mm / Mm2PuConst;
        }

        /// <summary>
        /// Conta o número de linhas que o texto vai ocupar.
        /// </summary>
        /// <param name="font">Fonte do texto</param>
        /// <param name="fontSize">Tamanho da fonte</param>
        /// <param name="width">Largura máxima do texto</param>
        /// <param name="text">Texto</param>
        /// <returns>Número de linhas</returns>
        internal static int CountTextLines(org.pdfclown.documents.contents.fonts.Font font, double fontSize, double width, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0;
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            if (fontSize <= 0)
            {
                throw new ArgumentOutOfRangeException("fontSize");
            }

            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException("width");
            }

            int lines = 0;
            int index = 0, end;
            org.pdfclown.documents.contents.composition.TextFitter tf = new org.pdfclown.documents.contents.composition.TextFitter(text, width, font, fontSize, false, '-');

            while (index < text.Length)
            {
                tf.Fit(index, width, true);
                end = tf.EndIndex;

                // Isso evita um loop infinito, impedindo que o index permaneça o mesmo.
                index = index == end ? end + 1 : end;
                lines++;
            }

            return lines;
        }

        /// <summary>
        /// Calcula a altura que o texto vai ocupar.
        /// </summary>
        /// <param name="font">Fonte do texto</param>
        /// <param name="fontSize">Tamanho da fonte</param>
        /// <param name="width">Largura máxima do texto</param>
        /// <param name="text">Texto</param>
        /// <returns>Altura que o texto vai ocupar.</returns>
        internal static double GetTextHeight(org.pdfclown.documents.contents.fonts.Font font, double fontSize, double width, string text)
        {
            int lines = CountTextLines(font, fontSize, width, text);
            double height = font.GetLineHeight(fontSize) * lines;
            return height;
        }

        /// <summary>
        /// Monta um Xml de processamento da NFe apartir do arquivo enviado e recebido ao Sefaz.
        /// </summary>
        /// <param name="xmlEnvio">Xml enviado ao Sefaz</param>
        /// <param name="xmlRetorno">Xml recebido do Sefaz</param>
        /// <returns>Xml de processamento da Nfe</returns>
        public static string MontarNfeProc(string xmlEnvio, string xmlRetorno)
        {
            if (string.IsNullOrWhiteSpace(xmlEnvio)) throw new ArgumentException("Argumento inválido.", "xmlEnvio");
            if (string.IsNullOrWhiteSpace(xmlRetorno)) throw new ArgumentException("Argumento inválido.", "xmlRetorno");

            try
            {
                XDocument envio = XDocument.Parse(xmlEnvio, LoadOptions.PreserveWhitespace);
                XElement nfe = envio.Document.Root.Element(XName.Get("NFe", Namespaces.NFe));

                if(nfe == null)
                {
                    throw new Exception("O elemento NFe não foi encontrado.");
                }

                string versaoNfe = nfe.Element(XName.Get("infNFe", Namespaces.NFe)).Attribute(XName.Get("versao")).Value;

                XDocument retorno = XDocument.Parse(xmlRetorno, LoadOptions.PreserveWhitespace);

                XElement protNfe = retorno.Document.Root.Element(XName.Get("protNFe", Namespaces.NFe));

                if (protNfe == null)
                {
                    throw new Exception("O elemento protNfe não foi encontrado.");
                }

                string versaoProtNfe = protNfe.Attribute(XName.Get("versao")).Value;

                if (versaoNfe != versaoProtNfe)
                {
                    throw new Exception(string.Format("versaoNfe:{0} != versaoProtNfe:{1}", versaoNfe, versaoProtNfe));
                }

                XDocument nfeProc = new XDocument();

                XElement nfeProcElement = new XElement(XName.Get("nfeProc", Namespaces.NFe));
                nfeProcElement.SetAttributeValue("versao", versaoNfe);
                nfeProcElement.Add(nfe);
                nfeProcElement.Add(protNfe);

                nfeProc.Add(nfeProcElement);

                return nfeProc.ToString();
            }
            catch (Exception e)
            {
                throw new Exception("Falha ao montar NfeProc.", e);
            }
        }
    }
}
