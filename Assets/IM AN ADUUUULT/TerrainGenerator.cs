using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    private void Draw1DNoiseMap(float[,] noiseMap)
    {
        int noiseWidth = noiseMap.GetLength(0);
        int noiseHeight = noiseMap.GetLength(1);

        Color[] colorMap = new Color[noiseWidth * noiseHeight];

        for (int y = 0; y < noiseHeight; y++)
        {
            for (int x = 0; x < noiseWidth; x++)
            {
                colorMap[(y * noiseWidth) + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
    }

    public static float[,] GenerteNoiseMap(Vector3 size)
    {
        int mapWidth = (int)size.x;
        int mapHeight = (int)size.z;
        float scale = TerrainTool.tScale;

        float offsetX = TerrainTool.tUseOffset ? TerrainTool.tOffsetX : 0;
        float offsetY = TerrainTool.tUseOffset ? TerrainTool.tOffsetY : 0;

        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float sampleX = (x / scale) + offsetX;
                float sampleY = (y / scale) + offsetY;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlinValue;
            }
        }

        return noiseMap;
    }

    public static float[,] GenerteTestNoiseMap(Vector3 size)
    {
        int mapWidth = (int)size.x;
        int mapHeight = (int)size.z;
        float scale = TerrainTool.tScale;

        float[,] FalloffMap = GenerateFallOffMap((int)size.x);

        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float sampleX;
                float sampleY;

                if (y > 10 && y < 90 & x > 30 & x < 110)
                {

                    sampleX = (x / scale);
                    sampleY = (y / scale);

                    //sampleX = (x / (scale * 2.3f));
                    //sampleY = (y / (scale * 2.3f));
                  
                }
                else
                {                    
                    sampleX = (x / scale);
                    sampleY = (y / scale);
                }
               

                float perlinValue = Mathf.Clamp01(Mathf.PerlinNoise(sampleX, sampleY) - FalloffMap[x, y]);
                //float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlinValue;                

            }
        }

        return noiseMap;
    }

    public static float[,] GenerateFallOffMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }

        return map;
    }

    public static float Evaluate(float value)
    {
        float a = 3f;
        float b = 2.2f;

        return Mathf.Pow(value, a) / ((Mathf.Pow(value, a)) + Mathf.Pow((b - (b * value)), a));
    }

}

//public static class TerrainExtensions
//{
//    public static void GenerateDesertTerrain(this GameObject t)
//    {
//        TerrainData td = new TerrainData();

//        td.heightmapResolution = TerrainTool.tWidth + 1;
//        td.size = new Vector3(TerrainTool.tWidth, TerrainTool.tHeight, TerrainTool.tLength);
//        td.name = string.IsNullOrEmpty(TerrainTool.tName) ? "Terrain(Desert)" : TerrainTool.tName;
//        td.SetHeights(0, 0, GenerteNoiseMap(td.size));

//        GameObject _terrain = Terrain.CreateTerrainGameObject(td);
//        _terrain.name = td.name;

//        _terrain.transform.parent = t.transform;
//    }

    

//}

