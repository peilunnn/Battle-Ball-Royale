using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class MyGameManager : NetworkBehaviour
{
    public List<GameObject> availablePlayers = new List<GameObject>();

    [SyncVar] public bool gameInProgress = false;

    [SyncVar] public bool redTeamWon = false;
    [SyncVar] public bool blueTeamWon = false;

    public int minPlayersPerTeam;
    public bool isPlaytesting;
    int i = 0;

    ScoreManager scoreManager;
    UIManager UIManager;


    void Start()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }


    public GameObject GetRandomPlayer()
    {
        if (!isPlaytesting)
            return availablePlayers[Random.Range(0, availablePlayers.Count)];

        return availablePlayers[i++];
    }


    [ClientRpc]
    public void RpcRemoveFromAvailablePlayers(GameObject playerPrefab) => availablePlayers.Remove(playerPrefab);


    void Update()
    {
        if (!isServer)
            return;

        // only start game if 4 players have joined
        if (!gameInProgress && NetworkServer.connections.Count >= minPlayersPerTeam * 2)
            StartCoroutine(StartGame());
    }


    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1);

        UIManager.RpcSetStartGameUI();
        gameInProgress = true;
    }
}