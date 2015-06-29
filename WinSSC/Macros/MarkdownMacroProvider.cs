//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WinSSC.Macros
{
    class MarkdownMacroProvider: IMacroProvider
    {
        private string rootDir;

        /// <param name="macroRootDirectory">The file system directory that contains Markdown macros. Path should end in a directory separator.</param>
        public MarkdownMacroProvider(string macroRootDirectory)
        {
            rootDir = macroRootDirectory;
        }

        /// <see cref="Macro(Expand)"/>
        public IMacro TryLoadMacro(string macroName)
        {
            string path = rootDir + macroName + ".md";
            if(File.Exists(path))
            {
                string content = File.ReadAllText(path);
                return new MarkdownMacro(macroName, content);
            }
            return null;
        }
    }
}
