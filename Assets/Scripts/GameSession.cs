using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public enum GameState
{
    Offline,
    Connecting,
    Lobby,
    Countdown,
    WaitingForRolls,
    Scoring,
    //MyShits    
    DuelRoundWaiting,
    DuelRoundStart,
    PowerUpWaiting,
    PowerUpStart,
    IdleRound,
    //End
    GameOver
}

public class GameSession : NetworkBehaviour
{
    public Text gameStateField;
    public Text gameRulesField;

    public static GameSession instance;

    GameListener networkListener;
    List<GamePlayerScript> players;
    string specialMessage = "";

    [SyncVar]
    public GameState gameState;

    [SyncVar]
    public string roundType = "Armor";

    [SyncVar]
    public string message = "";

    [SyncVar]
    public int RandomTime;

    public void OnDestroy()
    {
        if (gameStateField != null)
        {
            gameStateField.text = "";
            gameStateField.gameObject.SetActive(false);
        }
        if (gameRulesField != null)
        {
            gameRulesField.gameObject.SetActive(false);
        }
    }

    [ClientRpc]
    public void RpcShowFireSign()
    {
        GameObject.Find("txtCountdown").GetComponent<Text>().text = "";
        GameObject.Find("Warning").GetComponent<SpriteRenderer>().sortingOrder = 0;
        GameObject.Find("Hold").GetComponent<SpriteRenderer>().sortingOrder = 0;
        GameObject.Find("Fire").GetComponent<SpriteRenderer>().sortingOrder = 1;

        //GameObject.Find("StartSound").GetComponent<AudioSource>().Play();

        //GamePlayerScript.DuelRoundStart = true;
        gameState = GameState.DuelRoundStart;
    }

    [ClientRpc]
    public void RpcShowHoldSign()
    {
        counter = 3;
        GameObject.Find("txtCountdown").GetComponent<Text>().text = "";
        GameObject.Find("Hold").GetComponent<SpriteRenderer>().sortingOrder = 1;
        GameObject.Find("Warning").GetComponent<SpriteRenderer>().sortingOrder = 0;
        GameObject.Find("Fire").GetComponent<SpriteRenderer>().sortingOrder = 0;
        //GamePlayerScript.DuelRoundStart = false;
        gameState = GameState.DuelRoundWaiting;

        while (RandomTime != 0)
        {
            Invoke("RpcShowFireSign", RandomTime);
            break;
        }
    }

    [ClientRpc]
    public void RpcShowCountdown()
    {
        //GamePlayerScript.DuelRoundStart = false;
        if (counter == 3)
        {
            GameObject.Find("Warning").GetComponent<AudioSource>().Play();
        }
        GameObject.Find("Warning").GetComponent<SpriteRenderer>().sortingOrder = 1;
        GameObject.Find("Hold").GetComponent<SpriteRenderer>().sortingOrder = 0;
        GameObject.Find("Fire").GetComponent<SpriteRenderer>().sortingOrder = 0;
        GameObject.Find("txtCountdown").GetComponent<Text>().text = counter.ToString();
        counter--;
    }

    [Server]
    public override void OnStartServer()
    {
        networkListener = FindObjectOfType<GameListener>();
        gameState = GameState.Connecting;
    }

    [Server]
    public void OnStartGame(List<Player> aStartingPlayers)
    {
        players = aStartingPlayers.Select(p => p as GamePlayerScript).ToList();

        RpcOnStartedGame();
        foreach (GamePlayerScript p in players)
        {
            p.RpcOnStartedGame();
        }

        //StartCoroutine(ResetGame());
    }

    [Server]
    public void OnAbortGame()
    {
        RpcOnAbortedGame();
    }

    [Client]
    public override void OnStartClient()
    {
        if (instance)
        {
            Debug.LogError("ERROR: Another GameSession!");
        }
        instance = this;

        networkListener = FindObjectOfType<GameListener>();
        networkListener.gameSession = this;

        if (gameState != GameState.Lobby)
        {
            gameState = GameState.Lobby;
        }
    }

