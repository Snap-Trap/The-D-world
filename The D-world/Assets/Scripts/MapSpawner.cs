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
    public Terrain groundTerrain, waterTerrain;
    // Something class related
    public NoiseGenerator rockNoise;
    public NoiseGenerator treeNoise;
    public NoiseGenerator groundNoise;
    public float minRock, maxRock, densityRock, minTree, maxTree, densityTree, minGround, maxGround;

    // List variables so that prefabs don't spawn inside each other
    private List<Vector3> rockPositions = new List<Vector3>();
    private List<Vector3> treePositions = new List<Vector3>();

    // Temporary list so I can quickly destroy shit and respawn
    private List<GameObject> spawnedObjects = new List<GameObject>();

    // Basic variables
    public GameObject treePrefab, rockPrefab, groundMesh, waterPrefab;

    public int mapWidth, mapDepth, mapHeight;
    public float spacing, waterLevel;

    public void Start()
    {
        RandomizeNoiseOffsets();
        SetupTerrain();
        GenerateMap();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetMap();
        }
    }

    public void ResetMap()
    {
        if (Application.isPlaying)
        {
            DestroyMap();
            RandomizeNoiseOffsets();
            SetupTerrain();
            GenerateMap();
        }
    }

    public void GenerateMap()
    {
        Terrain terrain = groundMesh.GetComponent<Terrain>();
        TerrainData groundData = terrain.terrainData;
        // For loop that first spawns the ground, then spawns the trees
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapDepth; z++)
            {
                float offsetX = Random.Range(-spacing / 2f + 0.3f, spacing / 2f - 0.3f);
                float offsetZ = Random.Range(-spacing / 2f + 0.3f, spacing / 2f - 0.3f);
                Vector3 spawnPosition = new Vector3(x * spacing + offsetX, 3, z * spacing + offsetZ);

                spawnPosition.x += offsetX;
                spawnPosition.z += offsetZ;
                spawnPosition.y += terrain.SampleHeight(spawnPosition) + terrain.transform.position.y;

                float treeValue = treeNoise.GetValue(x, z);

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

        // Yes I copied this shit from a row above, cry about it
        // For loop that then only checks where the rocks are supposed
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapDepth; z++)
            {
                float offsetX = Random.Range(-spacing / 2f + 0.3f, spacing / 2f - 0.3f);
                float offsetZ = Random.Range(-spacing / 2f + 0.3f, spacing / 2f - 0.3f);
                Vector3 spawnPosition = new Vector3(x * spacing + offsetX, -1.5f, z * spacing + offsetZ);

                spawnPosition.x += offsetX;
                spawnPosition.z += offsetZ;
                spawnPosition.y += terrain.SampleHeight(spawnPosition) + terrain.transform.position.y;

                float rockValue = rockNoise.GetValue(x, z);

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

    //    var groundMaterial = groundMesh.GetComponent<Renderer>().material;

    //    Texture2D texture = new Texture2D(mapWidth, mapDepth);

    //    for (int x = 0; x < mapWidth; x++)
    //    {
    //        for (int y = 0; y < mapDepth; y++)
    //        {
    //            float groundValue = groundNoise.GetValue(x, y);

    //            // Color coding:
    //            // Trees: green where value is between treeMin and treeMax
    //            // Rocks: gray where value is between rockMin and rockMax
    //            // Else: normal grayscale of noise
    //            Color color = new Color(groundValue, 0, 0);
    //            texture.SetPixel(x, y, color);
    //        }
    //    }

    //    texture.Apply();
    //    groundMaterial.SetTexture("_HeightMap", texture);
    //    groundMesh.transform.position = new Vector3(mapWidth / 2 * spacing, 0, mapDepth / 2 * spacing);
    //    groundMesh.transform.localScale = new Vector3(mapWidth / 2 * spacing, mapDepth / 2 * spacing, mapHeight);
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

    private void RandomizeNoiseOffsets()
    {
        treeNoise.offset = new Vector3(Random.Range(0f, 9999f), 0, Random.Range(0f, 9999f));
        rockNoise.offset = new Vector3(Random.Range(0f, 9999f), 0, Random.Range(0f, 9999f));
        groundNoise.offset = new Vector3(Random.Range(0f, 9999f), 0, Random.Range(0f, 9999f));
    }
    private void SetupTerrain()
    {
        // This is for the ground terrain
        // Make sure your groundMesh is actually a Terrain
        if (groundTerrain == null || waterTerrain == null)
        {
            Debug.LogError("You haven't assigned the terrains in the inspector you dumb fuck");
            return;
        }

        TerrainData groundData = new TerrainData();
        groundData.heightmapResolution = mapWidth + 1; // Must be one more than width
        groundData.size = new Vector3(mapWidth * spacing, mapHeight, mapDepth * spacing);

        float[,] groundHeights = new float[groundData.heightmapResolution, groundData.heightmapResolution];

        for (int x = 0; x < groundData.heightmapResolution; x++)
        {
            for (int z = 0; z < groundData.heightmapResolution; z++)
            {
                float noiseValue = groundNoise.GetValue(x, z);
                groundHeights[x, z] = Mathf.Clamp01(noiseValue);
            }
        }

        groundData.SetHeights(0, 0, groundHeights);
        groundTerrain.terrainData = groundData;

        // Ensure the TerrainCollider uses this TerrainData
        TerrainCollider tCollider = groundMesh.GetComponent<TerrainCollider>();
        tCollider.terrainData = groundData;

        TerrainData waterData = new TerrainData();
        waterData.heightmapResolution = mapWidth + 1; // Must be one more than width
        waterData.size = new Vector3(mapWidth * spacing, mapHeight, mapDepth * spacing);

        float[,] waterHeights = new float[waterData.heightmapResolution, waterData.heightmapResolution];

        for (int x = 0; x < waterData.heightmapResolution; x++)
        {
            for (int z = 0; z < waterData.heightmapResolution; z++)
            {
                waterHeights[x, z] = waterLevel;
            }
        }

        waterData.SetHeights(0, 0, waterHeights);
        waterTerrain.terrainData = waterData;
    }
}
