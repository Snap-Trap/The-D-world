using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapSpawner))]
public class NoiseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapSpawner mapSpawner = (MapSpawner)target;
        Texture2D texture = new Texture2D(mapSpawner.mapWidth, mapSpawner.mapDepth);

        for (int x = 0; x < mapSpawner.mapWidth; x++)
        {
            for (int y = 0; y < mapSpawner.mapDepth; y++)
            {
                float rockValue = mapSpawner.rockNoise.GetValue(x, y);
                float treeValue = mapSpawner.treeNoise.GetValue(x, y);
                float groundValue = mapSpawner.groundNoise.GetValue(x, y);

                // Color coding:
                // Trees: green where value is between treeMin and treeMax
                // Rocks: gray where value is between rockMin and rockMax
                // Else: normal grayscale of noise
                Color color = new Color(rockValue, treeValue, groundValue);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        float imageWidth = EditorGUIUtility.currentViewWidth - 40;
        float imageHeight = imageWidth * texture.height / texture.width;
        Rect rect = GUILayoutUtility.GetRect(imageWidth, imageHeight);
        GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);

        base.OnInspectorGUI();

        if (GUILayout.Button("ResetMapFUCKYOU"))
        {
            mapSpawner.ResetMap();
        }
    }
}
