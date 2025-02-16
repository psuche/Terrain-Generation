﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class plgMapGenerator : MonoBehaviour
{

    public enum DrawMode { NoiseMap, ColourMap, Mesh, FallOffMap };
    public DrawMode drawMode;

    public plgNoise.NormalizeMode normalizeMode;

    public const int mapChunkSize = 239;
    [Range(0, 6)]
    public int editorPreviewLevelOfDetail;
    public float noiseScale;

    [Range(0, 6)]
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool useFallOffMap;

    public float meshHeighMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainType[] regions;

    float[,] fallOffMap;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();


    private void Awake()
    {
        fallOffMap = FallOffGenerator.GenerateFallOffMap(mapChunkSize);
    }

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);

        plgMapDisplay display = FindObjectOfType<plgMapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(plgTextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(plgTextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(plgMeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeighMultiplier, meshHeightCurve, editorPreviewLevelOfDetail), plgTextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.FallOffMap)
        {
            display.DrawTexture(plgTextureGenerator.TextureFromHeightMap(FallOffGenerator.GenerateFallOffMap(mapChunkSize)));
        }
    }

    public void RequestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(center);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lod, callback);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = plgMeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeighMultiplier, meshHeightCurve, lod);
        lock(meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    MapData GenerateMapData(Vector2 center)
    {
        float[,] noiseMap = plgNoise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, seed, noiseScale, octaves, persistance, lacunarity, center + offset, normalizeMode);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                if (useFallOffMap)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - fallOffMap[x, y]);
                }
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight >= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        //plgMapDisplay display = FindObjectOfType<plgMapDisplay>();
        //if (drawMode == DrawMode.NoiseMap)
        //{
        //    display.DrawTexture(plgTextureGenerator.TextureFromHeightMap(noiseMap));
        //}
        //else if (drawMode == DrawMode.ColourMap)
        //{
        //    display.DrawTexture(plgTextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        //}
        //else if (drawMode == DrawMode.Mesh)
        //{
        //    display.DrawMesh(plgMeshGenerator.GenerateTerrainMesh(noiseMap, meshHeighMultiplier, meshHeightCurve, levelOfDetail), plgTextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        //}
        return new MapData(noiseMap, colourMap);
    }

    private void OnValidate()
    {
        
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        fallOffMap = FallOffGenerator.GenerateFallOffMap(mapChunkSize);
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }

}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;

    public MapData(float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }

}



