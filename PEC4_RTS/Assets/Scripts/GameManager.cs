using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Canvas pauseCanvas;
    public Canvas confirmCanvas;
    public Animator sceneTransitionAnimator;


    private GameInfo gameInfo;
    private bool isGamePaused = false;
    private bool isGameOnConfirmedPage = false;
    private static GameManager gmInstance;
    private float transitionTime = 1f;
    private bool canIPlay = true;


    public static GameManager Instance { get { return gmInstance; } }


    // Start is called before the first frame update
    private void Awake()
    {
        if (gmInstance != null && gmInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            gmInstance = this;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                if (isGameOnConfirmedPage)
                {
                    ExitConfirmMenu();
                }
                else
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }

    }
    public void PauseGame()
    {
        pauseCanvas.gameObject.SetActive(true);
        isGamePaused = true;
        StopUnits();
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        pauseCanvas.gameObject.SetActive(false);
        isGamePaused = false;
        StartUnits();
        Time.timeScale = 1f;
    }

    public void ConfirmExitMenu()
    {
        pauseCanvas.gameObject.SetActive(false);
        confirmCanvas.gameObject.SetActive(true);
        isGameOnConfirmedPage = true;
    }

    public void ExitConfirmMenu()
    {
        isGameOnConfirmedPage = false;
        confirmCanvas.gameObject.SetActive(false);
        pauseCanvas.gameObject.SetActive(true);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void StopUnits()
    {
        canIPlay = false;
    }

    public void StartUnits()
    {
        canIPlay = true;
    }
    public bool CanIMove()
    {
        return canIPlay;
    }

    public void CompleteTutorial()
    {
        StartCoroutine(LoadLevel("Patrol"));
    }

    public void CompleteMission1()
    {
        StartCoroutine(LoadLevel("SearchAndDestroy"));
    }

    public void CompleteMission2()
    {
        StartCoroutine(LoadLevel("Rescue"));
    }

    public void CompleteMission3()
    {
        StartCoroutine(LoadLevel("Credits"));
    }

    public void RestartMission()
    {       
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().name));
    }

    IEnumerator LoadLevel(string level)
    {
        sceneTransitionAnimator.SetTrigger("StartTransition");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(level);
    }

}
