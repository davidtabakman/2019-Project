using Controller;
using System;
using System.Threading;

namespace Learning
{
    public class Actione
    {
        public int ID { get; private set; }

        public Actione(int ID)
        {
            this.ID = ID;
        }
    }

    public class State
    {
        public int ID { get; private set; }

        public State(int ID)
        {
            this.ID = ID;
        }
    }
    
    // TO DO:
    // FIX THIS BULLSHIT :
    // SPLIT INTO FUNCTIONS, MAKE SAFE
    public class Q_Leaning
    {
        public float[,] Q_Table { get; private set; }
        private int ActionNum;
        private int StateNum;
        private Random rand;
        private GameControlBase Control;
        public GameControlBase.Players BotTurn { get; private set; }
        public bool IsLearning { get; private set; }

        private void Swap(float[] arr, int i1, int i2)
        {
            float tmp = arr[i1];
            arr[i1] = arr[i2];
            arr[i2] = tmp;
        }

        private Actione getMaxAction(GameControlBase control, State state)
        {
            float[] tmp = new float[ActionNum];
            float[] tmpVals = new float[ActionNum];
            for (int x = 0; x < ActionNum; x++)
            {
                tmp[x] = x;
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
            
            for (int i = 0; i < ActionNum; i++)
            {
                if (control.IsLegalAction(new Actione((int)tmp[i])))
                    return new Actione((int)tmp[i]);
            }

            return null;
            
        }


        public void Learn(int EpocheNumber, float EpsilonLimit, float EpsilonDecrease, float LearningRate, float DiscountRate, Q_Leaning against)
        {

            IsLearning = true;
            int epoche_move_limit = 20;
            int current_epoche = 0;
            int last_epcohe = 0;

            float epsilon = 1.0f; // exploration / exploitation

            int wins = 0;

            // Observe state
            State state = Control.GetState();
            State newState;
            int reward = 0;
            Actione action;
            Actione botAction;
            while (current_epoche < EpocheNumber)
            {
                
                if (BotTurn != Control.CurrTurn)
                {
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
                if (!Control.IsTerminalState()) {
                    if (against != null)
                        against.TakeAction(Control, Control.GetState());
                    else
                    {
                        botAction = new Actione(rand.Next(Control.ActionNum));
                        while (!Control.IsLegalAction(botAction))
                            botAction = new Actione(rand.Next(Control.ActionNum));
                        Control.DoAction(botAction);
                    }
                } else
                {
                    if (Control.CheckWin() == BotTurn)
                    {
                        wins++;
                    }
                }

                // Get reward
                reward = Control.GetReward(BotTurn);

                // Adjust Q-table
                newState = Control.GetState();
                float deltaQ;
                if (!Control.IsTerminalState())
                    deltaQ = LearningRate * (reward + DiscountRate * Q_Table[newState.ID, getMaxAction(Control, newState).ID] - Q_Table[state.ID, action.ID]);
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
                    Console.WriteLine("Learning percentage: " + current_epoche / (float)EpocheNumber * 100 + "%, win rate: " + wins * 0.01f + "%");
                    wins = 0;
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
        public Q_Leaning(GameControlBase control, GameControlBase.Players player)
        {
            Control = control;
            Q_Table = new float[control.StateNum, control.ActionNum];
            rand = new Random();
            BotTurn = player;

            ActionNum = control.ActionNum;
            StateNum = control.StateNum;

            IsLearning = false;
        }

        public void TakeAction(GameControlBase control, State state)
        {
            Actione action = getMaxAction(control, state);
            control.DoAction(action);
        }

    }
}