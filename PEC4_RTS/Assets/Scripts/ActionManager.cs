using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionManager : MonoBehaviour
{

    private List<GameObject> selectedUnits;
    private LineRenderer line;
    private Vector2 startMousePosition, currentMousePosition;
    private GameObject enemySelected;

    public Texture2D regularCursorTexture;
    public Texture2D enemyCursorTexture;
    public Texture2D alyCursorTexture;
    public GameObject targetPositionPrefab;
    public GameObject unitDetailsParent;

    private void Start()
    {
        selectedUnits = new List<GameObject>();
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
        Cursor.SetCursor(regularCursorTexture, Vector2.zero, CursorMode.ForceSoftware);
        unitDetailsParent.SetActive(false);
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
            DiselectUnits();
            DiselectEnemy();
            var test = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            test.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, LayerMask.NameToLayer("Units"));
            
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Soldier"))
                {
                    selectedUnits.Add(hit.collider.gameObject);
                    SelectUnits();
                }
            }
            else
            {
                DiselectUnits();
                DiselectEnemy();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if(selectedUnits.Capacity > 0)
            {
                RaycastHit2D hitEnemy = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, LayerMask.NameToLayer("Units"));
                if(hitEnemy.collider != null)
                {
                    if (hitEnemy.collider.CompareTag("Enemy"))
                    {
                        DiselectEnemy();
                        enemySelected = hitEnemy.collider.gameObject;
                        hitEnemy.collider.GetComponent<EnemyController>().SelectEnemy();
                        AttackUnits();
                    }
                }
                else
                {
                    Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    newPosition.z = 0f;
                    MoveUnits(newPosition);
                    DiselectEnemy();
                }                
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
            Vector3 leftBottomCorner = new Vector3(startMousePosition.x, currentMousePosition.y, 0f);
            Vector3 rightBottomCorner = new Vector3(currentMousePosition.x, currentMousePosition.y, 0f);
            var test = new Vector3(
                Mathf.Abs(startMousePosition.x - currentMousePosition.x),
                Mathf.Abs(startMousePosition.y - currentMousePosition.y),
                0f
            );
            RaycastHit2D[] hits = Physics2D.BoxCastAll((leftUpperCorner + rightUpperCorner + leftBottomCorner + rightBottomCorner) / 4, test, 0, new Vector2(0,0));            
            foreach(RaycastHit2D raycastHit2D in hits)
            {
                if (raycastHit2D.collider.CompareTag("Soldier")) {
                    selectedUnits.Add(raycastHit2D.collider.gameObject);
                }                
            }
            SelectUnits();
        }        
    }

    void FixedUpdate()
    {
        RaycastHit2D hit1 = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, LayerMask.NameToLayer("Units"));
        if (hit1.collider != null)
        {
            switch (hit1.collider.tag)
            {
                case "Soldier":
                    Cursor.SetCursor(alyCursorTexture, Vector2.zero, CursorMode.ForceSoftware);
                    break;
                case "Enemy":
                    Cursor.SetCursor(enemyCursorTexture, Vector2.zero, CursorMode.ForceSoftware);
                    break;
            }
        }
        else
        {
            Cursor.SetCursor(regularCursorTexture, Vector2.zero, CursorMode.ForceSoftware);
        }
    }


    private void SelectUnits()
    {
        foreach(GameObject gameObject in selectedUnits)
        {
            SetUnitDetails(gameObject.GetComponent<SoldierController>());
            gameObject.GetComponent<SoldierController>().SelectSoldier();
        }
    }

    private void DiselectUnits()
    {
        foreach(GameObject gameObject in selectedUnits)
        {
            gameObject.GetComponent<SoldierController>().DiselectSoldier();
        }
        selectedUnits.Clear();
        unitDetailsParent.SetActive(false);
    }

    private void MoveUnits(Vector3 position)
    {
        if(selectedUnits.ToArray().Length > 0)
        {
            Instantiate(targetPositionPrefab, position, Quaternion.identity);
        }        
        foreach(GameObject gameObject in selectedUnits)
        {
            gameObject.GetComponent<SoldierController>().Move(position);
        }
    }

    private void AttackUnits()
    {
        foreach (GameObject unit in selectedUnits)
        {
            unit.GetComponent<SoldierController>().Attack(enemySelected);
        }
    }

    private void DiselectEnemy()
    {
        if(enemySelected != null)
        {
            enemySelected.GetComponent<EnemyController>().DiselectEnemy();
            enemySelected = null;
        }
    }

    private void SetUnitDetails(SoldierController soldierController)
    {
        GameObject image = unitDetailsParent.transform.Find("UnitImage").gameObject;
        GameObject health = unitDetailsParent.transform.Find("Health").gameObject.transform.Find("HealthText").gameObject;
        GameObject energy = unitDetailsParent.transform.Find("Energy").gameObject.transform.Find("EnergyText").gameObject;
        GameObject armor = unitDetailsParent.transform.Find("Armor").gameObject.transform.Find("ArmorText").gameObject;
        GameObject attackDamage = unitDetailsParent.transform.Find("AttackDamage").gameObject.transform.Find("AttackDamageText").gameObject;
        GameObject attackSpeed = unitDetailsParent.transform.Find("AttackSpeed").gameObject.transform.Find("AttackSpeedText").gameObject;
        GameObject movementSpeed = unitDetailsParent.transform.Find("MovementSpeed").gameObject.transform.Find("MovementSpeedText").gameObject;
        GameObject unitName = unitDetailsParent.transform.Find("UnitNameText").gameObject;

        image.GetComponent<Image>().sprite = soldierController.unitImage;
        health.GetComponent<TextMeshProUGUI>().SetText(soldierController.currenthealth.ToString() + "/" + soldierController.healthPoints.ToString());
        energy.GetComponent<TextMeshProUGUI>().SetText(soldierController.currentEnergy.ToString() + "/" + soldierController.energyPoints.ToString());
        armor.GetComponent<TextMeshProUGUI>().SetText(soldierController.armor.ToString());
        attackDamage.GetComponent<TextMeshProUGUI>().SetText(soldierController.attackDamage.ToString());
        attackSpeed.GetComponent<TextMeshProUGUI>().SetText(soldierController.attackSpeed.ToString());
        movementSpeed.GetComponent<TextMeshProUGUI>().SetText(soldierController.movementSpeed.ToString());
        unitName.GetComponent<TextMeshProUGUI>().SetText(soldierController.unitName);
        unitDetailsParent.SetActive(true);
    }
}
