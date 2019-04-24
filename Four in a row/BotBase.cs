using Controller;

namespace Learning
{
    public abstract class Bot
    {
        public abstract void TakeAction(GameControlBase control, State state);
    }

    public abstract class LearningBot : Bot
    {
        public abstract void Learn(int EpocheNumber, double EpsilonLimit, double EpsilonDecrease, double LearningRate, double DiscountRate, Bot against = null);
        public abstract void Stop();
        public abstract void Setup(GameControlBase control, GameControlBase.Players player);
        public GameControlBase.Players BotTurn { get; protected set; }
        public bool IsLearning { get; protected set; }
    }
}