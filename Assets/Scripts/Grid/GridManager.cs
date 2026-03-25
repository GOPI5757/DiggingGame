using UnityEngine;
using System.Collections.Generic;
using DiggingGame.Player;

namespace DiggingGame.Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private float defaultBlockSize;
        [SerializeField] private GameObject chunkPrefab;

        [SerializeField] private Vector3Int chunkSize;

        [SerializeField] private string chunkName;
        [SerializeField] private int totalChunks;

        [SerializeField] private int[] strengths;

        [SerializeField] private int maxDepthDistance;
        
        private List<GameObject> chunkObjects = new List<GameObject>();
        private GameObject player;
        private Transform chunkParent;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            chunkParent = GameObject.FindGameObjectWithTag("chunkParent").transform;
        }

        void Start()
        {
            DepthChangeDelegate.OnEvent += UpdateChunkObjects;
            SpawnChunk();
        }

        private void OnDestroy()
        {
            DepthChangeDelegate.OnEvent -= UpdateChunkObjects;
        }

        private void SpawnChunk()
        {
            for (int i = 0; i < totalChunks; i++)
            {
                Vector3 pos = (Vector3.zero - new Vector3(0f, i * chunkSize.y, 0f) - new Vector3(chunkSize.x / 2, chunkSize.y, chunkSize.z / 2));
                GameObject ChunkObj = Instantiate(chunkPrefab, pos, Quaternion.identity);
                ChunkObj.transform.parent = chunkParent;
                
                Chunk chunkScript = ChunkObj.GetComponent<Chunk>();
                if (chunkScript != null)
                {
                    int strengthIndex = i >= strengths.Length ? strengths.Length - 1 : i;

                    chunkScript.SetBaseBlockStrength(strengths[strengthIndex]);
                    chunkScript.SetChunk(chunkSize);
                    chunkScript.SetChunkName(chunkName);
                    chunkScript.SetChunkIndex(i);

                    chunkObjects.Add(ChunkObj);
                }
            }

            player.GetComponent<PlayerData>().RaiseDepthChangeDelegate();
        }

        private void UpdateChunkObjects(int currentDepth)
        {
            for(int i = 0; i < chunkObjects.Count; i++)
            {
                int depthDistance = Mathf.Abs(currentDepth - Mathf.FloorToInt(chunkObjects[i].transform.position.y));
                if(depthDistance > maxDepthDistance)
                {
                    HandleChunkActivation(chunkObjects[i], false);
                } else
                {
                    HandleChunkActivation(chunkObjects[i], true);
                }
            }
        }

        private void HandleChunkActivation(GameObject chunkObj, bool is_active)
        {
            Chunk chunkScript = chunkObj.GetComponent<Chunk>();
            if (chunkScript != null)
            {
                chunkScript.HandleChestObjects(is_active);
                chunkObj.SetActive(is_active);
            }
        }
    }
}
