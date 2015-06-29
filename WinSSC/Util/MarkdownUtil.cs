//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WinSSC.Util
{
    public static class MarkdownUtil
    {
        public static string ParseMarkdown(string sourceText, string relativePath, AttributeSet validAttributes, out IDictionary<string, IList<string>> attributes)
        {
            StringBuilder contentSb = new StringBuilder();
            attributes = new Dictionary<string, IList<string>>();
            sourceText = sourceText.Replace("\r", "");
            string[] sourceLines = sourceText.Split('\n');
            bool processingHeaders = false, processedHeaders = false;
            for (int i = 0; i < sourceLines.Length; i++)
            {
                string s = sourceLines[i];
                if (processingHeaders)
                {
                    if (s.Trim() == "---")
                    {
                        processingHeaders = false;
                        continue;
                    }
                    string[] parts = s.Split(':');
                    if (parts.Length != 2)
                    {
                        Logger.LogWarning("Attribute " + s + " ignored on " + relativePath + "(Malformed)");
                        continue;
                    }
                    AttributeDef attrDef;
                    try
                    {
                        attrDef = validAttributes.AttributeDefinitions.First(x => x.Name == parts[0]);
                    }
                    catch (InvalidOperationException)
                    {
                        attrDef = null;
                    }
                    if (attrDef == null)
                    {
                        Logger.LogWarning("Attribute " + s + " ignored on " + relativePath + "(No matching attribute in config)");
                        continue;
                    }
                    string[] vals = parts[1].Split(',');
                    if (attrDef.DataType == EAttributeType.String || attrDef.DataType == EAttributeType.Number || attrDef.DataType == EAttributeType.Date)
                    {
                        //String and number types may have exactly one value
                        if (vals.Length != 1)
                        {
                            Logger.LogWarning("Attribute " + s + " ignored on " + relativePath + "(Multiple values specified for single-value attribute type)");
                            continue;
                        }
                    }
                    if (attrDef.DataType == EAttributeType.Number)
                    {
                        double dummy;
                        if (!double.TryParse(vals[0], out dummy))
                        {
                            Logger.LogWarning("Attribute " + s + " ignored on " + relativePath + "(Could not parse value as number on numeric attribute)");
                            continue;
                        }
                    }
                    if (attrDef.DataType == EAttributeType.Date)
                    {
                        try
                        {
                            DateTime date = DateTime.Parse(vals[0]);
                            vals[0] = date.Year + "-" + date.Month.ToString("D2") + "-" + date.Day.ToString("D2");
                        }
                        catch (FormatException)
                        {
                            Logger.LogWarning("Attribute " + s + " ignored on " + relativePath + "(Could not parse value as date)");
                            continue;
                        }
                    }
                    attributes.Add(parts[0], vals);
                }
                else
                {
                    //Enter YAML front matter processing mode
                    if (!processedHeaders && s.Trim() == "---")
                    {
                        processingHeaders = true;
                        processedHeaders = true;
                        continue;
                    }
                    contentSb.AppendLine(s);
                }
            }
            return contentSb.ToString();
        }

        /// <summary>
        /// Identifies the locations of macros within article or template content.
        /// </summary>
        /// <remarks>
        /// The macro syntax is inspired by Aaron Parecki's extensions to Markdown (https://aaronparecki.com/articles/2012/09/01/1/some-enhancements-to-markdown#macros) in the 
        /// hope of maintaining some consistency and portability between dialects. The mechanics by which macros are implemented is rather different though.
        /// </remarks>
        public static List<MacroInvocation> LocateMarkdownMacros(string content)
        {
            List<MacroInvocation> invocations = new List<MacroInvocation>();

            //Tested on regexr.com - their tool is great (except JavaScript doesn't support lookbehind)
            string macroRegex = @"(?<!\\)!\[:(\w+)(\s[\w\d]+)*\]\s*(\(([^\)]+)\))?";

            Regex re = new Regex(macroRegex);
            MatchCollection matches = re.Matches(content);
            foreach (Match m in matches)
            {
                string macroName = m.Groups[1].Value;
                List<string> parms = new List<string>();
                List<string> vals = new List<string>();
                for (int i = 0; i < m.Groups[2].Captures.Count; i++)
                {
                    //Push paramters into the list
                    parms.Add(decodeParameterValue(m.Groups[2].Captures[i].Value.Trim()));
                }
                //Group 3 is the same as 
                for (int i = 0; i < m.Groups[4].Captures.Count; i++)
                {
                    //Push value(s?) into the list
                    vals.Add(m.Groups[4].Captures[i].Value.Trim());
                }

                MacroInvocation mi = new MacroInvocation()
                {
                    MacroName = macroName,
                    Parameters = parms,
                    Values = vals,
                    StartingCharIndex = m.Index,
                    EndingCharIndex = m.Index + m.Length
                };
                invocations.Add(mi);
            }

            return invocations;
        }

        public static string ReplaceEscapedMacros(string markdownInput)
        {
            string macroRegex = @"\\(?=!\[:(\w+)(\s[\w\d]+)*\]\s*(\(([^\)]+)\))?)";

            Regex re = new Regex(macroRegex);
            MatchCollection matches = re.Matches(markdownInput);
            for(int i = matches.Count - 1; i >= 0; i--)
            {
                Match m = matches[i];
                string pre = markdownInput.Substring(0, m.Index);
                string post = markdownInput.Substring(m.Index + 1);
                markdownInput = pre + post;
            }

            return markdownInput;
        }

        /// <summary>
        /// Decodes lone underscores to spaces for parameters. This is to get around the fact
        /// that parameters are space-delimited. As such, double-underscores are replaced with single underscores.
        /// </summary>
        /// <param name="originalValue"></param>
        /// <returns></returns>
        private static string decodeParameterValue(string originalValue)
        {
            Regex replaceRegex = new Regex(@"[^_]_");
            MatchCollection matches = replaceRegex.Matches(originalValue);
            foreach (Match m in matches)
            {
                originalValue = originalValue.Substring(0, m.Index + 1) + " " + originalValue.Substring(m.Index + m.Length);
            }
            originalValue = originalValue.Replace("__", "_");
            return originalValue;
        }

    }
}
