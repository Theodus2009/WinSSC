//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing WinSSC.Macros;

using WinSSC.ArticleProcessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using WinSSC.Macros;

namespace WinSSC.VirtualArticleGenerators
{
    /// <summary>
    /// Uses an XSL transform to generate article content in Markdown format.
    /// </summary>
    public class XslArticleGenerator: XslProcessorBase, IVirtualArticleGenerator
    {
        public string Name { get; set; }

        public XslArticleGenerator (AttributeSet validAttributes, string stylesheetPath):base(validAttributes, stylesheetPath)
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
                    XmlNode pathNode = articleNode.SelectSingleNode("Path");
                    XmlNode titleNode = articleNode.SelectSingleNode("Title");
                    if (contentNode != null && pathNode != null)
                    {
                        string content = contentNode.InnerText;
                        try
                        {
                            string path;
                            path = pathNode.InnerText;
                              
                            ArticleDto art = new ArticleDto(pathNode.InnerText, new PureTextArticleProcessor());
                            art.Attributes.Add("Title", new List<string>());

                            if(titleNode != null)
                            {
                                art.Attributes["Title"].Add(titleNode.InnerText);
                            }
                            else
                            {
                                art.Attributes["Title"].Add("");
                            }

                            art.SourceFileRelativePath = path;
                            art.OutputPath = path;
                            art.Attributes.Add("Path", new List<string>());
                            art.Attributes["Path"].Add(path);

                            art.Content = content;

                            generatedArticles.Add(art);
                        }
                        catch(Exception ex)
                        {
                            Logger.LogError("Unable to parse virtual Markdown article: " + ex.Message);
                            Logger.LogDebug(content);
                        }
                    }
                    else
                    {
                        Logger.LogWarning("Either no Path or no Content node specified for article " + index + " in " + Name);
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
