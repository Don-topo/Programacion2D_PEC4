using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private float scrollSpeed = 5f;
    private int maxScreenMargin = 30;
    private float screenOffset = 0.95f;
    private float minOrthographicSize = 1f;
    private float maxOrthographicSize = 7f;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CanIMove())
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f && cam.orthographicSize > minOrthographicSize)
            {
                cam.orthographicSize--;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f && cam.orthographicSize < maxOrthographicSize)
            {
                cam.orthographicSize++;
            }

            if (Input.mousePosition.y >= Screen.height * screenOffset)
            {
                transform.Translate(scrollSpeed * Time.deltaTime * Vector3.up, Space.World);
            }
            else if (Input.mousePosition.y <= maxScreenMargin * screenOffset)
            {
                transform.Translate(scrollSpeed * Time.deltaTime * Vector3.down, Space.World);
            }
            if (Input.mousePosition.x >= Screen.width * screenOffset)
            {
                transform.Translate(scrollSpeed * Time.deltaTime * Vector3.right, Space.World);
            }
            else if (Input.mousePosition.x <= maxScreenMargin * screenOffset)
            {
                transform.Translate(scrollSpeed * Time.deltaTime * Vector3.left, Space.World);
            }
        }
        
    }
}
