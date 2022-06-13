using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{

    public Canvas winCanvas;

    private bool trigger = false;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!trigger)
        {
            CheckIfPlayerEnter();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(transform.position, new Vector3(20, 1, 1));
    }

    private void CheckIfPlayerEnter()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, new Vector2(20, 1), 0, Vector2.zero);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Soldier"))
            {
                winCanvas.gameObject.SetActive(true);
                Destroy(this);
            }
        }
    }
}
