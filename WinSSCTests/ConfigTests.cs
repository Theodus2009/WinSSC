//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinSSC;

namespace WinSSC
{
    [TestClass]
    public class ConfigTests
    {
        [TestMethod]
        public void TestParsePathsConfigPositive()
        {
            string input =
                @"#This is a comment
                  articlesdir=C:\WebSite\Articles
                  templatesDir=C:\WebSite\Templates
                  outputDir=C:\WebSite\Output
                  MACROSDIR=C:\WebSite\Macros";
            Config.Paths output = Config.ParsePathsFile(input.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            Assert.AreEqual(@"C:\WebSite\Articles\", output.ArticlesRootDir);
            Assert.AreEqual(@"C:\WebSite\Templates\", output.TemplatesRootDir);
            Assert.AreEqual(@"C:\WebSite\Output\", output.OutputRootDir);
            Assert.AreEqual(@"C:\WebSite\Macros\", output.MacrosRootDir);
        }
    }
}
