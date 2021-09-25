using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UIManager : NetworkBehaviour
{
    Text startGameText;
    Text countdownText3;
    Text countdownText2;
    Text countdownText1;
    Text teamAPlayerCountText;
    Text teamBPlayerCountText;
    Text teamAWonText;
    Text teamBWonText;

    AudioManager audioManager;
    MyGameManager gameManager;
    ScoreManager scoreManager;


    // Start is called before the first frame update
    void Awake()
    {
        startGameText = GameObject.Find("StartGameText").GetComponent<Text>();
        countdownText3 = GameObject.Find("CountdownText3").GetComponent<Text>();
        countdownText2 = GameObject.Find("CountdownText2").GetComponent<Text>();
        countdownText1 = GameObject.Find("CountdownText1").GetComponent<Text>();

        teamAPlayerCountText = GameObject.Find("TeamAPlayerCountText").GetComponent<Text>();
        teamBPlayerCountText = GameObject.Find("TeamBPlayerCountText").GetComponent<Text>();

        teamAWonText = GameObject.Find("TeamAWonText").GetComponent<Text>();
        teamBWonText = GameObject.Find("TeamBWonText").GetComponent<Text>();

        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        gameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }

    [ClientRpc]
    public void RpcSetStartGameUI() => StartCoroutine(SetStartGameUI());

    IEnumerator SetStartGameUI()
    {
        startGameText.enabled = true;

        yield return new WaitForSeconds(3);

        startGameText.enabled = false;
        countdownText3.enabled = true;

        yield return new WaitForSeconds(1.5f);

        countdownText3.enabled = false;
        countdownText2.enabled = true;

        yield return new WaitForSeconds(1.5f);

        countdownText2.enabled = false;
        countdownText1.enabled = true;

        yield return new WaitForSeconds(1.5f);

        countdownText1.enabled = false;
        audioManager.startGameSound.Play();

        teamAPlayerCountText.text = $"Team A Players : {gameManager.minPlayersPerTeam}";
        teamBPlayerCountText.text = $"Team B Players : {gameManager.minPlayersPerTeam}";
    }

    public void UpdatePlayerCountTexts() => RpcUpdatePlayerCountTexts();

    [ClientRpc]
    public void RpcUpdatePlayerCountTexts()
    {
        teamAPlayerCountText.text = $"Team A Players : {gameManager.minPlayersPerTeam - scoreManager.teams["TeamA"].Count}";
        teamBPlayerCountText.text = $"Team B Players : {gameManager.minPlayersPerTeam - scoreManager.teams["TeamB"].Count}";
    }

    // [ClientRpc]
    // public void RpcSetWinningTeamText()
    // {
    //     if (gameManager.teamAWon)
    //         teamAWonText.enabled = true;
    //     else
    //         teamBWonText.enabled = true;
    // }

    // [ClientRpc]
    // public void RpcUpdatePlayerCountTexts()
    // {
    //     teamAPlayerCountText.text = $"Team A Players : {gameManager.minPlayersPerTeam - scoreManager.teams["TeamA"].Count}";
    //     teamBPlayerCountText.text = $"Team B Players : {gameManager.minPlayersPerTeam - scoreManager.teams["TeamB"].Count}";
    // }

    // [ClientRpc]
    // public void RpcSetWinningTeamText()
    // {
    //     if (gameManager.teamAWon)
    //         teamAWonText.enabled = true;
    //     else
    //         teamBWonText.enabled = true;
    // }
}
