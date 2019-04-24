using Controller;
using Network;
using System;
using System.Collections.Generic;
using static Helper.Helper;

namespace Learning
{
    public class DQN : LearningBot
    {
        private static List<int> Dimensions = new List<int> { 4, 3, 2 };
        private const int ReplayMemSize = 10000;

        private struct Transition
        {
            public State s;
            public Actione a;
            public double Reward;
            public State s1;

            public Transition(State s, Actione a, double Reward, State s1)
            {
                this.s = s;
                this.a = a;
                this.Reward = Reward;
                this.s1 = s1;
            }
        }

        private struct TargetResult
        {
            public double Target;
            public double Result;

            public TargetResult(double t, double r)
            {
                Target = t;
                Result = r;
            }
        }

        private NetworkVectors NeuralNet;
        private NetworkVectors OldNeuralNet;

        private Random rand;
        private GameControlBase Control;
        private List<Transition> ReplayMem;

        private void BotMove(Bot against)
        {
            Actione botAction;
            if (!Control.IsTerminalState())
            {
                if (against != null)
                    against.TakeAction(Control, Control.GetState());
                else
                {
                    botAction = new Actione(rand.Next(Control.ActionNum));
                    while (!Control.IsLegalAction(botAction))
                        botAction = new Actione(rand.Next(Control.ActionNum));
                    Control.DoAction(botAction);
                }
            }
        }

        public override void Learn(int EpocheNumber, double EpsilonLimit, double EpsilonDecrease, double LearningRate, double DiscountRate, Bot against = null)
        {
            IsLearning = true;
            int epoche_move_limit = 20;
            int current_epoche = 0;
            int last_epcohe = 0;
            int SampleSize = 100;
            int iterations = 0;
            List<Transition> miniBatch = null;
            List<Tuple<double[], double[]>> Q_Targets = new List<Tuple<double[], double[]>>();
            List<List<double[]>> Gradients = new List<List<double[]>>();

            for (int layer = 1; layer < Dimensions.Count; layer++)
            {
                Gradients.Add(new List<double[]>());
                for (int neuron = 0; neuron < Dimensions[layer]; neuron++)
                    Gradients[Gradients.Count - 1].Add(CreateInitArray(Dimensions[layer - 1], 0));
            }

            double epsilon = 1.0f; // exploration / exploitation

            int games = 0;
            int wins = 0;
            int losses = 0;
            int draws = 0;

            // Observe state
            State state = Control.GetState();
            State newState;
            double reward = 0;
            Actione action;
            double loss = 0;

            while (current_epoche < EpocheNumber && IsLearning)
            {

                if (BotTurn != Control.CurrTurn)
                {
                    BotMove(against);
                }

                // Observe state
                state = Control.GetState();

                // Take action
                if (rand.NextDouble() <= epsilon)
                {
                    action = new Actione(rand.Next(Control.ActionNum));
                }
                else
                {
                    action = getMaxAction(Control, state);
                }
                Control.DoAction(action);

                // Bot takes action if possible (at the moment, the bot is random)
                if (!Control.IsTerminalState())
                {
                    BotMove(against);
                }
                if (Control.IsTerminalState())
                {
                    GameControlBase.Players Winner = Control.CheckWin();
                    if (Winner == BotTurn)
                    {
                        wins++;
                        games++;
                    }
                    else if (Winner == GameControlBase.Players.NoPlayer)
                    {
                        draws++;
                        games++;
                    }
                    else
                    {
                        games++;
                        losses++;
                    }
                }


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
                    OldNeuralNet = (NetworkVectors)NeuralNet.Clone();
                }
                iterations++;

                // Sample random mini-batch of transitions from Replay Memory
                miniBatch = RandomSample(ReplayMem, SampleSize);

                // Compute Q-Learning targets
                Q_Targets.Clear();
                double[] Target = new double[NeuralNet.WeightedSums[NeuralNet.Activations.Count - 1].Length];
                double[] Result = new double[NeuralNet.WeightedSums[NeuralNet.Activations.Count - 1].Length];
                Zero(Gradients);

                double addLoss = 0;
                foreach (Transition transition in miniBatch)
                {
                    int maxID = getMaxAction(Control, transition.s1).ID;
                    OldNeuralNet.Feed(CreateInputArray(transition.s1.Board, maxID));

                    double t = 0;
                    if (Control.IsTerminalState(transition.s1))
                    {
                        t = transition.Reward;
                    }
                    else
                    {
                        t = transition.Reward + DiscountRate * OldNeuralNet.WeightedSums[OldNeuralNet.Activations.Count - 1][0];
                    }

                    /*OldNeuralNet.Feed(CreateInputArray(transition.s.Board, transition.a.ID));
                    OldNeuralNet.WeightedSums[NeuralNet.Activations.Count - 1].CopyTo(Target, 0);
                    Target[transition.a.ID] += t;*/
                    double[] target = new double[1] { t };
                    Q_Targets.Add(new Tuple<double[], double[]>(CreateInputArray(transition.s.Board, transition.a.ID), ApplyFunction(target, Activation_Functions.Sigmoid.Function)));
                    addLoss += 0.5 * Math.Pow(ApplyFunction(target, Activation_Functions.Sigmoid.Function)[0] - NeuralNet.Activations[NeuralNet.Activations.Count - 1][0], 2);
                }
                addLoss /= miniBatch.Count;
                loss += addLoss;

                // SGD
                NeuralNet.SGD(Q_Targets, LearningRate, 1);

                // Adjust learning variables
                if (epsilon > EpsilonLimit)
                    epsilon -= EpsilonDecrease;

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
                // Make the control ready for another move
                Control.Clean();

            }
            IsLearning = false;
        }

