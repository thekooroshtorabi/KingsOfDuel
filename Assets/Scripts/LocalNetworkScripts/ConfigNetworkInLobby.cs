using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ConfigNetworkInLobby : MonoBehaviour
{

    private LocalMultiplayer local;
    protected LocalNetworkManager networkManager;
    bool valid = false;
    bool allow = true;

    void Awake()
    {
        if (FindObjectOfType(typeof(LocalMultiplayer)) && NetworkManager.singleton)
        {
            networkManager = NetworkManager.singleton as LocalNetworkManager;

            networkManager.GetComponent<DebugGUI>().enabled = true;
            networkManager.playerPrefab = Resources.Load("PlayerProfile") as GameObject;
            local = FindObjectOfType(typeof(LocalMultiplayer)) as LocalMultiplayer;
        }

        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Open(Application.persistentDataPath + "/ndata.sav", FileMode.Open);
        //Globals.networkData = (NetwrokData)bf.Deserialize(file);
        //file.Close();
        ConfigNetworkScript.LoadNData();
    }

    // Use this for initialization
    void Start()
    {
        //print(Globals.playerAppearance.CharacterSpritePath);
    }

    IEnumerator CheckWhereHostIs()
    {
        yield return new WaitForSeconds(0.5f);
        //print("Server Scene : " + networkManager.discoveryServer.Scene + " | Current Scene : " + SceneManager.GetActiveScene().name);
        if (networkManager.discoveryServer.Scene != "Lobby" && networkManager.discoveryServer.Scene != null)
        {
            GameObject.Find("txtMessage").GetComponent<Text>().text = "Host is in game pls wait";
        }
        else
        {
            GameObject.Find("txtMessage").GetComponent<Text>().text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (valid == false && GameObject.Find("LocalNetworkManager") && GameObject.Find("LocalMultiplayer"))
        {
            valid = true;
            print("is Host : " + Globals.networkData.isHost + " | Connection State : " + Globals.networkData.ConnectionState);
            if (Globals.networkData.isHost == 1)
            {
                local.StartHosting();
                //print("Started Hosting...");
            }
            else if (Globals.networkData.isHost == 0 && Globals.networkData.ConnectionState == 1)
            {
                local.StartJoining();
                //print("Started Joining...");
            }
        }
        if (local.IsConnected() && GameObject.Find("LocalNetworkManager") && GameObject.Find("LocalMultiplayer") && allow == true)
        {
            StartCoroutine(CheckWhereHostIs());
        }
    }

    public void Leave()
    {
        ConfigNetworkScript.SaveNData(0, -1);
        SceneManager.LoadScene("MainMenu");        
    }
}
