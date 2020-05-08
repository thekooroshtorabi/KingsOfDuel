using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ConfigNetworkInGame : MonoBehaviour
{

    private LocalMultiplayer local;
    protected LocalNetworkManager networkManager;
    bool valid = false;

    void Awake()
    {
        networkManager = NetworkManager.singleton as LocalNetworkManager;

        networkManager.GetComponent<DebugGUI>().enabled = false;
        networkManager.playerPrefab = Resources.Load("Character") as GameObject;
        local = FindObjectOfType(typeof(LocalMultiplayer)) as LocalMultiplayer;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/ndata.sav", FileMode.Open);
        Globals.networkData = (NetwrokData)bf.Deserialize(file);
        file.Close();
    }

    void Start()
    {

    }

    void Update()
    {
        if (valid == false && GameObject.Find("LocalNetworkManager") && GameObject.Find("LocalMultiplayer"))
        {            
            valid = true;
            print("is Host : " + Globals.networkData.isHost + " | Connection State : " + Globals.networkData.ConnectionState);
            if (Globals.networkData.isHost == 1)
            {
                local.StartHosting();
                print("Started Hosting...");
            }
            else if (Globals.networkData.isHost == 0 && Globals.networkData.ConnectionState == 1)
            {
                local.StartJoining();
                print("Started Joining...");
            }            
        }        
    }

    public void Leave()
    {        
        ConfigNetworkScript.SaveNData(0, -1);
        SceneManager.LoadScene("Lobby");
    }
    public void Pause()
    {        
        ConfigNetworkScript.SaveNData(0, -1);
        SceneManager.LoadScene("MainMenu");
    }

}
