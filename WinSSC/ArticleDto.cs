using WinSSC.ArticleProcessors;
using WinSSC.TemplateProcessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC
{
    /// <summary>
    /// Represents a single article to be converted.
    /// </summary>
    public class ArticleDto
    {
        public ArticleDto(string relativeFilePath, IArticleProcessor reader)
        {
            SourceFileRelativePath = relativeFilePath;
            ArticleProcessorUsed = reader;
        }

        /// <summary>
        /// Textual content of the article. This must be text, but the specifics of the text need only be understood by the relevant IArticleProcessor.
        /// </summary>
        public string Content;

        /// <summary>
        /// Stores a copy of the the template content for the article. This is required because the macros will generate different output 
        /// 
        /// </summary>
        public string TemplateContent;

        /// <summary>
        /// Path to the original source file, relative to the content root specified in the config file.
        /// </summary>
        public string SourceFileRelativePath;

        /// <summary>
        /// The relative path that the article will reside on. May be based on the SourceFileRelativePath or may be aribtrary.
        /// </summary>
        public string OutputPath;

        /// <summary>
        /// Contains attributes found in an article's front-matter.
        /// </summary>
        public IDictionary<string, IList<string>> Attributes = new Dictionary<string, IList<string>>();

        /// <summary>
        /// Holds a reference to the reader that loaded the article.
        /// </summary>
        /// <remarks>
        /// This has been added to allow for varying types of artcle input data in future.
        /// </remarks>
        public IArticleProcessor ArticleProcessorUsed;

        /// <summary>
        /// Contains a reference to the TemplateProcessor that will be used to wrap this article, if a template was specified.
        /// Will be null if no template is to be applied.
        /// </summary>
        public TemplateDto Template = null;

         
    }
}
