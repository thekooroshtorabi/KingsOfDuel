using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScript : MonoBehaviour
{

    void OnCollisionEnter2D(Collision2D col)
    {
        //Destroy(col.gameObject,2f);
        if (col.collider.name == "RandomPowerup" || col.collider.name == "ArmorPowerup" || col.collider.name == "ArmorPowerup2")
        {
            GameObject GObj = (GameObject)Instantiate(Resources.Load("DustUpParticle"), col.transform.position, Quaternion.identity);
            Destroy(GObj, 2f);
        }
    }
}
