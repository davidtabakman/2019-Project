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

        /// <summary>
        /// Fill with zeroes
        /// </summary>
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

        /// <summary>
        /// Fill with zeroes
        /// </summary>
        public static void Zero(double[] what)
        {
            for(int x = 0; x < what.Length; x++)
            {
                what[x] = 0;
            }
        }

        /// <summary>
        /// Create an array filled with zeroes
        /// </summary>
        /// <param name="len">Length of the array</param>
        public static double[] CreateZero(int len)
        {
            double[] retVec = new double[len];
            Zero(retVec);
            return retVec;
        }

        /// <summary>
        /// Craete an array initiated with a value
        /// </summary>
        /// <param name="len">Length of the array</param>
        public static double[] CreateInitArray(int len, double value)
        {
            double[] retVec = new double[len];
            InitArray(retVec,value);
            return retVec;
        }
        
        /// <summary>
        /// Fill the array with some value
        /// </summary>
        public static void InitArray(double[] array, double with)
        {
            for(int i = 0; i < array.Length; i++)
            {
                array[i] = with;
            }
        }

        /// <summary>
        /// Translate the array into a string
        /// </summary>
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

        /// <summary>
        /// Return the sum of an array
        /// </summary>
        public static double Sum(double[] arr)
        {
            double sum = 0;
            foreach (double num in arr)
                sum += num;
            return sum;
        }

        /// <summary>
        /// Perform Dot multiplication on two arrays
        /// </summary>
        /// <returns>The dot product</returns>
        public static double Dot(double[] arr1, double[] arr2)
        {
             double[] target = new double[arr1.Length];
             for (int dim = 0; dim < arr1.Length; dim++)
             {
                target[dim] = arr1[dim] * arr2[dim];
             }
            return Sum(target);
        }

        /// <summary>
        /// Perform multiplication by a scalar. Return a new array.
        /// </summary>
        public static double[] ScalarMultiply(double[] vec, double value)
        {
            double[] retVec = new double[vec.Length];
            for(int i = 0; i < vec.Length; i++)
            {
                retVec[i] = vec[i] * value;
            }
            return retVec;
        }

        /// <summary>
        /// Substitude an array from another. Return a new array
        /// </summary>
        public static double[] Sub(double[] from, double[] what)
        {
            double[] retVec = new double[from.Length];
            for (int i = 0; i < from.Length; i++)
            {
                retVec[i] = from[i] - what[i];
            }
            return retVec;
        }

        /// <summary>
        /// Apply a delegate function to an array. Return a new array
        /// </summary>
        public static double[] ApplyFunction(double[] to, Func<double, double> func)
        {
            double[] retVec = new double[to.Length];
            for (int i = 0; i < to.Length; i++)
            {
                retVec[i] = func(to[i]);
            }
            return retVec;
        }

        /// <summary>
        /// Return whether a value exits in an array.
        /// </summary>
        /// <typeparam name="T">Has to be comparable</typeparam>
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

        /// <summary>
        /// Return a random sample of a given size from a list
        /// </summary>
        public static List<T> RandomSample<T>(List<T> from, int sampleSize)
        {
            int counter = 0;
            int BatchSize = from.Count;
            int[] indexes = new int[Math.Min(BatchSize, sampleSize)]; // The indexes to be returned
            for (int i = 0; i < indexes.Length; i++)
            {
                indexes[i] = -1; // Initialize the indexes to -1
            }
            // Fill the indexes array with random indexes, that do not repeat
            while (counter < sampleSize && counter < BatchSize)
            {
                int index = random.Next(BatchSize);
                while (ExistsIn(indexes, index))
                    index = random.Next(BatchSize);
                indexes[counter] = index;
                counter++;
            }
            // Translate the indexes to the actual objects
            List<T> Sample = new List<T>();
            foreach(int index in indexes)
            {
                Sample.Add(from[index]);
            }
            return Sample;
        }

        /// <summary>
        /// Return the index of the biggest element
        /// </summary>
        /// <typeparam name="T">Has to be comparable</typeparam>
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

        /// <summary>
        /// Return the maximum value in an array
        /// </summary>
        /// <typeparam name="T">Has to be comparable</typeparam>
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

        /// <summary>
        /// Add a scalar to every element in an array
        /// </summary>
        public static void ScalarAdd(double[] toWhat, double what)
        {
            for(int i = 0; i < toWhat.Length; i++)
            {
                toWhat[i] += what;
            }
            
        }

        /// <summary>
        /// Divide every element in an array by a scalar
        /// </summary>
        public static void ScalarDivide(double[] what, double by)
        {
            for (int i = 0; i < what.Length; i++)
            {
                what[i] /= by;
            }
        }

        /// <summary>
        /// Add a scalar to every element
        /// </summary>
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

        /// <summary>
        /// Divide every element by a scalar
        /// </summary>
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

        /// <summary>
        /// Perform an addition on every element with its represtitive in the second array
        /// </summary>
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
        /// <summary>
        /// Perform a substitution on every element with its represtitive in the second array
        /// </summary>
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

        /// <summary>
        /// Comparer class that compares two controls by their click priority.
        /// </summary>
        public class ByPriority : IComparer<ControlBase>
        {
            public int Compare(ControlBase x, ControlBase y)
            {
                return x.ClickPriority.CompareTo(y.ClickPriority);
            }
        }

    }
}
