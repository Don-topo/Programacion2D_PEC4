using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HabilityController : MonoBehaviour
{
    public int cooldown;
    public Slider slider;
    public Button button;

    private int currentCooldown = 0;

    // Start is called before the first frame update
    void Start()
    {
        slider.maxValue = cooldown;
        slider.minValue = 0;
        slider.value = 0;
    }

    public void UseHability()
    {
        // Disable button
        /*button.interactable = false;
        currentCooldown = cooldown;
        slider.value = currentCooldown;
        StartCoroutine(Cooldown());*/
        Debug.Log("Click");
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
