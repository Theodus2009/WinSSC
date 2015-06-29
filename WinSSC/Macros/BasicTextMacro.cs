using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC.Macros
{
    /// <summary>
    /// The very simplest type of macro - plain text with parm substitutions.
    /// </summary>
    public class BasicTextMacro: TextMacro
    {
        public BasicTextMacro(string name, string macroText) : base(name, macroText) { }
    }
}
