using DiggingGame.Grid;
using UnityEngine;

namespace DiggingGame.Events
{
    public struct UpdatePanelUIEvent
    {
        public string blockName;
        public string colorHex;
        public int currentStrength;
        public int maxStrength;

        public UpdatePanelUIEvent(string blockName, string colorHex, int currentStrength, int maxStrength)
        {
            this.blockName = blockName;
            this.colorHex = colorHex;   
            this.currentStrength = currentStrength;
            this.maxStrength = maxStrength;
        }
    }
}