using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using kcp2k;

public class MyNetworkManager : NetworkManager
{
    Dictionary<string, GameObject> teamADict = new Dictionary<string, GameObject>();

    Dictionary<string, GameObject> teamBDict = new Dictionary<string, GameObject>();

    int randomPlayerIndex;
    string allTeams = "AB";
    char randomTeamLetter;

    // Start is called before the first frame update
    public override void Start()
    {
        GameObject[] allPlayerPrefabsList = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        for (int j = 0; j < allPlayerPrefabsList.Length / 2; j++)
            teamADict.Add($"PlayerA{j}", allPlayerPrefabsList[j]);

        for (int k = allPlayerPrefabsList.Length / 2; k < allPlayerPrefabsList.Length; k++)
            teamBDict.Add($"PlayerB{k - allPlayerPrefabsList.Length / 2}", allPlayerPrefabsList[k]);

        // foreach (KeyValuePair<string, GameObject> item in teamBDict)
        //     Debug.Log($"{item.Key}, {item.Value}");

        base.Start();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        randomTeamLetter = allTeams[Random.Range(0, 2)];
        randomPlayerIndex = Random.Range(0, 6);
        Debug.Log($"team {randomTeamLetter}, player {randomPlayerIndex}");
        playerPrefab = randomTeamLetter == 'A' ? teamADict[$"PlayerA{randomPlayerIndex}"] : teamBDict[$"PlayerB{randomPlayerIndex}"];
        Debug.Log(playerPrefab);

        base.OnServerAddPlayer(conn);
    }
}
