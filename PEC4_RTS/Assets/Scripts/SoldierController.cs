using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    public float attackSpeed = 1f;
    public int attackDamage = 1;
    public int healthPoints = 1;
    public GameObject selectedGameObject;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectSoldier()
    {
        selectedGameObject.SetActive(true);
    }

    public void DiselectSoldier()
    {
        selectedGameObject.SetActive(false);
    }

    public void Move(Transform newPosition)
    {

    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    public void Death()
    {
        animator.SetTrigger("Death");
    }


}
