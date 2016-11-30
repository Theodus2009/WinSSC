//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WinSSC
{
    /// <summary>
    /// Contains a list of attributes that may be defined on an article.
    /// </summary>
    public class AttributeSet
    {
        private const string ATTRIBUTE_FILE = "Attributes.txt";

        public IList<AttributeDef> AttributeDefinitions = new List<AttributeDef>();

        /// <summary>
        /// Loads the AttributeSet from a predefined location (based on the executable path).
        /// </summary>
        /// <remarks>
        /// This probably should move elsewhere in a later revision.
        /// </remarks>
        /// <returns></returns>
        public static AttributeSet LoadFromConfig()
        {
            try
            {
                AttributeSet attrSet = new AttributeSet();
                string configText = File.ReadAllText(Config.ConfigDir + ATTRIBUTE_FILE);
                attrSet.AttributeDefinitions = ParseAttributeDefs(configText);

                //Apply hard-coded attributes
                applyHardAttribute(attrSet, "Template");
                applyHardAttribute(attrSet, "Path");

                return attrSet;
            }
            catch(FileNotFoundException)
            {
                Logger.LogError("Could not find " + ATTRIBUTE_FILE + " in " + Config.ConfigDir);
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                Logger.LogError("Could not find " + ATTRIBUTE_FILE + " in " + Config.ConfigDir);
                return null;
            }
        }

        private static void applyHardAttribute(AttributeSet attrSet, string attrName)
        {
            AttributeDef existingDef = attrSet.AttributeDefinitions.FirstOrDefault(x => x.Name == attrName);
            if (existingDef != null) attrSet.AttributeDefinitions.Remove(existingDef);
            attrSet.AttributeDefinitions.Add(new AttributeDef() { Name = attrName, DataType = EAttributeType.String });
        }

        /// <summary>
        /// Parses attribute definitions loaded from a file.
        /// </summary>
        /// <param name="configText"></param>
        /// <returns></returns>
        public static IList<AttributeDef> ParseAttributeDefs(string configText)
        {
            List<AttributeDef> defs = new List<AttributeDef>();
            string[] configLines = configText.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string l in configLines)
            {
                string str = l.Trim();
                if (str.StartsWith("#")) continue;
                string[] parts = str.Split(':');
                if (parts.Length != 2 || parts[0].Trim().Length == 0)
                {
                    Logger.LogWarning("Could not parse " + str + " in " + ATTRIBUTE_FILE);
                    continue;
                }
                EAttributeType attrType;
                try
                {
                    attrType = (EAttributeType)Enum.Parse(typeof(EAttributeType), parts[1]);
                }
                catch (ArgumentException)
                {
                    Logger.LogWarning("Attribute " + parts[0] + " in " + ATTRIBUTE_FILE + " was not a valid type.");
                    continue;
                }

                AttributeDef def = new AttributeDef() { Name = parts[0], DataType = attrType };
                defs.Add(def);
            }
            return defs;
        }
    }
}
