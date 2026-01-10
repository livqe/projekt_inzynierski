using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MagicSceneFader : MonoBehaviour
{
    public static MagicSceneFader Instance { get; private set; }

    [Header("Components")]
    public Animator fadeAnimator;
    public Image fadeImage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void FadeToScene(string sceneName)
    {
        if (fadeImage != null) fadeImage.raycastTarget = true;
        StartCoroutine(FadeAndSwitch(sceneName));
    }

    private IEnumerator FadeAndSwitch(string sceneName)
    {
        if (fadeAnimator != null) fadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.55f); // tyle trwa animacja
        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(0.1f); //czekamy na za³adowanie nowej sceny
        if (fadeAnimator != null) fadeAnimator.SetTrigger("FadeIn");
        if (fadeImage != null) fadeImage.raycastTarget = false;
    }
}
