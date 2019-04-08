using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Microsoft.Xna.Framework;
using Helper;

namespace Network
{
    class NetworkVectors
    {
        List<List<double[]>> Weights;
        List<double[]> Activations;
        List<double[]> WeightedSums;
        Activation_Function ActivationFunction;

        public NetworkVectors(List<int> Dimensions, Activation_Function activation_Function, double initWeight)
        {
            // Check if parameters are alright
            if (Dimensions.Count <= 1)
                throw new SystemException("Invalid dimensions");
            foreach (int layer in Dimensions)
                if (layer <= 1)
                    throw new SystemException("Invalid dimensions");

            // Set the activation function
            ActivationFunction = activation_Function;

            Weights = new List<List<double[]>>();
            Activations = new List<double[]>();
            WeightedSums = new List<double[]>();
            // Create the Neural Network Shape according to Dimensions
            
            Activations.Add(Helper.Helper.CreateZero(Dimensions[0]));
            WeightedSums.Add(Helper.Helper.CreateZero(Dimensions[0]));
            for (int layer = 1; layer < Dimensions.Count; layer++)
            {
                Activations.Add(Helper.Helper.CreateZero(Dimensions[layer]));
                WeightedSums.Add(Helper.Helper.CreateZero(Dimensions[layer]));
                Weights.Add(new List<double[]>());
                for (int neuron = 0; neuron < Dimensions[layer]; neuron++) {
                    Weights[layer - 1].Add(Helper.Helper.CreateInitArray(Dimensions[layer-1], initWeight));
                }
            }

        }

        public void Print()
        {
            for (int layer = 0; layer < Activations.Count; layer++)
            {
                Console.WriteLine("Layer {0} neuron activations: {1}", layer, Helper.Helper.ArrayToString(Activations[layer]));
            }
        }

        public void Feed(double[] input)
        {
            Activations[0] = input;
            for (int layer = 1; layer < Activations.Count; layer++)
            {
                for (int neuron = 0; neuron < Activations[layer].Length; neuron++)
                {
                    WeightedSums[layer][neuron] = Helper.Helper.Dot(Activations[layer - 1], Weights[layer - 1][neuron]);
                    Activations[layer][neuron] = ActivationFunction.Function(WeightedSums[layer][neuron]);
                }
            }
        }
    }
}
