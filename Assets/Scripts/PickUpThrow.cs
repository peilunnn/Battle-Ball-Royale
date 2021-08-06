using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;

public class PickUpThrow : NetworkBehaviour
{
    [SerializeField] bool isPicker = false;
    [SerializeField] bool isPickedUp = false;
    [SerializeField] bool activatedOtherPlayerRagdoll = false;
    [SerializeField] bool isLetGo = false;


    Transform destPos;
    public GameObject otherPlayer;
    PickUpThrow otherPlayerScript;
    Rigidbody otherPlayerRb;


    float throwForce = 1000;

    void Awake()
    {
        Physics.IgnoreLayerCollision(9, 8, true);
    }

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

        // IF PLAYER PRESSES E, SET STATES AND PICK UP THE OTHER PLAYER
        if (!isPicker && !isPickedUp && Input.GetKeyDown(KeyCode.E))
            CmdSetPickUpStates();
        if (isPicker && otherPlayerScript.isPickedUp && activatedOtherPlayerRagdoll)
        {
            CmdDrawLine();
            CmdActivateOtherPlayerRagdoll();
        }

        // IF PLAYER PRESSES E AGAIN, PUT THE OTHER PLAYER DOWN
        // OTHERWISE IF PLAYER LEFT CLICKS, THROW THE OTHER PLAYER
        if (isPicker && otherPlayerScript.isPickedUp)
        {
            if (Input.GetKeyDown(KeyCode.E))
                CmdPutDown();
            else if (Input.GetKeyDown(KeyCode.Mouse0))
                CmdThrow();
        }
    }

    [Command]
    void CmdSetPickUpStates()
    {
        RpcPickUpStates();
    }

    [ClientRpc]
    void RpcPickUpStates()
    {
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 500f))
        {
            if (hit.collider.tag != "Player")
                return;

            // RAYCAST HIT THE OTHER PLAYER, SET BOTH PLAYERS' STATES
            isPicker = true;
            activatedOtherPlayerRagdoll = true;
            otherPlayer = hit.collider.gameObject;
            otherPlayerScript = otherPlayer.GetComponent<PickUpThrow>();
            otherPlayerScript.isPickedUp = true;
            otherPlayerScript.isLetGo = false;
        }
    }

    [Command]
    void CmdActivateOtherPlayerRagdoll()
    {
        RpcActivateOtherPlayerRagdoll();
    }

    [ClientRpc]
    void RpcActivateOtherPlayerRagdoll()
    {
        otherPlayer.transform.position = destPos.position;
        otherPlayer.transform.parent = destPos.transform;
        otherPlayer.GetComponent<Animator>().enabled = false;
        GameObject[] otherPlayerRagdollObjects = GameObject.FindGameObjectsWithTag("Ragdoll");
        foreach (GameObject ragdollObj in otherPlayerRagdollObjects)
        {
            Rigidbody ragdollRb = ragdollObj.GetComponent<Rigidbody>();
            ragdollRb.useGravity = false;
        }
        otherPlayerRb = otherPlayer.GetComponent<Rigidbody>();
        otherPlayerRb.useGravity = false;
    }

    [Command]
    void CmdDrawLine()
    {
        RpcDrawLine();
    }

    [ClientRpc]
    void RpcDrawLine()
    {
        Debug.DrawRay(transform.position, otherPlayer.transform.position, Color.green);
        // Debug.Log(transform.position);
        // Debug.Log(otherPlayer.transform.position);
        // Handles.DrawBezier(new Vector3(-0.0f, 0.0f, 0.0f), new Vector3(-2.0f, 2.0f, 0.0f), Vector3.zero, Vector3.zero, Color.red, null, 2f);
        // Handles.DrawBezier(transform.position, Vector3.zero, Vector3.zero, Vector3.zero, Color.red, null, 2f);
    }

    void DeactivateOtherPlayerRagdoll()
    {
        GameObject[] otherPlayerRagdollObjects = GameObject.FindGameObjectsWithTag("Ragdoll");
        foreach (GameObject ragdollObj in otherPlayerRagdollObjects)
        {
            Rigidbody ragdollRb = ragdollObj.GetComponent<Rigidbody>();
            ragdollRb.useGravity = true;
        }
        otherPlayerRb = otherPlayer.GetComponent<Rigidbody>();
        otherPlayerRb.useGravity = true;
    }


    [Command]
    void CmdPutDown()
    {
        RpcPutDown();
    }

    [ClientRpc]
    void RpcPutDown()
    {
        SetPutDownOrThrowStates();
        DeactivateOtherPlayerRagdoll();
    }

    [Command]
    void CmdThrow()
    {
        RpcThrow();
    }

    [ClientRpc]
    void RpcThrow()
    {
        SetPutDownOrThrowStates();
        DeactivateOtherPlayerRagdoll();
        otherPlayerRb.AddForce(this.transform.forward * throwForce);
    }
    void SetPutDownOrThrowStates()
    {
        isPicker = false;
        activatedOtherPlayerRagdoll = false;
        otherPlayerScript.isPickedUp = false;
        otherPlayerScript.isLetGo = true;
        otherPlayer.transform.parent = null;
        otherPlayerRb.useGravity = true;
    }


}
