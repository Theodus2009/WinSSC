﻿//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarkdownSharp;


namespace WinSSC.Macros
{
    /// <summary>
    /// Processes a basic Markdown macro, which is just Markdown with paramater substitutions for items named in percentage signs.
    /// </summary>
    public class MarkdownMacro: TextMacro
    {
        public MarkdownMacro(string name, string macroText): base(name, macroText) { }

        public override string Expand(IList<string> parms, IList<string> vals, ArticleDto activeArticle, IList<ArticleDto> allArticles)
        {
            Markdown md = new Markdown();
            string html = md.Transform(base.Expand(parms, vals, activeArticle, allArticles));
            return html;
        }
    }
}
