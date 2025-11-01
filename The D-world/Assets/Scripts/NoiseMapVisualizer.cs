using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapVisualizer : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
    public float scale = 0.1f;
    public Vector2 offset;

    [Range(0, 1)] public float treeMin = 0.1f;
    [Range(0, 1)] public float treeMax = 0.3f;
    [Range(0, 1)] public float rockMin = 0.7f;
    [Range(0, 1)] public float rockMax = 0.8f;

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sample = Mathf.PerlinNoise((x + offset.x) * scale, (y + offset.y) * scale);

                // Color coding:
                // Trees: green where value is between treeMin and treeMax
                // Rocks: gray where value is between rockMin and rockMax
                // Else: normal grayscale of noise
                Color color = new Color(sample, sample, sample);

                if (sample >= treeMin && sample <= treeMax)
                    color = Color.green;
                else if (sample >= rockMin && sample <= rockMax)
                    color = Color.gray;

                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }
}
