using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    class Activation_Functions
    {
        public static double Default(double input) => input;

        public static double DefaultDerivative(double input) => 1;

        public static double Sigmoid(double input) => 1 / (1 + Math.Pow(Math.E, -input));

        public static double SigmoidDerivative(double input) => Sigmoid(input) * (1 - Sigmoid(input));
    }
}
