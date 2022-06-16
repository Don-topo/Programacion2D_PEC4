using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RemainingEnemiesController : MonoBehaviour
{
    public Canvas winCanvas;

    private int remainingEnemies;
    private GameObject[] remEnemies;
    // Start is called before the first frame update
    void Start()
    {
        remEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        StartCoroutine(CheckEnemies());
    }

    IEnumerator CheckEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            CheckVictory();
        }        
    }

    private void CheckVictory()
    {
        if(remEnemies.Where(c => c.GetComponent<EnemyController>().currentHealth > 0).ToArray().Length == 0)
        {
            StopAllCoroutines();
            winCanvas.gameObject.SetActive(true);
        }
    }
}
