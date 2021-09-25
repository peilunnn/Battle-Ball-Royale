using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PickUpThrow : NetworkBehaviour
{
    [SyncVar] public bool isPicker = false;
    [SyncVar] [SerializeField] bool isPickedUp = false;
    [SyncVar] [SerializeField] bool toActivateTeammateRagdoll = false;
    [SyncVar] public bool isLetGo = false;
    [SyncVar] public bool isDead = false;

    [SyncVar] GameObject teammate;
    PickUpThrow targetTeammateScript;
    PickUpThrow pickerScript;

    Transform destPos;
    float throwForce = 1000;
    Vector3 throwDirection;
    Rigidbody rb;

    MyGameManager gameManager;

    Image crosshairImage;


    void Start()
    {
        Physics.IgnoreLayerCollision(9, 8, true);

        destPos = transform.GetChild(0);
        rb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();
        crosshairImage = GameObject.Find("Crosshair").GetComponent<Image>();

        Cursor.visible = gameManager.isPlaytesting ? false : true;
    }


    // Update is called once per frame
    void Update()
    {
        if (!gameManager.gameInProgress || !isLocalPlayer || isDead)
            return;

        if (isLetGo)
            CmdDetach();

        if (Input.GetKeyDown(KeyCode.E) && !isPicker && !isPickedUp)
            CmdSetPickUpStates();

        // if successful pick up, activate teammate ragdoll every frame
        if (toActivateTeammateRagdoll)
            CmdActivateTeammateRagdoll();
    }

    void FixedUpdate()
    {
        if (!gameManager.gameInProgress || !isLocalPlayer || isDead)
            return;

        if (isPickedUp)
        {
            Aim();

            if (!(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1)))
                return;

            if (Input.GetKeyDown(KeyCode.Mouse0))
                CmdThrow();

            CmdDeactivateRagdoll();
            crosshairImage.enabled = false;
        }
    }

    [Command]
    void CmdSetPickUpStates()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 800f))
        {
            // if someone tries to pick up a non-teammate, don't do anything 
            if (hit.collider.tag != gameObject.tag)
                return;

            teammate = hit.collider.gameObject;
            targetTeammateScript = teammate.GetComponent<PickUpThrow>();

            // if teammate is already picking someone else, he is not pickable
            if (targetTeammateScript.isPicker)
                return;

            isPicker = true;
            toActivateTeammateRagdoll = true;
            targetTeammateScript.isPickedUp = true;
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

    void Aim()
    {
        crosshairImage.enabled = true;
        crosshairImage.transform.position = Input.mousePosition;
    }

    [Command]
    void CmdThrow() => RpcThrow();

    [ClientRpc]
    void RpcThrow()
    {
        Ray ray = Camera.main.ScreenPointToRay(crosshairImage.transform.position);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 200))
        {
            Debug.Log(raycastHit.transform.gameObject);
            throwDirection = raycastHit.point - transform.position;
            throwDirection += new Vector3(0, 2, 0);
            rb.AddForce(throwDirection.normalized * throwForce);
        }
    }

    [Command]
    public void CmdDeactivateRagdoll() => RpcDeactivateRagdoll();

    [ClientRpc]
    void RpcDeactivateRagdoll()
    {
        isLetGo = true;
        isPickedUp = false;
        isPicker = false;

        pickerScript = transform.parent.parent.gameObject.GetComponent<PickUpThrow>();
        pickerScript.isPicker = false;
        pickerScript.toActivateTeammateRagdoll = false;
    }

    [Command]
    void CmdDetach() => RpcDetach();

    [ClientRpc]
    void RpcDetach()
    {
        transform.parent = null;
        rb.useGravity = true;
    }
}