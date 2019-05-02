using Controller;
using System.Runtime.Serialization;

namespace Learning
{
    public abstract class Bot
    {
        protected abstract Actione getMaxAction(GameControlBase control, State state, bool isLegal);
        public void TakeAction(GameControlBase control, State state)
        {
            Actione action = getMaxAction(control, state, true);
            control.DoAction(action);
        }
    }

    public abstract class LearningBot : Bot, ISerializable
    {
        

        public abstract void Learn(int EpocheNumber, double EpsilonLimit, double EpsilonDecrease, double LearningRate, double DiscountRate, Bot against = null);
        public GameControlBase.Players BotTurn { get; protected set; }
        public bool IsLearning { get; protected set; }
        protected GameControlBase Control;
        public bool IsSetup { get; protected set; }

        public LearningBot()
        {
            IsSetup = false;
            IsLearning = false;
        }

        public virtual void Setup(GameControlBase control, GameControlBase.Players player)
        {
            IsSetup = true;
        }

        public void Stop()
        {
            IsLearning = false;
        }

        // Tracking
        protected int games = 0;
        protected int wins = 0;
        protected int losses = 0;
        protected int draws = 0;

        protected void Track()
        {
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
        }

        protected Actione TakeEpsilonGreedyAction(double epsilon, State state, System.Random rand)
        {
            Actione action;
            // Take action
            if (rand.NextDouble() <= epsilon)
            {
                action = new Actione(rand.Next(Control.ActionNum));
            }
            else
            {
                action = getMaxAction(Control, state, false);
            }
            Control.DoAction(action);

            
            return action;
        }

        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
    }
}