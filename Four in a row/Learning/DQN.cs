using Controller;
using Network;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static Helper.Helper;

namespace Learning
{
    [Serializable()]
    public class DQN : LearningBot
    {
        private List<int> Dimensions; // The neural network dimensions
        private const int ReplayMemSize = 10000;
        private List<Transition> ReplayMem; // Replay memory
        private double Epsilon; // The explore-exploit variable

        /// <summary>
        /// A transition structure that is used in the DQN Learning memory: state - action - reward - newstate.
        /// </summary>
        private struct Transition
        {
            public State s;
            public Actione a;
            public double Reward;
            public State s1; // New state

            public Transition(State s, Actione a, double Reward, State s1)
            {
                this.s = s;
                this.a = a;
                this.Reward = Reward;
                this.s1 = s1;
            }
        }

        private NetworkVectors NeuralNet; // The neural network that will be constantly updated
        private NetworkVectors OldNeuralNet; // The neural network that decisions will be taken by (polity iteration)

        public DQN() : base()
        {

        }

        /// <summary>
        /// Learning using reinforcement learning Q learning, that updates with gradient descent and keeps replay memory.
        /// </summary>
        /// <param name="EpocheNumber"></param>
        /// <param name="EpsilonLimit"></param>
        /// <param name="EpsilonDecrease">Decrease of epsilon every iteration</param>
        /// <param name="LearningRate"></param>
        /// <param name="DiscountRate"></param>
        /// <param name="against">optional, if not given opponent is random</param>
        public override void Learn(int EpocheNumber, double EpsilonLimit, double EpsilonDecrease, double LearningRate, double DiscountRate, Bot against = null)
        {
            IsLearning = true;
            int current_epoche = 0;
            int last_epcohe = 0;
            int SampleSize = 100; // Size of gradient descent sample
            int iterations = 0;
            List<Transition> miniBatch = null;
            List<Tuple<double[], double[]>> Q_Targets = new List<Tuple<double[], double[]>>(); 
            List<List<double[]>> Gradients = new List<List<double[]>>();

            // Initialize the Gradients matrix
            for (int layer = 1; layer < Dimensions.Count; layer++)
            {
                Gradients.Add(new List<double[]>());
                for (int neuron = 0; neuron < Dimensions[layer]; neuron++)
                    Gradients[Gradients.Count - 1].Add(CreateInitArray(Dimensions[layer - 1], 0));
            }

            // Initialize variables for tracking the progress
            games = 0;
            wins = 0;
            losses = 0;
            draws = 0;

            // Observe state and declare variables for learning
            State state = Control.GetState();
            State newState;
            double reward = 0;
            Actione action;
            double loss = 0;

            OldNeuralNet = (NetworkVectors)NeuralNet.Clone();
            double[] Target = new double[NeuralNet.WeightedSums[NeuralNet.Activations.Count - 1].Length];
            double[] Result = new double[NeuralNet.WeightedSums[NeuralNet.Activations.Count - 1].Length];
            double[] target = new double[1];
            double addLoss = 0;

            while (current_epoche < EpocheNumber && IsLearning)
            {

                if (BotTurn != Control.CurrTurn) // If its not this learningbot's turn in the control
                {
                    BotMove(against);
                }

                state = Control.GetState(); // Observe state

                action = TakeEpsilonGreedyAction(Epsilon, state, rand); // Take action

                if (!Control.IsTerminalState())
                {
                    BotMove(against);
                }

                Track(); // Update the tracking variables according to the current state of the game

                // Get reward and observe new state
                reward = Control.GetReward(BotTurn);
                newState = Control.GetState();

                // Store the transition in Replay Memory
                if (ReplayMem.Count >= ReplayMemSize)
                {
                    ReplayMem.RemoveAt(0);

                }
                ReplayMem.Add(new Transition(state, action, reward, newState));

                // If the x-th iteration, switch the old neural network with the new one
                if (iterations % 100 == 0 && iterations != 0)
                {
                    NeuralNet.Copy(OldNeuralNet);
                }
                iterations++;

                // Sample random mini-batch of transitions from Replay Memory
                miniBatch = RandomSample(ReplayMem, SampleSize);

                // Compute Q-Learning targets
                Q_Targets.Clear();

                Zero(Gradients);
                addLoss = 0;
                foreach (Transition transition in miniBatch)
                {
                    int maxID = getMaxAction(Control, transition.s1, false).ID; // Get the best action of the new state
                    OldNeuralNet.Feed(CreateInputArray(transition.s1.Board, maxID)); // Compute its Q value

                    // Bellman equation: Q += (Q' * DiscountRate + R) * learning rate
                    double t = 0;
                    if (Control.IsTerminalState(transition.s1))
                    {
                        t = transition.Reward;
                    }
                    else
                    {
                        t = transition.Reward + DiscountRate * OldNeuralNet.WeightedSums[OldNeuralNet.Activations.Count - 1][0];
                    }

                    target[0] = t;
                    Q_Targets.Add(new Tuple<double[], double[]>(CreateInputArray(transition.s.Board, transition.a.ID), ApplyFunction(target, Activation_Functions.Sigmoid.Function)));
                    OldNeuralNet.Feed(CreateInputArray(transition.s.Board, transition.a.ID));
                    addLoss += 0.5 * Math.Pow(ApplyFunction(target, Activation_Functions.Sigmoid.Function)[0] - OldNeuralNet.Activations[NeuralNet.Activations.Count - 1][0], 2);
                }
                addLoss /= miniBatch.Count;
                loss += addLoss;

                // SGD
                NeuralNet.SGD(Q_Targets, LearningRate, 1);

                // Adjust learning variables
                if (Epsilon > EpsilonLimit)
                    Epsilon -= EpsilonDecrease;

                // Report the progress
                if (current_epoche % 1000 == 0 && current_epoche != last_epcohe)
                {
                    last_epcohe = current_epoche;
                    Console.WriteLine("Learning percentage: {0}%, win rate: {1}%, loss rate: {3}%, draw rate: {4}%, avg cost: {2}", current_epoche / (double)EpocheNumber * 100, (double)wins * 100 / games, loss / 1000, (double)losses * 100 / games, (double)draws * 100 / games);
                    wins = 0;
                    draws = 0;
                    losses = 0;
                    loss = 0;
                    games = 0;
                }

                current_epoche++;
                Control.Clean(); // Make the control ready for another move

            }
            IsLearning = false;
        }

