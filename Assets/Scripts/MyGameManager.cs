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

    [SerializeField] Text startGameText;
    [SerializeField] Text countdownText3;
    [SerializeField] Text countdownText2;
    [SerializeField] Text countdownText1;


    void Update()
    {
        Debug.Log($"{availablePlayers.Count}, {temp}");
        // IF NO. OF PLAYERS LEFT TO JOIN IS LESS THAN 10 IE. 3 PLAYERS HAVE JOINED, START THE GAME 
        if (!gameInProgress && availablePlayers.Count < temp)
        {
            RpcSetGameInProgress();
            RpcSetStartGameUI();
        }
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

        yield return new WaitForSeconds(4);

        startGameText.enabled = false;
        countdownText3.enabled = true;

        yield return new WaitForSeconds(2);

        countdownText3.enabled = false;
        countdownText2.enabled = true;

        yield return new WaitForSeconds(2);

        countdownText2.enabled = false;
        countdownText1.enabled = true;

        yield return new WaitForSeconds(2);

        countdownText1.enabled = false;
    }

    public GameObject GetRandomPlayer()
    {
        return availablePlayers[Random.Range(0, availablePlayers.Count)];
    }
}
