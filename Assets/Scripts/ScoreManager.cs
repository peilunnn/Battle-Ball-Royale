using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ScoreManager : NetworkBehaviour
{
    public Dictionary<string, List<GameObject>> teams = new Dictionary<string, List<GameObject>>();

    MyGameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();
        teams.Add("TeamA", new List<GameObject>());
        teams.Add("TeamB", new List<GameObject>());
    }

    public void UpdateDict(GameObject player)
    {
        if (!(teams[$"{player.tag}"].Contains(player)))
            teams[$"{player.tag}"].Add(player);
    }
}
