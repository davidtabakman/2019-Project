using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    [Serializable()]
    public class Activation_Function // Activation function class
    {
        /// <summary>
        /// Create an Activation Function class, with an ID, a function and its derivative.
        /// </summary>
        public Activation_Function(int ID, Func<double, double> Function, Func<double, double> Derivative)
        {
            this.ID = ID;
            this.Function = Function;
            this.Derivative = Derivative;
        }

        public int ID { get; }
        // Deligate functions
        public Func<double, double> Function { get; } 

        public Func<double, double> Derivative { get; }
    }

    public static class Activation_Functions
    {
        // y=x
        public static double DefaultFunction(double input) => input;

        public static double DefaultDerivative(double input) => 1;
        // y=1/(1+e^-x)
        public static double SigmoidFunction(double input) => 1 / (1 + Math.Pow(Math.E, -input));

        public static double SigmoidDerivative(double input) => SigmoidFunction(input) * (1 - SigmoidFunction(input));

        public static Activation_Function Default = new Activation_Function(0, DefaultFunction, DefaultDerivative);

        public static Activation_Function Sigmoid = new Activation_Function(1, SigmoidFunction, SigmoidDerivative);

        public static Dictionary<int, Activation_Function> Functions = new Dictionary<int, Activation_Function>() {
            { Default.ID, Default },
            { Sigmoid.ID, Sigmoid }
        };

    }
}
