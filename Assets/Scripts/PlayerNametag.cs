using UnityEngine;
using Mirror;
using TMPro;

public class PlayerNametag : NetworkBehaviour
{
    string localName;

    MyGameManager gameManager;


    void Start()
    {
        if (!isLocalPlayer)
            return;

        gameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();
    }


    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (!gameManager.gameInProgress)
        {
            localName = GameObject.Find("NetworkManager").GetComponent<MyNetworkManager>().playerName;
            CmdSetPlayerName(localName);
        }
    }


    [Command]
    void CmdSetPlayerName(string localName) => RpcSetPlayerName(localName);


    [ClientRpc]
    void RpcSetPlayerName(string localName) => gameObject.transform.Find("PlayerNameText").GetComponent<TMP_Text>().text = localName;
}