using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedMesh : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private Mesh mesh;

    [SerializeField]
    private int mapSize = 10;

    [SerializeField]
    private float scale = 1.0f;
    [SerializeField]
    private int seed = 32132131;
    [SerializeField]
    private float maxHeight = 10.0f;
    [SerializeField]
    Vector3[] vectors;
    [SerializeField]
    int[] triangles;

    public void InitChunk()
    {
        this.meshFilter = GetComponent<MeshFilter>();
        this.meshRenderer = GetComponent<MeshRenderer>();
        mesh = new Mesh();

        StartCoroutine(GenerateMesh());

        meshFilter.mesh = mesh;
    }
    private void Update()
    {
        UpdateMesh();
    }

    IEnumerator GenerateMesh()
    {
        vectors = new Vector3[(mapSize) * (mapSize)];

        for (int i = 0, x = 0; x < mapSize; x++)
        {
            for (int z = 0; z < mapSize; z++)
            {
                vectors[i] = new Vector3(x, 0, z);
                i++;
            }
        }

        triangles = new int[mapSize * mapSize * 6];
        int tris = 0;
        int vect = 0;
        for(int i = 0; i < mapSize-1; i++)
        {
            for (int j = 0; j < mapSize-1; j++)
            {
                triangles[tris + 0] = vect;
                triangles[tris + 1] = vect + 1;
                triangles[tris + 2] = vect + mapSize;
                triangles[tris + 3] = vect + 1;
                triangles[tris + 4] = vect + mapSize + 1;
                triangles[tris + 5] = vect + mapSize;

                vect++;
                tris += 6;

            }
            vect++;
        }


        for(int i=0, x =0; x < mapSize; x++)
        {
            for(int z =0; z < mapSize; z++)
            {
                float xCoord = (seed + x) / (mapSize * scale);
                float yCoord = (seed + z) / (mapSize * scale);

                vectors[i].y = maxHeight * Mathf.PerlinNoise(xCoord, yCoord);
                i++;
            }
        }

        yield return null;

    }
    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vectors;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
