using DiggingGame.Enums;
using DiggingGame.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace DiggingGame.Grid
{
    public class ChestScript : MonoBehaviour
    {
        [SerializeField] private TreasureRarity rarity;
        [SerializeField] private GameObject KeyObj;
        [SerializeField] private TreasureStrengths treasureStrengths;
        
        private Chunk linkedChunk;
        private string treasureName;
        private string titleTextColorHEX;

        private Dictionary<TreasureRarity, string> rarityNames = new Dictionary<TreasureRarity, string>
        {
            {TreasureRarity.Common, "<color=#C3C3C3>C</color>ommon <color=#C3C3C3>C</color>hest" },
            {TreasureRarity.Uncommon, "<color=#1BE100>U</color>ncommon <color=#1BE100>C</color>hest" },
            {TreasureRarity.Rare, "<color=#00BEBF>R</color>are <color=#00BEBF>C</color>hest" },
            {TreasureRarity.Epic, "<color=#BF69FF>E</color>pic <color=#BF69FF>C</color>hest" },
            {TreasureRarity.Legendary, "<color=#FF692E>L</color>egendary <color=#FF692E>C</color>hest" },
        };

        private Dictionary<TreasureRarity, string> rarityColors = new Dictionary<TreasureRarity, string>
        {
            {TreasureRarity.Common, "#A5A5A5" },
            {TreasureRarity.Uncommon, "#15AC01" },
            {TreasureRarity.Rare, "#00A9AA" },
            {TreasureRarity.Epic, "#B756FF" },
            {TreasureRarity.Legendary, "#FF8756" }
        };

        public void SetRarity(TreasureRarity value) => rarity = value;
        public void SetTreasureNames() { treasureName = rarityNames[rarity]; }
        public string GetTreasureName() { return treasureName; }

        public void SetTitleTextColorHEX() { titleTextColorHEX = rarityColors[rarity]; }
        public string GetTitleTextColorHEX() {  return titleTextColorHEX; }

        public void SetLinkedChunk(Chunk value) { linkedChunk = value; }
        public Chunk GetLinkedChunk() { return linkedChunk; }

        private void Start()
        {
            for(int i = 0; i < treasureStrengths.t_stats.Length; i++)
            {
                if (treasureStrengths.t_stats[i].Rarity == rarity)
                {
                    KeyObj.GetComponent<Renderer>().material = treasureStrengths.t_stats[i].KeyMat;
                }
            }
        }
    }
}
