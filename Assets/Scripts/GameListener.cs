using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class GameListener : Listener
{

    public enum NetworkState
    {
        Init,
        Offline,
        Connecting,
        Connected,
        Disrupted
    };

    [HideInInspector]
    public static NetworkState networkState = NetworkState.Init;
    public Text networkStateField;

    public GameObject gameSessionPrefab;
    public GameSession gameSession;

    private GameObject WaitingBox;

    protected LocalNetworkManager networkManager;

    public void Start()
    {
        WaitingBox = GameObject.Find("WaitingBox");
        if (WaitingBox)
        {
            WaitingBox.SetActive(false);
        }
        networkState = NetworkState.Offline;
        networkManager = NetworkManager.singleton as LocalNetworkManager;
        ClientScene.RegisterPrefab(gameSessionPrefab);
    }

    public override void OnStartConnecting()
    {
        networkState = NetworkState.Connecting;
        networkStateField.text = "Searching";
        StartCoroutine(Searching());
    }
    int i = 0;
    IEnumerator Searching()
    {
        yield return new WaitForSeconds(0.5f);
        networkStateField.text += ".";
        i++;
        if (i > 3)
        {
            networkStateField.text = "Searching";
            i = 0;
        }
        StartCoroutine(Searching());

    }

    public override void OnStopConnecting()
    {
        networkState = NetworkState.Offline;
        networkStateField.text = "";
    }

    public override void OnServerCreated()
    {
        networkStateField.text = "";
        // Create game session                       
        ConfigNetworkScript.SaveNData(1, Globals.networkData.ConnectionState);

        GameSession oldSession = FindObjectOfType<GameSession>();
        if (oldSession == null)
        {
            GameObject serverSession = Instantiate(gameSessionPrefab);
            NetworkServer.Spawn(serverSession);
        }
        else
        {
            Debug.Log("GameSession already exists!");
        }
    }

    public override void OnJoinedLobby()
    {
        networkStateField.text = "";
        ConfigNetworkScript.SaveNData(Globals.networkData.isHost, 1);
        networkState = NetworkState.Connected;

        gameSession = FindObjectOfType<GameSession>();
        if (gameSession)
        {
            gameSession.OnJoinedLobby();
        }
    }

    public override void OnLeftLobby()
    {
        networkStateField.text = "";
        networkState = NetworkState.Offline;

        gameSession.OnLeftLobby();
    }

    public override void OnCountdownStarted()
    {
        gameSession.OnCountdownStarted();
    }

    public override void OnCountdownCancelled()
    {
        gameSession.OnCountdownCancelled();
    }

    public override void OnStartGame(List<Player> aStartingPlayers)
    {
        Debug.Log("GO!");
        gameSession.OnStartGame(aStartingPlayers);
    }

    public override void OnAbortGame()
    {
        Debug.Log("ABORT!");
        gameSession.OnAbortGame();
    }

    void Update()
    {
        if (!networkManager)
        {
            networkManager = NetworkManager.singleton as LocalNetworkManager;
        }

        if (networkManager.NumPlayers() == 1)
        {
            if (WaitingBox)
            {
                WaitingBox.SetActive(true);
            }
        }
        else
        {
            if (WaitingBox)
            {
                WaitingBox.SetActive(false);
            }
        }
        //networkStateField.text = networkState.ToString();
    }
}
