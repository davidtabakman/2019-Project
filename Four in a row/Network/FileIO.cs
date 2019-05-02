using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Network
{
    public class NetworkLoader
    {
        private const string FileExtention = ".dtp";

        /// <summary>
        /// Saves a neural network with a given name as a .dtp file.
        /// </summary>
        /// <param name="net"></param>
        /// <param name="NetName"></param>
        public static void SaveNetwork(Network net, string NetName)
        {
            // Creates a header of this shape (string): ActivationFuncID FirstLayerNeuronNum SecondLayerNeuronNum ... LastLayerNeuronNum
            NetName += FileExtention;
            string header = "";
            header += net.Activation.ID;
            header += " ";
            for (int i = 0; i < net.Layers.Count - 1; i++)
            {
                header += net.Layers[i].Neurons.Count;
                header += " ";
            }
            header += net.Layers[net.Layers.Count - 1].Neurons.Count;
            // Tries to save the file
            try
            {
                FileInfo fi = new FileInfo(NetName);
                StreamWriter w = fi.CreateText();
                // Writes the header
                w.WriteLine(header);
                w.WriteLine();
                string currLayer = "";
                // Writes all the wieghts, each layer on its line
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

        /// <summary>
        /// Loads a neural network file (.dtp)
        /// </summary>
        /// <param name="NetName">The network name, does not include the ending (.dtp)</param>
        /// <returns>The neural network</returns>
        public static Network LoadNetwork(string NetName)
        {
            NetName += FileExtention;
            try
            {
                StreamReader r = File.OpenText(NetName);
                // Reades the header and translates it
                string header = r.ReadLine();
                List<int> headerTranslate = StringToIntList(header);
                Activation_Function function = Activation_Functions.Functions[headerTranslate[0]];
                // Makes it dimensions only
                headerTranslate.RemoveAt(0);
                Network retNet = new Network(headerTranslate, function, 0.0);
                r.ReadLine();
                // Extracts the weights
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

        /// <summary>
        /// Converts a string to a list of integers, splitting the string by spaces. 
        /// Assumes input is correct.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Converts a string to a list of doubles, splitting the string by spaces. 
        /// Assumes input is correct.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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

        public static void SaveSerializable(NetworkVectors net, string NetName)
        {
            string path = NetName + FileExtention;

            try
            {
                Stream saveFile = File.Open(path, FileMode.Create);

                BinaryFormatter binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(saveFile, net);

                saveFile.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error saving the serializable object: {0}", e.Message);
            }
            
        }

        public static NetworkVectors LoadNetworkVectors(string NetName)
        {
            string path = NetName + FileExtention;

            try
            {
                Stream saveFile = File.Open(path, FileMode.Open);

                BinaryFormatter binaryFormatter = new BinaryFormatter();

                NetworkVectors networkVectors = (NetworkVectors)binaryFormatter.Deserialize(saveFile);

                saveFile.Close();

                return networkVectors;
            }
            catch
            {
                Console.WriteLine("Error saving the serializable object");
                return null;
            }
        }

        public static void SaveLearningBot(string BotName, Learning.LearningBot bot)
        {
            string path = BotName + FileExtention;

            try
            {
                Stream saveFile = File.Open(path, FileMode.Create);

                BinaryFormatter binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(saveFile, bot);

                saveFile.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error saving the serializable object: {0}", e.Message);
            }
        }

        public static Learning.LearningBot LoadLearningBot(string NetName)
        {
            string path = NetName + FileExtention;

            try
            {
                Stream saveFile = File.Open(path, FileMode.Open);

                BinaryFormatter binaryFormatter = new BinaryFormatter();

                Learning.LearningBot bot = (Learning.LearningBot)binaryFormatter.Deserialize(saveFile);

                saveFile.Close();

                return bot;
            }
            catch
            {
                Console.WriteLine("Error loading the serializable object");
                return null;
            }
        }
    }
}