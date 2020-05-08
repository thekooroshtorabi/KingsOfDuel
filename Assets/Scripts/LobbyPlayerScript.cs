using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayerScript : Player
{

    public Image profileImage;
    public Image box;
    public Text infoField;
    public GameObject LeaderCrown;
    public Button btnKick;
    public GameObject btnPlay;
    public Image txtPlay;
    public GameObject btnTurnoment;
    public Image txtTurnoment;


    [SyncVar]
    public int destoryVar;

    [SyncVar]
    public int loadPlayScene;

    [SyncVar]
    public Color myColour;

    public override void OnStartLocalPlayer()
    {

        base.OnStartLocalPlayer();

        btnPlay = GameObject.Find("btnPlay");
        txtPlay = GameObject.Find("txtPlay").GetComponent<Image>();

        btnTurnoment = GameObject.Find("btnTurnoment");
        txtTurnoment = GameObject.Find("txtTurnoment").GetComponent<Image>();


        btnPlay.GetComponent<Button>().onClick.AddListener(() =>
        {
            CmdLoadPlayScene();

        });

        btnTurnoment.GetComponent<Button>().onClick.AddListener(() =>
        {
            CmdLoadPlayScene();
        });




        // Send custom player info
        // This is an example of sending additional information to the server that might be needed in the lobby (eg. colour, player image, personal settings, etc.)

        myColour = Random.ColorHSV(0, 1, 1, 1, 1, 1);
        CmdSetCustomPlayerInfo(myColour);
    }

    [Command]
    public void CmdSetCustomPlayerInfo(Color aColour)
    {
        myColour = aColour;
    }

    [Command]
    public void CmdDie()
    {
        destoryVar = Random.Range(1, 3);
    }



    [Command]
    public void CmdLoadPlayScene()
    {
        loadPlayScene = Random.Range(1, 3);
    }

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        // Brief delay to let SyncVars propagate
        Invoke("ShowPlayer", 0.5f);
    }

    public override void OnClientExitLobby()
    {
        base.OnClientExitLobby();
        print("ClientExit");
        // Brief delay to let SyncVars propagate        
    }
    bool isOpen = true;
    public void Update()
    {
        if (destoryVar > 0)
        {
            Destroy(GameObject.Find("Player" + playerIndex));
            networkManager.RemoveSlot(playerIndex);
        }

        if (loadPlayScene > 0)
        {
            if (isOpen == true)
            {
                //networkManager.CloseServer();
                if (networkManager.discoveryServer.running)
                {
                    networkManager.discoveryServer.StopBroadcast();
                }
                isOpen = false;
            }
            if (!isServer)
            {
                ConfigNetworkScript.SaveNData(0, 1);
                SceneManager.LoadScene("LocalMultiplayerArena");
            }
            else if (networkManager.NumPlayers() == 1)
            {
                ConfigNetworkScript.SaveNData(1, 1);
                SceneManager.LoadScene("LocalMultiplayerArena");
            }
        }
    }

    void ShowPlayer()
    {
        //if ((playerIndex + 1) <= 6)
        //{
        //    transform.SetParent(GameObject.Find("Canvas/PlayerContainer1st").transform, false);
        //}
        //else if ((playerIndex + 1) > 6 && (playerIndex + 1) <= 12)
        //{
        //    transform.SetParent(GameObject.Find("Canvas/PlayerContainer2nd").transform, false);
        //}
        //else
        //{
        //    transform.SetParent(GameObject.Find("Canvas/PlayerContainer3rd").transform, false);
        //}

        transform.SetParent(GameObject.Find("Canvas/PlayerContainerCanvas/PlayerContainer").transform, false);

        //box.color = myColour;        

        infoField.text = "Player " + (playerIndex + 1);
        if (playerIndex != 0)
        {
            LeaderCrown.SetActive(false);
        }
        if (isServer && playerIndex != 0)
        {
            btnKick.onClick.AddListener(() =>
            {
                CmdDie();
            });
        }
        else
        {
            btnKick.gameObject.SetActive(false);
        }

    }

    void OnGUI()
    {
        if (isLocalPlayer)
        {
            GameSession gameSession = GameSession.instance;
            if (gameSession)
            {
                if (gameSession.gameState == GameState.Lobby)
                {
                    if (isServer)
                    {
                        if (networkManager.NumPlayers() == 2)
                        {                            
                            btnTurnoment.GetComponent<Image>().enabled = false;
                            btnTurnoment.GetComponent<Button>().interactable = false;
                            txtTurnoment.enabled = false;

                            btnPlay.GetComponent<Image>().enabled = true;
                            btnPlay.GetComponent<Button>().interactable = true;
                            txtPlay.enabled = true;
                        }
                        else if (networkManager.NumPlayers() > 2)
                        {
                            btnPlay.GetComponent<Image>().enabled = false;
                            btnPlay.GetComponent<Button>().interactable = false;
                            txtPlay.enabled = false;

                            btnTurnoment.GetComponent<Image>().enabled = true;
                            btnTurnoment.GetComponent<Button>().interactable = true;
                            txtTurnoment.enabled = true;
                        }
                    }
                    else if (isClient)
                    {

                    }

                }
            }
        }
    }
}
