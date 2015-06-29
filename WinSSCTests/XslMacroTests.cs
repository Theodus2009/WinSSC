//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinSSC.ArticleProcessors;
using System.Xml;
using System.Xml.Xsl;
using WinSSC;

namespace WinSSC.Macros
{
    [TestClass]
    public class XslMacroTests
    {
        private static AttributeSet getAttributeSet()
        {
            AttributeSet attrSet = new AttributeSet();
            string input = @"#A comment line
                             Title:String
                             Category:String
                             Tags:StringArray
                             Date:Date";
            attrSet.AttributeDefinitions = AttributeSet.ParseAttributeDefs(input);
            return attrSet;
        }

        private static IList<ArticleDto> getArticles()
        {
            List<ArticleDto> articles = new List<ArticleDto>();
            MarkdownArticleProcessor ar = new MarkdownArticleProcessor(getAttributeSet());
            ArticleDto a = ar.ParseArticleRawText(
@"---
Title:Xbox Controller Repair
Date:2014-08-02
Category:Electronics
Tags:Gaming,Electronics
---
![:youtube 800 600](https://www.youtube.com/embed/Di5AT4MI6BY)", "Some\\File.md");
            articles.Add(a);

            a = ar.ParseArticleRawText(
@"---
Title:Desk
Date:2014-12-03
Category:Woodwork
Tags:Carpentry,Woodwork
---
Chisels...","Some\\Desk.cd");
            articles.Add(a);
            return articles;
        }

        [TestMethod]
        public void ArticleTreeFlat()
        {
            XslMacro xm = new XslMacro(getAttributeSet(), "flatArticlesList.xsl");
            IList<ArticleDto> articles = getArticles();
            XslMacro.GroupingCriterion[] groups = new XslMacro.GroupingCriterion[0];
            
            XmlDocument output = xm.BuildArticleTree(articles, groups, articles[0]);
            string outputStr = output.Serialize();

            string correctOut = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Articles><Article><Title>Desk</Title><Category>Woodwork</Category><Tags><Value>Carpentry</Value><Value>Woodwork</Value></Tags><Date>2014-12-03</Date></Article><Article Active=\"true\"><Title>Xbox Controller Repair</Title><Category>Electronics</Category><Tags><Value>Gaming</Value><Value>Electronics</Value></Tags><Date>2014-08-02</Date></Article></Articles>";
            Assert.AreEqual(correctOut, outputStr);
        }

        [TestMethod]
        public void ArticleTreeOneLevel()
        {
            XslMacro xm = new XslMacro(getAttributeSet(), "flatArticlesList.xsl");
            IList<ArticleDto> articles = getArticles();
            XslMacro.GroupingCriterion[] groups = new XslMacro.GroupingCriterion[1];
            groups[0] = new XslMacro.GroupingCriterion() { AttributeName = "Category", Sort = XslMacro.SortType.None };

            XmlDocument output = xm.BuildArticleTree(articles, groups, articles[0]);
            string outputStr = output.Serialize();

            string correctOut = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Articles><Group Attribute=\"Category\" Value=\"Electronics\"><Article Active=\"true\"><Title>Xbox Controller Repair</Title><Category>Electronics</Category><Tags><Value>Gaming</Value><Value>Electronics</Value></Tags><Date>2014-08-02</Date></Article></Group><Group Attribute=\"Category\" Value=\"Woodwork\"><Article><Title>Desk</Title><Category>Woodwork</Category><Tags><Value>Carpentry</Value><Value>Woodwork</Value></Tags><Date>2014-12-03</Date></Article></Group></Articles>";
            Assert.AreEqual(correctOut, outputStr);
        }

        [TestMethod]
        public void MacroExpansionFlat()
        {
            XslMacro xm = new XslMacro(getAttributeSet(), "flatArticlesList.xsl");
            IList<ArticleDto> articles = getArticles();
            string[] parsVals = new string[0];

            string output = xm.Expand(parsVals, parsVals, articles[0], articles);
            Assert.AreEqual("Desk\r\nXbox Controller Repair\r\n", output);
        }

        [TestMethod]
        public void MacroExpansionGrouped()
        {
            XslMacro xm = new XslMacro(getAttributeSet(), "categorisedArticles.xsl");
            IList<ArticleDto> articles = getArticles();
            string[] parmsVals = new string[0];

            string output = xm.Expand(parmsVals, parmsVals, articles[0], articles);
            Assert.AreEqual("Electronics\r\n    Xbox Controller Repair\r\nWoodwork\r\n    Desk\r\n", output);
        }
    }
}
