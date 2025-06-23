using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OverlyDedicated : MonoBehaviour
{
    public SpriteRenderer image;
    public AudioSource audioSource;
    public AudioClip clip,clip2;
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = elapsed*Random.Range(-1f, 1f) * magnitude;
            float y = elapsed*Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
    }
    private void Start()
    {
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(StartColorChange(new Color32(118, 8, 39, 255),15));
    }
    public void TriggerShake()
    {
        StartCoroutine(Shake(15f, 0.003f)); // Example parameters: duration of 0.5 seconds and magnitude of 0.3
    }
    public IEnumerator ChangeColor(Color targetColor, float duration)
    {
        Color originalColor = image.color;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            image.color = Color.Lerp(originalColor, targetColor, elapsed / duration);
            yield return null;
        }

        image.color = targetColor;
    }

    public IEnumerator StartColorChange(Color targetColor, float duration)
    {
        audioSource.clip=clip;
        audioSource.Play();
        yield return new WaitForSeconds(12);
        StartCoroutine(ChangeColor(new Color32(118, 8, 39, 255), 15));
        yield return new WaitForSeconds(13);
        StartCoroutine(ChangeColor(targetColor, duration));
        yield return StartCoroutine(Shake(15f, 0.003f));
        yield return new WaitForSeconds(2);
        image.gameObject.SetActive(false);
        //yield return StartCoroutine(ChangeColor(new Color32(118, 8, 39, 0), 4));
        SceneManager.UnloadSceneAsync("OverlyDedicated");
    }
}
