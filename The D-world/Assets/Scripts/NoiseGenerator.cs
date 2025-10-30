using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseGenerator
{
    public float noiseScale = 0.1f;
    public Vector3 offset;

    public float GetValue(float x, float z)
    {
        return Mathf.PerlinNoise(x * noiseScale, z * noiseScale);
    }
}
