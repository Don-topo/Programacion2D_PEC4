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
    private bool isFacingRight = true;
    private Coroutine movingCoroutine;

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

    public void Move(Vector3 newPosition)
    {
        if(movingCoroutine != null)
        {
            StopCoroutine(movingCoroutine);
        }        
        movingCoroutine = StartCoroutine(Moving(newPosition));
    }

    IEnumerator Moving(Vector3 newPosition)
    {
        while(transform.position != newPosition)
        {
            animator.SetBool("Moving", true);            
            transform.position = Vector3.Lerp(transform.position, newPosition,  0.1f* Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        animator.SetBool("Moving", false);
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
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
}
