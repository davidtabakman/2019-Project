using Controller;
using System;
using System.Runtime.Serialization;
using System.Threading;

namespace Learning
{

    [Serializable()]
    public class Q_Learning : LearningBot
    {
        public double[,] Q_Table { get; private set; } // State and action matrix with Q values
        private int ActionNum;
        private int StateNum;

        /// <summary>
        /// Helper function to swap two array values
        /// </summary>
        private void Swap(double[] arr, int i1, int i2)
        {
            double tmp = arr[i1];
            arr[i1] = arr[i2];
            arr[i2] = tmp;
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
            double[] tmp = new double[ActionNum];
            double[] tmpVals = new double[ActionNum];
            for (int x = 0; x < ActionNum; x++) // Fill a temporary array
            {
                tmp[x] = x;
                tmpVals[x] = Q_Table[state.ID, x];
            }
            for (int i = 0; i < ActionNum; i++) // Selection Sort
            {
                for (int i2 = i; i2 < ActionNum; i2++)
                {
                    if (tmpVals[i2] > tmpVals[i])
                    {
                        Swap(tmp, i, i2);
                        Swap(tmpVals, i, i2);
                    }
                }
            }
            if (isLegal) // Return the best action (first in sorted array), or if it is required to be legal return the first legal action.
            {
                for (int i = 0; i < ActionNum; i++)
                {
                    if (control.IsLegalAction(new Actione((int)tmp[i])))
                        return new Actione((int)tmp[i]);
                }

                return null;
            } else
            {
                return new Actione((int)tmp[0]);
            }
        }

        /// <summary>
        /// Learning using reinforcement learning Q learning, that updates a Q table using the bellman equation.
        /// </summary>
        /// <param name="EpocheNumber"></param>
        /// <param name="EpsilonLimit"></param>
        /// <param name="EpsilonDecrease">Decrease of epsilon every iteration</param>
        /// <param name="LearningRate"></param>
        /// <param name="DiscountRate"></param>
        /// <param name="against">optional, if not given opponent is random</param>
        public override void Learn(int EpocheNumber, double EpsilonLimit, double EpsilonDecrease, double LearningRate, double DiscountRate, Bot against=null)
        {
            IsLearning = true;
            int current_epoche = 0;
            int last_epcohe = 0;

            double epsilon = 1.0f; // exploration / exploitation

            // Tracking variables
            games = 0;
            wins = 0;
            losses = 0;
            draws = 0;

            // Observe state and declare learning variables
            State state = Control.GetState();
            State newState;
            double reward = 0;
            Actione action;
            
            while (current_epoche < EpocheNumber && IsLearning)
            {
                
                if (BotTurn != Control.CurrTurn) // If its not this learningbot's turn in the control
                {
                    BotMove(against);
                }
                
                state = Control.GetState(); // Observe state

                action = TakeEpsilonGreedyAction(epsilon, state, rand); // Take action

                if (!Control.IsTerminalState()) {
                    BotMove(against);
                }

                Track(); // Update the tracking variables according to the current state of the game

                reward = Control.GetReward(BotTurn); // Get reward

                // Adjust Q-table using the bellman equation: Q += (Q' * DiscountRate + R) * learning rate
                newState = Control.GetState();
                double deltaQ;
                if (!Control.IsTerminalState())
                    deltaQ = LearningRate * (reward + DiscountRate * Q_Table[newState.ID, getMaxAction(Control, newState, true).ID] - Q_Table[state.ID, action.ID]);
                else
                {
                    deltaQ = LearningRate * (reward - Q_Table[state.ID, action.ID]);
                    current_epoche++;
                }
                Q_Table[state.ID, action.ID] += deltaQ;
                
                // Adjust learning variables
                if (epsilon > EpsilonLimit)
                    epsilon -= EpsilonDecrease;

                if (current_epoche % 10000 == 0 && current_epoche != last_epcohe) // Print retport of the last 10000 iterations
                {
                    last_epcohe = current_epoche;
                    Console.WriteLine("Learning percentage: {0}%, win rate: {1}%, loss rate: {2}%, draw rate: {3}%", current_epoche / (double)EpocheNumber * 100, (double)wins * 100 / games, (double)losses * 100 / games, (double)draws * 100 / games);
                    wins = 0;
                    draws = 0;
                    losses = 0;
                    games = 0;
                }

                Control.Clean(); // Make the control ready for another move

            }
            IsLearning = false;
        }

        /// <summary>
        /// Fuction will create a Q-Learning bot that uses a regular Q-function value table (still has to be setup
        /// </summary>
        /// <param name="control">The game that is to be learned, bot attaches to it</param>
        public Q_Learning() : base()
        {
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
            Control = control;
            if (Q_Table == null) 
                Q_Table = new double[control.StateNum, control.ActionNum];
            BotTurn = player;

            ActionNum = control.ActionNum;
            StateNum = control.StateNum;
        }

        // For serialization
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Q_Table", Q_Table);
            info.AddValue("Control", Control);
            info.AddValue("Player", BotTurn);
        }

        // Constructor for serialization
        public Q_Learning(SerializationInfo info, StreamingContext context)
        {
            Q_Table = (double[,])info.GetValue("Q_Table", typeof(double[,]));
            Control = (GameControlBase)info.GetValue("Control", typeof(GameControlBase));
            BotTurn = (GameControlBase.Players)info.GetValue("Player", typeof(GameControlBase.Players));
            Setup(Control, BotTurn);
        }
    }
}