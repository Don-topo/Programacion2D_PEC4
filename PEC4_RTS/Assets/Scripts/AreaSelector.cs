using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSelector : MonoBehaviour
{
    private List<GameObject> selectedSoldiers;
    private int numColliders = 10;
    private Collider2D[] colliders;
    private ContactFilter2D contactFilter = new ContactFilter2D();
    private int numSelectedColliders;
   
    private void Awake()
    {
        selectedSoldiers = new List<GameObject>();
        Collider2D collider2D = GetComponent<Collider2D>();
        colliders = new Collider2D[numColliders];
        //numSelectedColliders = collider2D.OverlapCollider(contactFilter, colliders);
        //SetUnits();       
    }
   
    private void OnDestroy()
    {
        GetComponentInParent<ActionManager>().SetAreaSelected(selectedSoldiers);
    }

    private void SetUnits()
    {
        Debug.Log("Selected Units " + numSelectedColliders);
        for(int i = 0; i < numSelectedColliders; i++)
        {
            selectedSoldiers.Add(colliders[i].gameObject);
        }
    }

}
