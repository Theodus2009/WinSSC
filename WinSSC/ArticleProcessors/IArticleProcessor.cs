//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC.ArticleProcessors
{
    public interface IArticleProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceText">Raw text that has been read from an article file.</param>
        /// <param name="relativePath">The relative path of the source file from the article root directory.</param>
        /// <returns>An initialised ArticleDto.</returns>
        ArticleDto ParseArticleRawText(string sourceText, string relativePath);

        /// <summary>
        /// Locates macros in the content text of an article.
        /// </summary>
        /// <param name="article"></param>
        /// <returns>A list of macro invocations in the larticle content.</returns>
        IList<MacroInvocation> LocateMacrosInContent(ArticleDto article);

        /// <summary>
        /// This is when the main transformation from source markup to HTML will occur, if required to be separate to loading.
        /// This is called after macro expansion has occurred.
        /// </summary>
        /// <param name="article">The article to have its content prepared.</param>
        void FinaliseArticleHtml(ArticleDto article);

        /// <summary>
        /// The main file extension for the articles that can be read by the ArticleProcessor, including the dot/period.
        /// </summary>
        /// <remarks>
        /// Thi item is not case sensitive, and should include a dot/period.
        /// E.g. ".md" for MarkDown.
        /// If an ArticleProcessor needs multiple file types, this should return a "root" extension that refers to the main article data,
        /// and derive other paths within its own logic.
        /// </remarks>
        string PrimaryFileExtension { get; }
    }
}
