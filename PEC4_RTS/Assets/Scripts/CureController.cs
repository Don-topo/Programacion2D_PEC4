using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CureController : MonoBehaviour
{
    public int cooldown;
    public Slider slider;
    public Button button;    
    public float areaRange;
    public GameObject areaEffect;
    public int curePoints;
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

    // Update is called once per frame
    void Update()
    {
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
        if (GameManager.Instance.CanIMove())
        {
            button.interactable = false;
            areaEffect.SetActive(true);
            ActionManager.usingHability = true;
            //ActionManager.habilityController = this;
            soldierGameobject = ActionManager.GetSelectedUnit();
            soldierGameobject.GetComponent<SoldierController>().ShowArea();
        }        
    }
}
