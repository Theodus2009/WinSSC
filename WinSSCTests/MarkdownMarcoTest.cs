//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using WinSSC.Macros;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinSSC.ArticleProcessors;
using WinSSC.Util;
using System.Collections.Generic;

namespace WinSSC
{
    [TestClass]
    public class MarkdownMarcoTest
    {
        private static AttributeSet getAttributeSet()
        {
            AttributeSet attrSet = new AttributeSet();
            string input = @"#A comment line
                             Title:String
                             Cateogry:String
                             Tags:StringArray
                             Date:Date";
            attrSet.AttributeDefinitions = AttributeSet.ParseAttributeDefs(input);
            return attrSet;
        }

        [TestMethod]
        public void ExpandMacroTest()
        {
            MarkdownArticleProcessor ar = new MarkdownArticleProcessor(getAttributeSet());
            ArticleDto a = ar.ParseArticleRawText(
@"---
Title:Xbox Controller Repair
Date:2014-08-02
Category:Electronics
Tags:Gaming,Electronics
---
![:youtube 800 600](https://www.youtube.com/embed/Di5AT4MI6BY)", "Some\\File.md");

            MacroInvocation mi = ar.LocateMacrosInContent(a)[0];
            MarkdownMacro mdm = new MarkdownMacro("youtube", "<iframe width=\"%p1%\" height=\"%p2%\" src=\"%v1%\" frameborder=\"0\"></iframe>");
            ArticleDto[] allArticles = new ArticleDto[1];
            allArticles[0] = a;
            string finalText = mdm.Expand(mi.Parameters, mi.Values, a, allArticles);
            Assert.AreEqual(
"<iframe width=\"800\" height=\"600\" src=\"https://www.youtube.com/embed/Di5AT4MI6BY\" frameborder=\"0\"></iframe>",
finalText.Trim());
        }

        [TestMethod]
        public void ExpandArticleMacrosTest()
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
![:youtube 512 400](http://www.youtube.com/embed/Di5AT4MI6BY)", "Some\\File.md");

            MacroInvocation mi = ar.LocateMacrosInContent(a)[0];
            MarkdownMacro mdm = new MarkdownMacro("youtube", "<iframe width=\"%p1%\" height=\"%p2%\" src=\"%v1%\" frameborder=\"0\"></iframe>");
            ArticleDto[] allArticles = new ArticleDto[1];
            allArticles[0] = a;
            string finalText = mdm.Expand(mi.Parameters, mi.Values, a, allArticles);
            Assert.AreEqual(
"<iframe width=\"800\" height=\"600\" src=\"https://www.youtube.com/embed/Di5AT4MI6BY\" frameborder=\"0\"></iframe>",
finalText.Trim());
        }

        [TestMethod]
        public void EscapedArticleMacrosTest()
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
\![:noMatchMacro parm1]", "Some\\File.md");

            MacroInvocation mi = ar.LocateMacrosInContent(a)[0];
            MarkdownMacro mdm = new MarkdownMacro("youtube", "<iframe width=\"%p1%\" height=\"%p2%\" src=\"%v1%\" frameborder=\"0\"></iframe>");
            ArticleDto[] allArticles = new ArticleDto[1];
            allArticles[0] = a;

            IList<MacroInvocation> miList = MarkdownUtil.LocateMarkdownMacros(a.Content);

            Assert.AreEqual(1, miList.Count);
            Assert.AreEqual("youtube", miList[0].MacroName);
        }
    }
}
