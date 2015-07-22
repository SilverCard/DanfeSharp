using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DanfeSharp
{
    /// <summary>
    /// Essa classe contém métodos de extenção
    /// </summary>
    public static class Extentions
    {

        public static RectangleF GetPaddedRectangleMm(this RectangleF rect, float padding)
        {
            padding = Utils.Mm2Pu(padding);
            return rect.GetPaddedRectangle(padding, padding, padding, padding);
        }

        public static RectangleF GetPaddedRectangleMm(this RectangleF rect, float pLeft, float pRight, float pTop, float pBottom)
        {
            return rect.GetPaddedRectangle(Utils.Mm2Pu(pLeft), Utils.Mm2Pu(pRight), Utils.Mm2Pu(pTop), Utils.Mm2Pu(pBottom));
        }

        public static RectangleF GetPaddedRectangleMm(this RectangleF rect, float vertical, float horizontal)
        {
            horizontal = Utils.Mm2Pu(horizontal);
            vertical = Utils.Mm2Pu(vertical);
            return rect.GetPaddedRectangle(horizontal, horizontal, vertical, vertical);
        }


        /// <summary>
        /// Pega um retângulo interno com determinado padding.
        /// </summary>
        /// <param name="rect">Retângulo.</param>
        /// <param name="padding">Padding.</param>
        /// <returns>Retângulo interno.</returns>
        public static RectangleF GetPaddedRectangle(this RectangleF rect, float padding)
        {
            return rect.GetPaddedRectangle(padding, padding, padding, padding);
        }

        public static RectangleF GetPaddedRectangle(this RectangleF rect, float vertical, float horizontal)
        {
            return rect.GetPaddedRectangle(horizontal, horizontal, vertical, vertical);
        }

        public static RectangleF GetPaddedRectangle(this RectangleF rect, float pLeft, float pRight, float pTop, float pBottom )
        {
            RectangleF rect2 = new RectangleF(rect.X + pLeft, rect.Y + pTop, rect.Width - pLeft - pRight, rect.Height - pTop - pBottom);
            return rect2;
        }

        public static RectangleF GetLeftRectangle(this RectangleF rect, float width)
        {
            return new RectangleF(rect.Left - width, rect.Y, width, rect.Height);
        }

        public static RectangleF GetRightRectangle(this RectangleF rect, float width)
        {
            return new RectangleF(rect.Right + width, rect.Y, width, rect.Height);
        }

        /// <summary>
        /// Verifica se a posição e os tamanhos do retângulo possuem um valor negativo.
        /// </summary>
        /// <param name="rect">Retânngulo.</param>
        public static Boolean IsNegative(this RectangleF rect)
        {
            if(rect.X < 0 || rect.Y < 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Divide um retângulo em n partes iguais.
        /// </summary>
        /// <param name="rect">Retângulo a ser dividido</param>
        /// <param name="n">Número de partes</param>
        /// <returns>Divisões do retângulo</returns>
        public static RectangleF[] SplitRectangle(this RectangleF rect, int n)
        {
            RectangleF[] result = new RectangleF[n];

            float w = rect.Width / (float)n;

            result[0] = rect;
            result[0].Width = w;

            for (int i = 1; i < n; i++)
            {
                result[i] = result[0];
                result[i].X = result[i - 1].Right;
            }

            return result;
        }

        [DebuggerStepThrough] 
        private static void VerificarRetangulo(RectangleF frame, SizeF size)
        {
            if (frame.IsNegative())
            {
                throw new ArgumentException("O retângulo não é válido.", "frame");
            }

            if (frame.Y + frame.Height > size.Height ||
              frame.X + frame.Width > size.Width)
            {
                throw new ArgumentException("O retângulo esta posicionado fora da área desenhável.", "frame");
            }
        }

        /// <summary>
        /// Extende a funcionalidade do Begin, adicionando uma verificação do retângulo.
        /// </summary>
        [DebuggerStepThrough] 
        internal static void SafeBegin(this org.pdfclown.documents.contents.composition.BlockComposer blockComposer, RectangleF frame,
            org.pdfclown.documents.contents.composition.XAlignmentEnum xAlignment = org.pdfclown.documents.contents.composition.XAlignmentEnum.Left,
            org.pdfclown.documents.contents.composition.YAlignmentEnum yAlignment = org.pdfclown.documents.contents.composition.YAlignmentEnum.Top)
        {
            VerificarRetangulo(frame, blockComposer.BaseComposer.Scanner.CanvasSize);
            blockComposer.Begin(frame, xAlignment, yAlignment);
        }

        /// <summary>
        /// Estende a funcionalidade do DrawRectangle, adicionando uma verificação do retângulo.
        /// </summary>
        [DebuggerStepThrough] 
        internal static void SafeDrawRectangle(this org.pdfclown.documents.contents.composition.PrimitiveComposer composer, RectangleF frame)
        {
            VerificarRetangulo(frame, composer.Scanner.CanvasSize);
            composer.DrawRectangle(frame);
        }
    }
}
