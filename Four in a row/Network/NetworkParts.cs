using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    class Pulse
    {
        public double Value { get; set; }
    }

    class Connection
    {
        public Pulse InputPulse { get; set; }
        public double Weight { get; set; }
        public bool isLearnable { get; set; }
    }

    class Neuron
    {

        public List<Connection> InputConnections { get; set; }
        public Pulse OutputPulse { get; set; }
        public double WSum { get; private set; }
        public Func<double, double> ActivationFunc { get; set; }

        public Neuron(Func<double, double> activationFunc)
        {
            ActivationFunc = activationFunc;
            InputConnections = new List<Connection>();
            OutputPulse = new Pulse();
        }

        public double WeightedSum()
        {
            double sum = 0;
            foreach(Connection connection in InputConnections)
            {
                sum += connection.Weight * connection.InputPulse.Value;
            }

            return sum;
        }

        public double Activation(double sum, Func<double, double> activationFunc)
        {
            return activationFunc(sum);
        }

        public void Output()
        {
            WSum = WeightedSum();

            OutputPulse.Value = Activation(WSum, ActivationFunc);
        }
    }

    class NeuronLayer
    {
        public List<Neuron> Neurons { get; set; }

        public NeuronLayer(int numOfNeurons, double initWeight, Func<double, double> ActivationFunc)
        {
            Neurons = new List<Neuron>();
            for (int i = 0; i < numOfNeurons; i++)
            {
                Neuron newNeuron = new Neuron(ActivationFunc);
                
                Neurons.Add(new Neuron(Activation_Functions.Default));
            }
        }

        public void Output()
        {
            foreach (Neuron n in Neurons)
                n.Output();
        }
    }
}
