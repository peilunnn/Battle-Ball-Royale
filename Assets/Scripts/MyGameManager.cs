﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class MyGameManager : NetworkBehaviour
{
    public List<GameObject> availablePlayers = new List<GameObject>();

    [SyncVar] public bool gameInProgress = false;

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

    public int tempPlayerCount = 2;

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

    public GameObject GetRandomPlayer()
    {
        return availablePlayers[Random.Range(0, availablePlayers.Count)];
    }

    void Update()
    {
        if (!isServer)
            return;

        // IF 4 PLAYERS HAVE JOINED, START THE GAME 
        if (!gameInProgress && NetworkServer.connections.Count > 3)
            StartCoroutine(StartGame());

        if (teamAPlayerCountText.enabled && teamBPlayerCountText.enabled)
            RpcUpdatePlayerCountTexts();

        // IF ANY OF THE TEAMS WIN, SET THEIR WINNING TEXT
        if (gameInProgress && scoreManager.teamAWon || scoreManager.teamBWon)
            RpcSetWinningTeamText();
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1);

        RpcSetStartGameUI();
        RpcSetGameInProgress();
        RpcSetPlayerCountTexts();
    }



    [ClientRpc]
    void RpcSetGameInProgress()
    {
        gameInProgress = true;
    }

    [ClientRpc]
    void RpcSetStartGameUI()
    {
        StartCoroutine(SetStartGameUI());
    }

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
    }

    [ClientRpc]
    void RpcSetPlayerCountTexts()
    {
        teamAPlayerCountText.enabled = true;
        teamBPlayerCountText.text = $"Team A Players Remaining : {tempPlayerCount}";

        teamAPlayerCountText.enabled = true;
        teamBPlayerCountText.text = $"Team B Players Remaining : {tempPlayerCount}";
    }

    [ClientRpc]
    void RpcUpdatePlayerCountTexts()
    {
        teamAPlayerCountText.text = $"Team A Players Remaining: {tempPlayerCount - scoreManager.teams["TeamA"].Count}";
        teamBPlayerCountText.text = $"Team B Players Remaining: {tempPlayerCount - scoreManager.teams["TeamB"].Count}";
    }


    [ClientRpc]
    void RpcSetWinningTeamText()
    {
        if (scoreManager.teamAWon)
            teamAWonText.enabled = true;
        else
            teamBWonText.enabled = true;
    }
}
