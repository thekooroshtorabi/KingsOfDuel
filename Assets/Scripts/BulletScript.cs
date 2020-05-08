using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public static int velocity = 100;
    
    void FixedUpdate()
    {
        transform.position += transform.right * Time.deltaTime * -velocity;
    }
}
