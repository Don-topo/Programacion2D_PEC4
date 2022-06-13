using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{

    public TextMeshProUGUI remainingTimeText;
    public int remainingTime;
    public Canvas failedMissionCanvas;

    private int minutes;
    private int seconds;

    // Start is called before the first frame update
    void Start()
    {
        CalculateRemainingTime();
        UpdateText();
        StartCoroutine(CountDown());
    }    

    private void CalculateRemainingTime()
    {
        minutes = remainingTime / 60;
        seconds = remainingTime % 60;
    }

    private void UpdateText()
    {
        remainingTimeText.text = minutes.ToString() + ":" + seconds;
    }

    private void CheckIfMissionFailed()
    {
        if(remainingTime <= 0)
        {
            StopAllCoroutines();
            failedMissionCanvas.gameObject.SetActive(true);
            GameManager.Instance.StopUnits();
        }        
    }

    IEnumerator CountDown()
    {
        while(remainingTime > 0)
        {
            yield return new WaitForSeconds(1);
            if (GameManager.Instance.CanIMove())
            {
                remainingTime--;
                CheckIfMissionFailed();
                CalculateRemainingTime();
                UpdateText();
            }            
        }        
    }
}
