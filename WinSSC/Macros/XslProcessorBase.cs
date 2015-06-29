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
    /// Provides common logic for XSL driven processors
    /// </summary>
    public abstract class XslProcessorBase
    {

        protected AttributeSet validAttributes;
        protected XslCompiledTransform transform;
        protected IList<GroupingCriterion> groupingCriteria;

        public XslProcessorBase(AttributeSet validAttributes, string stylesheetPath)
        {
            this.validAttributes = validAttributes;
            groupingCriteria = new List<GroupingCriterion>();
            if (System.IO.File.Exists(stylesheetPath))
            {
                transform = new XslCompiledTransform();

                try
                {
                    transform.Load(stylesheetPath);
                }
                catch (Exception ex)
                {
                    Logger.LogError("Could not load macro " + stylesheetPath + ":\r\n" + ex.Message);
                    transform = null;
                }

                //See if we have a compoanion file specifying attributes to group on
                string dir = Path.GetDirectoryName(stylesheetPath);
                if (dir.Length > 0 && !dir.EndsWith("\\")) dir += "\\";
                string companionPath = dir + Path.GetFileNameWithoutExtension(stylesheetPath) + ".groups";
                if (File.Exists(companionPath))
                {
                    //A grouping criteria file has been found. Load it up.
                    string[] criteria = File.ReadAllLines(companionPath);
                    foreach (string l in criteria)
                    {
                        GroupingCriterion criterion = new GroupingCriterion();
                        criterion.Sort = SortType.None;
                        string[] parts = l.Split(" \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 0) continue;

                        //See if first section contains a valid attribute name. If so, use it
                        if (validAttributes.AttributeDefinitions.Any(x => x.Name == parts[0]))
                        {
                            criterion.AttributeName = parts[0];
                        }
                        else
                        {
                            Logger.LogWarning("Macro " + Path.GetFileName(companionPath) + " includes non-existant attribute " + l);
                            continue;
                        }

                        //Set sort order, if present
                        if (parts.Length > 1)
                        {
                            parts[1] = parts[1].ToLower();
                            switch (parts[1])
                            {
                                case "asc":
                                    criterion.Sort = SortType.Ascending;
                                    break;
                                case "ascending":
                                    criterion.Sort = SortType.Ascending;
                                    break;
                                case "desc":
                                    criterion.Sort = SortType.Descending;
                                    break;
                                case "descending":
                                    criterion.Sort = SortType.Descending;
                                    break;
                            }
                        }
                        groupingCriteria.Add(criterion);
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("Could not find stylesheet " + stylesheetPath);
            }
        }

        /// <summary>
        /// Constructs an XML document listing articles, optionally grouped by particular fields.
        /// </summary>
        /// <param name="articles"></param>
        /// <param name="groupingAttribute"></param>
        /// <param name="activeArticle"></param>
        /// <returns></returns>
        public XmlDocument BuildArticleTree(IList<ArticleDto> articles, GroupingCriterion[] groupingAttribute, ArticleDto activeArticle)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Articles");
            doc.AppendChild(root);

            processLayer(articles, activeArticle, groupingAttribute, 0, true, root, doc);

            return doc;
        }

        private void processLayer(IList<ArticleDto> articles, ArticleDto activeArticle, GroupingCriterion[] groupingAttribute, int attrIndex, bool includeNulls, XmlNode parent, XmlDocument doc)
        {
            if (attrIndex < groupingAttribute.Length)
            {
                //Keep splitting
                Dictionary<string, IList<ArticleDto>> buckets = assignArticleBuckets(articles, groupingAttribute[attrIndex], includeNulls);
                foreach (var pair in buckets)
                {
                    XmlNode groupNode = doc.CreateElement("Group");
                    parent.AppendChild(groupNode);
                    XmlAttribute attrAttr = doc.CreateAttribute("Attribute");
                    attrAttr.Value = groupingAttribute[attrIndex].AttributeName;
                    groupNode.Attributes.Append(attrAttr);
                    XmlAttribute valueAttr = doc.CreateAttribute("Value");
                    valueAttr.Value = pair.Key;
                    groupNode.Attributes.Append(valueAttr);

                    //Recursively call this function to process next level of tree
                    processLayer(pair.Value, activeArticle, groupingAttribute, attrIndex + 1, includeNulls, groupNode, doc);
                }
            }
            else
            {
                //Sort articles
                if (validAttributes.AttributeDefinitions.Any(x => x.Name.ToLower() == "title" && x.DataType == EAttributeType.String))
                {
                    //TODO:Extract this into something more flexible. This is some awful magic...
                    AttributeDef titleAttr = validAttributes.AttributeDefinitions.First(x => x.Name.ToLower() == "title");
                    articles = new List<ArticleDto>((IEnumerable<ArticleDto>)articles.OrderBy(x => x.Attributes[titleAttr.Name].FirstOrDefault()));
                }

                //Write leaf nodes
                foreach (ArticleDto a in articles)
                {
                    XmlNode articleNode = doc.CreateElement("Article");
                    parent.AppendChild(articleNode);

                    if (a == activeArticle)
                    {
                        XmlAttribute activeNode = doc.CreateAttribute("Active");
                        activeNode.Value = "true";
                        articleNode.Attributes.Append(activeNode);
                    }

                    foreach (AttributeDef ad in validAttributes.AttributeDefinitions)
                    {
                        XmlNode dataNode = doc.CreateElement(ad.Name);
                        articleNode.AppendChild(dataNode);
                        if (a.Attributes.ContainsKey(ad.Name))
                        {
                            if (ad.DataType == EAttributeType.StringArray)
                            {
                                foreach (string s in a.Attributes[ad.Name])
                                {
                                    XmlNode valNode = doc.CreateElement("Value");
                                    valNode.AppendChild(doc.CreateTextNode(s));
                                    dataNode.AppendChild(valNode);
                                }
                            }
                            else
                            {
                                dataNode.AppendChild(doc.CreateTextNode(a.Attributes[ad.Name][0]));
                            }
                        }
                    }
                }
            }
        }

        private Dictionary<string, IList<ArticleDto>> assignArticleBuckets(IList<ArticleDto> articles, GroupingCriterion criterion, bool includeNulls)
        {
            Dictionary<string, IList<ArticleDto>> buckets = new Dictionary<string, IList<ArticleDto>>();

            foreach (ArticleDto a in articles)
            {
                if (a.Attributes.ContainsKey(criterion.AttributeName))
                {
                    foreach (string val in a.Attributes[criterion.AttributeName])
                    {
                        if (!buckets.ContainsKey(val))
                        {
                            buckets.Add(val, new List<ArticleDto>());
                        }
                        buckets[val].Add(a);
                    }
                }
                else if (includeNulls)
                {
                    if (!buckets.ContainsKey(""))
                    {
                        buckets.Add("", new List<ArticleDto>());
                    }
                    buckets[""].Add(a);
                }
            }

            var grouped = buckets.OrderBy(x => x.Key, new invertableStringConparer(criterion.Sort == SortType.Descending));

            buckets = new Dictionary<string, IList<ArticleDto>>();
            foreach (var bucketElem in grouped)
            {
                buckets.Add(bucketElem.Key, new List<ArticleDto>(bucketElem.Value));
            }

            return buckets;
        }

        /// <summary>
        /// Stores attribute grouping criteria informaton
        /// </summary>
        public struct GroupingCriterion
        {
            public string AttributeName;
            public SortType Sort;
        }

        public enum SortType
        {
            None,
            Ascending,
            Descending
        }

        /// <summary>
        /// Simple string comparer that allows for optional reverse sorting
        /// </summary>
        protected class invertableStringConparer : IComparer<string>
        {
            private bool invert;
            public invertableStringConparer(bool invert)
            {
                this.invert = invert;
            }

            public int Compare(string x, string y)
            {
                if (invert) return -x.CompareTo(y);
                return x.CompareTo(y);
            }
        }
    }
}
