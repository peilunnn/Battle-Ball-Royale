using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] MyGameManager gameManager;
    [SerializeField] ScoreManager scoreManager;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        playerPrefab = gameManager.GetRandomPlayer();
        base.OnServerAddPlayer(conn);

        gameManager.RpcRemoveFromAvailablePlayers(playerPrefab);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // FOR DC AT "LOBBY" BEFORE GAME SESSION STARTS: ADD BACK PREFAB TO LIST SO THAT MORE PLAYERS CAN JOIN
        if (!gameManager.gameInProgress)
            gameManager.availablePlayers.Add(playerPrefab);

        base.OnServerDisconnect(conn);
    }
}
