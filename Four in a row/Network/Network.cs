using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    class Network
    {
        List<NeuronLayer> Layers { get; set; }

        public Network(List<int> Dimensions, Func<double, double> ActivationFunc)
        {
            Layers = new List<NeuronLayer>();

            if (Dimensions.Count() > 0)
            {
                Layers.Add(new NeuronLayer(Dimensions[0], 0.0, ActivationFunc));
                for (int LayerNum = 1; LayerNum < Dimensions.Count(); LayerNum++)
                {
                    int LastLayerSize = Layers[LayerNum - 1].Neurons.Count;
                    Layers.Add(new NeuronLayer(Dimensions[LayerNum], 0.0, ActivationFunc));
                    foreach (Neuron nto in Layers[LayerNum].Neurons)
                    {
                        foreach (Neuron nfrom in Layers[LayerNum - 1].Neurons)
                        {
                            Connection addingConnection = new Connection();
                            addingConnection.InputPulse = nfrom.OutputPulse;
                            addingConnection.Weight = 0.0;
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
    }
}