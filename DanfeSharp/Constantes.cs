namespace DanfeSharp
{
    internal static class Constantes
    {
        /// <summary>
        /// Altura do campo em milímetros.
        /// </summary>
        public const float CampoAltura = 6.75F;

        /// <summary>
        /// Margem do DANFE.
        /// </summary>
        public const float Margem = 5;

        public const float A4Largura = 210;
        public const float A4Altura = 297;

        public const float LarguraDesenhavel = A4Largura - 2 * Margem;
        public const float AlturaDesenhavel = A4Altura - 2 * Margem;
    }
}
