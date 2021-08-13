﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ScoreManager : NetworkBehaviour
{
    public Dictionary<string, List<GameObject>> teams = new Dictionary<string, List<GameObject>>();

    [SerializeField] int tempPlayerCount;

    MyGameManager gameManager;

    [SyncVar] public bool teamAWon = false;
    [SyncVar] public bool teamBWon = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();
        teams.Add("TeamA", new List<GameObject>());
        teams.Add("TeamB", new List<GameObject>());
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.gameInProgress)
            return;

        CheckIfTeamWon();
    }

    public void UpdateDict(GameObject player)
    {
        teams[$"{player.tag}"].Add(player);

        // foreach (GameObject deadPlayer in teams[$"{player.tag}"])
        //     Debug.Log(deadPlayer);
    }

    void CheckIfTeamWon()
    {
        if (teams["TeamA"].Count == tempPlayerCount)
            teamBWon = true;

        else if (teams["TeamB"].Count == tempPlayerCount)
            teamAWon = true;
    }
}