using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.IO;

namespace WinSSC.Macros
{
    /// <summary>
    /// Allows a XSL macro to be stored and executed. XSL macros get access to data that Markdown macros don't, as they are
    /// passed an XML document containing the entire article tree, organised by specified grouping criteria.
    /// </summary>
    public class XslMacro: XslProcessorBase, IMacro
    {
        public string Name { get; set; }
  
        public XslMacro(AttributeSet validAttributes, string stylesheetPath):base(validAttributes, stylesheetPath)
        {
            //Nothing to do - it is the base class that needs initialising
        }

        public virtual string Expand(IList<string> parms, IList<string> vals, ArticleDto activeArticle, IList<ArticleDto> allArticles)
        {
            if (transform == null) return "";

            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            try
            {
                XmlDocument articleTree = BuildArticleTree(allArticles, groupingCriteria.ToArray(), activeArticle);
                if(parms.Count > 0 || vals.Count > 0)
                {
                    //Add a parms node to the XML document as a L2 element.
                    XmlNode parmsNode = articleTree.CreateElement("MacroParms");
                    articleTree.FirstChild.AppendChild(parmsNode);
                    for(int i = 0; i < parms.Count; i++)
                    {
                        XmlNode pNode = articleTree.CreateElement("p" + (i + 1));
                        parmsNode.AppendChild(pNode);
                        pNode.InnerText = parms[i];
                    }
                    for (int i = 0; i < vals.Count; i++)
                    {
                        XmlNode pNode = articleTree.CreateElement("v" + (i + 1));
                        parmsNode.AppendChild(pNode);
                        pNode.InnerText = vals[i];
                    }
                }
                transform.Transform(articleTree, new XsltArgumentList(), writer);
                if(Logger.DebugOn)
                {
                    Logger.LogDebug("Data for macro " + Name + ":\r\n" + articleTree.Serialize());
                }
            }
            catch(System.Xml.Xsl.XsltException ex)
            {
                Logger.LogError("Could not apply transformation on XSL macro " + Name + ":\r\n" + ex.Message);
            }
            return sb.ToString();
        }
    }
}
