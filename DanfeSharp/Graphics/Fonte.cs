using pcf = org.pdfclown.documents.contents.fonts;
using System;

namespace DanfeSharp.Graphics
{
    internal class Fonte
    {
        private float _Tamanho;

        public pcf.Font FonteInterna { get; private set; }
        public float Tamanho
        {
            get => _Tamanho;
            set
            {
                if (value <= 0) throw new InvalidOperationException("O tamanho deve ser maior que zero.");
                _Tamanho = value;
            }
        }

        public Fonte(pcf.Font font, float tamanho)
        {
            FonteInterna = font ?? throw new ArgumentNullException(nameof(font));
            Tamanho = tamanho;
        }

        public float MedirLarguraTexto(String str) => (float)FonteInterna.GetWidth(str, Tamanho).ToMm();
        public float MedirLarguraChar(char c) => (float)FonteInterna.GetWidth(c, Tamanho).ToMm();

        public float AlturaLinha => (float)FonteInterna.GetLineHeight(Tamanho).ToMm();


    }
}
