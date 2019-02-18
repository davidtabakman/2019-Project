using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Network
{
    public class NetworkLoader
    {
        private const string NetworkFileExtention = ".dtp";

        public static void SaveNetwork(Network net, string NetName)
        {
            NetName += NetworkFileExtention;
            string header = "";
            for (int i = 0; i < net.Layers.Count - 1; i++)
            {
                header += net.Layers[i].Neurons.Count;
                header += " ";
            }
            header += net.Layers[net.Layers.Count - 1].Neurons.Count;
            try
            {
                FileInfo fi = new FileInfo(NetName);
                StreamWriter w = fi.CreateText();
                w.WriteLine(header);
                w.WriteLine();
                string currLayer = "";
                for (int x = 1; x < net.Layers.Count; x++)
                {
                    foreach (Neuron neuron in net.Layers[x].Neurons)
                    {
                        for (int i = 0; i < neuron.InputConnections.Count; i++)
                        {
                            currLayer += neuron.InputConnections[i].Weight;
                            currLayer += " ";
                        }

                    }
                    currLayer = currLayer.Remove(currLayer.Length - 1);
                    w.WriteLine(currLayer);
                    currLayer = "";
                }
                w.Close();
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static Network LoadNetwork(string NetName)
        {
            NetName += NetworkFileExtention;
            try
            {
                StreamReader r = File.OpenText(NetName);
                string header = r.ReadLine();
                Network retNet = new Network(StringToIntList(header), Activation_Functions.Sigmoid, 0.0);
                r.ReadLine();
                string readWeights;
                int currLayer = 1;
                List<double> layerWeights;
                while ((readWeights = r.ReadLine()) != null)
                {
                    layerWeights = StringToDoubleList(readWeights);
                    int lastLayerCount = retNet.Layers[currLayer - 1].Neurons.Count;
                    int thisLayerCount = retNet.Layers[currLayer].Neurons.Count;
                    for (int neuron = 0; neuron < thisLayerCount; neuron++)
                    {
                        for (int connection = 0; connection < lastLayerCount; connection++)
                        {
                            retNet.Layers[currLayer].Neurons[neuron].InputConnections[connection].Weight = layerWeights[lastLayerCount * neuron + connection];
                        }
                    }
                    currLayer++;
                }
                r.Close();
                return retNet;
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private static List<int> StringToIntList(string input)
        {
            string[] splitString = input.Split(' ');
            List<int> dimentions = new List<int>();
            foreach(string layer in splitString)
            {
                int layerCount = int.Parse(layer);
                dimentions.Add(layerCount);
            }
            return dimentions;
        }

        private static List<double> StringToDoubleList(string input)
        {
            string[] splitString = input.Split(' ');
            List<double> retList = new List<double>();
            foreach (string layer in splitString)
            {
                double num = double.Parse(layer);
                retList.Add(num);
            }
            return retList;
        }
    }
}