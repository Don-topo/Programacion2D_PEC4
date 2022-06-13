using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ArmorBuffController : MonoBehaviour
{
    public int range;
    public int cost;
    public int armorBuff;
    public int duration;
    public Slider slider;
    public Button button;
    public int cooldown;

    private GameObject soldierGameobject;
    private RaycastHit2D[] aliesAffected;
    private int[] originalValues;
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
            GetAliedUnits();
            soldierGameobject.GetComponent<SoldierController>().currentEnergy -= cost;
            currentCooldown = cooldown;
            slider.value = currentCooldown;
            StartCoroutine(Cooldown());
            StartCoroutine(Buff());
        }        
    }

    private void GetAliedUnits()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(soldierGameobject.transform.position, range, Vector2.zero);
        aliesAffected = hits.Where(c => c.collider.gameObject.CompareTag("Soldier")).ToArray();
        originalValues = new int[aliesAffected.Length];
    }

    IEnumerator Buff()
    {
        for (int i = 0; i < aliesAffected.Length; i++)
        {
            originalValues[i] = aliesAffected[i].collider.GetComponent<SoldierController>().armor;
            aliesAffected[i].collider.GetComponent<SoldierController>().armor += armorBuff;
        }
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < aliesAffected.Length; i++)
        {
            aliesAffected[i].collider.GetComponent<SoldierController>().armor = originalValues[i];
        }
        originalValues = null;
        aliesAffected = null;
    }

    IEnumerator Cooldown()
    {
        while (currentCooldown > 0)
        {
            yield return new WaitForSeconds(1);
            currentCooldown--;
            slider.value = currentCooldown;
        }
        button.interactable = true;
    }
}
