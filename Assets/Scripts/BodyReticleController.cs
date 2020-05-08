using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyReticleController : MonoBehaviour {

    Rigidbody2D rgbdy;    
    Vector3 StartSwipePosition;

    // Use this for initialization
    void Start()
    {

    }

    void Awake()
    {
        rgbdy = GameObject.Find("Reticle").GetComponent<Rigidbody2D>();
    }    

    void OnMouseDown()
    {
        StartSwipePosition =  GameObject.Find("Reticle").transform.position;
        
    }

    void OnMouseDrag()
    {
        
        Vector3 EndSwipePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(EndSwipePosition);
        //Debug.Log("Start : " + StartSwipePosition + " , END : " + EndSwipePosition);

        Vector3 SwipeValues = new Vector3(EndSwipePosition.x - StartSwipePosition.x, EndSwipePosition.y - StartSwipePosition.y, 0);
        if ((SwipeValues.x < -0.3 || SwipeValues.x > 0.3) || (SwipeValues.y < -0.3 || SwipeValues.y > 0.3))
        {
            Vector2 v = new Vector2(SwipeValues.x, SwipeValues.y);
            rgbdy.velocity = v;
        }
    }    

	// Update is called once per frame
	void Update () {
		
	}
}
