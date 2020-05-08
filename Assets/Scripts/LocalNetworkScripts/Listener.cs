using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Listener : MonoBehaviour {

    [HideInInspector]
    public LocalMultiplayer lp;

    public void Awake()
    {
        lp = FindObjectOfType(typeof(LocalMultiplayer)) as LocalMultiplayer;        
    }

    public virtual void OnStartConnecting()
    {
        // it's Overrided in GameListener.cs        
    }

    public virtual void OnStopConnecting()
    {
        // it's Overrided in GameListener.cs        
    }

    public virtual void OnServerCreated()
    {
        // it's Overrided in GameListener.cs'        
    }

    public virtual void OnReceivedBroadcast(string aFromAddress, string aData)
    {
        // it's Overrided in GameListener.cs
    }

    public virtual void OnDiscoveredServer(DiscoveredServer aServer)
    {
        // it's Overrided in GameListener.cs
        
    }

    public virtual void OnJoinedLobby()
    {
        // it's Overrided in GameListener.cs
    }

    public virtual void OnLeftLobby()
    {
        // it's Overrided in GameListener.cs
        
    }

    public virtual void OnCountdownStarted()
    {
        // it's Overrided in GameListener.cs
    }

    public virtual void OnCountdownCancelled()
    {
        // it's Overrided in GameListener.cs
    }

    public virtual void OnStartGame(List<Player> aStartingPlayers)
    {
        // it's Overrided in GameListener.cs
        
    }

    public virtual void OnAbortGame()
    {
        // it's Overrided in GameListener.cs
    }
}
