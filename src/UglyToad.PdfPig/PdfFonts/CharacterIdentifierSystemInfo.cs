namespace UglyToad.PdfPig.PdfFonts
{
    using System.Globalization;
    using CidFonts;

    /// <summary>
    /// Specifies the character collection associated with the <see cref="ICidFont"/> (CIDFont).
    /// </summary>
    public readonly struct CharacterIdentifierSystemInfo
    {
        /// <summary>
        /// Identifies the issuer of the character collection.
        /// </summary>
        public string Registry { get; }

        /// <summary>
        /// Uniquely identifies the character collection within the parent registry.
        /// </summary>
        public string Ordering { get; }

        /// <summary>
        /// The supplement number of the character collection.
        /// </summary>
        public int Supplement { get; }

        /// <summary>
        /// Create a new <see cref="CharacterIdentifierSystemInfo"/>.
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="ordering"></param>
        /// <param name="supplement"></param>
        public CharacterIdentifierSystemInfo(string registry, string ordering, int supplement)
        {
            Registry = registry;
            Ordering = ordering;
            Supplement = supplement;
        }

        public override string ToString()
        {
            return $"{Registry}-{Ordering}-{Supplement.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}