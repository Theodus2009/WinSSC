//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WinSSC.Macros
{
    public class XslMacroProvider: IMacroProvider
    {
        private string rootDir;
        private AttributeSet validAttributes;

        public XslMacroProvider(string macroRootDirectory, AttributeSet validAttributes)
        {
            rootDir = macroRootDirectory;
            this.validAttributes = validAttributes;
        }

        public IMacro TryLoadMacro(string macroName)
        {
            string path = rootDir + macroName + ".xsl";
            if(File.Exists(path))
            {
                string content = File.ReadAllText(path);
                return new XslMacro(validAttributes, path);
            }
            return null;
        }
    }


}
