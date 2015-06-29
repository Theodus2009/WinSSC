using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC.VirtualArticleGenerators
{
    /// <summary>
    /// 
    /// </summary>
    public interface IVirtualArticleGeneratorLoader
    {
        /// <summary>
        /// Creates a new article generator instance.
        /// </summary>
        /// <param name="sourceText"></param>
        /// <param name="filePath">Path to the file.</param>
        /// <returns></returns>
        IVirtualArticleGenerator ParseGeneratorFromFile(string filePath);

        /// <summary>
        /// The main file extension for virtual article specifications that can be read by the ArticleProcessor, including the dot/period.
        /// </summary>
        /// <remarks>
        /// Thi item is not case sensitive, and should include a dot/period.
        /// E.g. ".xsl2md" for an XSL driven markdown article generator.
        /// If a VirtualArticleProcessor needs multiple file types, this should return a "root" extension that refers to the main article data,
        /// and derive other paths within its own logic.
        /// </remarks>
        string PrimaryFileExtension { get; }
    }
}
