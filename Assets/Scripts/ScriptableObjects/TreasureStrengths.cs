using DiggingGame.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace DiggingGame.ScriptableObjects 
{
    [System.Serializable]
    public struct TreasureStats
    {
        public TreasureRarity Rarity;
        public int Strength;
        public Material Mat;
    }

    [CreateAssetMenu(fileName = "TreasureStrengthSO", menuName = "Treasure/SO/TreasureStrengthSO")]
    public class TreasureStrengths : ScriptableObject
    {
        [SerializeField] private TreasureStats[] t_stats;
    }
}