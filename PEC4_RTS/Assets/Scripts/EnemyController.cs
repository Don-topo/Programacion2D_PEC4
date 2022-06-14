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
    public NavMeshAgent agent;
    public bool iAmPassive;
    public bool flyingUnit;
    public float attackDetection;


    private Animator animator;
    private RaycastHit2D[] soldiersInRange;
    private GameObject soldierTarget;
    private bool iAmAttacking = false;
    private AudioSource audioSource;
    private bool isFacingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!iAmPassive && currentHealth > 0)
        {
            FaceEnemy();
            if (soldierTarget == null)
            {
                CheckSoldiersInRange();
                CheckSoldiersVision();
                MoveToPosition();
            }
            else
            {
                if (Vector2.Distance(transform.position, soldierTarget.transform.position) <= attackRange)
                {
                    if (!iAmAttacking)
                    {
                        StopMoving();
                        iAmAttacking = true;
                        StartCoroutine(Attack());
                    }
                }
                else
                {
                    iAmAttacking = false;
                    StopAllCoroutines();
                    MoveToPosition();
                }
            }
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
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackDetection);
    }

    private void Death()
    {
        animator.SetTrigger("Death");
        StopAllCoroutines();
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
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, attackDetection, Vector2.zero);
        soldiersInRange = hits.Where(c => c.collider.gameObject.CompareTag("Soldier")).ToArray();
    }

    private void CheckSoldiersVision()
    {
        foreach(RaycastHit2D soldierInRange in soldiersInRange)
        {
            Vector2 enemyPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 soldierPosition = new Vector2(soldierInRange.transform.position.x, soldierInRange.transform.position.y);
            GetComponent<BoxCollider2D>().enabled = false;
            RaycastHit2D enemyHit = Physics2D.Linecast(enemyPosition, soldierPosition);
            if (enemyHit.collider != null)
            {
                if (enemyHit.collider.CompareTag("Soldier"))
                {
                    if(enemyHit.collider.GetComponent<SoldierController>().currenthealth > 0)
                    {
                        soldierTarget = enemyHit.collider.gameObject;
                        GetComponent<BoxCollider2D>().enabled = true;
                        break;
                    }                    
                }
            }
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    private void MoveToPosition()
    {
        if (!flyingUnit)
        {
            animator.SetBool("Moving", true);
        }        
        if(soldierTarget != null)
        {
            agent.SetDestination(soldierTarget.transform.position);
        }        
    }

    private void StopMoving()
    {
        agent.SetDestination(transform.position);
        if (!flyingUnit)
        {
            animator.SetBool("Moving", false);
        }        
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void FaceEnemy()
    {
        if(soldierTarget != null)
        {
            if (soldierTarget.transform.position.x >= transform.position.x && !isFacingRight)
            {
                Flip();
            }
            else if (soldierTarget.transform.position.x < transform.position.x && isFacingRight)
            {
                Flip();
            }
        }        
    }

    IEnumerator Attack()
    {
        while (iAmAttacking)
        {
            // Perform attack
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.3f);
            audioSource.Play();
            soldierTarget.GetComponent<SoldierController>().GetHit(attackDamage);
            yield return new WaitForSeconds(attackSpeed);
            if(soldierTarget.GetComponent<SoldierController>().currenthealth <= 0)
            {
                soldierTarget = null;
                iAmAttacking = false;
            }
        }        
    }
}
