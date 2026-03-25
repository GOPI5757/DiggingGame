using DiggingGame.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace DiggingGame.ScriptableObjects 
{
    [System.Serializable]
    public struct TreasureStats
    {
        public TreasureRarity Rarity;
        public int Chances;
        public int miniGame;
        public Material Mat;
        public Material KeyMat;
    }

    [CreateAssetMenu(fileName = "TreasureStrengthSO", menuName = "Treasure/SO/TreasureStrengthSO")]
    public class TreasureStrengths : ScriptableObject
    {
        public TreasureStats[] t_stats;
    }
}