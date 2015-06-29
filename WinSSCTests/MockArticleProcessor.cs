using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinSSC.ArticleProcessors;

namespace WinSSC
{
    class MockArticleProcessor: IArticleProcessor
    {
        public ArticleDto ParseArticleRawText(string sourceText, string relativePath)
        {
            ArticleDto article = new ArticleDto(@"Articles\MyArticle.md", this);
            article.Content = 
@"#My Article
Some important text";
            article.Attributes = new Dictionary<string, IList<string>>();
            article.Attributes.Add("Title", new List<string>());
            article.Attributes["Title"].Add("My Great Article");
            return article;
        }

        public IList<MacroInvocation> LocateMacrosInContent(ArticleDto article)
        {
            return new List<MacroInvocation>();
        }

        public void FinaliseArticleHtml(ArticleDto article)
        {
            article.Content =
@"<html><heade><title>My Great Aticle</title></head><body><h1>My Article</h1>
Some important text</body></html>";
        }

        public string PrimaryFileExtension
        {
            get { return ".md"; }
        }
    }
}
