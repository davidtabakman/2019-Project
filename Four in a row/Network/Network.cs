using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    public class Network
    {
        public List<NeuronLayer> Layers { get; set; }

        public Network(List<int> Dimensions, Func<double, double> ActivationFunc, double initWeight)
        {
            Layers = new List<NeuronLayer>();

            if (Dimensions.Count() > 0)
            {
                Layers.Add(new NeuronLayer(Dimensions[0], initWeight, ActivationFunc));
                for (int LayerNum = 1; LayerNum < Dimensions.Count(); LayerNum++)
                {
                    int LastLayerSize = Layers[LayerNum - 1].Neurons.Count;
                    Layers.Add(new NeuronLayer(Dimensions[LayerNum], initWeight, ActivationFunc));
                    foreach (Neuron nto in Layers[LayerNum].Neurons)
                    {
                        foreach (Neuron nfrom in Layers[LayerNum - 1].Neurons)
                        {
                            Connection addingConnection = new Connection();
                            addingConnection.InputPulse = nfrom.OutputPulse;
                            addingConnection.Weight = initWeight;
                            nto.InputConnections.Add(addingConnection);
                        }
                    }
                }
            }
        }

        public void Print()
        {
            for(int layer = 0; layer < Layers.Count; layer++)
            {
                Console.WriteLine("Layer {0} has {1} nuerons.", layer, Layers[layer].Neurons.Count);
            }
        }

        public void Feed(int[] input)
        {
            if (input.Length == Layers[0].Neurons.Count)
            {
                for (int n = 0; n < input.Length; n++)
                {
                    Layers[0].Neurons[n].OutputPulse.Value = input[n];
                }
                for (int l = 1; l < Layers.Count; l++)
                {
                    Layers[l].Output();
                }
                for (int n = 0; n < Layers[Layers.Count - 1].Neurons.Count; n++)
                {
                    Console.WriteLine(Layers[Layers.Count - 1].Neurons[n].OutputPulse.Value);
                }
            }
        }
    }
}