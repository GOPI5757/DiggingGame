namespace DiggingGame.Delegates
{
    public static class BlockBreakDelegate
    {
        public delegate void Event(int chunkSizeY, int BlockY);
        public static Event OnEvent;

        public static void Raise(int chunkSizeY, int BlockY) => OnEvent?.Invoke(chunkSizeY, BlockY);
    }
}