using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickUpThrow : NetworkBehaviour
{
    public bool isPicker = false;
    public bool isPickedUp = false;
    Transform destPos;

    // Start is called before the first frame update
    void Start()
    {
        destPos = transform.Find("Destination");
    }

    // Update is called once per frame
    void Update()
    {
        PickUp();
    }

    void PickUp()
    {
        if (!isLocalPlayer || isPickedUp)
            return;

        // WHEN PLAYER PRESSES E, SEND A RAYCAST OUT
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(this.transform.position, this.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                // RAYCAST HIT THE OTHER PLAYER, SET BOTH PLAYERS' STATES
                isPicker = true;
                GameObject otherPlayer = hit.transform.gameObject;
                PickUpThrow otherPlayerScript = otherPlayer.GetComponent<PickUpThrow>();  
                otherPlayerScript.isPickedUp = true;
                otherPlayerScript.isPicker = false;

                // SUSPEND OTHER PLAYER
                Rigidbody otherPlayerRb = otherPlayer.GetComponent<Rigidbody>();
                otherPlayerRb.useGravity = false;
                otherPlayer.transform.position = destPos.position;
                otherPlayer.transform.parent = destPos.transform;
            }
        }
    }
}
