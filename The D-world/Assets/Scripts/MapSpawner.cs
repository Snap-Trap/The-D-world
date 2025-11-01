using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using EasyButtons;

public class MapSpawner : MonoBehaviour
{
    // Something class related
    public NoiseGenerator rockNoise;
    public NoiseGenerator treeNoise;
    public float minRock, maxRock, densityRock, minTree, maxTree, densityTree;

    // List variables so that prefabs don't spawn inside each other
    private List<Vector3> rockPositions = new List<Vector3>();
    private List<Vector3> treePositions = new List<Vector3>();

    // Temporary list so I can quickly destroy shit and respawn
    private List<GameObject> spawnedObjects = new List<GameObject>();

    // Basic variables
    public GameObject treePrefab, rockPrefab, groundPrefab, waterPrefab;

    public int mapWidth, mapDepth;
    public float spacing;

    public void Start()
    {
        treeNoise.offset = new Vector2(Random.Range(0f, 9999f), Random.Range(0f, 9999f));
        rockNoise.offset = new Vector2(Random.Range(0f, 9999f), Random.Range(0f, 9999f));

        GenerateMap();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetMap();
        }
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
                var gameObject2 = Instantiate(groundPrefab, basePosition, Quaternion.identity);
                spawnedObjects.Add(gameObject2);

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
                        var gameObject = Instantiate(treePrefab, spawnPosition + Vector3.up * 3f, Quaternion.identity);

                        treePositions.Add(spawnPosition);
                        spawnedObjects.Add(gameObject);
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

                // Offset so rocks n trees spawn a bit different per tile
                float offsetX = Random.Range(-spacing / 2f + 0.3f, spacing / 2f - 0.3f);
                float offsetZ = Random.Range(-spacing / 2f + 0.3f, spacing / 2f - 0.3f);
                Vector3 spawnPosition = basePosition + new Vector3(offsetX, 0, offsetZ);

                // I NEED the Vector3.up or else shit spawns in the ground
                if (NoiseRange(rockValue, minRock, maxRock, densityRock) && !IsNearTree(spawnPosition, treePositions, 10f))
                {
                    if (CanSpawn(rockPositions, spawnPosition, 5f))
                    {
                        var gameObject3 = Instantiate(rockPrefab, spawnPosition + Vector3.up * 2.5f, Quaternion.identity);
                        rockPositions.Add(spawnPosition);
                        spawnedObjects.Add(gameObject3);
                    }
                }
            }
        }
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

    public void DestroyMap()
    {
        foreach (GameObject prefabs in spawnedObjects.ToArray())
        {
            if (prefabs != null)
            {
                Destroy(prefabs);
            }
            spawnedObjects.Clear();
            treePositions.Clear();
            rockPositions.Clear();
        }
    }
    [Button]
    public void ResetMap()
    {
        if (Application.isPlaying)
        {
            DestroyMap();
            RandomizeNoiseOffsets();
            GenerateMap();
        }
    }

    private void RandomizeNoiseOffsets()
    {
        treeNoise.offset = new Vector2(Random.Range(0f, 9999f), Random.Range(0f, 9999f));
        rockNoise.offset = new Vector2(Random.Range(0f, 9999f), Random.Range(0f, 9999f));
    }
}