    public void OnJoinedLobby()
    {
        gameState = GameState.Lobby;
    }

    public void OnLeftLobby()
    {
        gameState = GameState.Offline;
    }

    public void OnCountdownStarted()
    {
        //gameState = GameState.Countdown;
    }

    public void OnCountdownCancelled()
    {
        gameState = GameState.Lobby;
    }

    [ClientRpc]
    public void RpcStartNewRound()
    {
        if (gameState == GameState.DuelRoundStart)
        {
            gameState = GameState.Countdown;
            counter = 3;
            RandomTime = 0;
            if (isServer)
            {
                if (roundType == "Armor" || roundType == "PowerUp")
                {
                    roundType = "Kill";
                    if (isServer)
                    {
                        RpcDestoryPUBoxes();
                    }
                }
                else if (roundType == "Kill" && gameState == GameState.Countdown)
                {                    
                    roundType = "PowerUp";
                    if (isServer)
                    {
                        RpcSpawnPowerup();
                    }
                }

                StartCoroutine(Countdown());
            }
        }

    }

    [ClientRpc]
    public void RpcDestoryPUBoxes()
    {
        if (GameObject.Find("ArmorPowerup"))
        {
            Destroy(GameObject.Find("ArmorPowerup"));
        }
        if (GameObject.Find("ArmorPowerup2"))
        {
            Destroy(GameObject.Find("ArmorPowerup2"));
        }
        if (GameObject.Find("RandomPowerup"))
        {
            Destroy(GameObject.Find("RandomPowerup"));
        }
    }

    [Server]
    IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(0.1f);
    }

    [Server]
    int MaxScore()
    {
        return players.Max(p => p.totalPoints);
    }

    [Server]
    GamePlayerScript PlayerWithHighestHealth()
    {
        int highestHealth = players.Max(p => p.health);
        return players.Where(p => p.health == highestHealth).First();
    }

    [Server]
    public void PlayAgain()
    {
        StartCoroutine(ResetGame());
    }

    [ClientRpc]
    public void RpcRematch()
    {
        gameState = GameState.Lobby;
    }

    bool var = true;
    private bool Sign = true;
    void Update()
    {
        if (isServer)
        {
            if (gameState == GameState.Countdown && var == true)
            {
                var = false;
                GameObject.Find("Warning").GetComponent<SpriteRenderer>().sortingOrder = 1;
                if (isServer)
                {
                    RpcSpawnPowerup();
                    StartCoroutine(Countdown());
                }
            }
        }

        //gameStateField.text = message;
    }


    int counter = 3;
    IEnumerator Countdown()
    {
        RpcShowCountdown();

        yield return new WaitForSeconds(1);
        if (counter > 0)
        {
            StartCoroutine(Countdown());
        }
        else
        {
            SetRandomTime();
            RpcShowHoldSign();
        }

    }

    [Server]
    public void SetRandomTime()
    {
        RandomTime = Random.Range(3, 9);
    }

    // Client RPCs

    [ClientRpc]
    public void RpcSpawnPowerup()
    {
        GameObject obj;
        if (roundType == "Armor")
        {
            obj = (GameObject)Instantiate(Resources.Load("ArmorPowerup"), GameObject.Find("PowerUpsSpawnPoint").transform.position, Quaternion.identity);
            obj.name = "ArmorPowerup";
            GameObject obj2 = Instantiate(obj);
            obj2.name = "ArmorPowerup2";
        }
        else
        {
            obj = (GameObject)Instantiate(Resources.Load("RandomPowerup"), GameObject.Find("PowerUpsSpawnPoint").transform.position, Quaternion.identity);
            obj.name = "RandomPowerup";
        }
    }

    [ClientRpc]
    public void RpcOnStartedGame()
    {
        gameRulesField.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void RpcOnAbortedGame()
    {
        gameRulesField.gameObject.SetActive(false);
    }
}
