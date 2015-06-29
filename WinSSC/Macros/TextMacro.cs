using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WinSSC.Macros
{
    /// <summary>
    /// Base class for simple text-based macros. These macros create their output by taking the original macro text and performing
    /// substitutions, as opposed to the XSL approach where the macro text is a set of instructions.
    /// Parameters to substitute are in the format %&lt;parameter name&gt:%.
    /// </summary>
    public abstract class TextMacro: IMacro
    {
        protected string macroText;

        public string Name { get; set; }

        public TextMacro (string name, string macroText)
        {
            Name = name;
            this.macroText = macroText;
        }

        public virtual string Expand(IList<string> parms, IList<string> vals, ArticleDto activeArticle, IList<ArticleDto> allArticles)
        {
            IDictionary<string, string> vars = prepareVariables(parms, vals, activeArticle);

            Regex replaceRegex = new Regex(@"%([^\)%]+)%");

            string expandedText = macroText;

            MatchCollection matches = replaceRegex.Matches(expandedText);
            Match[] matchArr = new Match[matches.Count];
            matches.CopyTo(matchArr, 0);
            IEnumerable<Match> sortedMatches = (IEnumerable<Match>)matchArr.OrderByDescending(x => x.Index);

            foreach (Match match in sortedMatches)
            {
                string preText = expandedText.Substring(0, match.Index);
                string postText = expandedText.Substring(match.Index + match.Length);
                string parmName = match.Groups[1].Value;
                string value = "";
                if (vars.ContainsKey(parmName)) value = vars[parmName];
                expandedText = preText + value + postText;
            }

            return expandedText;
        }

        private IDictionary<string, string> prepareVariables(IList<string> parms, IList<string> vals, ArticleDto activeArticle)
        {
            for (int i = 0; i < parms.Count; i++)
            {
                parms[i] = parms[i];
            }

            Dictionary<string, string> vars = new Dictionary<string, string>();

            for (int i = 0; i < parms.Count; i++)
            {
                string key = "p" + (i + 1);
                if (vars.ContainsKey(key))
                {
                    vars[key] = parms[i];
                }
                else
                {
                    vars.Add(key, parms[i]);
                }
            }

            for (int i = 0; i < vals.Count; i++)
            {
                string key = "v" + (i + 1);
                if (vars.ContainsKey(key))
                {
                    vars[key] = vals[i];
                }
                else
                {
                    vars.Add(key, vals[i]);
                }
            }

            for (int i = 0; i < vals.Count; i++)
            {
                string key = (i + 1).ToString();
                if (vars.ContainsKey(key))
                {
                    vars[key] = vals[i];
                }
                else
                {
                    vars.Add(key, vals[i]);
                }
            }

            foreach (var kvp in activeArticle.Attributes)
            {
                if (kvp.Value.Count > 0)
                {
                    if (vars.ContainsKey(kvp.Key))
                    {
                        vars[kvp.Key] = kvp.Value[0];
                    }
                    else
                    {
                        vars.Add(kvp.Key, kvp.Value[0]);
                    }
                }
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    string key = kvp.Key + (i + 1);
                    if (vars.ContainsKey(key))
                    {
                        vars[key] = kvp.Value[0];
                    }
                    else
                    {
                        vars.Add(key, kvp.Value[0]);
                    }
                }
            }
            return vars;

        }
    }
}
