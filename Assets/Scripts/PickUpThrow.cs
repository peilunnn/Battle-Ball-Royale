using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PickUpThrow : NetworkBehaviour
{
    [SyncVar] public bool isPicker = false;
    [SyncVar] public bool isPickedUp = false;
    [SyncVar] [SerializeField] bool toActivateTeammateRagdoll = false;
    [SyncVar] public bool isLetGo = false;
    [SyncVar] public bool isDead = false;

    [SyncVar] GameObject teammate;
    PickUpThrow targetTeammateScript;
    PickUpThrow pickerScript;

    Transform destPos;
    [SerializeField] RaycastHit[] hits = new RaycastHit[0];
    float throwForce = 1000;
    Vector3 throwDirection;
    int crosshairMaskIndex;
    Rigidbody rb;

    MyGameManager gameManager;

    Image crosshairImage;


    void Awake()
    {
        Physics.IgnoreLayerCollision(9, 8, true);

        destPos = transform.GetChild(0);
        rb = GetComponent<Rigidbody>();

        gameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();

        crosshairImage = GameObject.Find("Crosshair").GetComponent<Image>();
        Cursor.visible = gameManager.isPlaytesting ? false : true;

        crosshairMaskIndex = LayerMask.GetMask("Crosshair");
    }


    // Update is called once per frame
    void Update()
    {
        if (!gameManager.gameInProgress || !isLocalPlayer || isDead)
            return;

        if (Input.GetKeyDown(KeyCode.E) && !isPicker && !isPickedUp)
            CmdSetPickUpStates();
    }


    void FixedUpdate()
    {
        if (!gameManager.gameInProgress || !isLocalPlayer)
            return;

        if (isPickedUp)
        {
            Aim();

            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                crosshairImage.enabled = false;
                CmdDeactivateRagdoll();

                if (Input.GetKeyDown(KeyCode.Mouse0))
                    CmdThrow();
            }
        }

        if (isLetGo)
            CmdDetach();

        // if successful pick up, activate teammate ragdoll every frame
        if (toActivateTeammateRagdoll)
            CmdActivateTeammateRagdoll();
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
        if (Physics.Raycast(ray, out RaycastHit hit, 1000, 1 << crosshairMaskIndex))
        {
            throwDirection = hit.point - transform.position;
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