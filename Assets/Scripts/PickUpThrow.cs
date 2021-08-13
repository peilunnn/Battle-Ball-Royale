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

    [SerializeField] Transform destPos;
    [SerializeField] LineRenderer lineOfFire;
    float throwForce = 1000;

    MyGameManager gameManager;

    void Awake()
    {
        Physics.IgnoreLayerCollision(9, 8, true);
        gameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();
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
                // CmdDeactivateTeammateRagdoll();

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

    [ClientRpc]
    void RpcRemoveLineOfFire()
    {
        lineOfFire.enabled = false;
    }

    [Command]
    void CmdDeactivateTeammateRagdoll()
    {
        RpcDeactivateTeammateRagdoll();
    }

    [ClientRpc]
    void RpcDeactivateTeammateRagdoll()
    {
        if (!teammate)
            return;

        Debug.Log("in deactivate teammate ragdoll");

        teammate.transform.parent = null;
        teammate.GetComponent<Rigidbody>().useGravity = true;
    }

    [Command]
    void CmdDeactivateOwnRagdoll()
    {
        RpcDeactivateOwnRagdoll();
    }

    [ClientRpc]
    void RpcDeactivateOwnRagdoll()
    {
        Debug.Log("in deactivate own ragdoll");
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

    // void OnCollisionEnter(Collision other)
    // {
    //     if (!isLocalPlayer)
    //         return;

    //     if (isLetGo)
    //     {
    //         // MAKE COLLIDER BIGGER FOR EASIER TARGETING IN THE AIR
    //         sphereCollider.center = enlargedCenter;

    //         // SET STATES
    //         transform.parent = null;
    //         GetComponent<Rigidbody>().useGravity = true;

    //         // HIT OPPONENT
    //         if ((gameObject.tag == "TeamA" && other.gameObject.tag == "TeamB") || (gameObject.tag == "TeamB" && other.gameObject.tag == "TeamA"))
    //         {
    //             GameObject opponent = other.gameObject;
    //             CmdOnCollisionWithOpponent(opponent);
    //         }

    //         // HIT GROUND OR TEAMMATE
    //         else
    //             CmdOnCollisionWithGround();
    //     }
    // }

    // [Command]
    // void CmdOnCollisionWithGround()
    // {
    //     RpcOnCollisionWithGround();
    // }

    // [ClientRpc]
    // void RpcOnCollisionWithGround()
    // {
    //     StartCoroutine(OnCollisionWithGround());
    // }

    // IEnumerator OnCollisionWithGround()
    // {
    //     // MAKE TEAMMATE FALL TO THE FLOOR FIRST, THEN PLAY SMOKE EFFECT AND MAKE HIM GET UP
    //     sphereCollider.center = shrunkCenter;

    //     yield return new WaitForSeconds(1);

    //     GetComponent<ParticleSystem>().Play();

    //     yield return new WaitForSeconds(2);

    //     GetComponent<Animator>().enabled = true;
    //     sphereCollider.center = originalCenter;

    //     isLetGo = false;
    // }

    // [Command]
    // void CmdOnCollisionWithOpponent(GameObject opponent)
    // {
    //     RpcOnCollisionWithOpponent(opponent);
    // }

    // [ClientRpc]
    // void RpcOnCollisionWithOpponent(GameObject opponent)
    // {
    //     // SET OWN STATES
    //     impactSound.Play();
    //     GetComponent<ParticleSystem>().Play();
    //     GetComponent<Animator>().enabled = true;
    //     sphereCollider.center = originalCenter;
    //     isLetGo = false;

    //     // MAKE OPPONENT PERMANENT RAGDOLL
    //     opponent.GetComponent<Animator>().enabled = false;
    //     opponent.GetComponent<PickUpThrow>().enabled = false;
    //     opponent.GetComponent<SphereCollider>().center = enlargedCenter;

    //     // UPDATE DICTIONARY
    // }
}
