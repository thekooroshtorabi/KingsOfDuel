using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ReticleScript : MonoBehaviour
{

    Rigidbody2D rgbdy;
    int x = 3;
    int y = 3;
    Vector3 StartSwipePosition;
    private float draggedTime = 0f;

    //--------------SETTINGS-------------
    public float aimBumpSpeed = 3f;
    public float aimBumpRate = 2.6f;
    private bool isDraging = false;
    private float maxDragTime = 1.0f;
    private float dragSpeed = 1.5f;


    // Use this for initialization
    void Start()
    {
        x = Random.Range(0, 3);
        //Debug.Log("x=" + x);
        y = Random.Range(0, 2);
        //Debug.Log("y=" + y);
        StartCoroutine(RMove());
    }

    void Awake()
    {
        rgbdy = GetComponent<Rigidbody2D>();
    }

    IEnumerator RMove()
    {
        x = Random.Range(0, 3);
        //Debug.Log("x=" + x);
        y = Random.Range(0, 2);
        //Debug.Log("y=" + y);


        if (x == 0)
        {
            //transform.Translate(Vector3.right * speed * Time.deltaTime);
            Vector2 v = new Vector2(Random.Range(1f, aimBumpSpeed), 0);
            rgbdy.velocity += v;
        }
        else if (x == 1)
        {
            //transform.Translate(Vector3.left * speed * Time.deltaTime);
            Vector2 v = new Vector2(Random.Range(-aimBumpSpeed, -1f), 0);
            rgbdy.velocity += v;
        }

        if (y == 0)
        {
            //transform.Translate(Vector3.up * speed * Time.deltaTime);
            Vector2 v = new Vector2(0, Random.Range(1f, aimBumpSpeed));
            rgbdy.velocity += v;
        }
        else if (y == 1)
        {
            //transform.Translate(Vector3.down * speed * Time.deltaTime);
            Vector2 v = new Vector2(0, Random.Range(-aimBumpSpeed, -1f));
            rgbdy.velocity += v;
        }
        yield return new WaitForSeconds(aimBumpRate);        
        StartCoroutine(RMove());
        isDraging = false;

    }

    void OnMouseDown()
    {
        StartSwipePosition = transform.position;
        draggedTime = 0f;
    }

    void OnMouseDrag()
    {
        isDraging = true;
    }

    void OnMouseUp()
    {
        isDraging = false;
        //Debug.Log(SwipeValues);

        //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));       
    }

    // Update is called once per frame
    void Update()
    {
        draggedTime += Time.deltaTime;
        if (isDraging == true && maxDragTime > draggedTime)
        {
            Vector3 EndSwipePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log(EndSwipePosition);
            //Debug.Log("Start : " + StartSwipePosition + " , END : " + EndSwipePosition);

            Vector3 SwipeValues = new Vector3(EndSwipePosition.x - StartSwipePosition.x, EndSwipePosition.y - StartSwipePosition.y, 0);
            if ((SwipeValues.x < -0.3 || SwipeValues.x > 0.3) || (SwipeValues.y < -0.3 || SwipeValues.y > 0.3))
            {
                Vector2 v = new Vector2(SwipeValues.x * dragSpeed, SwipeValues.y * dragSpeed);
                rgbdy.velocity = v;
            }
        }
    }
}
