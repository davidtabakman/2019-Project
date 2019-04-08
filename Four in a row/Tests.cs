using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Learning;
using System.Diagnostics;
using Microsoft.Xna.Framework;

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

        public static void Zero(double[] what)
        {
            for(int x = 0; x < what.Length; x++)
            {
                what[x] = 0;
            }
        }

        public static double[] CreateZero(int len)
        {
            double[] retVec = new double[len];
            Zero(retVec);
            return retVec;
        }

        public static double[] CreateInitArray(int len, double value)
        {
            double[] retVec = new double[len];
            InitArray(retVec,value);
            return retVec;
        }
        
        public static void InitArray(double[] array, double with)
        {
            for(int i = 0; i < array.Length; i++)
            {
                array[i] = with;
            }
        }

        public static string ArrayToString(double[] array)
        {
            string retArr = "<";
            for(int i = 0; i < array.Length - 1; i++)
            {
                retArr += array[i] + ",";
            }
            retArr += array[array.Length-1] + ">";
            return retArr;
        } 

        public static double Sum(double[] arr)
        {
            double sum = 0;
            foreach (double num in arr)
                sum += num;
            return sum;
        }

        public static double Dot(double[] arr1, double[] arr2)
        {
             double[] target = new double[arr1.Length];
             for (int dim = 0; dim < arr1.Length; dim++)
             {
                target[dim] = arr1[dim] * arr2[dim];
             }
            return Sum(target);
        }
    }
}