        private Actione getMaxAction(GameControlBase control, State state)
        {
            int maxID = 0;
            double max = 0;
            for (int id = 0; id < control.ActionNum; id++)
            {
                NeuralNet.Feed(CreateInputArray(state.Board, id));
                if (NeuralNet.Activations[NeuralNet.Activations.Count - 1][0] > max)
                {
                    maxID = id;
                    max = NeuralNet.Activations[NeuralNet.Activations.Count - 1][0];
                }
            }
            return new Actione(maxID);
        }

        private Actione getMaxLegalAction(GameControlBase control, State state)
        {
            int maxID = 0;
            double max = 0;
            for (int id = 0; id < control.ActionNum; id++)
            {
                NeuralNet.Feed(CreateInputArray(state.Board, id));
                if (NeuralNet.Activations[NeuralNet.Activations.Count - 1][0] > max && control.IsLegalAction(new Actione(id)))
                {
                    maxID = id;
                    max = NeuralNet.Activations[NeuralNet.Activations.Count - 1][0];
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
            Input[x_len * y_len * 3 + actionID] = 1;
            for (int x = 0; x < x_len; x++)
            {
                for (int y = 0; y < y_len; y++)
                {
                    if (board[x, y] == (int)GameControlBase.Players.Player1)
                        Input[(x * x_len + y) * 3] = 1;
                    else if(board[x, y] == (int)GameControlBase.Players.Player2)
                        Input[(x * x_len + y) * 3 + 1] = 1;
                    else
                        Input[(x * x_len + y) * 3 + 2] = 1;
                }
            }

            return Input;
        }

        public override void Setup(GameControlBase control, GameControlBase.Players player)
        {
            // Create a new neural network with some dimensions defined before
            rand = new Random();
            Dimensions = new List<int> { control.FeatureNum + control.ActionNum, 28, 17, 1 };
            NeuralNet = new NetworkVectors(Dimensions, 0.1);
            OldNeuralNet = (NetworkVectors)NeuralNet.Clone();
            Control = control;
            BotTurn = player;
            ReplayMem = new List<Transition>();
           
        }

        public override void Stop()
        {
            IsLearning = false;
        }

        public override void TakeAction(GameControlBase control, State state)
        {
            Actione action = getMaxLegalAction(control, state);
            control.DoAction(action);
        }
    }
}