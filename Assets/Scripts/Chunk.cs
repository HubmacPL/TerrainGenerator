using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    public string ChunkName;

    private Mesh mesh;


    [SerializeField]
    private Vector2 chunkPoss;
    private MapGenerator mapGenerator;

    private TerrainGeneratorSettings genSets;

    [SerializeField]
    private Vector3[] vectors;
    public Vector3[] Verticles
    {
        get { return this.vectors; }
        set { this.vectors = value;}
    }
    int[] triangles;
    [SerializeField]
    private Vector2[] uvs;
    [SerializeField]
    private Material material;
    private MeshCollider meshCollider;

    public void InitChunk(Vector2 chunkPoss, TerrainGeneratorSettings tgS)
    {
        GetComponent<MeshRenderer>().material = material;
        meshCollider = GetComponent<MeshCollider>();

        ChunkName = Chunk.GenerateChunkName(chunkPoss);

        this.mapGenerator = FindObjectOfType<MapGenerator>() as MapGenerator;
        this.genSets = tgS;

        this.chunkPoss = chunkPoss;

        this.meshFilter = GetComponent<MeshFilter>();
        this.meshRenderer = GetComponent<MeshRenderer>();
        mesh = new Mesh();

        material.mainTexture = mapGenerator.textureAtlas.atlas;


        GenerateMesh();


        meshFilter.mesh = mesh;

        UpdateMesh();

        meshCollider.sharedMesh = mesh;
    }

    private void GenerateMesh()
    {
        vectors = new Vector3[(genSets.chunkSize) * (genSets.chunkSize)];

        for (int i = 0, x = 0; x < genSets.chunkSize; x++)
        {
            for (int z = 0; z < genSets.chunkSize; z++)
            {
                vectors[i] = new Vector3(x, 0, z);
                i++;
            }
        }

        triangles = new int[genSets.chunkSize * genSets.chunkSize * 6];
        int tris = 0;
        int vect = 0;
        for (int i = 0; i < genSets.chunkSize - 1; i++)
        {
            for (int j = 0; j < genSets.chunkSize - 1; j++)
            {
                triangles[tris + 0] = vect;
                triangles[tris + 1] = vect + 1;
                triangles[tris + 2] = vect + genSets.chunkSize;
                triangles[tris + 3] = vect + 1;
                triangles[tris + 4] = vect + genSets.chunkSize + 1;
                triangles[tris + 5] = vect + genSets.chunkSize;

                vect++;
                tris += 6;

            }
            vect++;
        }
        UpdateMesh();
    }
    private void UpdateMesh()
    {
        mesh.Clear();

        GeneratePerlinNoise();
        PaintChunk();

        mesh.vertices = vectors;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
    }
    private void GeneratePerlinNoise()
    {
        this.genSets = mapGenerator.GeneratorSettings;
        for (int i = 0, x = 0; x < genSets.chunkSize; x++)
        {
            for (int z = 0; z < genSets.chunkSize; z++)
            {
                float xCoord = ((x + (chunkPoss.x * (genSets.chunkSize - 1))) + genSets.seed) / (genSets.chunkSize * genSets.scale);
                float yCoord = ((z + (chunkPoss.y * (genSets.chunkSize - 1))) + genSets.seed) / (genSets.chunkSize * genSets.scale);

                float value = genSets.maxHeight * (Mathf.PerlinNoise(xCoord, yCoord) * genSets.layer2Value * Mathf.PerlinNoise(xCoord, yCoord));

                vectors[i].y = value;
                i++;
            }
        }
    }
    private void PaintChunk()
    {
        uvs = new Vector2[vectors.Length];

        for(int i=0, x=0; x < genSets.chunkSize; x++)
        {
            for(int y=0; y < genSets.chunkSize; y++)
            {
                string selectedTexture = "";

                float mediumHeight = vectors[i].y;

                if(mediumHeight > genSets.maxHeight * 0.25f)
                {
                    selectedTexture = "Rock";
                }
                else if(mediumHeight > genSets.maxHeight * 0.1f)
                {
                    selectedTexture = "Grass";
                }
                else if(mediumHeight > genSets.maxHeight * 0.05f)
                {
                    selectedTexture = "Sand";
                }
                else
                {
                    selectedTexture = "Water";
                }


                Rect rect;

                mapGenerator.textureAtlas.atlasDictionary.TryGetValue(selectedTexture, out rect);

                Vector2 calculateUV = new Vector2(((rect.x + rect.width) - rect.width / 2), ((rect.y + rect.height) - rect.height / 2));

                uvs[i] = calculateUV;

                i++;
            }
        }

    }
    public static string GenerateChunkName(Vector2 chunkPoss)
    {
        string str = "X: " + chunkPoss.x + " Y: " + chunkPoss.y;

        return str;
    }
}
