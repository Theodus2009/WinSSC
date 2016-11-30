using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC.ArticleProcessors
{
    class PureTextArticleProcessor: IArticleProcessor
    {
        public ArticleDto ParseArticleRawText(string sourceText, string relativePath)
        {
            ArticleDto article = new ArticleDto(relativePath, this);
            article.Content = sourceText;
            article.OutputPath = relativePath;
            article.Template = null; //No template
            return article;
        }

        public IList<MacroInvocation> LocateMacrosInContent(ArticleDto article)
        {
            //Nothing to do
            return new List<MacroInvocation>();
        }

        public void FinaliseArticleHtml(ArticleDto article)
        {
            //Nothing to do
        }

        public string PrimaryFileExtension
        {
            get { return ".txt"; }
        }
    }
}
