using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC.Macros
{
    /// <summary>
    /// Provides a way to find and load macros of various types
    /// </summary>
    public interface IMacroProvider
    {
        /// <summary>
        /// Attempts to locate a macro with the given name.
        /// </summary>
        /// <param name="macroName">The name of the macro (case insensitive)</param>
        /// <returns>An instance of Macro that is ready to run, or null if this provider doesn't have any macro with the given name.</returns>
        IMacro TryLoadMacro(string macroName);
    }
}
