//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WinSSC
{
    [TestClass]
    public class AttributeTransformationsTests
    {
        [TestMethod]
        public void BasicPathCreation()
        {
            ArticleDto article = new ArticleDto(@"Articles\MyArticle.md", new MockArticleProcessor());
            string calcPath = AttributeTranformations.CalculateArticlePath(article);
            Assert.AreEqual("Articles/MyArticle.html", calcPath);
        }
    }
}
