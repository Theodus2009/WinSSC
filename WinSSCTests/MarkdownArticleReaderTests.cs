using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinSSC;
using System.Collections.Generic;
using WinSSC.ArticleProcessors;
using WinSSC.Macros;

namespace WinSSC
{
    [TestClass]
    public class MarkdownArticleReaderTests
    {
        private static AttributeSet getAttributeSet()
        {
            AttributeSet attrSet = new AttributeSet();
            string input = @"#A comment line
                             Title:String
                             Index:Number
                             Tags:StringArray
                             Date:Date";
            attrSet.AttributeDefinitions = AttributeSet.ParseAttributeDefs(input);
            return attrSet;
        }

        [TestMethod]
        public void LoadArticlePositive()
        {
            string input =
@"---
Title:MyArticle
Date:2015-01-02
---
##The internals
Some article content...
";
            MarkdownArticleProcessor ar = new MarkdownArticleProcessor(getAttributeSet());
            ArticleDto output = ar.ParseArticleRawText(input, "TestArticle");
            Assert.AreEqual(2, output.Attributes.Count);
            Assert.AreEqual("MyArticle", output.Attributes["Title"][0]);
            Assert.AreEqual("2015-01-02", output.Attributes["Date"][0]);
            Assert.AreEqual("##The internals\r\nSome article content...\r\n\r\n", output.Content);
        }

        [TestMethod]
        public void LoadArticleBadDate()
        {
            string input =
@"---
Title:MyArticle
Date:XXXXXX
---
##The internals
Some article content...
";
            MarkdownArticleProcessor ar = new MarkdownArticleProcessor(getAttributeSet());
            ArticleDto output = ar.ParseArticleRawText(input, "TestArticle");
            Assert.AreEqual(1, output.Attributes.Count);
            Assert.AreEqual("MyArticle", output.Attributes["Title"][0]);
        }

        [TestMethod]
        public void LocateMacro()
        {
            MarkdownArticleProcessor ar = new MarkdownArticleProcessor(getAttributeSet());
            ArticleDto a = ar.ParseArticleRawText(
@"---
Title:Xbox Controller Repair
Date:2014-08-02
Category:Electronics
Tags:Gaming,Electronics
---
##The internals
![:youtube 800 600](https://www.youtube.com/embed/Di5AT4MI6BY)
![:youtube 512 400](http://www.youtube.com/embed/Di5AT4MI6BY)
![My image caption](imgs/someImageNotAMacro.jpg", "Some\\File.md");

            MacroInvocation mi = ar.LocateMacrosInContent(a)[0];
            MarkdownMacro mdm = new MarkdownMacro("youtube", "<iframe width=\"%p1%\" height=\"%p2%\" src=\"%v1%\" frameborder=\"0\"></iframe>");
            ArticleDto[] allArticles = new ArticleDto[1];
            IList<MacroInvocation> mis = ar.LocateMacrosInContent(a);
            Assert.AreEqual(2, mis.Count);
            Assert.AreEqual(17, mis[0].StartingCharIndex);
            Assert.AreEqual(79, mis[0].EndingCharIndex);
        }
    }
}
