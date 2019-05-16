using Controller;
using System;
using System.Runtime.Serialization;

namespace Learning
{
    public abstract class Bot
    {
        /// <summary>
        /// Go over all actions and return the one with the highest value.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="state">The state of control to use.</param>
        /// <param name="isLegal">If true, the returned action has to be considered legal in the control.</param>
        /// <returns></returns>
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
        protected Random rand;

        public LearningBot()
        {
            IsSetup = false;
            IsLearning = false;
            rand = new Random();
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

        /// <summary>
        /// Execute a move by the given bot
        /// </summary>
        /// <param name="against">A bot, if null then takes random action</param>
        protected void BotMove(Bot against)
        {
            Actione botAction;
            if (!Control.IsTerminalState())
            {
                if (against != null)
                    against.TakeAction(Control, Control.GetState());
                else // Find a random legal action
                {
                    botAction = new Actione(rand.Next(Control.ActionNum));
                    while (!Control.IsLegalAction(botAction))
                        botAction = new Actione(rand.Next(Control.ActionNum));
                    Control.DoAction(botAction);
                }
            }
        }

        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
    }
}