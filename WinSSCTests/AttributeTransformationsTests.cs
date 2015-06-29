using System;
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
