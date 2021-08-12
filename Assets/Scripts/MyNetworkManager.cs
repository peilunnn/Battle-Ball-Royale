using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using kcp2k;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] MyGameManager gameManager;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // GET ALR RANDOMIZED PLAYER PREFAB FROM GAME MANAGER
        playerPrefab = gameManager.GetRandomPlayer();

        // ADD PLAYER FOR CONNECTION 
        base.OnServerAddPlayer(conn);

        // REMOVE THAT PREFAB FROM THE LIST SO IT WONT BE AVAILABLE TO OTHER CLIENTS JOINING
        gameManager.allPlayersBeforeGame.Remove(playerPrefab);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        // FOR DC AT "LOBBY" BEFORE GAME SESSION STARTS: ADD BACK PREFAB TO LIST SO THAT MORE PLAYERS CAN JOIN
        if (!gameManager.gameInProgress)
            gameManager.allPlayersBeforeGame.Add(playerPrefab);

        base.OnClientDisconnect(conn);
    }
}
