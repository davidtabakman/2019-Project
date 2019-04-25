using Controller;
using System;
using System.Threading;

namespace Learning
{
    
    
    public class Q_Learning : LearningBot
    {
        public double[,] Q_Table { get; private set; }
        private int ActionNum;
        private int StateNum;
        private Random rand;

        private void Swap(double[] arr, int i1, int i2)
        {
            double tmp = arr[i1];
            arr[i1] = arr[i2];
            arr[i2] = tmp;
        }

        protected override Actione getMaxAction(GameControlBase control, State state, bool isLegal)
        {
            double[] tmp = new double[ActionNum];
            double[] tmpVals = new double[ActionNum];
            for (int x = 0; x < ActionNum; x++)
            {
                tmp[x] = x;
                if (state.ID < 0)
                    Console.WriteLine("wtf");
                tmpVals[x] = Q_Table[state.ID, x];
            }
            // Selection Sort
            for (int i = 0; i < ActionNum; i++)
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

            if (isLegal)
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

        public override void Learn(int EpocheNumber, double EpsilonLimit, double EpsilonDecrease, double LearningRate, double DiscountRate, Bot against=null)
        {

            IsLearning = true;
            int epoche_move_limit = 20;
            int current_epoche = 0;
            int last_epcohe = 0;

            double epsilon = 1.0f; // exploration / exploitation

            games = 0;
            wins = 0;
            losses = 0;
            draws = 0;

            // Observe state
            State state = Control.GetState();
            State newState;
            double reward = 0;
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
                action = TakeEpsilonGreedyAction(epsilon, state, rand);

                // Bot takes action if possible (at the moment, the bot is random)
                if (!Control.IsTerminalState()) {
                    BotMove(against);
                }
                Track();


                // Get reward
                reward = Control.GetReward(BotTurn);

                // Adjust Q-table
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

                if (current_epoche % 10000 == 0 && current_epoche != last_epcohe)
                {
                    last_epcohe = current_epoche;
                    Console.WriteLine("Learning percentage: {0}%, win rate: {1}%, loss rate: {2}%, draw rate: {3}%", current_epoche / (double)EpocheNumber * 100, (double)wins * 100 / games, (double)losses * 100 / games, (double)draws * 100 / games);
                    wins = 0;
                    draws = 0;
                    losses = 0;
                    games = 0;
                }

                // Make the control ready for another move
                Control.Clean();

            }
            IsLearning = false;
        }

        /// <summary>
        /// Fuction will create a Q-Learning bot that uses a regular Q-function value table
        /// </summary>
        /// <param name="control">The game that is to be learned, bot attaches to it</param>
        public Q_Learning()
        {
            
            rand = new Random();
            
            IsLearning = false;
        }

        public override void Setup(GameControlBase control, GameControlBase.Players player)
        {
            Control = control;
            Q_Table = new double[control.StateNum, control.ActionNum];
            BotTurn = player;

            ActionNum = control.ActionNum;
            StateNum = control.StateNum;
        }


    }
}