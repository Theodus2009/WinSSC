//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WinSSC
{
    public static class AttributeTranformations
    {
        /// <summary>
        /// Determines the correct output path for an article.
        /// </summary>
        /// <param name="article">The article whose path is to be calculated. This object is not updated.</param>
        /// <returns>The new article output path.</returns>
        public static string CalculateArticlePath(ArticleDto article)
        {
            if(article.Attributes.Any(x => x.Key == "Path" && x.Value.Count > 0))
            {
                string path = article.Attributes.First(x => x.Key == "Path").Value[0];
                if(path.IndexOfAny(Path.GetInvalidPathChars()) > -1)
                {
                    Logger.LogWarning("Article " + article.SourceFileRelativePath + " specified invalid path " + path + " which was ignored.");
                }
                else
                {
                    return path.Replace('\\', '/');
                }
            }

            string noExtPath = Path.GetDirectoryName(article.SourceFileRelativePath) + "/" + Path.GetFileNameWithoutExtension(article.SourceFileRelativePath);
            noExtPath = noExtPath.Replace('\\', '/');
            noExtPath += ".html";
            return noExtPath;
        }
    }
}
