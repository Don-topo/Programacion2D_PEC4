using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField]
    public Dialog dialog;

    private bool trigger = false;


    private void FixedUpdate()
    {
        if (!trigger)
        {
            CheckIfPlayerEnter();
        }        
    }

    public void TriggerDialoge()
    {
        DialogManager.Instance.StartDialog(dialog);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(transform.position, new Vector3(20,1,1));
    }

    private void CheckIfPlayerEnter()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, new Vector2(20,1), 0, Vector2.zero);
        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Soldier"))
            {
                trigger = true;
                TriggerDialoge();
                Destroy(this);
            }
        }
    }

}
