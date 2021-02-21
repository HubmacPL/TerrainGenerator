using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAtlas : MonoBehaviour
{
    public Texture2D[] textures;
    public Texture2D atlas;
    [SerializeField]
    private Rect[] rects;
    private int atlasSize;

    public Dictionary<string, Rect> atlasDictionary = new Dictionary<string, Rect>();

    public void Init()
    {
        GenerateTextureAtlas();
    }

    private void GenerateTextureAtlas()
    {
        if (textures.Length <= 0)
            return;
        atlasSize = textures.Length * textures[0].width;

        atlas = new Texture2D(atlasSize, atlasSize);
        rects = atlas.PackTextures(textures, 0, atlasSize, false);
        atlas.Apply();

        for(int i =0; i < textures.Length; i++)
        {
            atlasDictionary.Add(textures[i].name, rects[i]);
        }
    }
}
