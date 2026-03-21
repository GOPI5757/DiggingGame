using UnityEngine;
using System.Collections.Generic;

namespace DiggingGame.Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private float defaultBlockSize;
        [SerializeField] private GameObject chunkPrefab;

        [SerializeField] private Vector3Int chunkSize;

        [SerializeField] private GameObject chestPrefab;

        [SerializeField] private string chunkName;
        [SerializeField] private int totalChunks;

        [SerializeField] private int[] strengths;

        private void Awake()
        {
            
        }

        void Start()
        {
            for(int i = 0; i < totalChunks; i++)
            {
                Vector3 pos = (Vector3.zero - new Vector3(0f, i * chunkSize.y, 0f) - new Vector3(chunkSize.x / 2, chunkSize.y, chunkSize.z / 2));
                GameObject ChunkObj = Instantiate(chunkPrefab, pos, Quaternion.identity);
                Chunk chunkScript = ChunkObj.GetComponent<Chunk>();
                if (chunkScript != null)
                {
                    int strengthIndex = i > strengths.Length ? strengths.Length - 1 : i;
                    chunkScript.SetBaseBlockStrength(strengthIndex);
                    chunkScript.SetChunk(chunkSize);
                    chunkScript.SetChunkName(chunkName);
                }
            }
        }

        void Update()
        {

        }
    }
}
