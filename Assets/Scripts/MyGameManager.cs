using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    GameObject playerPrefab;
    public List<GameObject> allPlayersBeforeGame = new List<GameObject>();
    public Dictionary<string, GameObject> teamADict = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> teamBDict = new Dictionary<string, GameObject>();
    public string allTeamsString = "AB";
    public char randomTeamLetter;
    public int randomPlayerIndex;
    public bool gameInProgress = false;


    // Start is called before the first frame update
    void Start()
    {
        for (int j = 0; j < allPlayersBeforeGame.Count / 2; j++)
            teamADict.Add($"PlayerA{j}", allPlayersBeforeGame[j]);

        for (int k = allPlayersBeforeGame.Count / 2; k < allPlayersBeforeGame.Count; k++)
            teamBDict.Add($"PlayerB{k - allPlayersBeforeGame.Count / 2}", allPlayersBeforeGame[k]);

        // foreach (KeyValuePair<string, GameObject> item in teamBDict)
        //     Debug.Log($"{item.Key}, {item.Value}");
    }

    void Update()
    {
        if (allPlayersBeforeGame.Count < 11)
        {
            Debug.Log("start game");
            gameInProgress = true;
        }
    }

    public GameObject GetRandomPlayer()
    {
        // GENERATE RANDOM TEAM LETTER AND INDEX
        randomTeamLetter = allTeamsString[Random.Range(0, 2)];
        randomPlayerIndex = Random.Range(0, 6);

        // SET RANDOM PLAYER PREFAB AND SET HIS TAG
        if (randomTeamLetter == 'A')
        {
            playerPrefab = teamADict[$"PlayerA{randomPlayerIndex}"];
            playerPrefab.tag = "TeamA";
        }

        else
        {
            playerPrefab = teamBDict[$"PlayerB{randomPlayerIndex}"];
            playerPrefab.tag = "TeamB";
        }

        return playerPrefab;
    }
}
