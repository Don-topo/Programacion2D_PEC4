using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{

    private GameObject selectedUnit = null;
    private LineRenderer line;
    private Vector2 startMousePosition, currentMousePosition;


    private void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
    }

    void Update()
    {        
        if (Input.GetMouseButtonDown(0))
        {
            startMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            line.positionCount = 4;
            line.SetPosition(0, new Vector2(startMousePosition.x, startMousePosition.y));
            line.SetPosition(1, new Vector2(startMousePosition.x, startMousePosition.y));
            line.SetPosition(2, new Vector2(startMousePosition.x, startMousePosition.y));
            line.SetPosition(3, new Vector2(startMousePosition.x, startMousePosition.y));

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
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                newPosition.z = 0f;
                Debug.Log("Click position " + newPosition);
                selectedUnit.GetComponent<SoldierController>().Move(newPosition);
                // TODO Move unit
            }
        }

        if (Input.GetMouseButton(0))
        {
            currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            line.SetPosition(0, new Vector2(startMousePosition.x, startMousePosition.y));
            line.SetPosition(1, new Vector2(startMousePosition.x, currentMousePosition.y));
            line.SetPosition(2, new Vector2(currentMousePosition.x, currentMousePosition.y));
            line.SetPosition(3, new Vector2(currentMousePosition.x, startMousePosition.y));

            var selectedArea = Mathf.Abs(
                (startMousePosition.x - currentMousePosition.x) * 
                (startMousePosition.y - currentMousePosition.y));
        }

        if(Input.GetMouseButtonUp(0))
        {
            line.positionCount = 0;
        }
    }
}
