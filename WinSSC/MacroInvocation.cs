using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC
{
    /// <summary>
    /// Indicates a location in an article where a macro is to be invoked, incliding macro name and parameters
    /// </summary>
    public class MacroInvocation
    {
        /// <summary>
        /// Start position of the macro invocation, including any markup
        /// </summary>
        public int StartingCharIndex;

        /// <summary>
        /// Position of the last character macro invocation, including any markup
        /// </summary>
        public int EndingCharIndex;

        /// <summary>
        /// Name of of the macro to be invoked. This value is not case sensitive.
        /// </summary>
        public string MacroName;

        /// <summary>
        /// List of invocation paramter values.
        /// </summary>
        /// <remarks>
        ///  At this time parameters are not named, as this class was built around Markdown macro format.
        /// </remarks>
        public IList<string> Parameters;

        /// <summary>
        /// List of macro data. Typically will have just one entry.
        /// </summary>
        public IList<string> Values;
    }
}
