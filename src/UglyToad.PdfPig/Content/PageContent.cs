namespace UglyToad.PdfPig.Content
{
    using Core;
    using Filters;
    using Graphics;
    using Graphics.Operations;
    using System;
    using System.Collections.Generic;
    using Tokenization.Scanner;
    using XObjects;

    /// <summary>
    /// Wraps content parsed from a page content stream for access.
    /// </summary>
    /// <remarks>
    /// This should contain a replayable stack of drawing instructions for page content
    /// from a content stream in addition to lazily evaluated state such as text on the page or images.
    /// </remarks>
    public class PageContent
    {
        private readonly IReadOnlyList<Union<XObjectContentRecord, InlineImage>> images;
        private readonly IReadOnlyList<MarkedContentElement> markedContents;
        private readonly IPdfTokenScanner pdfScanner;
        private readonly ILookupFilterProvider filterProvider;
        /// <summary>
        ///  The resource store for the page content. fonts file and so on
        /// </summary>
        public readonly IResourceStore PageResourceStore;

        internal IReadOnlyList<IGraphicsStateOperation> GraphicsStateOperations { get; }

        /// <summary>
        /// The letters on the page.
        /// </summary>
        public IReadOnlyList<Letter> Letters { get; }

        /// <summary>
        /// The paths on the page.
        /// </summary>
        public IReadOnlyList<PdfPath> Paths { get; }

        /// <summary>
        /// The number of images on the page.
        /// </summary>
        public int NumberOfImages => images.Count;

        internal PageContent(IReadOnlyList<IGraphicsStateOperation> graphicsStateOperations, IReadOnlyList<Letter> letters,
            IReadOnlyList<PdfPath> paths,
            IReadOnlyList<Union<XObjectContentRecord, InlineImage>> images,
            IReadOnlyList<MarkedContentElement> markedContents,
            IPdfTokenScanner pdfScanner,
            ILookupFilterProvider filterProvider,
            IResourceStore resourceStore)
        {
            GraphicsStateOperations = graphicsStateOperations;
            Letters = letters;
            Paths = paths;
            this.images = images;
            this.markedContents = markedContents;
            this.pdfScanner = pdfScanner ?? throw new ArgumentNullException(nameof(pdfScanner));
            this.filterProvider = filterProvider ?? throw new ArgumentNullException(nameof(filterProvider));
            this.PageResourceStore = resourceStore ?? throw new ArgumentNullException(nameof(resourceStore));
        }

        /// <summary>
        /// Get the images on the page.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IPdfImage> GetImages()
        {
            foreach (var image in images)
            {
                if (image.TryGetFirst(out var xObjectContentRecord))
                {
                    yield return XObjectFactory.ReadImage(xObjectContentRecord, pdfScanner, filterProvider, PageResourceStore);
                }
                else if (image.TryGetSecond(out var inlineImage))
                {
                    yield return inlineImage;
                }
            }
        }

        /// <summary>
        /// Get the marked content elements on the page.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<MarkedContentElement> GetMarkedContents() => markedContents;
    }
}
