using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Learning;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Controller;

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
        private static Random random = new Random();

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

        public static void Zero(List<List<double[]>> what)
        {
            for (int i = 0; i < what.Count; i++)
            {
                for (int j = 0; j < what[i].Count; j++)
                {
                    for (int k = 0; k < what[i][j].Length; k++)
                    {
                        what[i][j][k] = 0;
                    }
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

        public static double[] ScalarMultiply(double[] vec, double value)
        {
            double[] retVec = new double[vec.Length];
            for(int i = 0; i < vec.Length; i++)
            {
                retVec[i] = vec[i] * value;
            }
            return retVec;
        }

        public static double[] Sub(double[] from, double[] what)
        {
            double[] retVec = new double[from.Length];
            for (int i = 0; i < from.Length; i++)
            {
                retVec[i] = from[i] - what[i];
            }
            return retVec;
        }

        public static double[] Multiply(double[] vec1, double[] vec2)
        {
            double[] retVec = new double[vec1.Length];
            for (int i = 0; i < vec1.Length; i++)
            {
                retVec[i] = vec1[i] * vec2[i];
            }
            return retVec;
        }

        public static double[] ApplyFunction(double[] to, Func<double, double> func)
        {
            double[] retVec = new double[to.Length];
            for (int i = 0; i < to.Length; i++)
            {
                retVec[i] = func(to[i]);
            }
            return retVec;
        }

        public static bool ExistsIn<T>(T[] inWhat, T what) where T : IComparable<T>
        {
            foreach (T obj in inWhat)
            {
                if (obj.CompareTo(what) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<T> RandomSample<T>(List<T> from, int sampleSize)
        {
            int counter = 0;
            int BatchSize = from.Count;
            int[] indexes = new int[Math.Min(BatchSize, sampleSize)];
            for (int i = 0; i < indexes.Length; i++)
            {
                indexes[i] = -1;
            }
            while (counter < sampleSize && counter < BatchSize)
            {
                int index = random.Next(BatchSize);
                while (ExistsIn(indexes, index))
                    index = random.Next(BatchSize);
                indexes[counter] = index;
                counter++;
            }
            List<T> Sample = new List<T>();
            foreach(int index in indexes)
            {
                Sample.Add(from[index]);
            }
            return Sample;
        }

        public static T[] Flatten<T>(T[,] what)
        {
            int x_len = what.GetLength(0);
            int y_len = what.GetLength(1);
            T[] Flattened = new T[x_len * y_len];
            
            for (int x = 0; x < x_len; x++)
            {
                for(int y = 0; y < y_len; y++)
                {
                    Flattened[x * x_len + y] = what[x, y];
                }
            }

            return Flattened;
        }

        public static int MaxArg<T>(T[] inWhat) where T : IComparable<T>
        {
            int maxArg = 0;
            for(int arg = 0; arg < inWhat.Length; arg++)
            {
                if(inWhat[maxArg].CompareTo(inWhat[arg]) < 0)
                {
                    maxArg = arg;
                }
            }
            return maxArg;
        }

        public static T Max<T>(T[] inWhat) where T : IComparable<T>
        {
            int maxArg = 0;
            for (int arg = 0; arg < inWhat.Length; arg++)
            {
                if (inWhat[maxArg].CompareTo(inWhat[arg]) < 0)
                {
                    maxArg = arg;
                }
            }
            return inWhat[maxArg];
        }

        public static void ScalarAdd(double[] toWhat, double what)
        {

            for(int i = 0; i < toWhat.Length; i++)
            {
                toWhat[i] += what;
            }
            
        }

        public static void ScalarDivide(double[] what, double by)
        {

            for (int i = 0; i < what.Length; i++)
            {
                what[i] /= by;
            }
            
        }

        public static void ScalarAdd(List<List<double[]>> toWhat, double what)
        {

            for (int i = 0; i < toWhat.Count; i++)
            {
                for (int j = 0; j < toWhat[i].Count; j++)
                {
                    ScalarAdd(toWhat[i][j], what);
                }
            }

        }

        public static void ScalarDivide(List<List<double[]>> toWhat, double what)
        {

            for (int i = 0; i < toWhat.Count; i++)
            {
                for (int j = 0; j < toWhat[i].Count; j++)
                {
                    ScalarDivide(toWhat[i][j], what);
                }
            }

        }

        public static void Add(List<List<double[]>> toWhat, List<List<double[]>> what)
        {
            for (int i = 0; i < toWhat.Count; i++)
            {
                for (int j = 0; j < toWhat[i].Count; j++)
                {
                    for( int k = 0; k < toWhat[i][j].Length; k++)
                    {
                        toWhat[i][j][k] += what[i][j][k];
                    }
                }
            }
        }

        public static void Sub(List<List<double[]>> from, List<List<double[]>> what)
        {
            for (int i = 0; i < from.Count; i++)
            {
                for (int j = 0; j < from[i].Count; j++)
                {
                    for (int k = 0; k < from[i][j].Length; k++)
                    {
                        from[i][j][k] -= what[i][j][k];
                    }
                }
            }
        }

        public class ByPriority : IComparer<ControlBase>
        {
            public int Compare(ControlBase x, ControlBase y)
            {
                return x.ClickPriority.CompareTo(y.ClickPriority);
            }
        }

    }
}
