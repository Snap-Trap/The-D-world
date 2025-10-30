using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapSpawner : MonoBehaviour
{
    // Something class related
    public NoiseGenerator rockNoise;
    public NoiseGenerator treeNoise;

    // List variables so that prefabs don't spawn inside each other
    private List<Vector3> rockPositions = new List<Vector3>();
    private List<Vector3> treePositions = new List<Vector3>();

    // Basic variables
    public GameObject treePrefab, rockPrefab, groundPrefab, waterPrefab;

    public int mapWidth, mapDepth;
    public float spacing, noiseScale;

    public void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        // 2 for loops, runs for however big the map is gonna be
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapDepth; z++)
            {
                // Makes it spawn in a grid
                Vector3 basePosition = new Vector3(x * spacing, 0, z * spacing);
                Instantiate(groundPrefab, basePosition, Quaternion.identity);

                float rockValue = rockNoise.GetValue(x, z);
                float treeValue = treeNoise.GetValue(x, z);

                // Offset so rocks n trees spawn a bit different per tile
                float offsetX = Random.Range(-spacing / 2f + 0.3f, spacing / 2f - 0.3f);
                float offsetZ = Random.Range(-spacing / 2f + 0.3f, spacing / 2f - 0.3f);
                Vector3 spawnPosition = basePosition + new Vector3(offsetX, 0, offsetZ);

                // I NEED the Vector3.up or else shit spawns in the ground
                if (treeValue > 0.1f && treeValue < 0.3f)
                {
                    if (CanSpawn(treePositions, spawnPosition, 2.0f))
                    {
                        Instantiate(treePrefab, spawnPosition + Vector3.up * 3f, Quaternion.identity);
                        treePositions.Add(spawnPosition);
                    }
                }
                else if (rockValue > 0.7f && rockValue < 0.8f)
                {
                    if (CanSpawn(rockPositions, spawnPosition, 4.5f))
                    {
                        Instantiate(rockPrefab, spawnPosition + Vector3.up * 2.5f, Quaternion.identity);
                        rockPositions.Add(spawnPosition);
                    }
                }
            }
        }
    }

    private bool CanSpawn(List<Vector3> existingPositions, Vector3 newPosition, float minDistance)
    {
        foreach (Vector3 position in existingPositions)
        {
            if (Vector3.Distance(position, newPosition) < minDistance)
            {
                return false;
            }
        }
        return true;
    }
}
