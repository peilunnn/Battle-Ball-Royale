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

        if (isPickedUp)
        {
            AimRotation();

            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                    CmdThrow();

                CmdDeactivateRagdoll();
            }
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


    void AimRotation()
    {
        crosshairImage.enabled = true;
        Vector3 mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
    }

    [Command]
    void CmdThrow() => RpcThrow();

    [ClientRpc]
    void RpcThrow() => rb.AddForce(transform.forward * throwForce);

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