        /// <summary>
        /// Go over all actions and return the one with the highest Q value.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="state">The state of control to use.</param>
        /// <param name="isLegal">If true, the returned action has to be considered legal in the control.</param>
        /// <returns></returns>
        protected override Actione getMaxAction(GameControlBase control, State state, bool isLegal)
        {
            int maxID = 0;
            double max = 0;
            // Regular max searching by value
            for (int id = 0; id < control.ActionNum; id++)
            {
                NeuralNet.Feed(CreateInputArray(state.Board, id));
                
                    if (NeuralNet.Activations[NeuralNet.Activations.Count - 1][0] > max)
                    { 
                        if (isLegal)
                        {
                            if(control.IsLegalAction(new Actione(id)))
                            {
                                maxID = id;
                                max = NeuralNet.Activations[NeuralNet.Activations.Count - 1][0];
                            }
                        } else
                        {
                            maxID = id;
                            max = NeuralNet.Activations[NeuralNet.Activations.Count - 1][0];
                        }
                    }
                
            }
            return new Actione(maxID);
        }
            
        /// <summary>
        /// Transform a two dimensional baord array to a one dimensional input array for a neural network of a specific shape
        /// </summary>
        /// <param name="board"></param>
        /// <returns>Each tile corresponds to three input neurons: first is Player1, second is Player2, third is NoPlayer.
        /// </returns>
        private double[] CreateInputArray(double [,] board, int actionID)
        {
            int x_len = board.GetLength(0);
            int y_len = board.GetLength(1);

            double[] Input = CreateZero(x_len * y_len * 3 + Control.ActionNum);
            Input[x_len * y_len * 3 + actionID] = 1; // Fire the correct action neuron, all others are zeroes
            for (int x = 0; x < x_len; x++)
            {
                for (int y = 0; y < y_len; y++)
                {
                    if (board[x, y] == (int)GameControlBase.Players.Player1)
                        Input[(x * y_len + y) * 3] = 1;
                    else if(board[x, y] == (int)GameControlBase.Players.Player2)
                        Input[(x * y_len + y) * 3 + 1] = 1;
                    else
                        Input[(x * y_len + y) * 3 + 2] = 1;
                }
            }

            return Input;
        }

        /// <summary>
        /// Initialize some learning and technical variables and ready the bot for learning.
        /// Has to be called before Learn()
        /// </summary>
        /// <param name="control"></param>
        /// <param name="player">The player that the network will be playing</param>
        public override void Setup(GameControlBase control, GameControlBase.Players player)
        {
            base.Setup(control, player);
            rand = new Random();
            Dimensions = new List<int> { control.FeatureNum + control.ActionNum, 130, 70, 1 };
            if(NeuralNet == null)
                NeuralNet = new NetworkVectors(Dimensions);
            OldNeuralNet = (NetworkVectors)NeuralNet.Clone();
            Control = control;
            BotTurn = player;
            ReplayMem = new List<Transition>();
            Epsilon = 1;
        }

        public void SetNetwork(NetworkVectors newNet)
        {
            NeuralNet = newNet;
        }

        public NetworkVectors GetNetwork()
        {
            return NeuralNet;
        }

        // For serialization
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Network", NeuralNet);
            info.AddValue("Player", BotTurn);
        }

        // Constructor for serialization
        public DQN(SerializationInfo info, StreamingContext context)
        {
            NeuralNet = (NetworkVectors)info.GetValue("Network", typeof(NetworkVectors));
            BotTurn = (GameControlBase.Players)info.GetValue("Player", typeof(GameControlBase.Players));
        }
    }
}