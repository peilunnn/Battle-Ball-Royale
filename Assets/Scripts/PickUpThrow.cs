using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickUpThrow : NetworkBehaviour
{
    [SerializeField] bool isPicker = false;
    [SerializeField] bool isPickedUp = false;
    Transform destPos;

    // Start is called before the first frame update
    void Start()
    {
        destPos = transform.Find("Destination");
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer && !isPickedUp && Input.GetKeyDown(KeyCode.E))
            CmdSendPickUp();
    }

    [Command]
    void CmdSendPickUp()
    {
        RpcUpdatePickUp();
    }

    [ClientRpc]
    void RpcUpdatePickUp()
    {
        Debug.Log("in rpc");
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        Debug.Log(ray);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Debug.Log("hit other player");
            // RAYCAST HIT THE OTHER PLAYER, SET BOTH PLAYERS' STATES
            isPicker = true;
            GameObject otherPlayer = hit.collider.gameObject;
            PickUpThrow otherPlayerScript = otherPlayer.GetComponent<PickUpThrow>();
            otherPlayerScript.isPickedUp = true;
            otherPlayerScript.isPicker = false;

            // SUSPEND OTHER PLAYER
            Rigidbody otherPlayerRb = otherPlayer.GetComponent<Rigidbody>();
            otherPlayerRb.useGravity = false;
            otherPlayerRb.isKinematic = true;
            otherPlayer.transform.position = destPos.position;
            otherPlayer.transform.parent = destPos.transform;
        }
    }
}
