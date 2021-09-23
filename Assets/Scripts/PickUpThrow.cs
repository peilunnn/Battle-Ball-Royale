using UnityEngine;
using Mirror;

public class PickUpThrow : NetworkBehaviour
{
    [SyncVar] [SerializeField] bool isPicker = false;
    [SyncVar] [SerializeField] bool isPickedUp = false;
    [SyncVar] [SerializeField] bool toActivateTeammateRagdoll = false;
    [SyncVar] public bool isLetGo = false;
    [SyncVar] public bool isDead = false;

    [SyncVar] [SerializeField] GameObject teammate;
    PickUpThrow teammateScript;

    Transform destPos;
    LineRenderer lineOfFire;
    float throwForce = 1000;
    Rigidbody rb;

    MyGameManager gameManager;

    void Awake()
    {
        Physics.IgnoreLayerCollision(9, 8, true);
        gameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();
        lineOfFire = GetComponent<LineRenderer>();
        destPos = transform.GetChild(0);
        rb = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!gameManager.gameInProgress || !isLocalPlayer)
            return;

        if (isLetGo)
            CmdDeactivateOwnRagdoll();

        if (isDead)
            return;

        if (Input.GetKeyDown(KeyCode.E) && !isPicker && !isPickedUp)
            CmdSetPickUpStates();

        // if successful pick up, activate teammate ragdoll every frame
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
    }

    [Command]
    void CmdSetPickUpStates()
    {
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 800f))
        {
            // if someone tries to pick up a non-teammate, don't do anything 
            if (hit.collider.tag != gameObject.tag)
                return;

            teammate = hit.collider.gameObject;
            teammateScript = teammate.GetComponent<PickUpThrow>();

            // if teammate is already picking someone else, he is not pickable
            if (teammateScript.isPicker)
                return;

            isPicker = true;
            toActivateTeammateRagdoll = true;
            teammateScript.isPickedUp = true;
        }
    }

    [Command]
    void CmdActivateTeammateRagdoll() => RpcActivateTeammateRagdoll();

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
    void CmdRemoveLineOfFire() => RpcRemoveLineOfFire();

    [TargetRpc]
    void RpcRemoveLineOfFire() => lineOfFire.enabled = false;

    [Command]
    public void CmdDeactivateOwnRagdoll() => RpcDeactivateOwnRagdoll();

    [ClientRpc]
    void RpcDeactivateOwnRagdoll()
    {
        transform.parent = null;
        rb.useGravity = true;
    }

    [Command]
    void CmdThrow() => RpcThrow();

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
    }
}
