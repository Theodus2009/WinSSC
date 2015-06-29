
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC.VirtualArticleGenerators
{
    /// <summary>
    /// A virtual article processor is able to generate new ArticleDtos by doing a transofmration 
    /// on existing article metadata, rather than reading article content from a file on disk.
    /// 
    /// This is designed to enable capabilities such as a generated category listing page for each category on a site.
    /// </summary>
    public interface IVirtualArticleGenerator
    {
        string Name { get; set; }

        /// <summary>
        /// Creates new ArticleDtos from the data contained in existing articles.
        /// </summary>
        /// <param name="concreteArticles">A list of all ArticleDtos for articles that have been loaded from disk.</param>
        /// <returns></returns>
        /// <remarks>
        /// The input list of ArticleDto is limited to articles loaded from disk to isolate multiple virtual article processors from each other.
        /// </remarks>
        IEnumerable<ArticleDto> GenerateArticles(IList<ArticleDto> concreteArticles);


    }
}
