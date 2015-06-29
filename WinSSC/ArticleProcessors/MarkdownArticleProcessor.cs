//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing MarkdownSharp;
using WinSSC.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC.ArticleProcessors
{
    /// <summary>
    /// Provides functions to read a StaticCms article file, which is esstially just a Markdown text file with some extra metadata at the top.
    /// </summary>
    public class MarkdownArticleProcessor: IArticleProcessor
    {
        AttributeSet attrs;

        public string PrimaryFileExtension
        {
            get
            {
                return ".md";
            }
        }

        public MarkdownArticleProcessor(AttributeSet attributeDefs) 
        {
            attrs = attributeDefs;
        }

        /// <summary>
        /// Processes article text to produce data structures for further processing.
        /// </summary>
        /// <param name="sourceText">The text to </param>
        /// <param name="relativePath">Used for error reporting and creation of indexes later on.</param>
        /// <returns></returns>
        public ArticleDto ParseArticleRawText(string sourceText, string relativePath)
        {
            ArticleDto article = new ArticleDto(relativePath, this);
            IDictionary<string, IList<string>> attributes;
            article.Content = WinSSC.Util.MarkdownUtil.ParseMarkdown(sourceText, relativePath, attrs, out attributes);
            article.Attributes = attributes;
            article.SourceFileRelativePath = relativePath;
            return article;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        /// <see cref="WinSSC.Util.MarkdownUtil.LocateMarkdownMacros"/>
        public IList<MacroInvocation> LocateMacrosInContent(ArticleDto article)
        {
            return WinSSC.Util.MarkdownUtil.LocateMarkdownMacros(article.Content);
        }

        public void FinaliseArticleHtml(ArticleDto article)
        {
            Markdown md = new Markdown();
            article.Content = MarkdownUtil.ReplaceEscapedMacros(article.Content);
            article.Content = md.Transform(article.Content);
        }
    }
}
