using UnityEngine;

namespace DiggingGame.Grid
{
    [System.Serializable]
    public struct BlockData
    {
        public Vector2Int coord;
        public BlockType blockType;
        public int strength;
        public Vector3 worldPos;
    }

    public class Chunk : MonoBehaviour
    {
        [SerializeField] private BlockData[] blockDatas;
    }
}