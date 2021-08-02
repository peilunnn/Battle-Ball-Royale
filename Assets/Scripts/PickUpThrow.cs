using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickUpThrow : NetworkBehaviour
{
    [SerializeField] bool isPicker = false;
    [SerializeField] bool isPickedUp = false;
    Transform destPos;
    GameObject otherPlayer;
    Rigidbody otherPlayerRb;
    float throwForce = 1000;


    // Start is called before the first frame update
    void Start()
    {
        destPos = transform.Find("Destination");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        // IF PLAYER PRESSES E, PICK UP THE OTHER PLAYER
        if (!isPickedUp && Input.GetKeyDown(KeyCode.E))
            CmdPickUp();

        // IF PLAYER PRESSES E AGAIN, PUT THE OTHER PLAYER DOWN
        if (isPicker && Input.GetKeyDown(KeyCode.E))
            CmdPutDown();

        // IF PLAYER LEFT CLICKS, THROW THE OTHER PLAYER
        if (isPicker && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("throwing");
            CmdThrow();
        }
    }

    [Command]
    void CmdPickUp()
    {
        RpcUpdatePickUp();
    }

    [ClientRpc]
    void RpcUpdatePickUp()
    {
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // RAYCAST HIT THE OTHER PLAYER, SET BOTH PLAYERS' STATES
            isPicker = true;
            otherPlayer = hit.collider.gameObject;
            PickUpThrow otherPlayerScript = otherPlayer.GetComponent<PickUpThrow>();
            otherPlayerScript.isPickedUp = true;

            // SUSPEND OTHER PLAYER
            otherPlayerRb = otherPlayer.GetComponent<Rigidbody>();
            otherPlayerRb.useGravity = false;
            otherPlayerRb.isKinematic = true;
            otherPlayer.transform.position = destPos.position;
            otherPlayer.transform.parent = destPos.transform;
        }
    }

    [Command]
    void CmdPutDown()
    {
        RpcUpdatePutDown();
    }

    [ClientRpc]
    void RpcUpdatePutDown()
    {
        isPicker = false;
        otherPlayerRb.useGravity = true;
        otherPlayerRb.isKinematic = false;
        otherPlayer.transform.parent = destPos.transform;
    }

    [Command]
    void CmdThrow()
    {
        RpcUpdateThrow();
    }

    [ClientRpc]
    void RpcUpdateThrow()
    {
        isPicker = false;
        otherPlayerRb.useGravity = true;
        otherPlayerRb.isKinematic = false;
        otherPlayerRb.AddForce(this.transform.forward * throwForce);
    }
}
