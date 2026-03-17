using UnityEngine;
using System.Collections.Generic;

namespace DiggingGame.Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private float defaultBlockSize;
        [SerializeField] private GameObject chunkPrefab;

        [SerializeField] private Vector3Int chunkSize;

        [SerializeField] private string chunkName;

        private void Awake()
        {
            
        }

        void Start()
        {
            Vector3 pos = Vector3.zero - new Vector3(chunkSize.x / 2, chunkSize.y, chunkSize.z / 2);
            GameObject ChunkObj = Instantiate(chunkPrefab, pos, Quaternion.identity);
            Chunk chunkScript = ChunkObj.GetComponent<Chunk>();
            if (chunkScript != null)
            {
                chunkScript.SetChunk(chunkSize);
                chunkScript.SetChunkName(chunkName);
            }
        }

        void Update()
        {

        }
    }
}
