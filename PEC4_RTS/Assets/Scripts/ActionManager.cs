using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{

    private GameObject selectedUnit = null;

    void Update()
    {        
        if (Input.GetMouseButtonDown(0))
        {                        
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Soldier"))
                {
                    Debug.Log("Soldier selected");
                    selectedUnit = hit.collider.gameObject;
                    selectedUnit.GetComponent<SoldierController>().SelectSoldier();
                }
            }
            else
            {
                if(selectedUnit != null)
                {
                    selectedUnit.GetComponent<SoldierController>().DiselectSoldier();
                    selectedUnit = null;
                }
                
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if(selectedUnit != null)
            {
                // TODO Obtain point
                // TODO Move unit
            }
        }

    }
}
