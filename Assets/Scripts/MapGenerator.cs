using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct TerrainGeneratorSettings
{
    public int chunkSize;
    public float scale;
    public float maxHeight;
    public float seed;
    public int worldRadius;
    public float layer2Value;
}
public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject chunkPrefab;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Vector2 playerChunkPoss = new Vector2(0, 0);
    private Vector2 lastPlayerPoss;

    [SerializeField]
    private TerrainGeneratorSettings generatorSettings;
    public TerrainGeneratorSettings GeneratorSettings
    {
        get { return this.generatorSettings; }
    }

    public TextureAtlas textureAtlas;
    private Dictionary<string, Chunk> chunks = new Dictionary<string, Chunk>();

    private void Start()
    {
        if (!TryGetComponent(out textureAtlas))
            textureAtlas = gameObject.AddComponent<TextureAtlas>();

        textureAtlas.Init();

        GenerateSeed();
        CreateChunks();
    }
    private void Update()
    {
        UpdatePlayerPossition();

        if(lastPlayerPoss != playerChunkPoss)
            CreateChunks();
    }

    private void CreateChunks()
    {
        int x1 = -generatorSettings.worldRadius + (int)playerChunkPoss.x - 1;
        int x2 = generatorSettings.worldRadius + (int)playerChunkPoss.x + 1;
        int z1 = -generatorSettings.worldRadius + (int)playerChunkPoss.y - 1;
        int z2 = generatorSettings.worldRadius + (int)playerChunkPoss.y + 1;

        for (int x = x1; x <= x2; x++)
        {
            for(int z = z1; z <= z2; z++)
            {
                if(x == x1 || x == x2 || z == z1 || z == z2)
                {
                    if(chunks.TryGetValue(Chunk.GenerateChunkName(new Vector2(x, z)), out Chunk c))
                    {
                        chunks.Remove(c.ChunkName);
                        Destroy(c.gameObject);
                    }
                    continue;
                }

                if (!chunks.TryGetValue(Chunk.GenerateChunkName(new Vector2(x, z)), out Chunk outChunk))
                {
                    GameObject gm = Instantiate(chunkPrefab, gameObject.transform);
                    gm.transform.position = new Vector3(x * (generatorSettings.chunkSize - 1), 0, z * (generatorSettings.chunkSize - 1));

                    Chunk chunk = gm.GetComponent<Chunk>();
                    chunk.InitChunk(new Vector2(x, z), generatorSettings);

                    chunks.Add(chunk.ChunkName, chunk);
                }
            }
        }
    }

    private void GenerateSeed()
    {
        int newSeed = UnityEngine.Random.Range(1000000, 9999999);

        this.generatorSettings.seed = newSeed;
    }

    private void UpdatePlayerPossition()
    {
        lastPlayerPoss = playerChunkPoss;
        playerChunkPoss = new Vector2(playerTransform.position.x / generatorSettings.chunkSize, playerTransform.position.z / generatorSettings.chunkSize);
        playerChunkPoss = new Vector2(Mathf.Floor(playerChunkPoss.x), Mathf.Floor(playerChunkPoss.y));
    }
}
