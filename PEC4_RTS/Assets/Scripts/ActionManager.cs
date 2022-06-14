using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionManager : MonoBehaviour
{

    private static List<GameObject> selectedUnits;
    private LineRenderer line;
    private Vector2 startMousePosition, currentMousePosition;
    private GameObject enemySelected;
    private bool uiClicked = false;

    public Texture2D regularCursorTexture;
    public Texture2D enemyCursorTexture;
    public Texture2D alyCursorTexture;
    public GameObject targetPositionPrefab;
    public GameObject unitDetailsParent;
    [HideInInspector]
    public static bool usingHability;
    [HideInInspector]
    public static HabilityController habilityController;

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
        if (GameManager.Instance.CanIMove())
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (CheckIfUIPressed())
                {
                    // Habilies stuff
                    uiClicked = true;
                }
                else
                {
                    if (usingHability)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, LayerMask.NameToLayer("Units"));
                        habilityController.ConfirmHability(hit);
                    }
                    else
                    {
                        uiClicked = false;
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
                }
            }

            if (Input.GetMouseButtonDown(1) && !CheckIfUIPressed())
            {
                if (selectedUnits.Capacity > 0)
                {
                    if (usingHability)
                    {
                        usingHability = false;
                        habilityController.CancelHability();
                    }
                    else
                    {
                        RaycastHit2D hitEnemy = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, LayerMask.NameToLayer("Units"));
                        if (hitEnemy.collider != null)
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
            }

            if (Input.GetMouseButton(0))
            {
                if (!CheckIfUIPressed() && !uiClicked)
                {
                    currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    line.SetPosition(0, new Vector2(startMousePosition.x, startMousePosition.y));
                    line.SetPosition(1, new Vector2(startMousePosition.x, currentMousePosition.y));
                    line.SetPosition(2, new Vector2(currentMousePosition.x, currentMousePosition.y));
                    line.SetPosition(3, new Vector2(currentMousePosition.x, startMousePosition.y));
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                line.positionCount = 0;
                Vector3 leftUpperCorner = new Vector3(startMousePosition.x, startMousePosition.y, 0f);
                Vector3 rightUpperCorner = new Vector3(currentMousePosition.x, startMousePosition.y, 0f);
                Vector3 leftBottomCorner = new Vector3(startMousePosition.x, currentMousePosition.y, 0f);
                Vector3 rightBottomCorner = new Vector3(currentMousePosition.x, currentMousePosition.y, 0f);
                var size = new Vector3(
                    Mathf.Abs(startMousePosition.x - currentMousePosition.x),
                    Mathf.Abs(startMousePosition.y - currentMousePosition.y),
                    0f
                );
                RaycastHit2D[] hits = Physics2D.BoxCastAll((leftUpperCorner + rightUpperCorner + leftBottomCorner + rightBottomCorner) / 4, size, 0, new Vector2(0, 0));
                foreach (RaycastHit2D raycastHit2D in hits)
                {
                    if (raycastHit2D.collider.CompareTag("Soldier"))
                    {
                        if(raycastHit2D.collider.GetComponent<SoldierController>().currenthealth > 0)
                        {
                            selectedUnits.Add(raycastHit2D.collider.gameObject);
                        }                        
                    }
                }
                if (!CheckIfUIPressed())
                {
                    SelectUnits();
                }

            }
            UpdateUI();
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

    private bool CheckIfUIPressed()
    {                
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void UpdateUI()
    {
        if(selectedUnits.ToArray().Length > 0)
        {
            SoldierController soldier = selectedUnits[0].GetComponent<SoldierController>();
            SetUnitDetails(soldier);
        }
    }

    public static GameObject GetSelectedUnit()
    {
        if(selectedUnits.Count > 0)
        {
            return selectedUnits[0];
        }
        else
        {
            return null;
        }
        
    }

    private void SelectUnits()
    {
        if(selectedUnits.ToArray().Length > 0)
        {
            SetUnitDetails(selectedUnits[0].GetComponent<SoldierController>());
            selectedUnits[0].GetComponent<SoldierController>().SelectSoldierSound();
        }
        foreach(GameObject gameObject in selectedUnits)
        {            
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
        float offset1 = 1.5f;
        Vector3[] offsets = {
                new Vector3(position.x - offset1, position.y + offset1, position.z),
                new Vector3(position.x + offset1, position.y + offset1, position.z),
                new Vector3(position.x - offset1, position.y - offset1, position.z),
                new Vector3(position.x + offset1, position.y - offset1, position.z),
            };
        if (selectedUnits.ToArray().Length > 0)
        {
            Instantiate(targetPositionPrefab, position, Quaternion.identity);
            selectedUnits[0].GetComponent<SoldierController>().MovementSound();
        }        
        foreach(GameObject gameObject in selectedUnits)
        {
            if(selectedUnits.ToArray().Length == 1)
            {
                gameObject.GetComponent<SoldierController>().Move(position);
            }
            else
            {
                gameObject.GetComponent<SoldierController>().Move(offsets[selectedUnits.IndexOf(gameObject)]);
            }            
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
