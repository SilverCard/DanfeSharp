using org.pdfclown.documents.contents.composition;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace DanfeSharp
{
    /// <summary>
    /// Classes para gerar o código de barras Code 128C 
    /// </summary>
    internal class Barcode128C
    {
        private static byte[][] Dic;

        public static readonly float MargemVertical = Utils.Mm2Pu(1);
        public static readonly float MargemHorizontal = Utils.Mm2Pu(4);

        /// <summary>
        /// Tamanho do código de barras, inclui as margens.
        /// </summary>
        public SizeF Size { get; private set; }

        /// <summary>
        /// Código a ser codificado em barras.
        /// </summary>
        public String Code { get; private set; }

        static Barcode128C()
        {
            
            Dic = new byte[][]
            {
                new byte[] { 2,1,2,2,2,2},
                new byte[] { 2,2,2,1,2,2},
                new byte[] { 2,2,2,2,2,1},
                new byte[] { 1,2,1,2,2,3},
                new byte[] { 1,2,1,3,2,2},
                new byte[] { 1,3,1,2,2,2},
                new byte[] { 1,2,2,2,1,3},
                new byte[] { 1,2,2,3,1,2},
                new byte[] { 1,3,2,2,1,2},
                new byte[] { 2,2,1,2,1,3},
                new byte[] { 2,2,1,3,1,2},
                new byte[] { 2,3,1,2,1,2},
                new byte[] { 1,1,2,2,3,2},
                new byte[] { 1,2,2,1,3,2},
                new byte[] { 1,2,2,2,3,1},
                new byte[] { 1,1,3,2,2,2},
                new byte[] { 1,2,3,1,2,2},
                new byte[] { 1,2,3,2,2,1},
                new byte[] { 2,2,3,2,1,1},
                new byte[] { 2,2,1,1,3,2},
                new byte[] { 2,2,1,2,3,1},
                new byte[] { 2,1,3,2,1,2},
                new byte[] { 2,2,3,1,1,2},
                new byte[] { 3,1,2,1,3,1},
                new byte[] { 3,1,1,2,2,2},
                new byte[] { 3,2,1,1,2,2},
                new byte[] { 3,2,1,2,2,1},
                new byte[] { 3,1,2,2,1,2},
                new byte[] { 3,2,2,1,1,2},
                new byte[] { 3,2,2,2,1,1},
                new byte[] { 2,1,2,1,2,3},
                new byte[] { 2,1,2,3,2,1},
                new byte[] { 2,3,2,1,2,1},
                new byte[] { 1,1,1,3,2,3},
                new byte[] { 1,3,1,1,2,3},
                new byte[] { 1,3,1,3,2,1},
                new byte[] { 1,1,2,3,1,3},
                new byte[] { 1,3,2,1,1,3},
                new byte[] { 1,3,2,3,1,1},
                new byte[] { 2,1,1,3,1,3},
                new byte[] { 2,3,1,1,1,3},
                new byte[] { 2,3,1,3,1,1},
                new byte[] { 1,1,2,1,3,3},
                new byte[] { 1,1,2,3,3,1},
                new byte[] { 1,3,2,1,3,1},
                new byte[] { 1,1,3,1,2,3},
                new byte[] { 1,1,3,3,2,1},
                new byte[] { 1,3,3,1,2,1},
                new byte[] { 3,1,3,1,2,1},
                new byte[] { 2,1,1,3,3,1},
                new byte[] { 2,3,1,1,3,1},
                new byte[] { 2,1,3,1,1,3},
                new byte[] { 2,1,3,3,1,1},
                new byte[] { 2,1,3,1,3,1},
                new byte[] { 3,1,1,1,2,3},
                new byte[] { 3,1,1,3,2,1},
                new byte[] { 3,3,1,1,2,1},
                new byte[] { 3,1,2,1,1,3},
                new byte[] { 3,1,2,3,1,1},
                new byte[] { 3,3,2,1,1,1},
                new byte[] { 3,1,4,1,1,1},
                new byte[] { 2,2,1,4,1,1},
                new byte[] { 4,3,1,1,1,1},
                new byte[] { 1,1,1,2,2,4},
                new byte[] { 1,1,1,4,2,2},
                new byte[] { 1,2,1,1,2,4},
                new byte[] { 1,2,1,4,2,1},
                new byte[] { 1,4,1,1,2,2},
                new byte[] { 1,4,1,2,2,1},
                new byte[] { 1,1,2,2,1,4},
                new byte[] { 1,1,2,4,1,2},
                new byte[] { 1,2,2,1,1,4},
                new byte[] { 1,2,2,4,1,1},
                new byte[] { 1,4,2,1,1,2},
                new byte[] { 1,4,2,2,1,1},
                new byte[] { 2,4,1,2,1,1},
                new byte[] { 2,2,1,1,1,4},
                new byte[] { 4,1,3,1,1,1},
                new byte[] { 2,4,1,1,1,2},
                new byte[] { 1,3,4,1,1,1},
                new byte[] { 1,1,1,2,4,2},
                new byte[] { 1,2,1,1,4,2},
                new byte[] { 1,2,1,2,4,1},
                new byte[] { 1,1,4,2,1,2},
                new byte[] { 1,2,4,1,1,2},
                new byte[] { 1,2,4,2,1,1},
                new byte[] { 4,1,1,2,1,2},
                new byte[] { 4,2,1,1,1,2},
                new byte[] { 4,2,1,2,1,1},
                new byte[] { 2,1,2,1,4,1},
                new byte[] { 2,1,4,1,2,1},
                new byte[] { 4,1,2,1,2,1},
                new byte[] { 1,1,1,1,4,3},
                new byte[] { 1,1,1,3,4,1},
                new byte[] { 1,3,1,1,4,1},
                new byte[] { 1,1,4,1,1,3},
                new byte[] { 1,1,4,3,1,1},
                new byte[] { 4,1,1,1,1,3},
                new byte[] { 4,1,1,3,1,1},
                new byte[] { 1,1,3,1,4,1},
                new byte[] { 1,1,4,1,3,1},
                new byte[] { 3,1,1,1,4,1},
                new byte[] { 4,1,1,1,3,1},
                new byte[] { 2,1,1,4,1,2},
                new byte[] { 2,1,1,2,1,4},
                new byte[] { 2,1,1,2,3,2},
                new byte[] { 2,3,3,1,1,1,2}

            };

        }

        public Barcode128C(String code, SizeF size)
        {
            if(String.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException("O código não pode ser vazio.", "code");
            }

            if (!Regex.IsMatch(code, @"^\d+$"))
            {
                throw new ArgumentException("O código deve apenas conter digítos numéricos.", "code");
            }

            if (size.Width <=0 || size.Height <= 0)
            {
                throw new ArgumentException("O tamanho  é inválido.", "size");
            }

            if(code.Length % 2 != 0)
            {
                Code = "0" + code;
            }
            else
            {
                Code = code;
            }           

            Size = size;
        }

        public org.pdfclown.documents.contents.objects.ContentObject ToInlineObject(org.pdfclown.documents.contents.composition.PrimitiveComposer composer)
        {
            var barcodeObject = composer.BeginLocalState();

            RectangleF innerRect = new RectangleF(MargemHorizontal, MargemVertical, Size.Width - 2 * MargemHorizontal, Size.Height - 2 * MargemVertical);
            List<byte> codeBytes = new List<byte>();

            codeBytes.Add(105);

            for (int i = 0; i < this.Code.Length; i += 2)
            {
                byte b = byte.Parse(this.Code.Substring(i, 2));
                codeBytes.Add(b);
            }

            // Calcular dígito verificador
            int cd = 105;

            for (int i = 1; i < codeBytes.Count; i++)
            {
                cd += i * codeBytes[i];
                cd %= 103;
            }

            codeBytes.Add((byte)cd);
            codeBytes.Add(106);

            float n = codeBytes.Count * 11 + 2;
            float w = innerRect.Width / n;

            float x = 0;

            for (int i = 0; i < codeBytes.Count; i++)
            {
                byte[] pt = Barcode128C.Dic[codeBytes[i]];

                for (int i2 = 0; i2 < pt.Length; i2++)
                {
                    if (i2 % 2 == 0)
                    {
                        composer.SafeDrawRectangle(new RectangleF(innerRect.X + x, innerRect.Y, w * pt[i2], innerRect.Height));
                    }

                    x += w * pt[i2];
                }
            }

            composer.Fill();
            composer.End();
            return barcodeObject;
        }

        public org.pdfclown.documents.contents.xObjects.XObject ToXObject(org.pdfclown.documents.Document context)
        {
            var xObject = new org.pdfclown.documents.contents.xObjects.FormXObject(context, Size);

            PrimitiveComposer composer = new PrimitiveComposer(xObject);
            this.ToInlineObject(composer);
            composer.Flush();

            return xObject;
        }
    }
}
