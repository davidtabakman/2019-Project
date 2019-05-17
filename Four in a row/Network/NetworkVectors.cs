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
        private Random random;

        /// <summary>
        /// Create a neural network of some dimensions and an initial weight
        /// </summary>
        /// <param name="Dimensions">The dimensions in a form like this: { 10 ,2, 5 ..., numberofneurons}</param>
        public NetworkVectors(List<int> Dimensions)
        {
            // Check if the dimensions are legit
            if (Dimensions.Count <= 1)
                throw new SystemException("Invalid dimensions");
            foreach (int layer in Dimensions)
                if (layer < 1)
                    throw new SystemException("Invalid dimensions");

            ActivationFunction = Activation_Functions.Sigmoid; // Set the activation function

            // Initialize all of the variables of the network
            this.Dimensions = Dimensions;
            Weights = new List<List<double[]>>();
            Activations = new List<double[]>();
            WeightedSums = new List<double[]>();
            Gradient = new List<List<double[]>>();

            // Create the Neural Network Shape according to Dimensions
            // Layer 0 - Activations and Weighted sums
            // Layer 1 (between the n and n+1 layer) - Weights and Gradients
            // Layer 1 - Activations and Weighted sums and so on.
            Activations.Add(CreateZero(Dimensions[0]));
            WeightedSums.Add(CreateZero(Dimensions[0]));
            random = new Random();
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
                        Weights[layer - 1][neuron][connection] = random.NextDouble() * 2 - 1; // Random double from -1 to 1 to initialize the weights
                    }
                }
            }

        }

        /// <summary>
        /// Print all the activations of all the layers.
        /// </summary>
        public void Print()
        {
            for (int layer = 0; layer < Activations.Count; layer++)
            {
                Console.WriteLine("Layer {0} neuron activations: {1}", layer, ArrayToString(Activations[layer]));
            }
        }

        /// <summary>
        /// Perform a feed-forward: weighted sum = weights * activations (dot product), activation = AFunc(weighted sum).
        /// </summary>
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

        /// <summary>
        /// Derivative of the cost function that works on arrays.
        /// </summary>
        public double[] CostDerivative(double[] expected, double[] activation)
        {
            return Sub(activation, expected);
        }

        /// <summary>
        /// Derivative of the cost function that works on single units.
        /// </summary>
        public double CostDerivative(double expected, double activation)
        {
            return activation - expected;
        }
        /// <summary>
        /// Performs a backpropagation sweep to calculate the gradient, using the chain rule, using sigmoid function as activation function
        /// </summary>
        public List<List<double[]>> GetGradient(double[] ExpectedOutput)
        {
            // Initialize the deltas matrix
            List<double[]> Deltas = new List<double[]>();
            for (int layer = 0; layer < Dimensions.Count; layer++)
            {
                Deltas.Add(CreateZero(Dimensions[layer]));
            }

            int lastLayer = Gradient.Count - 1;
            double[] Delta = new double[Gradient[lastLayer].Count];
            for (int neuron = 0; neuron < Gradient[lastLayer].Count; neuron++)
            {
                // According to the chain rule: dC/dW = dC/dA * dA/dZ * dZ/dW = dC/dA * sig'(Z) * A
                double ActDerivative = CostDerivative(ExpectedOutput[neuron], Activations[lastLayer + 1][neuron]); // dC/dA
                Delta[neuron] = ActDerivative * Activation_Functions.Sigmoid.Derivative(WeightedSums[lastLayer + 1][neuron]); // dC/dA * sig'(Z)
                for(int connection = 0; connection < Activations[lastLayer].Length; connection++)
                {
                    Gradient[lastLayer][neuron][connection] = Delta[neuron] * Activations[lastLayer][connection]; // dC/dA * sig'(Z) * A
                }
            }
            Deltas[lastLayer+1] = Delta;
            for (int layer = lastLayer; layer > 0; layer--)
            {
                for (int neuron = 0; neuron < Dimensions[layer]; neuron++)
                {
                    // According to the chain rule (in the middle of the network): dC/dW = dC/dZ+1 * dZ+1/dZ * dZ/dW =
                    // = dC/dZ+1 * W * sig'(Z) * A
                    double delta = 0;
                    for (int next_neuron = 0; next_neuron < Dimensions[layer+1]; next_neuron++)
                    {
                        delta += Deltas[layer+1][next_neuron] * Weights[layer][next_neuron][neuron]; // dC/dZ+1 * W
                    }
                    Deltas[layer][neuron] = delta * Activation_Functions.Sigmoid.Derivative(WeightedSums[layer][neuron]); // dC/dZ+1 * W * sig'(Z)
                    for (int connection = 0; connection < Dimensions[layer-1]; connection++)
                    {
                        Gradient[layer - 1][neuron][connection] = Activations[layer - 1][connection] * Deltas[layer][neuron]; // dC/dZ+1 * W * sig'(Z) * A
                    }
                }
            }
            return Gradient;
        }

        /// <summary>
        /// Supervised learning using stochastic gradient descent.
        /// </summary>
        /// <param name="Data">Dataset that consists of: Tuple(input, expected output).</param>
        /// <param name="EpocheNum">Number of times to iterate on the dataset.</param>
        public void SGD(List<Tuple<double[], double[]>> Data, double LearningRate, int EpocheNum)
        {
            // Initialize Gradients matrix to empty arrays
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
                // One iteration of SGD
                Zero(Gradients);
                // Sum the gradients
                foreach (Tuple<double[], double[]> DataPiece in Data)
                {
                    Feed(DataPiece.Item1);
                    Add(Gradients, GetGradient(DataPiece.Item2));
                }
                // Decrease the weights by the average of the gradients
                ScalarDivide(Gradients, Data.Count / LearningRate);

                Sub(Weights, Gradients);

            }
        }

        // Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Weights", Weights);
            info.AddValue("Gradient", Gradient);
            info.AddValue("Activation Function", ActivationFunction);
            info.AddValue("Activations", Activations);
            info.AddValue("Weighted Sums", WeightedSums);
            info.AddValue("Dimensions", Dimensions);
        }

        // Serialization constructor
        public NetworkVectors(SerializationInfo info, StreamingContext context)
        {
            Weights = (List<List<double[]>>)info.GetValue("Weights", typeof(List<List<double[]>>));
            Gradient = (List<List<double[]>>)info.GetValue("Gradient", typeof(List<List<double[]>>));
            ActivationFunction = (Activation_Function)info.GetValue("Activation Function", typeof(Activation_Function));
            Activations = (List<double[]>)info.GetValue("Activations", typeof(List<double[]>));
            WeightedSums = (List<double[]>)info.GetValue("Weighted Sums", typeof(List<double[]>));
            Dimensions = (List<int>)info.GetValue("Dimensions", typeof(List<int>));
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
