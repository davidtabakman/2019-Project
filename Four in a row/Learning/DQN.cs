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
        private const int ReplayMemSize = 1000;

        private struct Transition
        {
            public State s;
            public Actione a;
            public float Reward;
            public State s1;

            public Transition(State s, Actione a, float Reward, State s1)
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

        public override void Learn(int EpocheNumber, float EpsilonLimit, float EpsilonDecrease, float LearningRate, float DiscountRate, Bot against = null)
        {
            IsLearning = true;
            int epoche_move_limit = 20;
            int current_epoche = 0;
            int last_epcohe = 0;
            int SampleSize = 50;
            int iterations = 0;
            List<Transition> miniBatch = null;
            List<TargetResult> Q_Targets = new List<TargetResult>();
            List<List<double[]>> Gradients = new List<List<double[]>>();

            for (int layer = 1; layer < Dimensions.Count; layer++)
            {
                Gradients.Add(new List<double[]>());
                for(int neuron = 0; neuron < Dimensions[layer]; neuron++)
                    Gradients[Gradients.Count - 1].Add(CreateInitArray(Dimensions[layer - 1], 0));
            }

            float epsilon = 1.0f; // exploration / exploitation

            int wins = 0;

            // Observe state
            State state = Control.GetState();
            State newState;
            int reward = 0;
            Actione action;

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
                    while (!Control.IsLegalAction(action))
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

                else
                {
                    if (Control.CheckWin() == BotTurn)
                    {
                        wins++;
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
                if (iterations % 1000 == 0 && iterations != 0)
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

                foreach (Transition transition in miniBatch)
                {
                    OldNeuralNet.Feed(Flatten(transition.s1.Board));
                    NeuralNet.Feed(Flatten(transition.s.Board));
                    NeuralNet.WeightedSums[NeuralNet.Activations.Count - 1].CopyTo(Target, 0);
                    Target[transition.a.ID] = transition.Reward + DiscountRate * Max(OldNeuralNet.WeightedSums[OldNeuralNet.Activations.Count - 1]);
                    ApplyFunciton(Target, Activation_Functions.Sigmoid.Function);
                    NeuralNet.Activations[NeuralNet.Activations.Count - 1].CopyTo(Result, 0);
                    Add(Gradients, NeuralNet.GetGradient(Target));
                }
                ScalarDivide(Gradients, SampleSize / LearningRate);

                // SGD with mean expectations and results cost function
                Sub(NeuralNet.Weights, Gradients);

                // Adjust learning variables
                if (epsilon > EpsilonLimit)
                    epsilon -= EpsilonDecrease;

                if (current_epoche % 1000 == 0 && current_epoche != last_epcohe)
                {
                    last_epcohe = current_epoche;
                    Console.WriteLine("Learning percentage: " + current_epoche / (float)EpocheNumber * 100 + "%, win rate: " + wins * 0.1f + "%");
                    wins = 0;
                }

                current_epoche++;
                // Make the control ready for another move
                Control.Clean();
            }
            IsLearning = false;
        }

        private Actione getMaxAction(GameControlBase control, State state)
        {
            NeuralNet.Feed(Flatten(state.Board));
            int maxArg = 0;
            for (int arg = 0; arg < NeuralNet.Activations[NeuralNet.Activations.Count - 1].Length; arg++)
            {
                if (maxArg.CompareTo(arg) < 0 && control.IsLegalAction(new Actione(arg)))
                {
                    maxArg = arg;
                }
            }
            return new Actione(maxArg);
        }

        public override void Setup(GameControlBase control, GameControlBase.Players player)
        {
            // Create a new neural network with some dimensions defined before
            rand = new Random();
            Dimensions = new List<int> { control.FeatureNum, 27, 18, control.ActionNum };
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
            Actione action = getMaxAction(control, state);
            control.DoAction(action);
        }
    }
}