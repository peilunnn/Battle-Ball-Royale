using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    GameObject[] allPlayers;
    GameObject[] teamAPrefabs;
    GameObject[] teamBPrefabs;

    int randomNumber;

    // Transform[] teamASpawnPoints;
    // Transform[] teamBSpawnPoints;

    // Start is called before the first frame update
    new void Start()
    {
        // GameObject[] allPlayers = Resources.LoadAll("SpawnablePrefabs", typeof(GameObject)).Cast<GameObject>().ToArray();
        allPlayers = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        for (int i=0; i<allPlayers.Length; i++)
            Debug.Log($"name is {allPlayers[i].name}, type is {allPlayers[i].GetType()}");

        randomNumber = Random.Range(0,5);
    }

    void Update () 
    {
        randomNumber=Random.Range(0,5);
    }

    //  public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    //  {
    //     if (randomNumber == 0) {
    //          Debug.Log("player0");
    //          playerPrefab = (GameObject)GameObject.Instantiate(allPlayers[0], Vector3.zero, Quaternion.identity);
    //          NetworkServer.AddPlayerForConnection(conn, allPlayers[0], playerControllerId);        }
    //     else
    //         Debug.Log("not 0");
    //     // if (randomNumber == 1) {
    //     //      Debug.Log("playerPrefab1");
    //     //      playerPrefab = (GameObject)GameObject.Instantiate(playerPrefab1, Vector3.zero, Quaternion.identity);
    //     //      NetworkServer.AddPlayerForConnection(conn, playerPrefab, playerControllerId);        }
    //     // if (randomNumber == 2) {
    //     //      Debug.Log("playerPrefab2");
    //     //      playerPrefab = (GameObject)GameObject.Instantiate(playerPrefab2, Vector3.zero, Quaternion.identity);
    //     //      NetworkServer.AddPlayerForConnection(conn, playerPrefab, playerControllerId);        }
    //     // if (randomNumber == 3) {
    //     //      Debug.Log("playerPrefab3");
    //     //      playerPrefab = (GameObject)GameObject.Instantiate(playerPrefab3, Vector3.zero, Quaternion.identity);
    //     //      NetworkServer.AddPlayerForConnection(conn, playerPrefab, playerControllerId);        }
    //     // if (randomNumber == 4) {
    //     //      Debug.Log("playerPrefab4");
    //     //      playerPrefab = (GameObject)GameObject.Instantiate(playerPrefab4, Vector3.zero, Quaternion.identity);
    //     //      NetworkServer.AddPlayerForConnection(conn, playerPrefab, playerControllerId);        }
    //     // if (randomNumber == 5) {
    //     //      Debug.Log("playerPrefab4");
    //     //      playerPrefab = (GameObject)GameObject.Instantiate(playerPrefab4, Vector3.zero, Quaternion.identity);
    //     //      NetworkServer.AddPlayerForConnection(conn, playerPrefab, playerControllerId);        }
    //  }
}
