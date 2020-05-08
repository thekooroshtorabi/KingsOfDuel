using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotDetector : MonoBehaviour
{

    [SerializeField]
    private Transform eyeLine;
    private Text txt;
    // Use this for initialization
    void Start()
    {
         txt = GameObject.Find("ShotStatus").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 v = new Vector2(0, 0);
        var hit = Physics2D.Raycast(eyeLine.position, v, 0f);
        Debug.DrawRay(eyeLine.position, v, Color.magenta, 0f);
        if (hit && hit.transform.name != "Reticle")
        {            
            txt.text = hit.transform.name;
        }
    }
}
