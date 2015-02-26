using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum.Quantum.Utils
{
    class GamePrints
    {
        public static bool enablePrint;
        private static long printAccumulator;

        public static void WriteLine(String message)
        {
            if (!enablePrint) return;

            Console.WriteLine(message);
        }

        public static void PrintPerformance(String message)
        {
            WriteLine(message);
        }


        public static void Toggle(long deltaTime)
        {
            printAccumulator += deltaTime;

            if (printAccumulator > 0)
            {
                printAccumulator -= 30000000;
                enablePrint = true;
            }
            else
            {
                enablePrint = false;
            }

            WriteLine("");
        }
    }
}
