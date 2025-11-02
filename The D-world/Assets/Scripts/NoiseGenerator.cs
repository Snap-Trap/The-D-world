using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseGenerator
{
    // L: I'm just going to write a guide for later.
    // The lower the noiseScale is, the more clustered objects spawned, would recommend a minimum of 0.05f
    // Vise versa for a higher noiseScale value, it will spread/scatter across the spawning area
    public float baseNoiseScale = 0.1f;
    public int octaves = 4; // Amount of layers
    public float persistance = 0.5f;
    public float lacunarity = 2f;
    public Vector3 offset;

    public float GetValue(float x, float z)
    {
        float total = 0;
        float amplitude = 1f;
        float frequency = baseNoiseScale;
        float maxValue = 0f;

        if (octaves > 20)
        {
            octaves = 20;
        }

        for (int i = 0; i < octaves; i++)
        {
            float noiseValue = Mathf.PerlinNoise(x * frequency + offset.x, z * frequency + offset.z);

            total += noiseValue * amplitude;
            maxValue += amplitude;

            amplitude *= persistance;
            frequency *= lacunarity;
        }

        return total / maxValue;
    }
}
