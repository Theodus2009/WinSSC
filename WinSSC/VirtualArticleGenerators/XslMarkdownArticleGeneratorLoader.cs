//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC.VirtualArticleGenerators
{
    class XslMarkdownArticleGeneratorLoader: IVirtualArticleGeneratorLoader
    {
        private AttributeSet validAttributes;

        public XslMarkdownArticleGeneratorLoader(AttributeSet validAttributes)
        {
            this.validAttributes = validAttributes;
        }

        public IVirtualArticleGenerator ParseGeneratorFromFile(string filePath)
        {
            try
            {
                return new XslMarkdownArticleGenerator(validAttributes, filePath);
            }
            catch(Exception ex)
            {
                Logger.LogError("Could not load Xsl-Markdown article generator from " + filePath + ":\r\n" + ex.Message);
                return null;
            }
        }

        public string PrimaryFileExtension
        {
            get { return ".xslmd"; }
        }
    }
}
