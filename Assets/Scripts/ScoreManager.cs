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

        teams.Add("RedTeam", new List<GameObject>());
        teams.Add("BlueTeam", new List<GameObject>());
    }

    [ClientRpc]
    public void RpcUpdateDict(GameObject player)
    {
        if (!(teams[$"{player.tag}"].Contains(player)))
            teams[$"{player.tag}"].Add(player);
    }
}
