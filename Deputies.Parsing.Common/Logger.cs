using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.Parsing.Common
{
    public static class Logger
    {
        public static bool EnableInfo = false;

        public static void Log(string text)
        {
            Console.WriteLine(" * " + DateTime.UtcNow.ToString() + " - " + text);
            Console.WriteLine();
        }

        public static void LogException(string text, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" * " + DateTime.UtcNow.ToString() + " - " + text + " - " + ex);
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void LogInfo(string text)
        {
            if(EnableInfo)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" * " + DateTime.UtcNow.ToString() + " - " + text);
                Console.ResetColor();
                Console.WriteLine();
            }
        }
    }
}
