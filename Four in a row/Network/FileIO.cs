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
        /// Save a neural network under a certain name in the program's folder.
        /// </summary>
        public static void SaveSerializable(NetworkVectors net, string NetName)
        {
            string path = NetName + FileExtention;
            try
            {
                Stream saveFile = File.Open(path, FileMode.Create); // Open the save file
                // Serialize and save the file
                BinaryFormatter binaryFormatter = new BinaryFormatter(); 
                binaryFormatter.Serialize(saveFile, net);
                saveFile.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error saving the serializable object: {0}", e.Message);
            }
            
        }

        /// <summary>
        /// Loads a neural network with a specific name from the program's folder
        /// </summary>
        public static NetworkVectors LoadNetworkVectors(string NetName)
        {
            string path = NetName + FileExtention;
            try
            {
                Stream saveFile = File.Open(path, FileMode.Open); // Open the save file
                // Load, deserialize and return the network
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

        /// <summary>
        /// Save a Learning Bot under a certain name in the program's folder.
        /// </summary>
        public static void SaveLearningBot(string BotName, Learning.LearningBot bot)
        {
            string path = BotName + FileExtention;
            try
            {
                Stream saveFile = File.Open(path, FileMode.Create); // Open the save file
                // Serialize and save the file
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(saveFile, bot);
                saveFile.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error saving the serializable object: {0}", e.Message);
            }
        }

        /// <summary>
        /// Loads a Learning Bot with a specific name from the program's folder
        /// </summary>
        public static Learning.LearningBot LoadLearningBot(string NetName)
        {
            string path = NetName + FileExtention;
            try
            {
                Stream saveFile = File.Open(path, FileMode.Open); // Open the save file
                // Load, deserialize and return the bot
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