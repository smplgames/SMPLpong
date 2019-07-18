using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeSceneManager
{
    public static IEnumerator FadeInScene(Image fadeImage)
    {
        while (fadeImage.color.a > 0.1)
        {
            fadeImage.color = Color.Lerp(fadeImage.color, new Color(0f, 0f, 0f, 0f), 0.3f);
            yield return null;
        }

        yield return null;
    }

    public static IEnumerator FadeOutScene(Image fadeImage, string scene)
    {
        while (fadeImage.color.a <= 0.95)
        {
            Debug.Log("Change Transparency");
            fadeImage.color = Color.Lerp(fadeImage.color, new Color(0f, 0f, 0f, 1f), 0.3f);
            yield return null;
        }

        SceneManager.LoadScene(scene, LoadSceneMode.Single);
        yield return null;
    }
}
