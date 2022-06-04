using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private float scrollSpeed = 15f; 

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0f && cam.orthographicSize > 1f)
        {
            cam.orthographicSize--;
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0f && cam.orthographicSize < 7f)
        {
            cam.orthographicSize++;
        }
        // TODO Move camera if mouse is on corners
        if (Input.mousePosition.y >= Screen.height * 0.95)
        {
            transform.Translate(scrollSpeed * Time.deltaTime * Vector3.right, Space.World);
        }
    }

    private bool CanResize()
    {
        return ((cam.orthographicSize - 1) > 1f) && ((cam.orthographicSize + 1) < 7f);
    }
}
