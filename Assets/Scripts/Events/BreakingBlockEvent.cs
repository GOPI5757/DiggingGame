namespace DiggingGame.Events
{
    public struct BreakingBlockEvent
    {
        public bool isBreakingBlock;
        public bool isBlockOver;

        public BreakingBlockEvent(bool value, bool isBlockOver)
        {
            this.isBreakingBlock = value;
            this.isBlockOver = isBlockOver;
        }
    }
}