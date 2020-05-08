// A simplified version of Unity's NetworkLobbyManager to prevent it changing scenes on connection

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class LocalLobbyManager : NetworkManager
{
    public int minPlayers = 2;
    public int maxPlayers = 16;
    public Player[] lobbySlots;
    // !IMPORTANT
    public int maxPlayersPerConnection = 2;


    protected LocalNetworkManager networkManager;

    public void SetMaxPlayers(int value)
    {
        maxPlayers = value;
        maxConnections = maxPlayers;
    }

    // ==================== SERVER ====================

    public override void OnStartServer()
    {
        networkManager = singleton as LocalNetworkManager;

        if (lobbySlots == null || lobbySlots.Length == 0)
        {
            lobbySlots = new Player[maxPlayers];
        }
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxPlayers)
        {
            conn.Disconnect();
            return;
        }

        // cannot join game in progress
        if (HasGameStarted())
        {
            conn.Disconnect();
            return;
        }

        base.OnServerConnect(conn);
        OnLobbyServerConnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {        
        Destroy(GameObject.Find("Player" + conn.connectionId));
        networkManager.RemoveSlot(conn.connectionId);

        //base.OnServerDisconnect(conn);

        //// if lobbyplayer for this connection has not been destroyed by now, then destroy it here
        //for (int i = 0; i < lobbySlots.Length; i++)
        //{
        //    var player = lobbySlots[i];
        //    if (player == null)
        //        continue;

        //    if (player.connectionToClient == conn)
        //    {
        //        lobbySlots[i] = null;
        //        NetworkServer.Destroy(player.gameObject);
        //    }
        //}

        //OnLobbyServerDisconnect(conn);
    }

    Byte FindSlot()
    {
        for (byte i = 0; i < maxPlayers; i++)
        {
            if (lobbySlots[i] == null)
            {
                return i;
            }
        }
        return Byte.MaxValue;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {        
        // check MaxPlayersPerConnection
        int numPlayersForConnection = 0;
        foreach (var player in conn.playerControllers)
        {
            if (player.IsValid)
                numPlayersForConnection += 1;
        }

        if (numPlayersForConnection >= maxPlayersPerConnection)
        {
            if (LogFilter.logWarn) { Debug.LogWarning("NetworkLobbyManager no more players for this connection."); }

            var errorMsg = new EmptyMessage();
            conn.Send(MsgType.LobbyAddPlayerFailed, errorMsg);
            return;
        }

        byte slot = FindSlot();
        if (slot == Byte.MaxValue)
        {
            if (LogFilter.logWarn) { Debug.LogWarning("NetworkLobbyManager no space for more players"); }

            var errorMsg = new EmptyMessage();
            conn.Send(MsgType.LobbyAddPlayerFailed, errorMsg);
            return;
        }

        GameObject newLobbyGameObject = OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);        

        if (newLobbyGameObject == null)
        {
            newLobbyGameObject = (GameObject)Instantiate(playerPrefab.gameObject, Vector3.zero, Quaternion.identity);            
        }

        var newLobbyPlayer = newLobbyGameObject.GetComponent<Player>();
        newLobbyPlayer.slot = slot;
        lobbySlots[slot] = newLobbyPlayer;

        NetworkServer.AddPlayerForConnection(conn, newLobbyGameObject, playerControllerId);
    }

    public void DisconnectPlayer(int PlayerIndex)
    {
        Destroy(GameObject.Find("Player" + PlayerIndex));
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        print("PlayerRemoved");
        byte slot = player.gameObject.GetComponent<Player>().slot;
        lobbySlots[slot] = null;
        base.OnServerRemovePlayer(conn, player);
    }

    // ==================== CLIENT ====================

    public override void OnStartClient(NetworkClient lobbyClient)
    {
        if (lobbySlots == null || lobbySlots.Length == 0)
        {
            lobbySlots = new Player[maxPlayers];
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        CallOnClientEnterLobby();
        base.OnClientConnect(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        print("disconected2");
    }

    public override void OnStopClient()
    {
        CallOnClientExitLobby();
    }

    void CallOnClientEnterLobby()
    {
        OnLobbyClientEnter();
        foreach (Player player in lobbySlots)
        {
            if (player == null)
                continue;

            player.OnClientEnterLobby();
        }
    }

    void CallOnClientExitLobby()
    {
        OnLobbyClientExit();
        foreach (Player player in lobbySlots)
        {
            if (player == null)
                continue;

            player.OnClientExitLobby();
        }
    }

    public virtual bool HasGameStarted()
    {
        return false;
    }

    // ------------------------ lobby server virtuals ------------------------

    public virtual void OnLobbyServerConnect(NetworkConnection conn)
    {
    }

    public virtual void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        print("disconected3");
    }

    public virtual GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
    {
        return null;
    }

    public virtual void OnLobbyServerPlayersReady()
    {
    }

    // ------------------------ lobby client virtuals ------------------------

    public virtual void OnLobbyClientEnter()
    {
    }

    public virtual void OnLobbyClientExit()
    {
    }
}
