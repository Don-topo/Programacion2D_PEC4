using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{

    private GameObject selectedUnit = null;
    private GameObject[] selectedUnits = null;
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
                    DiselectMultipleUnits();
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
            Vector3 leftUpperCorner = new Vector3(startMousePosition.x, startMousePosition.y, 0f);
            Vector3 rightUpperCorner = new Vector3(currentMousePosition.x, startMousePosition.y, 0f);
            Vector3 leftBottomCorner = new Vector3(currentMousePosition.x, startMousePosition.y, 0f);
            Vector3 rightBottomCorner = new Vector3(currentMousePosition.x, currentMousePosition.y, 0f);
            var test = new Vector3(
                Mathf.Abs(startMousePosition.x - currentMousePosition.x),
                Mathf.Abs(startMousePosition.y - currentMousePosition.y),
                0f
            );
            RaycastHit2D[] hits = Physics2D.BoxCastAll(((leftUpperCorner + rightUpperCorner + leftBottomCorner + rightBottomCorner) / 4), test, 0, new Vector2(0,0));
            foreach(RaycastHit2D raycastHit2D in hits)
            {
                raycastHit2D.collider.gameObject.GetComponent<SoldierController>().SelectSoldier();
            }
        }       
    }

    public void SetAreaSelected(List<GameObject> newSelected)
    {
        selectedUnits = newSelected.ToArray();
        SelectMultipleUnits();
    }

    private void SelectMultipleUnits()
    {
        foreach(GameObject go in selectedUnits)
        {
            go.GetComponent<SoldierController>().SelectSoldier();
        }
    }

    private void DiselectMultipleUnits()
    {
        foreach(GameObject gameObject in selectedUnits)
        {
            gameObject.GetComponent<SoldierController>().DiselectSoldier();
        }
    }
}
