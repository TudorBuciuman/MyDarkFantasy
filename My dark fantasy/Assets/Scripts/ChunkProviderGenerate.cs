using System.Collections;
using System;
using UnityEngine;

public class ChunkProviderGenerate
{
    /*
    // RNG.
    private Random rand;
    private NoiseGeneratorOctaves field_147431_j;
    private NoiseGeneratorOctaves field_147432_k;
    private NoiseGeneratorOctaves field_147429_l;
    //private NoiseGeneratorPerlin field_147430_m;

    // A NoiseGeneratorOctaves used in generating terrain
    public NoiseGeneratorOctaves noiseGen5;

    // A NoiseGeneratorOctaves used in generating terrain
    public NoiseGeneratorOctaves noiseGen6;
    public NoiseGeneratorOctaves mobSpawnerNoise;

    // Reference to the World object.
    private World worldObj;

    // Are map structures going to be generated (e.g. strongholds)
    private readonly bool mapFeaturesEnabled;
    private WorldType field_177475_o;
    private readonly double[] field_147434_q;
    private readonly float[] parabolicField;
    private ChunkProviderSettings settings;
    private Block oceanBlockTmpl = Blocks.water;
    private double[] stoneNoise = new double[256];
    private MapGenBase caveGenerator = new MapGenCaves();

    // The biomes that are used to generate the chunk
    private BiomeGenBase[] biomesForGeneration;
    private double[] mainNoiseArray;
    private double[] lowerLimitNoiseArray;
    private double[] upperLimitNoiseArray;
    private double[] depthNoiseArray;

    public ChunkProviderGenerate(World worldIn, int seed, bool generateStructures, string structuresJson)
    {
        this.worldObj = worldIn;
        this.mapFeaturesEnabled = generateStructures;
        this.field_177475_o = worldIn.GetWorldInfo().GetTerrainType();
        this.rand = new Random(seed);
        this.field_147431_j = new NoiseGeneratorOctaves(this.rand, 16);
        this.field_147432_k = new NoiseGeneratorOctaves(this.rand, 16);
        this.field_147429_l = new NoiseGeneratorOctaves(this.rand, 8);
        //this.field_147430_m = new NoiseGeneratorPerlin(this.rand, 4);
        this.noiseGen5 = new NoiseGeneratorOctaves(this.rand, 10);
        this.noiseGen6 = new NoiseGeneratorOctaves(this.rand, 16);
        this.mobSpawnerNoise = new NoiseGeneratorOctaves(this.rand, 8);
        this.field_147434_q = new double[825];
        this.parabolicField = new float[25];

        for (int i = -2; i <= 2; ++i)
        {
            for (int j = -2; j <= 2; ++j)
            {
                float f = 10.0F / ((float)(i * i + j * j) + 0.2F);
                this.parabolicField[i + 2 + (j + 2) * 5] = f;
            }
        }
        /*
        if (structuresJson != null)
        {
            this.settings = ChunkProviderSettings.Factory.JsonToFactory(structuresJson).Func_177864_b();
            this.oceanBlockTmpl = this.settings.useLavaOceans ? Blocks.lava : Blocks.water;
            worldIn.SetSeaLevel(this.settings.seaLevel);
        }
        
    }
    public void SetBlocksInChunk(int x, int z, ChunkPrimer primer)
    {
        this.biomesForGeneration = this.worldObj.GetWorldChunkManager().GetBiomesForGeneration(this.biomesForGeneration, x * 4 - 2, z * 4 - 2, 10, 10);
        this.Func_147423_a(x * 4, 0, z * 4);

        for (int i = 0; i < 4; ++i)
        {
            int j = i * 5;
            int k = (i + 1) * 5;

            for (int l = 0; l < 4; ++l)
            {
                int i1 = (j + l) * 33;
                int j1 = (j + l + 1) * 33;
                int k1 = (k + l) * 33;
                int l1 = (k + l + 1) * 33;

                for (int i2 = 0; i2 < 32; ++i2)
                {
                    double d0 = 0.125D;
                    double d1 = this.field_147434_q[i1 + i2];
                    double d2 = this.field_147434_q[j1 + i2];
                    double d3 = this.field_147434_q[k1 + i2];
                    double d4 = this.field_147434_q[l1 + i2];
                    double d5 = (this.field_147434_q[i1 + i2 + 1] - d1) * d0;
                    double d6 = (this.field_147434_q[j1 + i2 + 1] - d2) * d0;
                    double d7 = (this.field_147434_q[k1 + i2 + 1] - d3) * d0;
                    double d8 = (this.field_147434_q[l1 + i2 + 1] - d4) * d0;

                    for (int j2 = 0; j2 < 8; ++j2)
                    {
                        double d9 = 0.25D;
                        double d10 = d1;
                        double d11 = d2;
                        double d12 = (d3 - d1) * d9;
                        double d13 = (d4 - d2) * d9;

                        for (int k2 = 0; k2 < 4; ++k2)
                        {
                            double d14 = 0.25D;
                            double d16 = (d11 - d10) * d14;
                            double lvt_45_1_ = d10 - d16;

                            for (int l2 = 0; l2 < 4; ++l2)
                            {
                                if ((lvt_45_1_ += d16) > 0.0D)
                                {
                                    primer.SetBlockState(i * 4 + k2, i2 * 8 + j2, l * 4 + l2, Blocks.Stone.GetDefaultState());
                                }
                                else if (i2 * 8 + j2 < this.settings.seaLevel)
                                {
                                    primer.SetBlockState(i * 4 + k2, i2 * 8 + j2, l * 4 + l2, this.oceanBlockTmpl.GetDefaultState());
                                }
                            }

                            d10 += d12;
                            d11 += d13;
                        }

                        d1 += d5;
                        d2 += d6;
                        d3 += d7;
                        d4 += d8;
                    }
                }
            }
        }
    }
    public void ReplaceBlocksForBiome(int x, int z, ChunkPrimer primer, BiomeGenBase[] biomeGens)
    {
        double d0 = 0.03125D;
        this.stoneNoise = this.field_147430_m.Func_151599_a(this.stoneNoise, (double)(x * 16), (double)(z * 16), 16, 16, d0 * 2.0D, d0 * 2.0D, 1.0D);

        for (int i = 0; i < 16; ++i)
        {
            for (int j = 0; j < 16; ++j)
            {
                BiomeGenBase biomegenbase = biomeGens[j + i * 16];
                biomegenbase.GenTerrainBlocks(this.worldObj, this.rand, primer, x * 16 + i, z * 16 + j, this.stoneNoise[j + i * 16]);
            }
        }
    }
    */
    
    }