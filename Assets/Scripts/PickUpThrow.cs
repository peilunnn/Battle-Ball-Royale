using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;

public class PickUpThrow : NetworkBehaviour
{
    [SerializeField] bool isPicker = false;
    [SerializeField] bool isPickedUp = false;
    [SerializeField] bool toActivateTeammateRagdoll = false;
    [SerializeField] bool isLetGo = false;


    Transform destPos;
    [SerializeField] GameObject teammate;
    PickUpThrow teammateScript;
    Rigidbody teammateRb;

    SphereCollider sphereCollider;
    Vector3 originalCenter;
    float throwForce = 1000;

    void Awake()
    {
        Physics.IgnoreLayerCollision(9, 8, true);
    }

    // Start is called before the first frame update
    void Start()
    {
        destPos = transform.Find("Destination");
        sphereCollider = GetComponent<SphereCollider>();
        originalCenter = sphereCollider.center;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        // IF PLAYER PRESSES E, SET STATES AND PICK UP THE OTHER PLAYER
        if (!isPicker && !isPickedUp && Input.GetKeyDown(KeyCode.E))
            CmdSetPickUpStates();

        if (toActivateTeammateRagdoll)
        {
            // CmdDrawLine();
            CmdActivateTeammateRagdoll();
        }

        // IF PLAYER RIGHT CLICKS, PUT THE OTHER PLAYER DOWN
        // OTHERWISE IF PLAYER LEFT CLICKS, THROW THE OTHER PLAYER
        if (isPicker && teammateScript.isPickedUp)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
                CmdPutDown();
            else if (Input.GetKeyDown(KeyCode.Mouse0))
                CmdThrow();
        }
    }

    [Command]
    void CmdSetPickUpStates()
    {
        RpcSetPickUpStates();
    }

    [ClientRpc]
    void RpcSetPickUpStates()
    {
        // SEND RAY
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 800f))
        {
            if (hit.collider.tag != "Player")
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
        {
            Debug.Log("2OVER HEREEEEEEEEEEEEEE");
            return;
        }
        teammate.transform.position = destPos.position;
        teammate.transform.parent = destPos.transform;
        teammate.GetComponent<Animator>().enabled = false;
        GameObject[] teammateRagdollObjects = GameObject.FindGameObjectsWithTag("Ragdoll");
        foreach (GameObject ragdollObj in teammateRagdollObjects)
        {
            Rigidbody ragdollRb = ragdollObj.GetComponent<Rigidbody>();
            ragdollRb.useGravity = false;
        }
        teammateRb = teammate.GetComponent<Rigidbody>();
        teammateRb.useGravity = false;
    }

    [Command]
    void CmdDrawLine()
    {
        RpcDrawLine();
    }

    [ClientRpc]
    void RpcDrawLine()
    {
        if (!teammate)
            Debug.Log("3OVER HEREEEEEEEEEEEEEE");
        Debug.DrawRay(transform.position, teammate.transform.position, Color.green);
        // Debug.Log(transform.position);
        // Debug.Log(teammate.transform.position);
        // Handles.DrawBezier(new Vector3(-0.0f, 0.0f, 0.0f), new Vector3(-2.0f, 2.0f, 0.0f), Vector3.zero, Vector3.zero, Color.red, null, 2f);
        // Handles.DrawBezier(transform.position, Vector3.zero, Vector3.zero, Vector3.zero, Color.red, null, 2f);
    }

    void DeactivateTeammateRagdoll()
    {
        if (!teammate)
            Debug.Log("4OVER HEREEEEEEEEEEEEEE");
        GameObject[] teammateRagdollObjects = GameObject.FindGameObjectsWithTag("Ragdoll");
        foreach (GameObject ragdollObj in teammateRagdollObjects)
        {
            Rigidbody ragdollRb = ragdollObj.GetComponent<Rigidbody>();
            ragdollRb.useGravity = true;
        }
        teammateRb = teammate.GetComponent<Rigidbody>();
        teammateRb.useGravity = true;
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
        DeactivateTeammateRagdoll();
        teammate = null;
    }

    [Command]
    void CmdThrow()
    {
        RpcThrow();
    }

    [ClientRpc]
    void RpcThrow()
    {
        DeactivateTeammateRagdoll();
        SetPutDownOrThrowStates();
        teammateRb.AddForce(this.transform.forward * throwForce);
        teammate = null;
    }

    void SetPutDownOrThrowStates()
    {
        if (!teammate)
            Debug.Log("5OVER HEREEEEEEEEEEEEEE");
        isPicker = false;
        toActivateTeammateRagdoll = false;
        teammateScript.isPickedUp = false;
        teammateScript.isLetGo = true;
        teammate.transform.parent = null;
        teammateRb.useGravity = true;
    }

    void OnCollisionEnter(Collision other)
    {
        if (!isLocalPlayer)
            return;

        if (isLetGo && other.gameObject.tag == "Ground")
            CmdOnCollisionWithGround();
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
        sphereCollider.center = new Vector3(-0.03f, 0.90f, -0.03f);
        yield return new WaitForSeconds(1);
        GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(2);
        GetComponent<Animator>().enabled = true;
        sphereCollider.center = originalCenter;
        isLetGo = false;
    }
}
