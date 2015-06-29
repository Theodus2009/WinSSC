using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC
{
    /// <summary>
    /// Provides a central point for logging of error information.
    /// </summary>
    static class Logger
    {
        private static int errorLevel = 0;

        public static bool DebugOn = false;

        /// <summary>
        /// Call this when initialising the application.
        /// </summary>
        public static void StartLog()
        {
            Console.WriteLine("Starting at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
        }

        public static void LogDebug(string message)
        {
            if (DebugOn)
            {
                Console.Write("DBG ");
                Console.WriteLine(message);
            }
        }

        public static void LogMessage(string message)
        {
            Console.Write("    ");
            Console.WriteLine(message);
        }
        public static void LogWarning(string message)
        {
            if (errorLevel < 1) errorLevel = 1;
            Console.Write("WRN ");
            Console.WriteLine(message);
        }

        public static void LogError(string message)
        {
            if (errorLevel < 2) errorLevel = 2;
            Console.Write("ERR ");
            Console.WriteLine(message);
        }

        /// <summary>
        /// Call this when the application has finished doing work.
        /// </summary>
        public static void FinishLog()
        {

        }
    }
}
