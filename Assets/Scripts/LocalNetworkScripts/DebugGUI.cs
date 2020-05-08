using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DebugGUI : MonoBehaviour
{
    private LocalMultiplayer local;
    private LocalNetworkManager networkManager;
    public static bool ShowGUI = true;
    private Button btnHost;
    private Button btnCancel;
    private Button btnJoin;

    public void Awake()
    {
        if (GameObject.Find("btnCancel"))
        {
            btnCancel = GameObject.Find("btnCancel").GetComponent<Button>();
            btnCancel.onClick.AddListener(() =>
                    {
                        local.Cancel();
                        ShowGUI = true;
                    });
            btnCancel.gameObject.SetActive(false);
        }

        if (GameObject.Find("btnHost") && GameObject.Find("btnJoin"))
        {
            btnHost = GameObject.Find("btnHost").GetComponent<Button>();
            btnHost.onClick.AddListener(() =>
                   {
                       local.StartHosting();
                       ShowGUI = false;
                   });

            btnJoin = GameObject.Find("btnJoin").GetComponent<Button>();
            btnJoin.onClick.AddListener(() =>
            {
                local.StartJoining();
                ShowGUI = false;
            });

            btnHost.gameObject.SetActive(false);
        }


        local = FindObjectOfType(typeof(LocalMultiplayer)) as LocalMultiplayer;
        networkManager = GetComponent<LocalNetworkManager>();
    }

    void Start()
    {
        if (Globals.networkData.ConnectionState == 0)
        {
            local.AutoConnect();
        }
    }

    void Update()
    {
        if (networkManager.IsConnected())
        {
            btnHost.gameObject.SetActive(false);
            btnJoin.gameObject.SetActive(false);
            btnCancel.gameObject.SetActive(false);
        }
        else if ((networkManager.IsBroadcasting() || networkManager.IsJoining()) && Globals.networkData.ConnectionState != 1)
        {
            btnHost.gameObject.SetActive(false);
            btnJoin.gameObject.SetActive(false);
            btnCancel.gameObject.SetActive(true);
        }
        else if ((ShowGUI == true && Globals.networkData.ConnectionState != 1) || (Globals.networkData.ConnectionState == -1 && Globals.networkData.isHost == 0))
        {
            btnCancel.gameObject.SetActive(false);
            btnHost.gameObject.SetActive(true);
            btnJoin.gameObject.SetActive(true);
        }
    }

    void OnGUI()
    {

    }
}
