using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HabilityController : MonoBehaviour
{
    public int cooldown;
    public Slider slider;
    public Button button;
    public GameObject areaEffect;
    public float areaRange;
    public int damage;
    public int cost;

    private GameObject soldierGameobject;
    private int currentCooldown = 0;

    // Start is called before the first frame update
    void Start()
    {
        slider.maxValue = cooldown;
        slider.minValue = 0;
        slider.value = 0;
    }

    private void Update()
    {
        if (areaEffect.activeSelf)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPosition.z = 0;
            areaEffect.transform.position = newPosition;            
        }
        soldierGameobject = ActionManager.GetSelectedUnit();
        if (soldierGameobject != null)
        {
            if (!soldierGameobject.GetComponent<SoldierController>().CanIUseHability(cost))
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;
            }
        }        
    }

    public void UseHability()
    {
        button.interactable = false;
        areaEffect.SetActive(true);        
        ActionManager.usingHability = true;
        ActionManager.habilityController = this;
        soldierGameobject = ActionManager.GetSelectedUnit();
        soldierGameobject.GetComponent<SoldierController>().ShowArea();
    }

    public void ConfirmHability()
    {
        SoldierController soldierController = soldierGameobject.GetComponent<SoldierController>();

        if (soldierController.CanIUseHability(cost))
        {
            if (CheckAttackRange())
            {
                RaycastHit2D[] hits = Physics2D.CircleCastAll(areaEffect.transform.position, areaRange, Vector2.zero);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        hit.collider.GetComponent<EnemyController>().GetHit(damage);
                    }
                }
                areaEffect.SetActive(false);
                currentCooldown = cooldown;
                slider.value = currentCooldown;
                StartCoroutine(Cooldown());
                ActionManager.usingHability = false;
                ActionManager.habilityController = null;
                soldierController.HideArea();
                soldierController.Usehability(cost);
            }            
        }        
    }

    public void CancelHability()
    {
        areaEffect.SetActive(false);
        ActionManager.usingHability = false;
        ActionManager.habilityController = null;
        soldierGameobject.GetComponent<SoldierController>().HideArea();
    }

    private bool CheckAttackRange()
    {
        Vector2 playerPosition = new Vector2(soldierGameobject.transform.position.x, soldierGameobject.transform.position.y);
        Vector2 cursorPostion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector2.Distance(playerPosition, cursorPostion);
        return distance < areaRange;
    }

    IEnumerator Cooldown()
    {
        while(currentCooldown > 0)
        {
            yield return new WaitForSeconds(1);
            currentCooldown--;
            slider.value = currentCooldown;
        }
        button.interactable = true;
    }

}
