using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{

    public float startPoint;
    public float finishPoint;
    public float velocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x >= finishPoint)
        {
            transform.position = new Vector3(startPoint, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x + (velocity * Time.deltaTime), transform.position.y, transform.position.z);
        }
    }
}
