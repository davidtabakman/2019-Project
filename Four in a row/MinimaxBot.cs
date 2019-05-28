﻿
using Controller;
using System;
using static Controller.GameControlBase;

namespace Learning
{
    public class MinimaxBot : Bot
    {
        private Random rand;
        private int MaxDepth;

        public MinimaxBot() : base()
        {
            rand = new Random();
        }

        public void SetMaxDepth(int val)
        {
            MaxDepth = val;
        }

        protected override Actione getMaxAction(GameControlBase control, State state, bool isLegal)
        {
            return BestMove(control, state, int.MinValue, int.MaxValue, control.CurrTurn);
        }

        private Actione BestMove(GameControlBase control, State s, double alpha, double beta, Players player)
        {
            Players opponent;
            if (player == Players.Player1)
                opponent = Players.Player2;
            else
                opponent = Players.Player1;

            int maxID = 0;
            double maxReward = -1000000;
            double Reward = 0;

            bool allSame = true;
            Actione botAction;

            State currState = (State)s.Clone();

            for (int i = 0; i < control.ActionNum; i++)
            {
                if (control.IsLegalAction(new Actione(i), currState))
                {
                    s.Copy(currState);
                    control.RegisterAction(currState, new Actione(i), player);
                    if (control.IsTerminalState(currState))
                        Reward = control.GetReward(player, currState);
                    else
                        Reward = 0.9 * GetMaxRewardRec(control, currState, false, opponent, alpha, beta, 1, MaxDepth);
                    if (Reward != maxReward && i != 0)
                    {
                        allSame = false;
                    }
                    if (Reward > maxReward)
                    {
                        maxID = i;
                        maxReward = Reward;
                    }
                    alpha = Math.Max(alpha, Reward);
                    if (beta <= alpha)
                        break;
                }
            }
            if (!allSame)
                return new Actione(maxID);
            else
            {
                botAction = new Actione(rand.Next(control.ActionNum));
                while (!control.IsLegalAction(botAction))
                    botAction = new Actione(rand.Next(control.ActionNum));
                return botAction;
            }
        }

        private double GetMaxRewardRec(GameControlBase control, State s, bool IsMaxing, Players player, double alpha, double beta, int level, int maxLevel)
        {
            if (level > maxLevel)
                return 0;

            Players opponent;
            if (player == Players.Player1)
                opponent = Players.Player2;
            else
                opponent = Players.Player1;

            double BestVal;
            double Reward = 0;

            State currState = (State)s.Clone();

            if (IsMaxing)
            {
                BestVal = int.MinValue;
                for (int i = 0; i < control.ActionNum; i++)
                {
                    if (control.IsLegalAction(new Actione(i), currState))
                    {
                        s.Copy(currState);
                        control.RegisterAction(currState, new Actione(i), player);
                        if (control.IsTerminalState(currState))
                            Reward = control.GetReward(player, currState);
                        else
                            Reward = 0.9 * GetMaxRewardRec(control, currState, !IsMaxing, opponent, alpha, beta, level + 1, maxLevel);
                        BestVal = Math.Max(Reward, BestVal);
                        if (beta <= BestVal)
                            break;
                        alpha = Math.Max(alpha, Reward);
                    }
                }
            }
            else
            {
                BestVal = int.MaxValue;
                for (int i = 0; i < control.ActionNum; i++)
                {
                    if (control.IsLegalAction(new Actione(i), currState))
                    {
                        s.Copy(currState);
                        control.RegisterAction(currState, new Actione(i), player);
                        if (control.IsTerminalState(currState))
                            Reward = control.GetReward(opponent, currState);
                        else
                            Reward = 0.9 * GetMaxRewardRec(control, currState, !IsMaxing, opponent, alpha, beta, level + 1, maxLevel);
                        BestVal = Math.Min(Reward, BestVal);
                        beta = Math.Min(beta, Reward);
                        if (alpha >= BestVal)
                            break;
                        
                    }
                }
            }
            return BestVal;
        }
    }
}