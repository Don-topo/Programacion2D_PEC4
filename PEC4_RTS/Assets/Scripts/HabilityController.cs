using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HabilityController : MonoBehaviour
{
    public enum HabilityType
    {
        AreaDamage,
        AreaHeal,
        selectDamage,
        selectHeal
    }
    public int cooldown;
    public Slider slider;
    public Button button;
    public GameObject areaEffect;
    public float areaRange;
    public float habilityRange;
    public int damage;
    public int cost;
    public int duration;
    public int attackBuff;
    public int armorBuff;
    public float attackSpeedBuff;
    public float energyBuff;
    public float healthBuff;
    public int healAmount;
    public HabilityType habilityType;
    public GameObject grenadeExplotionPrefab;
    public GameObject healGrenadeExplotionPrefab;
    public AudioClip poweredAttack;
    public AudioClip buff;

    private GameObject soldierGameobject;
    private int currentCooldown = 0;
    private RaycastHit2D[] aliesAffected;
    private int[] originalIntValues;
    private float[] originalFloatValues;
    private Vector3 cursorPosition;

    // Start is called before the first frame update
    void Start()
    {
        slider.maxValue = cooldown;
        slider.minValue = 0;
        slider.value = 0;
    }

    private void Update()
    {
        cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        soldierGameobject = ActionManager.GetSelectedUnit();
        if (areaEffect.activeSelf)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPosition.z = 0;
            areaEffect.transform.position = newPosition;            
        }
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

    public void UseGrenadeHability()
    {
        if (GameManager.Instance.CanIMove())
        {
            button.interactable = false;
            areaEffect.SetActive(true);
            ActionManager.usingHability = true;
            ActionManager.habilityController = this;
            soldierGameobject = ActionManager.GetSelectedUnit();
            soldierGameobject.GetComponent<SoldierController>().ShowArea();
        }
    }

    public void UseBuffAttackDamageHability()
    {
        if (GameManager.Instance.CanIMove())
        {
            GetAliedUnits();
            currentCooldown = cooldown;
            slider.value = currentCooldown;
            soldierGameobject.GetComponent<SoldierController>().currentEnergy -= cost;
            StartCoroutine(Cooldown());
            StartCoroutine(BuffAttackDamage());
        }
    }

    public void UseBuffArmorHability()
    {
        if (GameManager.Instance.CanIMove())
        {
            GetAliedUnits();
            soldierGameobject.GetComponent<SoldierController>().currentEnergy -= cost;
            currentCooldown = cooldown;
            slider.value = currentCooldown;
            StartCoroutine(Cooldown());
            StartCoroutine(BuffArmor());
        }
    }

    public void UseBuffAttackSpeedHability()
    {
        if (GameManager.Instance.CanIMove())
        {
            GetAliedUnits();
            currentCooldown = cooldown;
            soldierGameobject.GetComponent<SoldierController>().currentEnergy -= cost;
            slider.value = currentCooldown;
            StartCoroutine(Cooldown());
            StartCoroutine(BuffAttackSpeed());
        }
    }

    public void UseBuffEnergyHability()
    {
        if (GameManager.Instance.CanIMove())
        {
            GetAliedUnits();
            currentCooldown = cooldown;
            soldierGameobject.GetComponent<SoldierController>().currentEnergy -= cost;
            slider.value = currentCooldown;
            StartCoroutine(Cooldown());
            StartCoroutine(BuffEnergy());
        }
    }

    public void UseBuffHealthHability()
    {
        if (GameManager.Instance.CanIMove())
        {
            GetAliedUnits();
            currentCooldown = cooldown;
            soldierGameobject.GetComponent<SoldierController>().currentEnergy -= cost;
            slider.value = currentCooldown;
            StartCoroutine(Cooldown());
            StartCoroutine(Buffhealth());
        }
    }

    public void UseHealHability()
    {
        if (GameManager.Instance.CanIMove())
        {
            button.interactable = false;
            areaEffect.SetActive(true);
            ActionManager.usingHability = true;
            ActionManager.habilityController = this;
            soldierGameobject = ActionManager.GetSelectedUnit();
            soldierGameobject.GetComponent<SoldierController>().ShowArea();
        }
    }

    public void UseGranadeHealHability()
    {
        if (GameManager.Instance.CanIMove())
        {
            button.interactable = false;
            areaEffect.SetActive(true);
            ActionManager.usingHability = true;
            ActionManager.habilityController = this;
            soldierGameobject = ActionManager.GetSelectedUnit();
            soldierGameobject.GetComponent<SoldierController>().ShowArea();
        }
    }

    public void UsePowerShootHability()
    {
        if (GameManager.Instance.CanIMove())
        {
            button.interactable = false;
            areaEffect.SetActive(true);
            ActionManager.usingHability = true;
            ActionManager.habilityController = this;
            soldierGameobject = ActionManager.GetSelectedUnit();
            soldierGameobject.GetComponent<SoldierController>().ShowArea();
        }
    }

    public void ConfirmHability(RaycastHit2D hitUnit)
    {
        SoldierController soldierController = soldierGameobject.GetComponent<SoldierController>();
        bool checkHability = true;

        if (soldierController.CanIUseHability(cost))
        {
            if (CheckAttackRange())
            {
                switch (habilityType)
                {
                    case HabilityType.AreaDamage:
                        ApplyGrenadeDamage();
                    break;
                    case HabilityType.AreaHeal:
                        ApplyHealGrenade();
                    break;
                    case HabilityType.selectDamage:
                        checkHability = DamageUnit(hitUnit);
                    break;
                    case HabilityType.selectHeal:
                       checkHability = HealUnit(hitUnit);
                    break;
                }
                if (checkHability)
                {
                    currentCooldown = cooldown;
                    slider.value = currentCooldown;
                    StartCoroutine(Cooldown());                                        
                    soldierController.Usehability(cost);
                }
                soldierController.HideArea();
                ActionManager.usingHability = false;
                ActionManager.habilityController = null;
            }            
        }        
    }

    private void ApplyGrenadeDamage()
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
        cursorPosition.z = 0;
        Instantiate(grenadeExplotionPrefab, cursorPosition, Quaternion.identity);
    }

    private void ApplyHealGrenade()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(areaEffect.transform.position, areaRange, Vector2.zero);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Soldier"))
            {
                hit.collider.GetComponent<SoldierController>().GetHealed(healAmount);
            }
        }
        areaEffect.SetActive(false);
        cursorPosition.z = 0;
        Instantiate(healGrenadeExplotionPrefab, cursorPosition, Quaternion.identity);
    }

    private bool HealUnit(RaycastHit2D hitUnit)
    {
        areaEffect.SetActive(false);
        if(hitUnit.collider == null)
        {
            return false;
        }
        else
        {
            if (hitUnit.collider.CompareTag("Soldier"))
            {
                hitUnit.collider.GetComponent<SoldierController>().GetHealed(healAmount);
                return true;
            }
            else
            {
                return false;
            }
        }        
    }

    private bool DamageUnit(RaycastHit2D hitUnit)
    {
        areaEffect.SetActive(false);
        if(hitUnit.collider == null)
        {
            return false;
        }
        else
        {
            if (hitUnit.collider.CompareTag("Enemy"))
            {
                GetComponent<AudioSource>().clip = poweredAttack;
                GetComponent<AudioSource>().Play();
                hitUnit.collider.GetComponent<EnemyController>().GetHit(damage);
                return true;
            }
            else
            {
                return false;
            }
        }        
    }

    private void GetAliedUnits()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(soldierGameobject.transform.position, areaRange, Vector2.zero);
        aliesAffected = hits.Where(c => c.collider.gameObject.CompareTag("Soldier")).ToArray();
        originalIntValues = new int[aliesAffected.Length];
        originalFloatValues = new float[aliesAffected.Length];
    }

    IEnumerator BuffAttackDamage()
    {
        GetComponent<AudioSource>().clip = buff;
        GetComponent<AudioSource>().Play();
        for (int i = 0; i < aliesAffected.Length; i++)
        {
            aliesAffected[i].collider.GetComponent<SoldierController>().GetBuffed();
            originalIntValues[i] = aliesAffected[i].collider.GetComponent<SoldierController>().attackDamage;
            aliesAffected[i].collider.GetComponent<SoldierController>().attackDamage += attackBuff;
        }
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < aliesAffected.Length; i++)
        {
            aliesAffected[i].collider.GetComponent<SoldierController>().attackDamage = originalIntValues[i];
        }
        originalIntValues = null;
        originalFloatValues = null;
        aliesAffected = null;
    }

    IEnumerator BuffArmor()
    {
        GetComponent<AudioSource>().clip = buff;
        GetComponent<AudioSource>().Play();
        for (int i = 0; i < aliesAffected.Length; i++)
        {
            aliesAffected[i].collider.GetComponent<SoldierController>().GetBuffed();
            originalIntValues[i] = aliesAffected[i].collider.GetComponent<SoldierController>().armor;
            aliesAffected[i].collider.GetComponent<SoldierController>().armor += armorBuff;
        }
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < aliesAffected.Length; i++)
        {
            aliesAffected[i].collider.GetComponent<SoldierController>().armor = originalIntValues[i];
        }
        originalIntValues = null;
        originalFloatValues = null;
        aliesAffected = null;
    }

    IEnumerator BuffAttackSpeed()
    {
        GetComponent<AudioSource>().clip = buff;
        GetComponent<AudioSource>().Play();
        for (int i = 0; i < aliesAffected.Length; i++)
        {
            aliesAffected[i].collider.GetComponent<SoldierController>().GetBuffed();
            originalFloatValues[i] = aliesAffected[i].collider.GetComponent<SoldierController>().attackSpeed;
            aliesAffected[i].collider.GetComponent<SoldierController>().attackSpeed -= attackSpeedBuff;
        }
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < aliesAffected.Length; i++)
        {
            aliesAffected[i].collider.GetComponent<SoldierController>().attackSpeed = originalFloatValues[i];
        }
        originalIntValues = null;
        originalFloatValues = null;
        aliesAffected = null;
    }

    IEnumerator BuffEnergy()
    {
        GetComponent<AudioSource>().clip = buff;
        GetComponent<AudioSource>().Play();
        for (int i = 0; i < aliesAffected.Length; i++)
        {
            aliesAffected[i].collider.GetComponent<SoldierController>().GetBuffed();
            originalFloatValues[i] = aliesAffected[i].collider.GetComponent<SoldierController>().energyRestore;
            aliesAffected[i].collider.GetComponent<SoldierController>().energyRestore -= energyBuff;
        }
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < aliesAffected.Length; i++)
        {
            aliesAffected[i].collider.GetComponent<SoldierController>().energyRestore = originalFloatValues[i];
        }
        originalIntValues = null;
        originalFloatValues = null;
        aliesAffected = null;
    }

    IEnumerator Buffhealth()
    {
        GetComponent<AudioSource>().clip = buff;
        GetComponent<AudioSource>().Play();
        for (int i = 0; i < aliesAffected.Length; i++)
        {
            aliesAffected[i].collider.GetComponent<SoldierController>().GetBuffed();
            originalFloatValues[i] = aliesAffected[i].collider.GetComponent<SoldierController>().healthRestore;
            aliesAffected[i].collider.GetComponent<SoldierController>().healthRestore -= healthBuff;
        }
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < aliesAffected.Length; i++)
        {
            aliesAffected[i].collider.GetComponent<SoldierController>().healthRestore = originalFloatValues[i];
        }
        originalIntValues = null;
        originalFloatValues = null;
        aliesAffected = null;
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
        return distance < habilityRange;
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
