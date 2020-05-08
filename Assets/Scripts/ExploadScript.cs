using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploadScript : MonoBehaviour
{
    public bool SelfDestruction = false;
    public float SelfDestructionDelay = 0;
    List<GameObject> fragments;

    private void DestroyFragments()
    {
        foreach (GameObject fragment in fragments)
        {
            Destroy(fragment, 0.3f);
        }
        fragments.Clear();
    }

    public void SpawnPowerup(string PowerupName)
    {
        new SpawnScript().SpawpRealPowerUp(transform.position, PowerupName);
    }

    private void Update()
    {
        if (SelfDestruction == true)
        {
            StartCoroutine(SelfDestruct());
        }
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(SelfDestructionDelay + 0.25f);
        if (GameObject.Find("ExplosionSound"))
        {
            GameObject.Find("ExplosionSound").GetComponent<AudioSource>().Play();
        }

        // Particle
        GameObject obj = (GameObject)Instantiate(Resources.Load("Praticle"), transform.position, Quaternion.identity);
        obj.AddComponent<SpawnScript>();
        Destroy(obj, 2.0f);

        //fragments = SpriteExploder.GenerateTriangularPieces(gameObject, 5, 0);
        //foreach (GameObject fragment in fragments)
        //{
        //    fragment.AddComponent<ExplosionForce>();
        //    fragment.AddComponent<ExploadScript>();
        //}

        Destroy(gameObject);
        //ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
        //ef.doExplosion(transform.position);
        //DestroyFragments();
    }

}
