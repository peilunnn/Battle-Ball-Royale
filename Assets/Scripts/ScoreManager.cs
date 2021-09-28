using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ScoreManager : NetworkBehaviour
{
    public Dictionary<string, List<GameObject>> teams = new Dictionary<string, List<GameObject>>();

    MyGameManager gameManager;

    UIManager UIManager; 
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();
        UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        teams.Add("TeamA", new List<GameObject>());
        teams.Add("TeamB", new List<GameObject>());
    }

    [ClientRpc]
    public void RpcUpdateDict(GameObject player)
    {
        if (!(teams[$"{player.tag}"].Contains(player)))
            teams[$"{player.tag}"].Add(player);
    }

    [ClientRpc]
    public void RpcCheckIfTeamWon()
    {
        gameManager.teamBWon = teams["TeamA"].Count == gameManager.minPlayersPerTeam;
        gameManager.teamAWon = teams["TeamB"].Count == gameManager.minPlayersPerTeam;

        if (gameManager.teamAWon || gameManager.teamBWon)
            UIManager.RpcSetWinningTeamText();
    }
}
