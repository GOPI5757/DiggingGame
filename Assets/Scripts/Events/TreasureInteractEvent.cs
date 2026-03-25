namespace DiggingGame.Events
{
    public struct TreasureInteractEvent
    {
        public bool canInteractTreasure;

        public TreasureInteractEvent(bool canInteractTreasure)
        {
            this.canInteractTreasure = canInteractTreasure;
        }
    }
}