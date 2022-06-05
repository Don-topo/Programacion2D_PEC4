using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierController : MonoBehaviour
{
    public float attackSpeed = 1f;
    public int attackDamage = 1;
    public int healthPoints = 1;
    public GameObject selectedGameObject;
    public NavMeshAgent agent;

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
        agent.SetDestination(newPosition);
        agent.updateRotation = false;
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
        // Check if I am in range
        // Move if need
        // If I am in range attack
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
        Gizmos.DrawWireSphere(transform.position, 15);
    }
}
