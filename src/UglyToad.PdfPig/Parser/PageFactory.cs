﻿namespace UglyToad.PdfPig.Parser
{
    using System;
    using System.Collections.Generic;
    using Annotations;
    using Content;
    using Core;
    using Filters;
    using Geometry;
    using Graphics;
    using Graphics.Operations;
    using Outline.Destinations;
    using Tokenization.Scanner;
    using Tokens;

    /// <summary>
    /// a factory to create pages
    /// 
    /// </summary>
    public class PageFactory : BasePageFactory<Page>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="pdfScanner"></param>
        /// <param name="resourceStore"></param>
        /// <param name="filterProvider"></param>
        /// <param name="pageContentParser"></param>
        /// <param name="parsingOptions"></param>
        public PageFactory(
            IPdfTokenScanner pdfScanner,
            IResourceStore resourceStore,
            ILookupFilterProvider filterProvider,
            IPageContentParser pageContentParser,
            ParsingOptions parsingOptions)
            : base(pdfScanner, resourceStore, filterProvider, pageContentParser, parsingOptions)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="dictionary"></param>
        /// <param name="namedDestinations"></param>
        /// <param name="mediaBox"></param>
        /// <param name="cropBox"></param>
        /// <param name="userSpaceUnit"></param>
        /// <param name="rotation"></param>
        /// <param name="initialMatrix"></param>
        /// <param name="operations"></param>
        /// <returns></returns>
        /// parse operations one after one
        protected override Page ProcessPage(int pageNumber,
            DictionaryToken dictionary,
            NamedDestinations namedDestinations,
            MediaBox mediaBox,
            CropBox cropBox,
            UserSpaceUnit userSpaceUnit,
            PageRotationDegrees rotation,
            TransformationMatrix initialMatrix,
            IReadOnlyList<IGraphicsStateOperation> operations)
        {
            var annotationProvider = new AnnotationProvider(PdfScanner,
                dictionary,
                initialMatrix,
                namedDestinations,
                ParsingOptions.Logger);

            if (operations.Count == 0)
            {
                PageContent emptyContent = new PageContent(
                    Array.Empty<IGraphicsStateOperation>(),
                    Array.Empty<Letter>(),
                    Array.Empty<PdfPath>(),
                    Array.Empty<Union<XObjectContentRecord, InlineImage>>(),
                    Array.Empty<MarkedContentElement>(),
                    PdfScanner,
                    FilterProvider,
                    ResourceStore);

                return new Page(pageNumber,
                    dictionary,
                    mediaBox,
                    cropBox,
                    rotation,
                    emptyContent,
                    annotationProvider,
                    PdfScanner);
            }

            var context = new ContentStreamProcessor(
                pageNumber,
                ResourceStore,
                PdfScanner,
                PageContentParser,
                FilterProvider,
                cropBox,
                userSpaceUnit,
                rotation,
                initialMatrix,
                ParsingOptions);

            PageContent content = context.Process(pageNumber, operations);

            return new Page(pageNumber,
                dictionary,
                mediaBox,
                cropBox,
                rotation,
                content,
                annotationProvider,
                PdfScanner);
        }
    }
}
