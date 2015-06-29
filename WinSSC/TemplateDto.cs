//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinSSC.TemplateProcessors;

namespace WinSSC
{
    /// <summary>
    /// Contains data for a template that has been loaded from the file system.
    /// Templates provide a wrapper for article content (such as header, footer, nav-bar etc) and default values for attributes.
    /// </summary>
    public class TemplateDto
    {
        public string Name;

        public string Content;

        /// <summary>
        /// Contains attributes found in an article's front-matter.
        /// </summary>
        public IDictionary<string, IList<string>> Attributes = new Dictionary<string, IList<string>>();

        /// <summary>
        /// The temaplte processor that can handle this DTO.
        /// </summary>
        public ITemplateProcessor TemplateProcessor; 
    }
}
