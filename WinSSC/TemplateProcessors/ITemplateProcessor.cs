//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC.TemplateProcessors
{
    public interface ITemplateProcessor
    {
        /// <summary>
        /// Processes th given raw text (from a file) and returns an itialised TemplateDto.
        /// </summary>
        /// <param name="templateText"></param>
        /// <returns></returns>
        TemplateDto ParseTemplateRawText(string templateText, string relativePath, string name);

        /// <summary>
        /// Locates macro invocations within the template text of an article.
        /// </summary>
        /// <param name="article">The temaplted article whose template text needs to be scanned for macros.</param>
        /// <returns>A list of macro invocations in the template text.</returns>
        IList<MacroInvocation> LocateMacrosInArticleTemplateContent(ArticleDto article);

        /// <summary>
        /// Rraps the existing article content with the expanded template content (on the article). The output is stored within the article content, 
        /// so it is then ready for writing to the output file.
        /// </summary>
        /// <param name="article">The article to be wrapped.</param>
        /// <param name="template">The template that the article will be wrapped in. Must have been created by this TemplateProcessor.</param>
        void WrapArticle(ArticleDto article);

        /// <summary>
        /// The main file extension for the templates that can be read by the TemplateProcessor, including the dot/period.
        /// </summary>
        /// <remarks>
        /// Thi item is not case sensitive, and should include a dot/period.
        /// E.g. ".mdt" for a MarkDown template.
        /// It is recommended for the sake of usability that the extension for a template be <i>different</i> to that for an article, even if the underlying format is the same.
        /// If an ArticleProcessor needs multiple file types, this should return a "root" extension that refers to the main template data,
        /// and derive other paths within its own logic.
        /// </remarks>
        string PrimaryFileExtension { get; }
    }
}
