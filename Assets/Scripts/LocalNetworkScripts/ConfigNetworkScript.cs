using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ConfigNetworkScript : MonoBehaviour
{
    void Awake()
    {
        ConfigNData();
        ConfigPAData();
        if (GameObject.Find("LocalNetworkManager"))
        {
            Destroy(GameObject.Find("LocalNetworkManager"));
        }
    }    

    public void ConfigNData()
    {

        if (File.Exists(Application.persistentDataPath + "/ndata.sav"))
        {
            LoadNData();
            SaveNData(0, 0);
        }
        else
        {
            Globals.networkData.isHost = 0;
            Globals.networkData.ConnectionState = 0;

            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Create(Application.persistentDataPath + "/ndata.sav");
            bf.Serialize(file, Globals.networkData);

            file.Close();
        }
    }
    public void ConfigPAData()
    {
        int i = Random.Range(0, 2);
        if (i == 0)
        {
            Globals.playerAppearance.CharacterSpritePath = "charactertest";
        }
        else if (i == 1)
        {
            Globals.playerAppearance.CharacterSpritePath = "Captain";
        }

        //BinaryFormatter bf = new BinaryFormatter();

        //FileStream file = File.Create(Application.persistentDataPath + "/padata.sav");
        //bf.Serialize(file, Globals.playerAppearance);

        //file.Close();

    }

    public static void SaveNData(int isHost, int ConnectionState)
    {
        Globals.networkData.isHost = isHost;
        Globals.networkData.ConnectionState = ConnectionState;

        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + "/ndata.sav");
        bf.Serialize(file, Globals.networkData);

        //print("Data Saved and the result is : " + Globals.networkData.isHost + "  " + Globals.networkData.ConnectionState);
        file.Close();
    }



    public static void LoadNData()
    {
        if (File.Exists(Application.persistentDataPath + "/ndata.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/ndata.sav", FileMode.Open);
            Globals.networkData = (NetwrokData)bf.Deserialize(file);

            file.Close();
        }
    }
    public static void LoadPAData()
    {
        if (File.Exists(Application.persistentDataPath + "/padata.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/padata.sav", FileMode.Open);
            Globals.playerAppearance = (PlayerAppearanceData)bf.Deserialize(file);
            file.Close();
        }
    }
}
