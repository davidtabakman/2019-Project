
using Controller;
using System;
using static Controller.GameControlBase;

namespace Learning
{
    public class MinimaxBot : Bot
    {
        private Random rand; // Random for when all moves are value the same
        private int MaxDepth; // Maximum recursive calls

        /// <summary>
        /// Create a new minimax bot
        /// </summary>
        public MinimaxBot() : base()
        {
            rand = new Random();
        }

        /// <summary>
        /// Set the maximum depth that the minimax bot will search in
        /// </summary>
        public void SetMaxDepth(int val)
        {
            MaxDepth = val;
        }

        /// <summary>
        /// Get the action that the minimax bot considers the best in the given state and the given control
        /// </summary>
        protected override Actione getMaxAction(GameControlBase control, State state, bool isLegal)
        {
            return BestMove(control, state, int.MinValue, int.MaxValue, control.CurrTurn);
        }

        /// <summary>
        /// A wrapper function to the recursive function, that returns the best action
        /// </summary>
        /// <param name="control">Control that the state is in</param>
        /// <param name="s">The current control state</param>
        /// <param name="alpha">Has to be int.MinValue</param>
        /// <param name="beta">Has to be int.MaxVakue</param>
        /// <param name="player">The player that is maximizing</param>
        private Actione BestMove(GameControlBase control, State s, double alpha, double beta, Players player)
        {
            // This is needed in order to register the next action as the opponent's
            Players opponent;
            if (player == Players.Player1)
                opponent = Players.Player2;
            else
                opponent = Players.Player1;

            int maxID = 0;
            double maxReward = -1000000;
            double Reward = 0;

            bool allSame = true; // Are all the rewards the same?
            Actione botAction;

            State currState = (State)s.Clone(); // Clone the state because it is a reference, so save the original state

            for (int i = 0; i < control.ActionNum; i++)
            {
                if (control.IsLegalAction(new Actione(i), currState))
                {
                    // Make the action as the player
                    s.Copy(currState);
                    control.RegisterAction(currState, new Actione(i), player);
                    if (control.IsTerminalState(currState)) // If its terminal, reward is the reward that the control returns
                        Reward = control.GetReward(player, currState);
                    else // If not terminal, return the minimax recursive function return
                        Reward = 0.9 * GetMaxRewardRec(control, currState, false, opponent, alpha, beta, 1, MaxDepth);
                    if (Reward != maxReward && i != 0) // If the reward changed along the call
                    {
                        allSame = false;
                    }
                    if (Reward > maxReward)
                    {
                        maxID = i;
                        maxReward = Reward;
                    }
                    // Alpha beta pruning
                    alpha = Math.Max(alpha, maxReward);
                    if (beta <= alpha)
                        break;
                }
            }
            if (!allSame)
                return new Actione(maxID);
            else // If all rewards were the same, do a random action
            {
                botAction = new Actione(rand.Next(control.ActionNum));
                while (!control.IsLegalAction(botAction))
                    botAction = new Actione(rand.Next(control.ActionNum));
                return botAction;
            }
        }

        /// <summary>
        /// The actual recursive function that return the max reward of a state
        /// </summary>
        /// <param name="IsMaxing">Is the current player the player that is maximizing</param>
        /// <param name="player">The current player</param>
        /// <param name="alpha">The best option for maximizing player</param>
        /// <param name="beta">The best option for minimizing player</param>
        private double GetMaxRewardRec(GameControlBase control, State s, bool IsMaxing, Players player, double alpha, double beta, int level, int maxLevel)
        {
            // If reached the max depth
            if (level > maxLevel)
                return 0;

            // This is needed in order to register the next action as the opponent's
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
                        if (control.IsTerminalState(currState)) // If its terminal, reward is the reward that the control returns
                            Reward = control.GetReward(player, currState);
                        else // If not terminal, return the minimax recursive function return
                            Reward = 0.9 * GetMaxRewardRec(control, currState, !IsMaxing, opponent, alpha, beta, level + 1, maxLevel);
                        BestVal = Math.Max(Reward, BestVal);
                        // Alpha beta pruning
                        alpha = Math.Max(alpha, BestVal);
                        if (beta <= alpha)
                            break;
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
                        if (control.IsTerminalState(currState)) // If its terminal, reward is the reward that the control returns
                            Reward = control.GetReward(opponent, currState);
                        else // If not terminal, return the minimax recursive function return 
                            Reward = 0.9 * GetMaxRewardRec(control, currState, !IsMaxing, opponent, alpha, beta, level + 1, maxLevel);
                        BestVal = Math.Min(Reward, BestVal);
                        // Alpha beta pruning
                        beta = Math.Min(beta, BestVal);
                        if (beta <= alpha)
                            break;
                    }
                }
            }
            return BestVal;
        }
    }
}