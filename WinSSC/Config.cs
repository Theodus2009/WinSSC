using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WinSSC
{
    /// <summary>
    /// Contains various config options...
    /// </summary>
    public class Config
    {
        private const string PATHS_FILE = "Paths.txt";

        public static string ExecutablePath
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().Location;
            }
        }

        public static string ExecutableDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(ExecutablePath) + "\\";
            }
        }

        public static string ConfigDir
        {
            get
            {
                if(File.Exists(Directory.GetCurrentDirectory() + "\\" + PATHS_FILE))
                {
                    return Directory.GetCurrentDirectory() + "\\";
                }
                return ExecutableDir + "Config\\";
            }
        }

        public static Paths LoadPaths()
        {
            try
            {
                string[] confLines = File.ReadAllLines(ConfigDir + PATHS_FILE);
                return ParsePathsFile(confLines);
            }
            catch(FileNotFoundException)
            {
                Logger.LogError("Could not find " + ConfigDir + PATHS_FILE);
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                Logger.LogError("Could not find " + ConfigDir + " directory.");
                return null;
            }
        }

        public static Paths ParsePathsFile(string[] fileLines)
        {
            Paths p = new Paths();
            foreach (string l in fileLines)
            {
                //Ignore whitspace lines
                if (l.Trim().Length == 0) continue;
                //Ignore comment lines
                if (l.Trim().StartsWith("#")) continue;
                string[] parts = l.Trim().Split('=');
                if (parts.Length < 2)
                {
                    Logger.LogWarning("Ignoring " + l + " in " + PATHS_FILE);
                    continue;
                }
                if (!parts[1].EndsWith("\\")) parts[1] += '\\';
                switch (parts[0].ToLower())
                {
                    case "articlesdir":
                        p.ArticlesRootDir = parts[1].Trim();
                        break;
                    case "virtualarticlesdir":
                        p.VirtualArticlesRootDir = parts[1].Trim();
                        break;
                    case "templatesdir":
                        p.TemplatesRootDir = parts[1].Trim();
                        break;
                    case "macrosdir":
                        p.MacrosRootDir = parts[1].Trim();
                        break;
                    case "outputdir":
                        p.OutputRootDir = parts[1].Trim();
                        break;
                    default:
                        break;
                }
            }
            return p;
        }

        /// <summary>
        /// Container for read/write directory information contained in config files
        /// </summary>
        public class Paths
        {
            public string ArticlesRootDir;
            public string VirtualArticlesRootDir;
            public string TemplatesRootDir;
            public string MacrosRootDir;
            public string OutputRootDir;
        }
    }
}
