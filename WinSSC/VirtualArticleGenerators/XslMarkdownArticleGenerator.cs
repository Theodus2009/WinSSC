//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing WinSSC.Macros;
using WinSSC.ArticleProcessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace WinSSC.VirtualArticleGenerators
{
    /// <summary>
    /// Uses an XSL transform to generate article content in Markdown format.
    /// </summary>
    public class XslMarkdownArticleGenerator: XslProcessorBase, IVirtualArticleGenerator
    {
        public string Name { get; set; }

        public XslMarkdownArticleGenerator (AttributeSet validAttributes, string stylesheetPath):base(validAttributes, stylesheetPath)
        {
            Name = "";
        }

        /// <summary>
        /// Generates Markdown articles using an XSL transformation that returns Article nodes containing a Content node, which in turns contains markdown text.
        /// </summary>
        /// <param name="concreteArticles">The articles loaded from disk.</param>
        /// <returns></returns>
        public IEnumerable<ArticleDto> GenerateArticles(IList<ArticleDto> concreteArticles)
        {
            MarkdownArticleProcessor artProce = new MarkdownArticleProcessor(validAttributes);
            List<ArticleDto> generatedArticles = new List<ArticleDto>();

            try
            {
                XmlDocument outputDoc = new XmlDocument();
                XmlDocument inputDoc = BuildArticleTree(concreteArticles, groupingCriteria.ToArray(), null);
                using (XmlWriter writer = outputDoc.CreateNavigator().AppendChild())  
                {
                    transform.Transform(inputDoc, writer);
                }

                XmlNodeList articles = outputDoc.SelectNodes("//Article");

                int index = 1;
                foreach(XmlNode articleNode in articles)
                {
                    XmlNode contentNode = articleNode.SelectSingleNode("Content");
                    if(contentNode != null)
                    {
                        string content = contentNode.InnerText;
                        try
                        {
                            generatedArticles.Add(artProce.ParseArticleRawText(content, "Virtual~" + index));
                        }
                        catch(Exception ex)
                        {
                            Logger.LogError("Unable to parse virtual Markdown article: " + ex.Message);
                            Logger.LogDebug(content);
                        }
                    }
                    index++;
                }
            }
            catch(XmlException ex)
            {
                Logger.LogError("Error on virtual article generator " + Name + ": " + ex.Message);
            }

            return generatedArticles;
        }
    }
}
