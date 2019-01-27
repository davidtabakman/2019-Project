using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Learning;
using System.Diagnostics;

namespace Helper
{
    class Tests
    {
        public static void CheckAll()
        {
            
        }
    }

    class Helper
    {
        public static void Zero(int[,] what)
        {
            for (int x = 0; x < what.GetLength(0); x++)
            {
                for (int y = 0; y < what.GetLength(1); y++)
                {
                    what[x, y] = 0;
                }
            }
        }
    }
}
