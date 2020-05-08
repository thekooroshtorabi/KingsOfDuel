using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{


    GameObject Powerup;

    public void SpawpRealPowerUp(Vector3 vec, string PowerUpSpriteName)
    {

        Powerup = (GameObject)Instantiate(Resources.Load("WoodenBox"), new Vector3(vec.x, vec.y, 90), Quaternion.identity);        

        Object[] SpriteSheet;
        SpriteSheet = Resources.LoadAll("Sprites/PowerUps");
        foreach (Object spt in SpriteSheet)
        {
            if ((spt.name == PowerUpSpriteName))
            {
                Powerup.GetComponent<SpriteRenderer>().sprite = (Sprite)spt;
            }
        }


        Powerup.name = "RealPowerUp";        

        Destroy(Powerup, 2.0f);

    }

}
