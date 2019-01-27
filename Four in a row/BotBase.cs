using Controller;

namespace Learning
{
    public abstract class Bot
    {
        public abstract void TakeAction(GameControlBase control, State state);
    }
}