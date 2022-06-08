using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierController : MonoBehaviour
{
    public float attackSpeed = 1f;
    public int attackRange = 1;
    public int armor = 2;
    public int attackDamage = 1;
    public float movementSpeed = 1;
    public int healthPoints = 10;
    public int currenthealth = 10;
    public int energyPoints = 10;
    public int currentEnergy = 10;
    public GameObject selectedGameObject;
    public NavMeshAgent agent;
    public Sprite unitImage;
    public string unitName = "Stg. Hammer";

    private Animator animator;
    private bool isFacingRight = true;
    private Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        lastPosition = transform.position;
        StartCoroutine(RestoreEnergy());
    }

    // Update is called once per frame
    void Update()
    {
        agent.updateRotation = false;
        if(lastPosition.x < transform.position.x && !isFacingRight)
        {
            Flip();
        } else if(lastPosition.x > transform.position.x && isFacingRight)
        {
            Flip();
        }
        lastPosition = transform.position;
        if (agent.remainingDistance == 0f)
        {
            animator.SetBool("Moving", false);
        }
    }

    public void SelectSoldier()
    {
        selectedGameObject.SetActive(true);
    }

    public void DiselectSoldier()
    {
        selectedGameObject.SetActive(false);
    }

    public void Move(Vector3 newPosition)
    {
        animator.SetBool("Moving", true);
        agent.SetDestination(newPosition);
        agent.updateRotation = false;
    }

    public void Attack(GameObject enemy)
    {
        // Check if I am in range
        if (CheckAttackRange(enemy))
        {
            // I am in range
            StopMovement();
            if (CheckVision(enemy))
            {
                // Can I shoot?
                enemy.GetComponent<EnemyController>().GetHit(attackDamage);
                animator.SetTrigger("Attack");
            }
            else
            {
                // There is a obstacle between us. We need to move
            }
        }
        else
        {
            // Out of range, need to move
        }
        // Move if need
        // If I am in range attack
        // Raycast to enemy and attack
        // Face the enemy
        if(enemy.transform.position.x >= transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if(enemy.transform.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    public void Death()
    {
        animator.SetTrigger("Death");
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void StopMovement()
    {
        agent.SetDestination(transform.position);
        animator.SetBool("Moving", false);
        agent.updateRotation = false;
    }

    private bool CheckAttackRange(GameObject enemy)
    {
        Vector2 playerPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 enemyPosition = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
        float distance = 1000f;
        GetComponent<BoxCollider2D>().enabled = false;
        RaycastHit2D enemyHit = Physics2D.Linecast(playerPosition, enemyPosition);
        if (enemyHit.collider != null)
        {
            distance = enemyHit.distance;
        }
        GetComponent<BoxCollider2D>().enabled = true;
        return distance < attackRange;
    }

    private bool CheckVision(GameObject enemy)
    {
        Vector2 playerPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 enemyPosition = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
        bool hit = false;
        GetComponent<BoxCollider2D>().enabled = false;
        RaycastHit2D enemyHit = Physics2D.Linecast(playerPosition, enemyPosition);
        if (enemyHit.collider != null)
        {
            if (enemyHit.collider.CompareTag("Enemy"))
            {
                hit = true;
            }
        }
        GetComponent<BoxCollider2D>().enabled = true;
        return hit;
    }


    IEnumerator RestoreEnergy()
    {
        while (true)
        {
            if(currentEnergy < energyPoints)
            {
                currentEnergy++;
            }
            yield return new WaitForSeconds(1);
        }
    }
}
