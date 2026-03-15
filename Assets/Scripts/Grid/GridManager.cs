using UnityEngine;
using System.Collections.Generic;

namespace DiggingGame.Grid
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private float defaultBlockSize;
        [SerializeField] private GameObject chunkPrefab;

        private Vector3[] vertices;
        private int[] triangles;
        Mesh mesh;


        void Start()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;

            CreateShape();
            UpdateMesh();
        }

        private void CreateShape()
        {
            vertices = new Vector3[]
            {
                new Vector3(0, 0, 0), // 0
                new Vector3(0, 1, 0), // 1
                new Vector3(1, 1, 0), // 2
                new Vector3(1, 0, 0), // 3
    
                new Vector3(1, 0, 0), // 4
                new Vector3(1, 1, 0), // 5 
                new Vector3(1, 1, 1), // 6
                new Vector3(1, 0, 1), // 7 
    
                new Vector3(1, 0, 1), // 8
                new Vector3(1, 1, 1), // 9
                new Vector3(0, 1, 1), // 10
                new Vector3(0, 0, 1), // 11
    
                new Vector3(0, 0, 1), // 12
                new Vector3(0, 1, 1), // 13
                new Vector3(0, 1, 0), // 14  <-- The stubborn stray!
                new Vector3(0, 0, 0), // 15
    
                new Vector3(0, 1, 0), // 16
                new Vector3(0, 1, 1), // 17
                new Vector3(1, 1, 1), // 18
                new Vector3(1, 1, 0), // 19
    
                new Vector3(0, 0, 0), // 20
                new Vector3(1, 0, 0), // 21
                new Vector3(1, 0, 1), // 22
                new Vector3(0, 0, 1)  // 23
            };

            triangles = new int[]
            {
                // Front Face
                0, 1, 2,
                0, 2, 3,
    
                // Right Face
                4, 5, 6,
                4, 6, 7,
    
                // Back Face
                8, 9, 10,
                8, 10, 11,
    
                // Left Face
                12, 13, 14,
                12, 14, 15,
    
                // Top Face
                16, 17, 18,
                16, 18, 19,
    
                // Bottom Face
                20, 21, 22,
                20, 22, 23
            };
        }

        private void UpdateMesh()
        {
            if (mesh == null) return;

            mesh.Clear(); ;
            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
        }

        void Update()
        {

        }
    }
}
