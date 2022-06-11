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
    public AudioClip[] selectUnitClips;

    private Animator animator;
    private bool isFacingRight = true;
    private Vector3 lastPosition;
    private bool canAttack = false;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        lastPosition = transform.position;
        StartCoroutine(RestoreEnergy());
        audioSource = GetComponent<AudioSource>();
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
        if (!audioSource.isPlaying)
        {
            audioSource.clip = selectUnitClips[Random.Range(0, selectUnitClips.Length)];
            audioSource.Play();
        }
        
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
        Debug.Log("Attack");
        // Check if I am in range
        if (CheckAttackRange(enemy))
        {
            PerformAttack(enemy);
        }
        else
        {
            // Out of range, need to move
            StartCoroutine(MoveOnRange(enemy));
        }        
    }

    public void Death()
    {
        animator.SetTrigger("Death");
    }

    private void PerformAttack(GameObject enemy)
    {
        Debug.Log("Perform attack");
        // I am in range
        StopMovement();
        if (CheckVision(enemy))
        {
            FaceEnemy(enemy);
            canAttack = true;
            StartCoroutine(AttackContinous(enemy));
        }
        else
        {
            // There is a obstacle between us. We need to move
            MoveToAvoidObstacle(enemy);
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void FaceEnemy(GameObject enemy)
    {
        if (enemy.transform.position.x >= transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (enemy.transform.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
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
        Debug.Log("Check Attack range");
        Vector2 playerPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 enemyPosition = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
        float distance = Vector2.Distance(playerPosition, enemyPosition);
        //Debug.Log("Distance: " + distance);
        return distance < attackRange;
    }

    private bool CheckVision(GameObject enemy)
    {
        Debug.Log("Check Vision");
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

    private void MoveToAvoidObstacle(GameObject enemy)
    {
        Debug.Log("Move to Avoid obstacle");
        Vector3 dir = (transform.position - enemy.transform.position).normalized;
        float offsetX = 1.5f;
        float offsetY = 1.5f;
        Debug.Log("Dir:" + dir);
        //Top left
        if(dir.x < 0 && dir.y > 0)
        {
            offsetY = 0f;
            StartCoroutine(MoveToAttack(enemy, offsetX, offsetY));
        }
        // Top right
        else if(dir.x > 0 && dir.y < 0)
        {
            offsetY = 0f;
            offsetX *= -1;
            StartCoroutine(MoveToAttack(enemy, offsetX, offsetY));
        }
        // Bottom left
        else if(dir.x < 0 && dir.y < 0){
            offsetY = 0f;
            StartCoroutine(MoveToAttack(enemy, offsetX, offsetY));
        }
        // Botom Right
        else if(dir.x > 0 && dir.y > 0)
        {
            offsetY = 0f;
            offsetX *= -1;
            StartCoroutine(MoveToAttack(enemy, offsetX, offsetY));
        }        
    }

    IEnumerator MoveOnRange(GameObject enemy)
    {
        Debug.Log("Move on Range");
        bool inRange = false;
        agent.SetDestination(enemy.transform.position);
        yield return new WaitForFixedUpdate();
        while (!inRange)
        {
            yield return new WaitForFixedUpdate();
            inRange = CheckAttackRange(enemy);
        }
        agent.SetDestination(transform.position);
        PerformAttack(enemy);
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

    IEnumerator AttackContinous(GameObject enemy)
    {
        Debug.Log("Attack Continous");
        while (canAttack)
        {
            animator.SetTrigger("Attack");
            enemy.GetComponent<EnemyController>().GetHit(attackDamage);
            yield return new WaitForSeconds(attackSpeed);
            if(enemy.GetComponent<EnemyController>().healthPoints <= 0)
            {
                canAttack = false;
            }
        }        
    }

    public bool IsOtherEnemyInRange()
    {
        Vector2 playerPosition = new Vector2(transform.position.x, transform.position.y);
        Collider2D[] hit = Physics2D.OverlapCircleAll(playerPosition, attackRange);
        foreach(Collider2D collision in hit)
        {
            if (collision.CompareTag("Enemy")){
                return true;
            }
        }
        return false;
    }

    IEnumerator MoveToAttack(GameObject enemy, float offsetX, float offsetY)
    {
        Debug.Log("Move to attack");
        int tries = 4;
        while(!CheckVision(enemy) && tries > 0)
        {
            agent.SetDestination(new Vector3(transform.position.x + offsetX, transform.position.y + offsetY, transform.position.z));
            animator.SetBool("Moving", true);
            yield return new WaitForFixedUpdate();
            while(agent.remainingDistance != 0)
            {
                yield return new WaitForSeconds(1);
            }
            tries--;
        }
        StopMovement();
        if (CheckVision(enemy) && CheckAttackRange(enemy))
        {            
            canAttack = true;
            FaceEnemy(enemy);
            StartCoroutine(AttackContinous(enemy));
        }
    }
}
