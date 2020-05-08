using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTest : MonoBehaviour {

    public Vector3 forward;

    [SerializeField]
    private Transform eyeLine;
    [SerializeField]
    private LayerMask visibleObjects;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 5f, visibleObjects);
        Debug.DrawRay(eyeLine.position, Vector2.right, Color.magenta, 0.1f);
        
            Debug.Log(hit.transform.name);
       


	}
}
