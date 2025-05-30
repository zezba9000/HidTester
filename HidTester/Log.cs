using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidTester
{
    static class Log
    {
        public static void WriteLine()
        {
            Console.WriteLine();
            Debug.WriteLine("");
        }

        public static void WriteLine(string message)
        {
            if (message == null) return;
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        public static void WriteLine(object o)
        {
            if (o == null) return;
            string message = o.ToString();
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
    }
}
