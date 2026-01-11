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

    [Header("Loading screen")]
    public Image loadingSpinnerImage;
    public Sprite[] loadingFrames;
    [Range(1, 60)] public float framesPerSecond = 24f;

    [Header("Settings")]
    public float transitionTime = 1f;
    public float minLoadingScreenDuration = 2.0f;

    private Coroutine loadingAnimationRoutine;

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

        yield return new WaitForSeconds(transitionTime);

        if (loadingSpinnerImage != null)
        {
            loadingSpinnerImage.gameObject.SetActive(true);
            loadingAnimationRoutine = StartCoroutine(PlayLoadingAnimation());
        }

        float startTime = Time.time;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        float elapsedTime = Time.time - startTime;
        if (elapsedTime < minLoadingScreenDuration)
        {
            yield return new WaitForSeconds(minLoadingScreenDuration - elapsedTime);
        }

        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            yield return null;
        }

        if (loadingAnimationRoutine != null) StopCoroutine(loadingAnimationRoutine);
        if (loadingSpinnerImage != null) loadingSpinnerImage.gameObject.SetActive(false);

        if (fadeAnimator != null) fadeAnimator.SetTrigger("FadeIn");

        yield return new WaitForSeconds(transitionTime);

        if (fadeImage != null) fadeImage.raycastTarget = false;
    }

    private IEnumerator PlayLoadingAnimation()
    {
        if (loadingFrames == null || loadingFrames.Length == 0) yield break;

        int index = 0;
        float waitTime = 1f / framesPerSecond;

        while (true)
        {
            loadingSpinnerImage.sprite = loadingFrames[index];
            index = (index + 1) % loadingFrames.Length;
            yield return new WaitForSeconds(waitTime);
        }
    }
}
