using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{

    public static int VERSION = 1;
    private LocalMultiplayer local;

    [SyncVar]
    public int version;

    [SyncVar]
    public string deviceName;
    [SyncVar]
    public string deviceId;
    [SyncVar]
    public string peerId;
    [SyncVar]
    public int playerIndex;

    [SyncVar(hook = "OnReadyChanged")]
    public bool ready;

    [SyncVar]
    public byte slot;


    // public string deviceModel;
    // public int memory;
    // public int processorFrequency;
    // public string operatingSystem;

    protected LocalNetworkManager networkManager;

    public override void OnStartClient()
    {
        local = FindObjectOfType(typeof(LocalMultiplayer)) as LocalMultiplayer;
        networkManager = NetworkManager.singleton as LocalNetworkManager;
        if (networkManager)
        {
            networkManager.lobbySlots[slot] = this;
            OnClientEnterLobby();
        }
        else
        {
            Debug.LogError("LocalPlayer could not find the NetworkManager.");
        }
    }

    public override void OnStartLocalPlayer()
    {
        networkManager = NetworkManager.singleton as LocalNetworkManager;
        networkManager.localPlayer = this;

#if UNITY_ANDROID
			deviceName = SystemInfo.deviceModel;
#else
        deviceName = SystemInfo.deviceName;
#endif

        version = VERSION;
        deviceId = networkManager.deviceId;
        peerId = networkManager.peerId;
        playerIndex = slot;
        ready = false;

        // deviceModel = SystemInfo.deviceModel;
        // memory = SystemInfo.systemMemorySize;
        // processorFrequency = SystemInfo.processorFrequency;
        // operatingSystem = SystemInfo.operatingSystem;
        // Debug.Log(String.Format("Device specs: {0}, {1}, {2} proc, {3} mem", deviceModel, operatingSystem, processorFrequency, memory));

        CmdSetBasePlayerInfo(version, deviceName, deviceId, peerId, playerIndex);
    }

    public void OnDestroy()
    {        
        if (playerIndex == 0)
        {
            local.Cancel();            
        }
        if (networkManager.localPlayer == this)
        {
            local.Cancel();           
            DebugGUI.ShowGUI = true;

            // Remove all the players for local player
            GameObject PContainer = GameObject.Find("PlayerContainer");
            if (PContainer != null && PContainer.transform.childCount > 1)
            {
                for (int i = 0; i < PContainer.transform.childCount; i++)
                {
                    Destroy(PContainer.transform.GetChild(i).gameObject);
                }
            }

            // Change Net State to Default
            GameListener.networkState = GameListener.NetworkState.Offline;

            // Remove Game Session
            Destroy(GameObject.Find("GameSession(Clone)"));

            // Reset Server and Client settings
            //Server.ResetServer = true;
            Client.ResetClient = true;

        }

        OnClientExitLobby();
        // If this is a client player on the server then OnClientExitLobby will not be called.
        // Call it here instead.

        //if (networkManager.IsHost() && networkManager.localPlayer != this)
        //{
        //    OnClientExitLobby();
        //}
    }

    [Command]
    public virtual void CmdSetBasePlayerInfo(int aVersion, string aDeviceName, string aDeviceId, string aPeerId, int aPlayerIndex)
    {
        version = aVersion;
        deviceName = aDeviceName;
        deviceId = aDeviceId;
        peerId = aPeerId;
        playerIndex = aPlayerIndex;
    }

    [Command]
    public void CmdSetReady(bool r)
    {
        ready = r;
    }    

    public bool IsReady()
    {
        return ready;
    }

    void OnReadyChanged(bool newValue)
    {
        ready = newValue;
        OnClientReady(ready);
        if (ready)
        {
            networkManager.CheckReadyToBegin();
        }
    }

    public void SendReadyToBeginMessage()
    {
        CmdSetReady(true);
    }

    public void SendNotReadyToBeginMessage()
    {
        CmdSetReady(false);
    }

    public virtual void OnClientEnterLobby()
    {
    }

    public virtual void OnClientExitLobby()
    {
    }

    public virtual void OnClientReady(bool readyState)
    {
    }

}
