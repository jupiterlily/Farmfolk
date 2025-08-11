using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public int width;
    public int height;
    public float waterLevel;
    public float rockLevel;
    public float sandLevel;
    public float grassLevel;
    public float lavaLevel;
    public float farmlandLevel;
    public float pondChance;
    public float lavaChance;
    public float farmlandChance;
    public float sedimentationRate;
    public float erosionRate;
    public float upliftRate;
    public float maxUpliftHeight;

    public GameObject waterPrefab;
    public GameObject rockPrefab;
    public GameObject sandPrefab;
    public GameObject grassPrefab;
    public GameObject lavaPrefab;
    public GameObject farmlandPrefab;
    public GameObject pondPrefab;

    private float[,] heightMap;

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        heightMap = new float[width, height];

        // Generate initial height map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
                heightMap[x, y] = noise;
            }
        }

        // Apply geological processes to the height map
        for (int i = 0; i < 50; i++)
        {
            Erode();
            Sediment();
            Uplift();
        }

        // Create terrain objects based on final height map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float heightValue = heightMap[x, y];

                if (heightValue < waterLevel)
                {
                    Instantiate(waterPrefab, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (heightValue < rockLevel)
                {
                    Instantiate(rockPrefab, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (heightValue < sandLevel)
                {
                    Instantiate(sandPrefab, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (heightValue < grassLevel)
                {
                    Instantiate(grassPrefab, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (heightValue < lavaLevel)
                {
                    // Check if lava should be generated
                    if (Random.Range(0f, 1f) < lavaChance)
                    {
                        Instantiate(lavaPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    }
                }
                else if (heightValue < farmlandLevel)
                {
                    // Check if farmland should be generated
                    if (Random.Range(0f, 1f) < farmlandChance)
                    {
                        Instantiate(farmlandPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(grassPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    }
                }
                else
                {
                    // Check if a pond should be generated
                    if (Random.Range(0f, 1f) < pondChance)
                    {
                        Instantiate(pondPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    }
                }
            }
        }
    }

    void Erode()
    {
        float[,] newHeightMap = new float[width, height];

        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                float deltaHeight = 0;

                for (int nx = -1; nx <= 1; nx++)
                {
                    for (int ny = -1; ny <= 1; ny++)
                    {
                        if (nx == 0 && ny == 0) continue;

                        float neighborHeight = heightMap[x + nx, y + ny];
                        float heightDifference = heightMap[x, y] - neighborHeight;

                        if (heightDifference > 0)
                        {
                            deltaHeight += heightDifference;
                        }
                    }
                }

                newHeightMap[x, y] = heightMap[x, y] - erosionRate * deltaHeight;
            }
        }

        heightMap = newHeightMap;
    }

    void Sediment()
    {
        float[,] newHeightMap = new float[width, height];

        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                float deltaHeight = 0;

                for (int nx = -1; nx <= 1; nx++)
                {
                    for (int ny = -1; ny <= 1; ny++)
                    {
                        if (nx == 0 && ny == 0) continue;

                        float neighborHeight = heightMap[x + nx, y + ny];
                        float heightDifference = heightMap[x, y] - neighborHeight;

                        if (heightDifference < 0)
                        {
                            deltaHeight += heightDifference;
                        }
                    }
                }

                newHeightMap[x, y] = heightMap[x, y] + sedimentationRate * deltaHeight;
            }
        }

        heightMap = newHeightMap;
    }

    void Uplift()
    {
        float[,] newHeightMap = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float deltaHeight = Random.Range(0f, maxUpliftHeight);

                newHeightMap[x, y] = heightMap[x, y] + upliftRate * deltaHeight;
            }
        }

        heightMap = newHeightMap;
    }
}