using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace WinSSC
{
    public static class Extensions
    {
        public static string Serialize(this XmlDocument xmlDoc)
        {
            //http://stackoverflow.com/questions/2407302/convert-xmldocument-to-string
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                xmlDoc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }
        }
    }
}
