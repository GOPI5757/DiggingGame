using DiggingGame.Enums;
using UnityEngine;

namespace DiggingGame.ScriptableObjects
{
    [System.Serializable]
    public struct TreasureCount
    {
        public TreasureRarity Rarity;
        public int Count;
    }

    [System.Serializable]
    public struct TreasuresPerChunk
    {
        [SerializeField] private TreasureCount[] treasureCounts;
    }


    [CreateAssetMenu(fileName = "TreasureChunkStatsSO", menuName = "Treasure/SO/TreasureChunkStatsSO")]
    public class ChunkStats : ScriptableObject
    {
        [SerializeField] private TreasuresPerChunk[] treasureCount;
    }
}
