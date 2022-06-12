using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BuffAttackSpeedController : MonoBehaviour
{

    public int range;
    public int cost;
    public float attackSpeedBuff;
    public int duration;
    public Slider slider;
    public Button button;
    public int cooldown;

    private GameObject soldierGameobject;
    private RaycastHit2D[] aliesAffected;
    private float[] originalValues;
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
        GetAliedUnits();
        currentCooldown = cooldown;
        soldierGameobject.GetComponent<SoldierController>().currentEnergy -= cost;
        slider.value = currentCooldown;
        StartCoroutine(Cooldown());
        StartCoroutine(Buff());
    }

    private void GetAliedUnits()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(soldierGameobject.transform.position, range, Vector2.zero);
        aliesAffected = hits.Where(c => c.collider.gameObject.CompareTag("Soldier")).ToArray();
        originalValues = new float[aliesAffected.Length];        
    }

    IEnumerator Buff()
    {
        for(int i = 0; i < aliesAffected.Length; i++)
        {
            originalValues[i] = aliesAffected[i].collider.GetComponent<SoldierController>().attackSpeed;
            aliesAffected[i].collider.GetComponent<SoldierController>().attackSpeed += attackSpeedBuff;
        }
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < aliesAffected.Length; i++)
        {
            aliesAffected[i].collider.GetComponent<SoldierController>().attackSpeed = originalValues[i];
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
