using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class MyGameManager : NetworkBehaviour
{
    GameObject playerPrefab;
    public List<GameObject> availablePlayers = new List<GameObject>();

    [SyncVar] public bool gameInProgress = false;

    [SerializeField] int temp;

    Text startGameText;
    Text countdownText3;
    Text countdownText2;
    Text countdownText1;
    Text teamAWonText;
    Text teamBWonText;

    AudioSource startGameSound;

    ScoreManager scoreManager;

    void Awake()
    {
        startGameText = GameObject.Find("StartGameText").GetComponent<Text>();
        countdownText3 = GameObject.Find("CountdownText3").GetComponent<Text>();
        countdownText2 = GameObject.Find("CountdownText2").GetComponent<Text>();
        countdownText1 = GameObject.Find("CountdownText1").GetComponent<Text>();
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
        // IF NO. OF PLAYERS LEFT TO JOIN IS LESS THAN 10 IE. 3 PLAYERS HAVE JOINED, START THE GAME 
        if (!gameInProgress && availablePlayers.Count < temp)
        {
            RpcSetStartGameUI();
            RpcSetGameInProgress();
        }

        // IF ANY OF THE TEAMS WIN, SET THEIR WINNING TEXT
        if (gameInProgress && scoreManager.teamAWon || scoreManager.teamBWon)
            RpcSetWinningTeamText();
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

    // [Command]
    // void CmdSetWinningTeamText()
    // {
    //     RpcSetWinningTeamText();
    // }

    [ClientRpc]
    void RpcSetWinningTeamText()
    {
        if (scoreManager.teamAWon)
            teamAWonText.enabled = true;
        else
            teamBWonText.enabled = true;
    }
}
