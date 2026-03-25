using TMPro;
using UnityEngine;

namespace DiggingGame.Config
{
    [System.Serializable]
    public class DisplayText
    {
        [field: SerializeField] public TMP_Text backpackStorageText { get; private set; }
        [field: SerializeField] public TMP_Text coinsDisplayText { get; private set; }
        [field: SerializeField] public TMP_Text depthDisplayText { get; private set; }
        [field: SerializeField] public TMP_Text ironDisplayText { get; private set; }
        [field: SerializeField] public TMP_Text goldDisplayText { get; private set; }
        [field: SerializeField] public TMP_Text diamondDisplayText { get; private set; }
    }
}