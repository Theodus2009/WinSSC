using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Any(x => x.ToLower() == "/d"))
            {
                Logger.DebugOn = true;
            }

            Orchestrator orc = new Orchestrator();
            orc.ConvertArticles();
            orc.ConvertImages();
        }
    }
}
