using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapSpawner : MonoBehaviour
{
    // Something class related
    public NoiseGenerator rockNoise;
    public NoiseGenerator treeNoise;
    public float minRock, maxRock, densityRock, minTree, maxTree, densityTree;

    // List variables so that prefabs don't spawn inside each other
    private List<Vector3> rockPositions = new List<Vector3>();
    private List<Vector3> treePositions = new List<Vector3>();

    // Basic variables
    public GameObject treePrefab, rockPrefab, groundPrefab, waterPrefab;

    public int mapWidth, mapDepth;
    public float spacing;

    public void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        // For loop that first spawns the ground, then spawns the trees
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
                if (NoiseRange(treeValue, minTree, maxTree, densityTree))
                {
                    if (CanSpawn(treePositions, spawnPosition, 2.5f))
                    {
                        Instantiate(treePrefab, spawnPosition + Vector3.up * 3f, Quaternion.identity);
                        treePositions.Add(spawnPosition);
                    }
                }
            }
        }

        // Yes I copied this shit from a row above, no I do not instantiate the map
        // For loop that then only checks where the rocks are supposed
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapDepth; z++)
            {
                // Makes it spawn in a grid
                Vector3 basePosition = new Vector3(x * spacing, 0, z * spacing);

                float rockValue = rockNoise.GetValue(x, z);
                float treeValue = treeNoise.GetValue(x, z);

                // Offset so rocks n trees spawn a bit different per tile
                float offsetX = Random.Range(-spacing / 2f + 0.3f, spacing / 2f - 0.3f);
                float offsetZ = Random.Range(-spacing / 2f + 0.3f, spacing / 2f - 0.3f);
                Vector3 spawnPosition = basePosition + new Vector3(offsetX, 0, offsetZ);

                // I NEED the Vector3.up or else shit spawns in the ground
                if (NoiseRange(rockValue, minRock, maxRock, densityRock) && !IsNearTree(spawnPosition, treePositions, 10f))
                {
                    if (CanSpawn(rockPositions, spawnPosition, 5f))
                    {
                        Instantiate(rockPrefab, spawnPosition + Vector3.up * 2.5f, Quaternion.identity);
                        rockPositions.Add(spawnPosition);
                    }
                }
            }
        }
    }

    public void LoopFunction(int MapWidth, int MapDepth, GameObject groundPrefab, float spacing, Func<int, int, float> noiseFunc, float minNoise, float maxNoise, float density, List<Vector3> existingPositions, float minDistance, bool avoidTrees)
    {

    }

    public bool NoiseRange(float value, float min, float max, float density)
    {
        return value > min && value < max && Random.value < density; ;
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
    private bool IsNearTree(Vector3 rockPosition, List<Vector3> treePositions, float minDistance)
    {
        foreach (Vector3 treePosition in treePositions)
        {
            if (Vector3.Distance(rockPosition, treePosition) < minDistance)
                return true;
        }
        return false;
    }
}
