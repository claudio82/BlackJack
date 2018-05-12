using System;

namespace BlackJack
{
    public class DelayedAction
    {
        public DelayedAction(Action action, float delay)
        {
            TimeRemaining = delay;
            Action = action;
            Delay = delay;
        }

        public Action Action { get; private set; }
        public float Delay { get; private set; }
        public float TimeRemaining { get; private set; }

        public bool Update(float deltaTime)
        {
            TimeRemaining -= deltaTime;

            if (TimeRemaining <= 0)
            {
                Action();
                return false;
            }

            return true;
        }
    }
}
