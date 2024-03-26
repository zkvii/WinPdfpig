namespace UglyToad.PdfPig.PdfFonts
{
    using Tokens;

    /// <summary>
    /// A factory for creating <see cref="IFont"/> instances from dictionaries.
    /// </summary>
    public interface IFontFactory
    {
        /// <summary>
        /// Get the font from the dictionary.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        IFont Get(DictionaryToken dictionary);
        
    }
}