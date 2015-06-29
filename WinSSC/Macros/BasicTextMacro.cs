//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
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
