using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemainingEnemiesController : MonoBehaviour
{
    public Canvas winCanvas;

    private int remainingEnemies;
    // Start is called before the first frame update
    void Start()
    {
        remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        StartCoroutine(CheckEnemies());
    }

    IEnumerator CheckEnemies()
    {
        yield return new WaitForSeconds(2);
        remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        CheckVictory();
    }

    private void CheckVictory()
    {
        if(remainingEnemies <= 0)
        {
            StopAllCoroutines();
            winCanvas.gameObject.SetActive(true);
        }
    }
}
