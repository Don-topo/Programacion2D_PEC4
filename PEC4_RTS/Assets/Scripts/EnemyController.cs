using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public GameObject selectedGameObject;
    public int healthPoints = 10;
    public float attackRange = 5f;
    public int attackDamage = 3;
    public float movementSpeed = 3f;
    public int armor = 3;

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

    public void SelectEnemy()
    {
        selectedGameObject.SetActive(true);
    }

    public void DiselectEnemy()
    {
        selectedGameObject.SetActive(false);
    }

    public void GetHit(int damage)
    {
        healthPoints -= damage;
        if(healthPoints <= 0)
        {
            Death();
        }
        else
        {
            // TODO hit with blood particles
        }
    }

    private void Death()
    {
        animator.SetTrigger("Death");
        DiselectEnemy();
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
