//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC.Macros
{
    /// <summary>
    /// Represents a macro - text that is used to replace a predefined marker in an Article, with the option to replace parameters in the macro text.
    /// </summary>
    public interface IMacro
    {
        /// <summary>
        /// THe name of the macro.
        /// </summary>
        string Name {get; set;}

        /// <summary>
        /// Expands the given macro.
        /// </summary>
        /// <param name="parms">Parameters included in the macro invocation. Normally modifies the presentation of the data.</param>
        /// <param name="vals">Data provided in the macro invocation. Typically the subject of the macro, such as a video URL.</param>
        /// <param name="activeArticle">The article in which the current macro appears.</param>
        /// <param name="allArticles">A list of all articles being processed. </param>
        /// <returns>The processed macro HTML.</returns>
        string Expand(IList<string> parms, IList<string> vals, ArticleDto activeArticle, IList<ArticleDto> allArticles);
    }
}
