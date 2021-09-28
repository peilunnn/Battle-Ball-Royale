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
    Text redTeamPlayerCountText;
    Text blueTeamPlayerCountText;
    Text redTeamWonText;
    Text blueTeamWonText;

    AudioManager audioManager;
    MyGameManager gameManager;
    ScoreManager scoreManager;


    // Start is called before the first frame update
    void Start()
    {
        startGameText = GameObject.Find("StartGameText").GetComponent<Text>();
        countdownText3 = GameObject.Find("CountdownText3").GetComponent<Text>();
        countdownText2 = GameObject.Find("CountdownText2").GetComponent<Text>();
        countdownText1 = GameObject.Find("CountdownText1").GetComponent<Text>();

        redTeamPlayerCountText = GameObject.Find("RedTeamPlayerCountText").GetComponent<Text>();
        blueTeamPlayerCountText = GameObject.Find("BlueTeamPlayerCountText").GetComponent<Text>();

        redTeamWonText = GameObject.Find("RedTeamWonText").GetComponent<Text>();
        blueTeamWonText = GameObject.Find("BlueTeamWonText").GetComponent<Text>();

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

        redTeamPlayerCountText.text = $"Red Team Players : {gameManager.minPlayersPerTeam}";
        blueTeamPlayerCountText.text = $"Blue Team Players : {gameManager.minPlayersPerTeam}";
    }


    [ClientRpc]
    public void RpcUpdatePlayerCountTexts()
    {
        redTeamPlayerCountText.text = $"Red Team Players : {gameManager.minPlayersPerTeam - scoreManager.teams["RedTeam"].Count}";
        blueTeamPlayerCountText.text = $"Blue Team Players : {gameManager.minPlayersPerTeam - scoreManager.teams["BlueTeam"].Count}";
    }


    [ClientRpc]
    public void RpcSetWinningTeamText()
    {
        gameManager.blueTeamWon = scoreManager.teams["RedTeam"].Count == gameManager.minPlayersPerTeam;
        gameManager.redTeamWon = scoreManager.teams["BlueTeam"].Count == gameManager.minPlayersPerTeam;

        if (gameManager.redTeamWon)
            redTeamWonText.enabled = true;

        else if (gameManager.blueTeamWon)
            blueTeamWonText.enabled = true;
    }
}
