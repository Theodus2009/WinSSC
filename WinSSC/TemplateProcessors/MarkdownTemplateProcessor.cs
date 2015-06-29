using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC.TemplateProcessors
{
    public class MarkdownTemplateProcessor: ITemplateProcessor
    {
        private const string CONTENT_MARKER = "%content%";

        private AttributeSet validAttributes;

        public MarkdownTemplateProcessor(AttributeSet validAttributes)
        {
            this.validAttributes = validAttributes;
        }

        public TemplateDto ParseTemplateRawText(string sourceText, string relativePath, string name)
        {
            IDictionary<string, IList<string>> attributes;
            string content = WinSSC.Util.MarkdownUtil.ParseMarkdown(sourceText, relativePath, validAttributes, out attributes);
            TemplateDto template = new TemplateDto()
            {
                Name = name,
                Content = content,
                Attributes = attributes
            };
            return template;
        }

        public void WrapArticle(ArticleDto article)
        {
            int startPos = article.TemplateContent.IndexOf(CONTENT_MARKER);
            if (startPos >= 0)
            {
                string preTemplate = article.TemplateContent.Substring(0, startPos);
                string postTemplate = article.TemplateContent.Substring(startPos + CONTENT_MARKER.Length);
                article.Content = preTemplate + article.Content + postTemplate;
            }
            else
            {
                Logger.LogError("Content marker (" + CONTENT_MARKER + " ) could not be found in " + article.Template.Name + " template");
            }
        }

        public string PrimaryFileExtension
        {
            get { return ".mdt"; }
        }

        public IList<MacroInvocation> LocateMacrosInArticleTemplateContent(ArticleDto article)
        {
            return WinSSC.Util.MarkdownUtil.LocateMarkdownMacros(article.TemplateContent);
        }
    }
}
