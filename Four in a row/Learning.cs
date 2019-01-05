using Controller;
using System;

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
        private float learning_rate;
        private float discount_rate;
        private int epoche_number;
        private float epsilon;
        private float epsilon_limit;
        private float epsilon_decrease;
        private int ActionNum;
        private int StateNum;

        
        private void swap(float[] arr, int i1, int i2)
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
                        swap(tmp, i, i2);
                        swap(tmpVals, i, i2);
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

        public Q_Leaning(GameControlBase control, int epoche_number)
        {
            Q_Table = new float[control.StateNum, control.ActionNum];
            Random rand = new Random();

            ActionNum = control.ActionNum;
            StateNum = control.StateNum;
            epoche_number = 10000;
            int epoche_move_limit = 20;
            int current_epoche = 0;

            epsilon = 1.0f; // exploration / exploitation
            learning_rate = 0.7f;
            discount_rate = 0.9f;
            epsilon_limit = 0.01f;
            epsilon_decrease = 0.001f;

            State state = control.GetState();
            State newState;
            int reward = 0;
            Actione action = new Actione(rand.Next(control.ActionNum));
            while (current_epoche < epoche_number)
            {
                //if (control.CurrTurn == TicTacToe.TicTacControl.Players.O)
                //{

                    //if (!control.IsTerminalState())
                        // Opponent's move
                     //   control.RandomAction();

                    state = control.GetState();
{
                        if (rand.NextDouble() <= epsilon)
                        {
                            while (!control.IsLegalAction(action))
                                action = new Actione(rand.Next(control.ActionNum));
                        }
                        else
                        {
                            action = getMaxAction(control, state);
                        }
                    }
                    
                    control.DoAction(action);
                    reward = control.GetReward();
                    newState = control.GetState();
                    float deltaQ;
                    if (!control.IsTerminalState())
                        deltaQ = learning_rate * (reward + discount_rate * getMaxAction(control, newState).ID);
                    else
                        deltaQ = learning_rate * reward;
                    Q_Table[state.ID, action.ID] += deltaQ;
                    current_epoche++;
                    if (epsilon > epsilon_limit)
                        epsilon -= epsilon_decrease;

                    control.Clean();

                System.Threading.Thread.Sleep(10);
                //}
            }
        }

        public void TakeAciton(GameControlBase control, State state)
        {
            Actione action = getMaxAction(control, state);
            control.DoAction(action);
        }

    }
}