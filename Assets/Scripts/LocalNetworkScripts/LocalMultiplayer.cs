using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LocalMultiplayer : MonoBehaviour {

    public string broadcastIdentifier = "CM";
    public int minPlayers = 2;
    public int maxPlayers = 4;
    public Player playerPrefab;
    public float countdownDuration = 3; // Wait for this many seconds after people are ready before starting the game
    public Listener listener;
    public bool verboseLogging = false;
    public bool useDebugGUI = true;
    public bool forceServer = false;

    private LocalNetworkManager networkManager;

    public void Awake()
    {
        ValidateConfig();

        // Create network manager
        networkManager = (Instantiate(Resources.Load("LocalNetworkManager")) as GameObject).GetComponent<LocalNetworkManager>();        
        if (networkManager != null)
        {
            //networkManager.logLevel = 0;

            networkManager.name = "LocalNetworkManager";
            //networkManager.dontDestroyOnLoad = false;
            networkManager.runInBackground = true; // runInBackground is not recommended on iOS
            networkManager.broadcastIdentifier = broadcastIdentifier;
            networkManager.minPlayers = minPlayers;                        
            networkManager.SetMaxPlayers(maxPlayers); //Setting maxPlayers and maxConnections
            networkManager.allReadyCountdownDuration = countdownDuration;
            networkManager.forceServer = forceServer;

            // I'm just using a single scene for everything
            networkManager.offlineScene = "";
            networkManager.onlineScene = "";

            networkManager.playerPrefab = playerPrefab.gameObject;
            networkManager.listener = listener;
            networkManager.verboseLogging = verboseLogging;

            // Optionally create Debug GUI
            if (useDebugGUI)
            {
                networkManager.GetComponent<DebugGUI>().enabled = true;
            }
        }
        else
        {
            Debug.LogError("Error creating network manager");
        }
    }

    public void ValidateConfig()
    {
        if (broadcastIdentifier == "Spaceteam")
        {
            Debug.LogError("You should pick a unique Broadcast Identifier for your game", this);
        }
        if (playerPrefab == null)
        {
            Debug.LogError("Please pick a Player prefab", this);
        }
        if (listener == null)
        {
            Debug.LogError("Please set a Listener object", this);
        }
    }

    public void Update()
    {
        if (networkManager == null)
        {
            networkManager = FindObjectOfType(typeof(LocalNetworkManager)) as LocalNetworkManager;
            networkManager.listener = listener;

            if (networkManager.verboseLogging)
            {
                Debug.Log("!! RECONNECTING !!");
            }
        }
    }

    public List<Player> Players()
    {
        return networkManager.LobbyPlayers();
    }

    public Player LocalPlayer()
    {
        return networkManager.localPlayer;
    }

    public void AutoConnect()
    {
        networkManager.InitNetworkTransport();
        networkManager.minPlayers = minPlayers;
        networkManager.AutoConnect();
    }

    public void StartHosting()
    {
        networkManager.InitNetworkTransport();
        networkManager.minPlayers = minPlayers;
        networkManager.StartHosting();
    }

    public void StartJoining()
    {        
        networkManager.InitNetworkTransport();
        networkManager.minPlayers = minPlayers;
        networkManager.StartJoining();
    }

    public void Cancel()
    {          
        networkManager.Cancel();
        networkManager.ShutdownNetworkTransport();
    }

    public bool AreAllPlayersReady()
    {
        return networkManager.AreAllPlayersReady();
    }

    public float CountdownTimer()
    {
        return networkManager.allReadyCountdown;
    }

    public void StartLocalGameForDebugging()
    {
        networkManager.InitNetworkTransport();
        networkManager.minPlayers = 1;
        networkManager.StartLocalGameForDebugging();
    }

    public bool IsConnected()
    {
        return networkManager.IsConnected();
    }

    public bool IsHost()
    {
        return networkManager.IsHost();
    }

    public void FinishGame()
    {
        networkManager.FinishGame();
    }

    public void SetForceServer(bool fs)
    {
        forceServer = fs;
        networkManager.forceServer = fs;
    }

    public void SetPrivateTeamKey(string key)
    {
        networkManager.SetPrivateTeamKey(key);
    }

    public int HighestConnectedVersion()
    {
        return networkManager.HighestConnectedVersion();
    }
}
