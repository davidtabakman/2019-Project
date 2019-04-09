using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Microsoft.Xna.Framework;
using static Helper.Helper;

namespace Network
{
    [Serializable()]
    public class NetworkVectors : ISerializable
    {
        public List<List<double[]>> Weights { get; }
        public List<double[]> Activations { get; }
        public List<double[]> WeightedSums { get; }
        private List<List<double[]>> Gradient;
        private Activation_Function ActivationFunction { get; }

        public NetworkVectors(List<int> Dimensions, double initWeight)
        {
            // Check if parameters are alright
            if (Dimensions.Count <= 1)
                throw new SystemException("Invalid dimensions");
            foreach (int layer in Dimensions)
                if (layer <= 1)
                    throw new SystemException("Invalid dimensions");

            // Set the activation function
            ActivationFunction = Activation_Functions.Sigmoid;

            Weights = new List<List<double[]>>();
            Activations = new List<double[]>();
            WeightedSums = new List<double[]>();
            Gradient = new List<List<double[]>>();
            // Create the Neural Network Shape according to Dimensions
            
            Activations.Add(CreateZero(Dimensions[0]));
            WeightedSums.Add(CreateZero(Dimensions[0]));
            for (int layer = 1; layer < Dimensions.Count; layer++)
            {
                Activations.Add(CreateZero(Dimensions[layer]));
                WeightedSums.Add(CreateZero(Dimensions[layer]));
                Weights.Add(new List<double[]>());
                Gradient.Add(new List<double[]>());
                for (int neuron = 0; neuron < Dimensions[layer]; neuron++) {
                    Gradient[layer-1].Add(CreateInitArray(Dimensions[layer - 1], 0));
                    Weights[layer - 1].Add(CreateInitArray(Dimensions[layer-1], initWeight));
                }
            }

        }

        public void Print()
        {
            for (int layer = 0; layer < Activations.Count; layer++)
            {
                Console.WriteLine("Layer {0} neuron activations: {1}", layer, ArrayToString(Activations[layer]));
            }
        }

        public void Feed(double[] input)
        {
            Activations[0] = input;
            for (int layer = 1; layer < Activations.Count; layer++)
            {
                for (int neuron = 0; neuron < Activations[layer].Length; neuron++)
                {
                    WeightedSums[layer][neuron] = Dot(Activations[layer - 1], Weights[layer - 1][neuron]);
                    Activations[layer][neuron] = ActivationFunction.Function(WeightedSums[layer][neuron]);
                }
            }
        }

        public double[] CostDerivative(double[] expected, double[] activation)
        {
            return Sub(activation, expected);
        }

        /// <summary>
        /// Performs a backpropagation sweep to calculate the gradient, using the chain rule, using sigmoid function as activation function
        /// </summary>
        /// <returns></returns>
        public List<List<double[]>> GetGradient(double[] ExpectedOutput)
        {
            int lastLayer = Gradient.Count - 1;
            double[] Error = new double[Gradient[lastLayer - 1].Count];
            for (int neuron = 0; neuron < Gradient[lastLayer].Count; neuron++)
            {
                double[] ActDerivative = CostDerivative(ExpectedOutput, Activations[lastLayer + 1]);
                Error = ScalarMultiply(ApplyFunciton(WeightedSums[lastLayer + 1], Activation_Functions.Sigmoid.Derivative), ActDerivative[neuron]);
                for(int connection = 0; connection < Activations[lastLayer].Length; connection++)
                {
                    Gradient[lastLayer][neuron][connection] = Activations[lastLayer][connection] * Error[neuron];
                }
            }
            for (int layer = lastLayer; layer >= 0; layer--)
            {
                for (int neuron = 0; neuron < Gradient[lastLayer].Count; neuron++)
                {
                    double[] SigmoidedDerived = ApplyFunciton(WeightedSums[layer], Activation_Functions.Sigmoid.Derivative);
                    Error = Multiply(SigmoidedDerived, Weights[layer][neuron]);
                    for (int connection = 0; connection < Activations[layer].Length; connection++)
                    {
                        Gradient[layer][neuron][connection] = Activations[layer][connection] * Error[neuron];
                    }
                }
            }
            return Gradient;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Weights", Weights);
            info.AddValue("Gradient", Gradient);
            info.AddValue("Activation Function", ActivationFunction);
            info.AddValue("Activations", Activations);
            info.AddValue("Weighted Sums", WeightedSums);
        }

        public NetworkVectors(SerializationInfo info, StreamingContext context)
        {
            Weights = (List<List<double[]>>)info.GetValue("Weights", typeof(List<List<double[]>>));
            Gradient = (List<List<double[]>>)info.GetValue("Gradient", typeof(List<List<double[]>>));
            ActivationFunction = (Activation_Function)info.GetValue("Activation Function", typeof(Activation_Function));
            Activations = (List<double[]>)info.GetValue("Activations", typeof(List<double[]>));
            WeightedSums = (List<double[]>)info.GetValue("Weighted Sums", typeof(List<double[]>));
        }
    }
}
