using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawnerScript : MonoBehaviour
{
    public GameObject[] prefabs;    

    void Start()
    {
        StartCoroutine(generate());
    }

    IEnumerator generate()
    {        
        GameObject obj = Instantiate(prefabs[Random.Range(0, prefabs.Length)], transform.position, Quaternion.identity);
        obj.GetComponent<Rigidbody2D>().velocity = new Vector2(-1,0);
        Destroy(obj, 25f);
        yield return new WaitForSeconds(Random.Range(5, 7));
        StartCoroutine(generate());
    }
}
