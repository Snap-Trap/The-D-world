using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseGenerator
{
    // L: I'm just going to write a guide for later.
    // The lower the noiseScale is, the more clustered objects spawned, would recommend a minimum of 0.05f
    // Vise versa for a higher noiseScale value, it will spread/scatter across the spawning area
    public float noiseScale = 0.1f;
    public Vector3 offset;

    public float GetValue(float x, float z)
    {
        return Mathf.PerlinNoise(x * noiseScale, z * noiseScale);
    }
}
