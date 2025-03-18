using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ExtremeGlitchVisualizer : MonoBehaviour
{
    public AudioSource audioSource;
    public static ExtremeGlitchVisualizer inst;
    public RawImage glitchImage;
    public int textureSize = 1080; // Resolution of glitch texture
    public float intensityMultiplier = 1f; // Controls glitch intensity
    public ComputeShader glitchComputeShader; // Reference to the compute shader
    public static bool ok = true;

    private RenderTexture glitchTexture;
    private float[] spectrumData = new float[128];

    void Start()
    {
        glitchTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGB32);
        glitchTexture.enableRandomWrite = true;
        glitchTexture.Create();
        glitchImage.texture = glitchTexture;
        StartCoroutine(changecol());
        StartCoroutine(upd());

    }

    public IEnumerator upd()
    {
        while(true)
        {
            intensityMultiplier += Time.deltaTime;
            audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
            GenerateGlitchTexture();
            yield return null;
        }
    }
    public IEnumerator changecol()
    {
        while (true)
        {
            if (ok)
            {
                float r = Random.Range(0.0f, 0.5f);
                float g = Random.Range(0.0f, 0.5f);
                float b = Random.Range(0.0f, 0.5f);
                float a = Random.Range(0.5f, 1.0f);
                Color randomColor = new Color(r, g, b,a);
                glitchImage.color = randomColor;
            }
            else
            {
                glitchImage.color = Color.black;
            }
            yield return new WaitForSeconds(Random.Range(0.5f, 2));
        }
    }
    public IEnumerator ThatPart()
    {
        ok = false;
        yield return new WaitForSeconds(14);
        ok = true;
    }
    void GenerateGlitchTexture()
    {
        float amplitude = spectrumData[0] * intensityMultiplier;

        glitchComputeShader.SetFloat("_Time", Time.time);
        glitchComputeShader.SetFloat("_Intensity", intensityMultiplier);
        glitchComputeShader.SetFloat("_AmplitudeMultiplier", amplitude);
        glitchComputeShader.SetInt("_TextureWidth", 1920);
        glitchComputeShader.SetInt("_TextureHeight", textureSize);
        glitchComputeShader.SetTexture(0, "Result", glitchTexture);
        int threadGroups = Mathf.CeilToInt(textureSize / 8.0f);
        glitchComputeShader.Dispatch(0, threadGroups, threadGroups, 1);
    }
}
