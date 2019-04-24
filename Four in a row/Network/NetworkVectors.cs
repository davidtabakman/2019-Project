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
using System.IO;

namespace Network
{
    [Serializable()]
    public class NetworkVectors : ISerializable, ICloneable
    {
        public List<List<double[]>> Weights { get; }
        public List<double[]> Activations { get; }
        public List<double[]> WeightedSums { get; }
        private List<List<double[]>> Gradient;
        private Activation_Function ActivationFunction { get; }
        private List<int> Dimensions { get; }

        public NetworkVectors(List<int> Dimensions, double initWeight)
        {
            // Check if parameters are alright
            if (Dimensions.Count <= 1)
                throw new SystemException("Invalid dimensions");
            foreach (int layer in Dimensions)
                if (layer < 1)
                    throw new SystemException("Invalid dimensions");

            // Set the activation function
            ActivationFunction = Activation_Functions.Sigmoid;

            this.Dimensions = Dimensions;

            Weights = new List<List<double[]>>();
            Activations = new List<double[]>();
            WeightedSums = new List<double[]>();
            Gradient = new List<List<double[]>>();
            // Create the Neural Network Shape according to Dimensions
            
            Activations.Add(CreateZero(Dimensions[0]));
            WeightedSums.Add(CreateZero(Dimensions[0]));
            Random random = new Random();
            for (int layer = 1; layer < Dimensions.Count; layer++)
            {
                Activations.Add(CreateZero(Dimensions[layer]));
                WeightedSums.Add(CreateZero(Dimensions[layer]));
                Weights.Add(new List<double[]>());
                Gradient.Add(new List<double[]>());
                for (int neuron = 0; neuron < Dimensions[layer]; neuron++) {
                    Gradient[layer-1].Add(CreateInitArray(Dimensions[layer - 1], 0));
                    Weights[layer - 1].Add(new double[Dimensions[layer - 1]]);
                    for (int connection = 0; connection < Dimensions[layer - 1]; connection++)
                    {
                        Weights[layer - 1][neuron][connection] = random.NextDouble() * 2 - 1; // Random double from -1 to 1
                    }
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

        public double CostDerivative(double expected, double activation)
        {
            return activation - expected;
        }
        /// <summary>
        /// Performs a backpropagation sweep to calculate the gradient, using the chain rule, using sigmoid function as activation function
        /// </summary>
        /// <returns></returns>
        public List<List<double[]>> GetGradient(double[] ExpectedOutput)
        {
            List<double[]> Deltas = new List<double[]>();
            Deltas.Add(CreateZero(Dimensions[0]));
            Random random = new Random();
            for (int layer = 1; layer < Dimensions.Count; layer++)
            {
                Deltas.Add(CreateZero(Dimensions[layer]));
            }
            int lastLayer = Gradient.Count - 1;
            double[] Delta = new double[Gradient[lastLayer].Count];
            for (int neuron = 0; neuron < Gradient[lastLayer].Count; neuron++)
            {
                double ActDerivative = CostDerivative(ExpectedOutput[neuron], Activations[lastLayer + 1][neuron]);
                Delta[neuron] = ActDerivative * Activation_Functions.Sigmoid.Derivative(WeightedSums[lastLayer + 1][neuron]);
                for(int connection = 0; connection < Activations[lastLayer].Length; connection++)
                {
                    Gradient[lastLayer][neuron][connection] = Delta[neuron] * Activations[lastLayer][connection];
                }
            }
            Deltas[lastLayer+1] = Delta;
            for (int layer = lastLayer; layer > 0; layer--)
            {
                for (int neuron = 0; neuron < Dimensions[layer]; neuron++)
                {
                    double delta = 0;
                    for (int next_neuron = 0; next_neuron < Dimensions[layer+1]; next_neuron++)
                    {
                        delta += Deltas[layer+1][next_neuron] * Weights[layer][next_neuron][neuron];
                    }
                    Deltas[layer][neuron] = delta * Activation_Functions.Sigmoid.Derivative(WeightedSums[layer][neuron]);
                    for (int connection = 0; connection < Dimensions[layer-1]; connection++)
                    {
                        Gradient[layer - 1][neuron][connection] = Activations[layer - 1][connection] * Deltas[layer][neuron];
                    }
                }
            }
            return Gradient;
        }

        public void SGD(List<Tuple<double[], double[]>> Data, double LearningRate, int EpocheNum)
        {
            List<List<double[]>> Gradients = new List<List<double[]>>();
            for(int layer = 0; layer < Weights.Count; layer++)
            {
                Gradients.Add(new List<double[]>());
                for (int neuron = 0; neuron < Weights[layer].Count; neuron++)
                {
                    Gradients[layer].Add(new double[Weights[layer][neuron].Length]);
                }
            }
            for (int i = 0; i < EpocheNum; i++)
            {
                Zero(Gradients);
                foreach (Tuple<double[], double[]> DataPiece in Data)
                {
                    Feed(DataPiece.Item1);
                    Add(Gradients, GetGradient(DataPiece.Item2));
                }

                ScalarDivide(Gradients, Data.Count / LearningRate);

                Sub(Weights, Gradients);

            }
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

        public object Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (GetType().IsSerializable)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    stream.Position = 0;
                    return formatter.Deserialize(stream);
                }
                return null;
            }

        }
    }
}
