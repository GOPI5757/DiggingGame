using TMPro;
using UnityEngine;
using DiggingGame.Delegates;
using Unity.VisualScripting.FullSerializer;
using DiggingGame.ScriptableObjects;
using DiggingGame.Grid;
using DiggingGame.Enums;

namespace DiggingGame.Player
{
    public class PlayerData : MonoBehaviour
    {
        [SerializeField] private int maxBackpackStorage;
        [SerializeField] private TMP_Text backpackStorageText;
        [SerializeField] private TMP_Text coinsDisplayText;
        [SerializeField] private TMP_Text depthDisplayText;
        [SerializeField] private TMP_Text ironDisplayText;
        [SerializeField] private TMP_Text goldDisplayText;
        [SerializeField] private TMP_Text diamondDisplayText;

        [SerializeField] private int coinsPerSand;

        [SerializeField] private RarityChance rarityChanceSO;
        [SerializeField] private float rarityPercentIncrease;

        private int currentBackpackStorage;
        private int currentCoins;
        private int currentDepth;
        private int lastDepth;

        private int currentIron;
        private int currentGold;
        private int currentDiamond;

        private int chanceIndex;

        private void Start()
        {
            BackpackDelegate.OnEvent += OnBackpack;
            BlockBreakDelegate.OnEvent += OnBlockBreak;
            UpdateBackpackText();
        }

        private void OnDestroy()
        {
            BackpackDelegate.OnEvent -= OnBackpack;
            BlockBreakDelegate.OnEvent -= OnBlockBreak;
        }

        private void OnBackpack(int value)
        {
            currentBackpackStorage += value;
            currentBackpackStorage = Mathf.Clamp(currentBackpackStorage, 0, maxBackpackStorage);

            UpdateBackpackText();
        }

        private void OnBlockBreak(int chunkSizeY, int BlockY)
        {
            chanceIndex = currentDepth / chunkSizeY;
            chanceIndex = Mathf.Clamp(chanceIndex, 0, rarityChanceSO.chances.Length - 1);

            float iron = Random.Range(0, 101);
            float percent = (((chunkSizeY - 1) - BlockY) * rarityPercentIncrease);
            percent = Mathf.Clamp(percent, 0, 100);
            float finalChance = (percent / 100) * rarityChanceSO.chances[chanceIndex].ironChance;
            
            if (iron < finalChance)
            {
                currentIron++;
                UpdateIronText();
            }
        }

        private void Update()
        {
            CalculateDepth();
        }

        private void CalculateDepth()
        {
            currentDepth = Mathf.Abs(Mathf.FloorToInt(transform.position.y + 0.01f));
            if (currentDepth != lastDepth)
            {
                UpdateDepthDisplay();
            }

            lastDepth = currentDepth;
        }

        private void UpdateDepthDisplay() => UpdateTMPText(depthDisplayText, "Depth\n" + currentDepth.ToString() + " Blocks");
        private void UpdateBackpackText() => UpdateTMPText(backpackStorageText, currentBackpackStorage.ToString() + " / " + maxBackpackStorage.ToString());

        private void UpdateIronText() => UpdateTMPText(ironDisplayText, currentIron.ToString());
        private void UpdateGoldText() => UpdateTMPText(ironDisplayText, currentGold.ToString());
        private void UpdateDiamondText() => UpdateTMPText(ironDisplayText, currentDiamond.ToString());

        private void UpdateCoinsText() => UpdateTMPText(coinsDisplayText, currentCoins.ToString());

        private void UpdateTMPText(TMP_Text text, string value) => text.text = value;

        private void UpdateCoins()
        {
            if (currentBackpackStorage <= 0) return;
            currentCoins += currentBackpackStorage * coinsPerSand;
            UpdateCoinsText();
            currentBackpackStorage = 0;
            UpdateBackpackText();
        }

        public int GetBackpackStorage() { return currentBackpackStorage; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("SellArea"))
            {
                UpdateCoins();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {

        }
    }
}