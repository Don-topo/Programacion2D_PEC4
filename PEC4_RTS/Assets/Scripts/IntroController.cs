using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    public Animator transitionAnimator;
    public int initialDelay = 3;
    public int sceneTransitionTime = 1;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartTransition());
    }

    IEnumerator StartTransition()
    {
        yield return new WaitForSeconds(initialDelay);
        transitionAnimator.SetTrigger("StartTransition");
        yield return new WaitForSeconds(sceneTransitionTime);
        SceneManager.LoadScene("MainMenu");
    }
}
