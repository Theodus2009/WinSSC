//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC.VirtualArticleGenerators
{
    class XslArticleGeneratorLoader: IVirtualArticleGeneratorLoader
    {
        private AttributeSet validAttributes;

        public XslArticleGeneratorLoader(AttributeSet validAttributes)
        {
            this.validAttributes = validAttributes;
        }

        public IVirtualArticleGenerator ParseGeneratorFromFile(string filePath)
        {
            try
            {
                return new XslArticleGenerator(validAttributes, filePath);
            }
            catch(Exception ex)
            {
                Logger.LogError("Could not load Xsl article generator from " + filePath + ":\r\n" + ex.Message);
                return null;
            }
        }

        public string PrimaryFileExtension
        {
            get { return ".xsl"; }
        }
    }
}
