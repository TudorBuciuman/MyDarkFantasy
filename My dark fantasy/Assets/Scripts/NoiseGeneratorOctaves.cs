using System.Collections;
using System.Collections.Generic;
using System;

public class NoiseGeneratorOctaves: ChunkProviderGenerate
{
    
    private NoiseGeneratorImproved[] noiseGenerators;
    private int octaveCount;

    public NoiseGeneratorOctaves(Random random, int octaves)
    {
        this.octaveCount = octaves;
        this.noiseGenerators = new NoiseGeneratorImproved[octaves];

        for (int i = 0; i < octaves; ++i)
        {
            this.noiseGenerators[i] = new NoiseGeneratorImproved(random);
        }
    }

    public double[] generateNoise(double[] noiseArray, int x, int y, int z, int width, int height, int depth, double scaleX, double scaleY, double scaleZ)
    {
        if (noiseArray == null)
        {
            noiseArray = new double[width * height * depth];
        }
        else
        {
            for (int i = 0; i < noiseArray.Length; ++i)
            {
                noiseArray[i] = 0.0D;
            }
        }

        double amplitude = 1.0D;

        for (int octave = 0; octave < this.octaveCount; ++octave)
        {
            double offsetX = (double)x * amplitude * scaleX;
            double offsetY = (double)y * amplitude * scaleY;
            double offsetZ = (double)z * amplitude * scaleZ;
            long floorX = (long)(float)(offsetX);
            long floorZ = (long)(offsetZ);
            offsetX = offsetX - (double)floorX;
            offsetZ = offsetZ - (double)floorZ;
            floorX = floorX % 16777216L;
            floorZ = floorZ % 16777216L;
            offsetX = offsetX + (double)floorX;
            offsetZ = offsetZ + (double)floorZ;
            this.noiseGenerators[octave].GenerateNoise(noiseArray, offsetX, offsetY, offsetZ, width, height, depth, scaleX * amplitude, scaleY * amplitude, scaleZ * amplitude, amplitude);
            amplitude /= 2.0D;
        }

        return noiseArray;
    }

    public double[] generateNoise2D(double[] noiseArray, int x, int z, int width, int depth, double scaleX, double scaleZ, double amplitude)
    {
        return this.generateNoise(noiseArray, x, 10, z, width, 1, depth, scaleX, 1.0D, scaleZ);
    }
}

