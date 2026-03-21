using UnityEngine;
using System.Collections.Generic;

namespace DiggingGame.Quests
{
    [System.Serializable]
    public struct Quests
    {
        public string QuestTitle;
        public string QuestDescription;
        public int CurrentValue;
        public int MaxValue;
        public bool IsCompleted;
    }

    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private Quests[] quests = new Quests[3];
        [SerializeField]
        private string[] typeOfQuests = new string[] {
            "Dig 10 Sand",
            "Collect 5 Iron",
            "Break 2 Common Chest"
        };

        private void Awake()
        {
            
        }
    }
}