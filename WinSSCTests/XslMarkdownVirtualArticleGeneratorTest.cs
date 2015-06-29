//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using WinSSC.VirtualArticleGenerators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinSSC;
using System.Collections.Generic;
using WinSSC.ArticleProcessors;

namespace StaticCmsTests
{
    [TestClass]
    public class XslMarkdownVirtualArticleGeneratorTest
    {
        [TestMethod]
        public void TestGenerate()
        {
            XslMarkdownArticleGenerator gen = new XslMarkdownArticleGenerator(getAttributeSet(), "virtualMarkdownArticles.xslmd");
            IList<ArticleDto> result = new List<ArticleDto>(gen.GenerateArticles(getArticles()));
            Assert.AreEqual(2, result.Count);

            if (result.Count < 2) return;
            Assert.AreEqual("Electronics", result[0].Attributes["Title"][0]);
            Assert.AreEqual("Woodwork", result[1].Attributes["Title"][0]);
        }

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
Chisels...", "Some\\Desk.cd");
            articles.Add(a);
            return articles;
        }
    }
}
