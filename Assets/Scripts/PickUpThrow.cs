using UnityEngine;
using Mirror;

public class PickUpThrow : NetworkBehaviour
{
    [SyncVar] [SerializeField] bool isPicker = false;
    [SyncVar] [SerializeField] bool isPickedUp = false;
    [SyncVar] [SerializeField] bool toActivateTeammateRagdoll = false;
    [SyncVar] public bool isLetGo = false;

    [SyncVar] [SerializeField] GameObject teammate;
    PickUpThrow teammateScript;

    Transform destPos;
    LineRenderer lineOfFire;
    float throwForce = 1000;

    MyGameManager gameManager;

    void Awake()
    {
        Physics.IgnoreLayerCollision(9, 8, true);
        gameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();
        lineOfFire = GetComponent<LineRenderer>();
        destPos = transform.GetChild(0);
    }


    // Update is called once per frame
    void Update()
    {
        if (!gameManager.gameInProgress || !isLocalPlayer)
            return;

        // IF PLAYER PRESSES E, SET STATES
        if (!isPicker && !isPickedUp && Input.GetKeyDown(KeyCode.E))
            CmdSetPickUpStates();

        // IF SUCCESSFUL PICK UP, ACTIVATE TEAMMATE RAGDOLL EVERY FRAME 
        if (toActivateTeammateRagdoll)
        {
            DrawLineOfFire();
            CmdActivateTeammateRagdoll();
        }

        // IF PLAYER RIGHT CLICKS, PUT TEAMMATE DOWN
        // OTHERWISE IF PLAYER LEFT CLICKS, THROW TEAMMATE
        if (isPicker)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                CmdRemoveLineOfFire();
                CmdSetPutDownOrThrowStates();

                if (Input.GetKeyDown(KeyCode.Mouse0))
                    CmdThrow();
            }
        }

        if (isLetGo)
            CmdDeactivateOwnRagdoll();
    }

    [Command]
    void CmdSetPickUpStates()
    {
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 800f))
        {
            // IF I TRY TO PICK UP SOMEONE WHO IS NOT MY TEAMMATE, DONT DO ANYTHING 
            if (hit.collider.tag != gameObject.tag)
                return;

            // RAYCAST HIT TEAMMATE, SET BOTH PLAYERS' STATES
            isPicker = true;
            toActivateTeammateRagdoll = true;
            teammate = hit.collider.gameObject;
            teammateScript = teammate.GetComponent<PickUpThrow>();
            teammateScript.isPickedUp = true;
        }
    }

    [Command]
    void CmdActivateTeammateRagdoll()
    {
        RpcActivateTeammateRagdoll();
    }

    [ClientRpc]
    void RpcActivateTeammateRagdoll()
    {
        if (!teammate)
            return;

        teammate.transform.position = destPos.position;
        teammate.transform.parent = destPos.transform;
        teammate.GetComponent<Animator>().enabled = false;
        teammate.GetComponent<Rigidbody>().useGravity = false;
    }


    void DrawLineOfFire()
    {
        lineOfFire.enabled = true;
        lineOfFire.SetPosition(0, teammate.transform.position + new Vector3(0, 0.5f, 0));
        lineOfFire.SetPosition(1, transform.forward * 10 + transform.position);
    }


    [Command]
    void CmdRemoveLineOfFire()
    {
        RpcRemoveLineOfFire();
    }

    [TargetRpc]
    void RpcRemoveLineOfFire()
    {
        lineOfFire.enabled = false;
    }

    [Command]
    void CmdDeactivateOwnRagdoll()
    {
        RpcDeactivateOwnRagdoll();
    }

    [ClientRpc]
    void RpcDeactivateOwnRagdoll()
    {
        transform.parent = null;
        GetComponent<Rigidbody>().useGravity = true;
    }

    [Command]
    void CmdThrow()
    {
        RpcThrow();
    }

    [ClientRpc]
    void RpcThrow()
    {
        if (!teammate)
            return;

        teammate.GetComponent<Rigidbody>().AddForce(this.transform.forward * throwForce);
    }

    [Command]
    void CmdSetPutDownOrThrowStates()
    {
        teammateScript.isLetGo = true;
        teammateScript.isPickedUp = false;
        isPicker = false;
        toActivateTeammateRagdoll = false;
        // teammate = null;
    }
}
