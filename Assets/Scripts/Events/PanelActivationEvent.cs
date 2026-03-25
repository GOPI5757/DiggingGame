namespace DiggingGame.Events
{
    public struct PanelActivationEvent
    {
        public bool shouldActivateMainPanel;
        public bool isTreasure;

        public PanelActivationEvent(bool shouldActivateMainPanel, bool isTreasure)
        {
            this.shouldActivateMainPanel = shouldActivateMainPanel;
            this.isTreasure = isTreasure;
        }
    }
}