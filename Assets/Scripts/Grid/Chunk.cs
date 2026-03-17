using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

namespace DiggingGame.Grid
{
    [System.Serializable]
    public struct BlockData
    {
        public Vector3Int coord;
        public BlockType blockType;
        public int strength;
        public int maxStrength;
        public Vector3 worldPos;
        public Vector3 localPos;
    
        public BlockData(Vector3Int coord, BlockType blockType, int strength, int maxStrength, Vector3 worldPos, Vector3 localPos)
        {
            this.coord = coord;
            this.blockType = blockType;
            this.strength = strength;
            this.maxStrength = maxStrength;
            this.worldPos = worldPos;
            this.localPos = localPos;
        }
    }

    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
    public class Chunk : MonoBehaviour
    {
        [SerializeField] private Vector3Int chunkSize;

        [SerializeField] private List<Vector3> vertices = new List<Vector3>();
        [SerializeField] private List<int> triangles = new List<int>();

        [SerializeField] private BlockData[,,] block_Datas;
        [SerializeField] private Material sandMat;

        private string chunkName;

        private Vector3[,] vertexData;
        private int[] triangleData;
        private Vector3Int[] boundCheckVector;

        private Mesh mesh;

        private void Start()
        {
            StoreInitialValues();
            GenerateMesh(true);
        }

        private void StoreInitialValues()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            block_Datas = new BlockData[chunkSize.x, chunkSize.y, chunkSize.z];

            vertexData = new Vector3[,]
            {
                {
                    new Vector3(0, 0, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(1, 1, 0), // Front
                    new Vector3(1, 0, 0),
                },
                {
                    new Vector3(1, 0, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(1, 1, 1), // Right
                    new Vector3(1, 0, 1),
                },
                {
                    new Vector3(1, 0, 1),
                    new Vector3(1, 1, 1),
                    new Vector3(0, 1, 1), // Back
                    new Vector3(0, 0, 1),
                },
                {
                    new Vector3(0, 0, 1),
                    new Vector3(0, 1, 1),
                    new Vector3(0, 1, 0), // Left
                    new Vector3(0, 0, 0),
                },
                {
                    new Vector3(0, 1, 0),
                    new Vector3(0, 1, 1),
                    new Vector3(1, 1, 1), // Top
                    new Vector3(1, 1, 0),
                },
                {
                    new Vector3(0, 0, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(1, 0, 1), // Bottom
                    new Vector3(0, 0, 1)
                }
            };

            triangleData = new int[] { 0, 1, 2, 0, 2, 3 };

            boundCheckVector = new Vector3Int[]
            {
                new Vector3Int(0, 0, -1),
                new Vector3Int(1, 0, 0),
                new Vector3Int(0, 0, 1),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, -1 ,0)
            };
        }

        private void PrepareBlockData()
        {
            for (int i = 0; i < chunkSize.x; i++)
            {
                for (int j = 0; j < chunkSize.y; j++)
                {
                    for (int k = 0; k < chunkSize.z; k++)
                    {
                        BlockData item = new BlockData(
                            new Vector3Int(i, j, k),
                            BlockType.Sand,
                            5,
                            5,
                            transform.position + new Vector3(i, j, k),
                            new Vector3(i, j, k)
                        );

                        block_Datas[i, j, k] = item;
                    }
                }
            }
        }
    
        private void CreateShape()
        {
            vertices.Clear();
            triangles.Clear();
            for (int i = 0; i < chunkSize.x; i++)
            {
                for(int j = 0; j < chunkSize.y; j++)
                {
                    for(int k = 0; k < chunkSize.z; k++)
                    {
                        if (block_Datas[i, j, k].blockType == BlockType.Sand)
                        {
                            for(int l = 0; l < 6; l++)
                            {
                                Vector3Int neighbour = block_Datas[i, j, k].coord + boundCheckVector[l];
                                bool flag = true;
                                if (CheckBounds(neighbour))
                                {
                                    flag = IsCoordFree(neighbour);
                                }

                                if(flag)
                                {
                                    for(int p = 0; p < 4; p++)
                                    {
                                        vertices.Add(block_Datas[i, j, k].localPos + vertexData[l, p]);
                                    }
                            
                                    for(int m = 0; m < 6; m++)
                                    {
                                        triangles.Add((vertices.Count - 4) + triangleData[m]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool CheckBounds(Vector3Int coord)
        {
            return (
                CheckCoordBound(coord.x, chunkSize.x) &&
                CheckCoordBound(coord.y, chunkSize.y) &&
                CheckCoordBound(coord.z, chunkSize.z)
            );
        }

        private bool CheckCoordBound(int val, int size)
        {
            return (val >= 0 && val < size);
        }

        private bool IsCoordFree(Vector3Int coord)
        {
            return block_Datas[coord.x, coord.y, coord.z].blockType == BlockType.Air;   
        }

        private void UpdateMesh()
        {
            if (mesh == null) return;

            mesh.Clear();

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            GetComponent<Renderer>().material = sandMat;
            GetComponent<MeshCollider>().sharedMesh = null;
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        private void GenerateMesh(bool flag = false)
        {
            if (flag) PrepareBlockData();
            CreateShape();
            UpdateMesh();
        }

        public void ReduceStrength(Vector3Int coord, int value)
        {
            if (block_Datas[coord.x, coord.y, coord.z].blockType != BlockType.Sand) return;
            block_Datas[coord.x, coord.y, coord.z].strength -= value;
            if (block_Datas[coord.x, coord.y, coord.z].strength <= 0)
            {
                block_Datas[coord.x, coord.y, coord.z].strength = 0;
                block_Datas[coord.x, coord.y, coord.z].blockType = BlockType.Air;
                GenerateMesh();
            }
        }

        public bool isBlockFree(Vector3 pos)
        {
            Vector3Int coord = CoordFromWorldPos(pos);
            return block_Datas[coord.x, coord.y, coord.z].blockType == BlockType.Air;
        }

        public Vector3 worldPosFromCoord(Vector3 pos)
        {
            Vector3Int coord = CoordFromWorldPos(pos);
            return block_Datas[coord.x, coord.y, coord.z].worldPos;
        }

        public Vector3Int CoordFromWorldPos(Vector3 pos)
        {
            Vector3 finalCoord = transform.position - pos;
            finalCoord = new Vector3(
                Mathf.Abs(finalCoord.x), 
                Mathf.Abs(finalCoord.y), 
                Mathf.Abs(finalCoord.z)
            );

            Vector3Int intCoord = Vector3Int.FloorToInt(finalCoord);
            return intCoord;
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;
            for(int i = 0; i < chunkSize.x; i++)
            {
                for(int j = 0; j < chunkSize.y; j++)
                {
                    for(int k = 0; k < chunkSize.z; k++)
                    {
                        Gizmos.DrawWireCube(block_Datas[i, j, k].worldPos + (Vector3.one / 2), Vector3.one);
                    }
                }
            }
        }

        public void SetChunk(Vector3Int val) { chunkSize = val; }
        public string GetChunkName() { return chunkName; }
        public void SetChunkName(string name) { chunkName = name; }
        public int GetStrength(Vector3Int coord) { return block_Datas[coord.x, coord.y, coord.z].strength; }
        public int GetMaxStrength(Vector3Int coord) { return block_Datas[coord.x, coord.y, coord.z].maxStrength; }
    }
}