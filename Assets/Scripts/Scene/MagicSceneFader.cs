using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MagicSceneFader : MonoBehaviour
{
    public Animator fadeAnimator;

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeAndSwitch(sceneName));
    }

    IEnumerator FadeAndSwitch(string sceneName)
    {
        fadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1f); // tyle trwa animacja
        SceneManager.LoadScene(sceneName);
    }
}
