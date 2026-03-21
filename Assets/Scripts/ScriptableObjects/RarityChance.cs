using UnityEngine;

namespace DiggingGame.ScriptableObjects
{
    [System.Serializable]
    public struct Chances
    {
        [Range(0f, 100f)] public float ironChance;
        [Range(0f, 100f)] public float goldChance;
        [Range(0f, 100f)] public float diamondChance;
    }

    [CreateAssetMenu(fileName = "RarityChanceSO", menuName = "Valuables/Rarity/RarityChanceSO")]
    public class RarityChance : ScriptableObject
    {
        public Chances[] chances;
    }
}