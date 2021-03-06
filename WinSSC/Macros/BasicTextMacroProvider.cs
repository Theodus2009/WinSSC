﻿//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WinSSC.Macros
{
    public class BasicTextMacroProvider: IMacroProvider
    {
        private string rootDir;

        /// <param name="macroRootDirectory">The file system directory that contains text macros. Path should end in a directory separator.</param>
        public BasicTextMacroProvider(string macroRootDirectory)
        {
            rootDir = macroRootDirectory;
        }

        public IMacro TryLoadMacro(string macroName)
        {
            string path = rootDir + macroName + ".txt";
            if(File.Exists(path))
            {
                string content = File.ReadAllText(path);
                return new BasicTextMacro(macroName, content);
            }
            return null;
        }
    }
}
