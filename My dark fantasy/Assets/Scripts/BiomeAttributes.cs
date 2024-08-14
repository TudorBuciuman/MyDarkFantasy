using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewBiome", menuName = "Biome Attributes", order = 1)]
public class BiomeAttributes : ScriptableObject
{
    public string BiomeName;
    public byte topblock;
    public byte middleblock;
    public int solidterrainheight;
    public float weight;
    public float scale;
    public float offset;
    public float lac;
    public float per;
    public float m;
    public float valley;
    public float multiplier;
    [Header("Fauna")]
    public float treezone = 1.3f;
    public float treethreshold = 0.6f;
    public float treesize=0.2f;
    public Lode[] lodes;
}

[System.Serializable]
public class Lode
{
    public string name;
    public byte Idblock;
    public int minheight;
    public int maxheight;
    public float scale;
    public float threshold;
    public float noiseOffSet;
}