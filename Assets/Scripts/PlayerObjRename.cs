using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObjRename : MonoBehaviour
{
    public GameObject Parent;
    public Text Index;

    void Start()
    {
        Invoke("Rename", 1f);
    }

    public void Rename()
    {
        if (Index != null)
        {
            Parent.name = "Player" + Index.text;
        }
    }
}
