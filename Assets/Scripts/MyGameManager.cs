using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class MyGameManager : NetworkBehaviour
{
    public List<GameObject> availablePlayers = new List<GameObject>();

    [SyncVar] public bool gameInProgress = false;
    [SyncVar] public bool teamAWon = false;
    [SyncVar] public bool teamBWon = false;

    int minPlayersPerTeam = 2;

    Text startGameText;
    Text countdownText3;
    Text countdownText2;
    Text countdownText1;
    Text teamAPlayerCountText;
    Text teamBPlayerCountText;
    Text teamAWonText;
    Text teamBWonText;

    AudioSource startGameSound;

    ScoreManager scoreManager;


    void Start()
    {
        startGameText = GameObject.Find("StartGameText").GetComponent<Text>();
        countdownText3 = GameObject.Find("CountdownText3").GetComponent<Text>();
        countdownText2 = GameObject.Find("CountdownText2").GetComponent<Text>();
        countdownText1 = GameObject.Find("CountdownText1").GetComponent<Text>();

        teamAPlayerCountText = GameObject.Find("TeamAPlayerCountText").GetComponent<Text>();
        teamBPlayerCountText = GameObject.Find("TeamBPlayerCountText").GetComponent<Text>();

        teamAWonText = GameObject.Find("TeamAWonText").GetComponent<Text>();
        teamBWonText = GameObject.Find("TeamBWonText").GetComponent<Text>();

        startGameSound = GameObject.Find("StartGameSound").GetComponent<AudioSource>();

        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }

    public GameObject GetRandomPlayer() => availablePlayers[Random.Range(0, availablePlayers.Count)];

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

        RpcSetStartGameUI();
        gameInProgress = true;
    }


    [ClientRpc]
    void RpcSetStartGameUI() => StartCoroutine(SetStartGameUI());

    IEnumerator SetStartGameUI()
    {
        startGameText.enabled = true;

        yield return new WaitForSeconds(3);

        startGameText.enabled = false;
        countdownText3.enabled = true;

        yield return new WaitForSeconds(1);

        countdownText3.enabled = false;
        countdownText2.enabled = true;

        yield return new WaitForSeconds(1);

        countdownText2.enabled = false;
        countdownText1.enabled = true;

        yield return new WaitForSeconds(1);

        countdownText1.enabled = false;
        startGameSound.Play();

        teamAPlayerCountText.text = $"Team A Players : {minPlayersPerTeam}";
        teamBPlayerCountText.text = $"Team B Players : {minPlayersPerTeam}";
    }


    [ClientRpc]
    public void RpcUpdatePlayerCountTexts()
    {
        teamAPlayerCountText.text = $"Team A Players : {minPlayersPerTeam - scoreManager.teams["TeamA"].Count}";
        teamBPlayerCountText.text = $"Team B Players : {minPlayersPerTeam - scoreManager.teams["TeamB"].Count}";
    }

    public void CheckIfTeamWon()
    {
        teamBWon = scoreManager.teams["TeamA"].Count == minPlayersPerTeam;
        teamAWon = scoreManager.teams["TeamB"].Count == minPlayersPerTeam;

        if (teamAWon || teamBWon)
            RpcSetWinningTeamText();
    }

    [ClientRpc]
    void RpcSetWinningTeamText()
    {
        if (teamAWon)
            teamAWonText.enabled = true;
        else
            teamBWonText.enabled = true;
    }
}
