using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;

public class PickUpThrow : NetworkBehaviour
{
    [SyncVar] [SerializeField] bool isPicker = false;
    [SyncVar] [SerializeField] bool isPickedUp = false;
    [SyncVar] [SerializeField] bool toActivateTeammateRagdoll = false;
    [SyncVar] [SerializeField] bool isLetGo = false;


    [SyncVar] [SerializeField] GameObject teammate;
    PickUpThrow teammateScript;

    [SerializeField] SphereCollider sphereCollider;
    Vector3 originalCenter;
    Vector3 shrunkCenter = new Vector3(-0.03f, 0.90f, -0.03f);
    Vector3 enlargedCenter = new Vector3(0, 0.45f, -0.03f);

    [SerializeField] Transform destPos;
    [SerializeField] LineRenderer lineOfFire;
    float throwForce = 1000;
    [SerializeField] AudioSource impactSound;


    void Awake()
    {
        Physics.IgnoreLayerCollision(9, 8, true);
        impactSound = GameObject.Find("Impact").GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        originalCenter = sphereCollider.center;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
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
                CmdDeactivateTeammateRagdoll();
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
            if (hit.collider.tag != gameObject.tag)
                return;

            // RAYCAST HIT THE OTHER PLAYER, SET BOTH PLAYERS' STATES
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

        teammate.transform.parent = null;
        teammate.GetComponent<Rigidbody>().useGravity = true;
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
        isPicker = false;
        toActivateTeammateRagdoll = false;
        teammateScript.isPickedUp = false;
        teammateScript.isLetGo = true;
        // teammate = null;
    }

    void OnCollisionEnter(Collision other)
    {
        if (!isLocalPlayer)
            return;

        if (isLetGo)
        {
            sphereCollider.center = enlargedCenter;

            // HIT OPPONENT
            if (other.gameObject.tag == "Player")
                CmdOnCollisionWithOpponent();

            // HIT GROUND OR TEAMMATE
            else
                CmdOnCollisionWithGround();
        }
    }

    [Command]
    void CmdOnCollisionWithGround()
    {
        RpcOnCollisionWithGround();
    }

    [ClientRpc]
    void RpcOnCollisionWithGround()
    {
        StartCoroutine(OnCollisionWithGround());
    }

    IEnumerator OnCollisionWithGround()
    {
        transform.parent = null;
        GetComponent<Rigidbody>().useGravity = true;

        sphereCollider.center = shrunkCenter;
        yield return new WaitForSeconds(1);
        GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(2);
        GetComponent<Animator>().enabled = true;
        sphereCollider.center = originalCenter;

        isLetGo = false;
    }

    [Command]
    void CmdOnCollisionWithOpponent()
    {
        RpcOnCollisionWithOpponent();
    }

    [ClientRpc]
    void RpcOnCollisionWithOpponent()
    {
        transform.parent = null;
        GetComponent<Rigidbody>().useGravity = true;

        impactSound.Play();

        GetComponent<ParticleSystem>().Play();
        GetComponent<Animator>().enabled = true;
        sphereCollider.center = originalCenter;

        isLetGo = false;
    }
}
