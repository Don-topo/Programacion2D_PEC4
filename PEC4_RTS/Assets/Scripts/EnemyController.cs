using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyController : MonoBehaviour
{

    public GameObject selectedGameObject;
    public GameObject healthbar;
    public GameObject remainingHealth;
    public int healthPoints = 10;
    public int currentHealth;
    public float attackRange = 5f;
    public int attackDamage = 3;
    public float attackSpeed = 3;
    public float movementSpeed = 3f;
    public int armor = 3;
    public AudioClip attackClip;
    public AudioClip deathClip;
    public NavMeshAgent agent;


    private Animator animator;
    private RaycastHit2D[] soldiersInRange;
    private GameObject soldierTarget;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(soldierTarget != null)
        {
            CheckSoldiersInRange();
            CheckSoldiersVision();
            
        }        
    }

    public void SelectEnemy()
    {
        selectedGameObject.SetActive(true);
        healthbar.SetActive(true);
    }

    public void DiselectEnemy()
    {
        selectedGameObject.SetActive(false);
        healthbar.SetActive(false);
    }

    public void GetHit(int damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();
        if(currentHealth <= 0)
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
        agent.enabled = false;
        GetComponent<SpriteRenderer>().sortingLayerName = "Floor";
    }

    private void UpdateHealthBar()
    {
        // Scale sprite
        remainingHealth.GetComponent<SpriteRenderer>().size = new Vector2((float)(currentHealth * 1) / healthPoints, 0.25f);
        // Adjust position
        float offset = (0.5f / healthPoints);
        Vector3 newPosition = new Vector3(healthbar.transform.position.x - offset * (healthPoints - currentHealth), remainingHealth.transform.position.y, remainingHealth.transform.position.z);
        remainingHealth.transform.position = newPosition;
    }

    private void CheckSoldiersInRange()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, attackRange, Vector2.zero);
        soldiersInRange = hits.Where(c => c.collider.gameObject.CompareTag("Soldier")).ToArray();
    }

    private void CheckSoldiersVision()
    {
        foreach(RaycastHit2D soldierInRange in soldiersInRange)
        {
            Vector2 enemyPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 soldierPosition = new Vector2(soldierInRange.transform.position.x, soldierInRange.transform.position.y);
            bool hit = false;
            GetComponent<BoxCollider2D>().enabled = false;
            RaycastHit2D enemyHit = Physics2D.Linecast(enemyPosition, soldierPosition);
            if (enemyHit.collider != null)
            {
                if (enemyHit.collider.CompareTag("Soldier"))
                {
                    soldierTarget = enemyHit.collider.gameObject;
                }
            }
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    private void Attack()
    {

    }

    IEnumerator AttackContinously()
    {
        yield return new WaitForSeconds(attackSpeed);

    }
}
