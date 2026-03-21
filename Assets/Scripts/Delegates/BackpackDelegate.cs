namespace DiggingGame.Delegates
{
    public static class BackpackDelegate
    {
        public delegate void Event(int value);
        public static Event OnEvent;

        public static void Raise(int value) => OnEvent?.Invoke(value);
    }
}