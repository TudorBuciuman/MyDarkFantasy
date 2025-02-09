using System;
public class NoiseGeneratorImproved
{
    private int[] permutation;
    private double offsetX, offsetY, offsetZ;

    public NoiseGeneratorImproved(Random random)
    {
        permutation = new int[512];

        offsetX = random.NextDouble() * 256.0;
        offsetY = random.NextDouble() * 256.0;
        offsetZ = random.NextDouble() * 256.0;

        int[] p = new int[256];
        for (int i = 0; i < 256; i++)
        {
            p[i] = i;
        }

        for (int i = 255; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (p[i], p[j]) = (p[j], p[i]);
        }

        for (int i = 0; i < 256; i++)
        {
            permutation[i] = p[i];
            permutation[i + 256] = p[i];
        }
    }

    public void GenerateNoise(double[] noiseArray, double x, double y, double z, int width, int height, int depth, double scaleX, double scaleY, double scaleZ, double amplitude)
    {
        if (height == 1)
        {
            Generate2DNoise(noiseArray, x, z, width, depth, scaleX, scaleZ, amplitude);
        }
        else
        {
            Generate3DNoise(noiseArray, x, y, z, width, height, depth, scaleX, scaleY, scaleZ, amplitude);
        }
    }

    private void Generate2DNoise(double[] noiseArray, double x, double z, int width, int depth, double scaleX, double scaleZ, double amplitude)
    {
        int index = 0;
        double scaleInv = 1.0 / amplitude;

        for (int i = 0; i < width; i++)
        {
            double sampleX = x + i * scaleX + offsetX;
            int intX = (int)Math.Floor(sampleX) & 255;
            sampleX -= Math.Floor(sampleX);
            double fadeX = Fade(sampleX);

            for (int j = 0; j < depth; j++)
            {
                double sampleZ = z + j * scaleZ + offsetZ;
                int intZ = (int)Math.Floor(sampleZ) & 255;
                sampleZ -= Math.Floor(sampleZ);
                double fadeZ = Fade(sampleZ);

                int a = permutation[intX] + 0;
                int aa = permutation[a] + intZ;
                int b = permutation[intX + 1] + 0;
                int ba = permutation[b] + intZ;

                double lerp1 = Lerp(fadeX, Grad(permutation[aa], sampleX, sampleZ), Grad(permutation[ba], sampleX - 1, sampleZ));
                double lerp2 = Lerp(fadeX, Grad(permutation[aa + 1], sampleX, sampleZ - 1), Grad(permutation[ba + 1], sampleX - 1, sampleZ - 1));
                double value = Lerp(fadeZ, lerp1, lerp2);

                noiseArray[index++] += value * scaleInv;
            }
        }
    }

    private void Generate3DNoise(double[] noiseArray, double x, double y, double z, int width, int height, int depth, double scaleX, double scaleY, double scaleZ, double amplitude)
    {
        int index = 0;
        double scaleInv = 1.0 / amplitude;
        int lastHash = -1;

        int a, b, aa, ab, ba, bb;
        double lerp1, lerp2, lerp3, lerp4;

        for (int i = 0; i < width; i++)
        {
            double sampleX = x + i * scaleX + offsetX;
            int intX = (int)Math.Floor(sampleX) & 255;
            sampleX -= Math.Floor(sampleX);
            double fadeX = Fade(sampleX);

            for (int j = 0; j < depth; j++)
            {
                double sampleZ = z + j * scaleZ + offsetZ;
                int intZ = (int)Math.Floor(sampleZ) & 255;
                sampleZ -= Math.Floor(sampleZ);
                double fadeZ = Fade(sampleZ);

                for (int k = 0; k < height; k++)
                {
                    double sampleY = y + k * scaleY + offsetY;
                    int intY = (int)Math.Floor(sampleY) & 255;
                    sampleY -= Math.Floor(sampleY);
                    double fadeY = Fade(sampleY);
                    double value = 0;
                    if (k == 0 || intY != lastHash)
                    {
                        lastHash = intY;
                        a = permutation[intX] + intY;
                        aa = permutation[a] + intZ;
                        ab = permutation[a + 1] + intZ;
                        b = permutation[intX + 1] + intY;
                        ba = permutation[b] + intZ;
                        bb = permutation[b + 1] + intZ;

                        lerp1 = Lerp(fadeX, Grad(permutation[aa], sampleX, sampleY, sampleZ), Grad(permutation[ba], sampleX - 1, sampleY, sampleZ));
                        lerp2 = Lerp(fadeX, Grad(permutation[ab], sampleX, sampleY - 1, sampleZ), Grad(permutation[bb], sampleX - 1, sampleY - 1, sampleZ));
                        lerp3 = Lerp(fadeX, Grad(permutation[aa + 1], sampleX, sampleY, sampleZ - 1), Grad(permutation[ba + 1], sampleX - 1, sampleY, sampleZ - 1));
                        lerp4 = Lerp(fadeX, Grad(permutation[ab + 1], sampleX, sampleY - 1, sampleZ - 1), Grad(permutation[bb + 1], sampleX - 1, sampleY - 1, sampleZ - 1));

                        value = Lerp(fadeZ, Lerp(fadeY, lerp1, lerp2), Lerp(fadeY, lerp3, lerp4));

                    }

                    noiseArray[index++] += value * scaleInv;
                }
            }
        }
    }

    private static double Fade(double t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static double Lerp(double t, double a, double b)
    {
        return a + t * (b - a);
    }

    private static double Grad(int hash, double x, double y = 0, double z = 0)
    {
        int h = hash & 15;
        double u = h < 8 ? x : y;
        double v = h < 4 ? y : h == 12 || h == 14 ? x : z;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }
}
