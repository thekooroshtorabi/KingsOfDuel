using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : MonoBehaviour
{

    public GameObject Target;
    public GameObject MainCamera;
    public bool SyncWithMainCam = true;
    public bool Zoom;
    public float CamMoveSpeed = 0.4f;
    public float ZoomSpeed = 0.1f;
    public int ZoomX = 2;
    private float DestinationX, DestinationY;
    private float CamDefaultX;
    private float CamDefaultY;
    private float CamDefaultSize;
    private Camera cam;
    int v = 0;

    void Awake()
    {
        cam = gameObject.GetComponent<Camera>();
        if (MainCamera != null && SyncWithMainCam == true)
        {
            cam.orthographicSize = MainCamera.GetComponent<Camera>().orthographicSize;
            transform.position = MainCamera.transform.position;
        }

        DestinationX = Target.transform.position.x;
        DestinationY = Target.transform.position.y;

        CamDefaultX = transform.position.x;
        CamDefaultY = transform.position.y;
        CamDefaultSize = gameObject.GetComponent<Camera>().orthographicSize;

    }

    IEnumerator SmoothMoveCam()
    {
        bool isMoving = false;
        // MOVE OVER X
        if (DestinationX < transform.position.x)
        {
            if (!(DestinationX + CamMoveSpeed > transform.position.x && DestinationX - CamMoveSpeed < transform.position.x))
            {
                transform.position += transform.right * -CamMoveSpeed;
                isMoving = true;
            }
        }
        else if (DestinationX > transform.position.x)
        {
            if (!(DestinationX + CamMoveSpeed > transform.position.x && DestinationX - CamMoveSpeed < transform.position.x))
            {
                transform.position += transform.right * CamMoveSpeed;
                isMoving = true;
            }
        }


        // MOVE OVER Y
        if (DestinationY < transform.position.y)
        {
            if (!(DestinationY + CamMoveSpeed > transform.position.y && DestinationY - CamMoveSpeed < transform.position.y))
            {
                transform.position += transform.up * -CamMoveSpeed;
                isMoving = true;
            }
        }
        else if (DestinationY > transform.position.y)
        {
            if (!(DestinationY + CamMoveSpeed > transform.position.y && DestinationY - CamMoveSpeed < transform.position.y))
            {
                transform.position += transform.up * CamMoveSpeed;
                isMoving = true;
            }
        }


        // Zooming

        if (cam.orthographicSize > (CamDefaultSize / ZoomX))
        {
            cam.orthographicSize -= ZoomSpeed;
            isMoving = true;
        }


        yield return new WaitForSeconds(0.01f);
        if (isMoving == true)
        {
            //StartCoroutine(SmoothMoveCam());
        }
    }

    IEnumerator BackToDefault()
    {
        bool isMoving = false;
        // MOVE OVER X
        if (CamDefaultX < transform.position.x)
        {
            if (!(CamDefaultX + CamMoveSpeed > transform.position.x && CamDefaultX - CamMoveSpeed < transform.position.x))
            {
                transform.position += transform.right * -CamMoveSpeed;
                isMoving = true;
            }
        }
        else if (CamDefaultX > transform.position.x)
        {
            if (!(CamDefaultX + CamMoveSpeed > transform.position.x && CamDefaultX - CamMoveSpeed < transform.position.x))
            {
                transform.position += transform.right * CamMoveSpeed;
                isMoving = true;
            }
        }


        // MOVE OVER Y
        if (CamDefaultY < transform.position.y)
        {
            if (!(CamDefaultY + CamMoveSpeed > transform.position.y && CamDefaultY - CamMoveSpeed < transform.position.y))
            {
                transform.position += transform.up * -CamMoveSpeed;
                isMoving = true;
            }
        }
        else if (CamDefaultY > transform.position.y)
        {
            if (!(CamDefaultY + CamMoveSpeed > transform.position.y && CamDefaultY - CamMoveSpeed < transform.position.y))
            {
                transform.position += transform.up * CamMoveSpeed;
                isMoving = true;
            }
        }


        // Zooming

        if (cam.orthographicSize < CamDefaultSize)
        {
            cam.orthographicSize += ZoomSpeed;
            isMoving = true;
        }

        yield return new WaitForSeconds(0.01f);
        if (isMoving == true)
        {
            //StartCoroutine(BackToDefault());
        }
        else
        {
            MainCamera.GetComponent<Camera>().enabled = true;
            cam.enabled = false;
            v = 0;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Zoom == true)
        {
            v++;
            StartCoroutine(SmoothMoveCam());
            MainCamera.GetComponent<Camera>().enabled = false;
            cam.enabled = true;
        }
        else if (Zoom == false)
        {
            v++;
            StartCoroutine(BackToDefault());
        }
    }
}
