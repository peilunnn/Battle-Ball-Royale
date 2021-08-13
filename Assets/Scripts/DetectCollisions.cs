using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DetectCollisions : NetworkBehaviour
{
    [SerializeField] PickUpThrow pickUpThrow;
    SphereCollider sphereCollider;
    Vector3 originalCenter;
    Vector3 shrunkCenter = new Vector3(-0.03f, 0.90f, -0.03f);
    Vector3 enlargedCenter = new Vector3(0, 0.45f, -0.03f);
    AudioSource impactSound;


    // Start is called before the first frame update
    void Start()
    {
        impactSound = GameObject.Find("Impact").GetComponent<AudioSource>();
        pickUpThrow = gameObject.GetComponent<PickUpThrow>();
        sphereCollider = gameObject.GetComponent<SphereCollider>();
        originalCenter = sphereCollider.center;
    }

    // // Update is called once per frame
    // void Update()
    // {
    //     if (pickUpThrow.isLetGo)
    //     {
    //         transform.parent = null;
    //         GetComponent<Rigidbody>().useGravity = true;
    //     }
    // }

    void OnCollisionEnter(Collision other)
    {
        if (!isLocalPlayer)
            return;

        if (pickUpThrow.isLetGo)
        {
            // MAKE COLLIDER BIGGER FOR EASIER TARGETING IN THE AIR
            sphereCollider.center = enlargedCenter;

            // HIT OPPONENT
            if ((gameObject.tag == "TeamA" && other.gameObject.tag == "TeamB") || (gameObject.tag == "TeamB" && other.gameObject.tag == "TeamA"))
            {
                GameObject opponent = other.gameObject;
                Debug.Log($"hit {opponent}");
                CmdOnCollisionWithOpponent(opponent);
            }

            // HIT GROUND OR TEAMMATE
            else
            {
                CmdOnCollisionWithGround();
            }
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
        // MAKE TEAMMATE FALL TO THE FLOOR FIRST, THEN PLAY SMOKE EFFECT AND MAKE HIM GET UP
        sphereCollider.center = shrunkCenter;

        yield return new WaitForSeconds(1);

        GetComponent<ParticleSystem>().Play();

        yield return new WaitForSeconds(2);

        GetComponent<Animator>().enabled = true;
        sphereCollider.center = originalCenter;

        pickUpThrow.isLetGo = false;
    }

    [Command]
    void CmdOnCollisionWithOpponent(GameObject opponent)
    {
        RpcOnCollisionWithOpponent(opponent);
    }

    [ClientRpc]
    void RpcOnCollisionWithOpponent(GameObject opponent)
    {
        // SET OWN STATES
        impactSound.Play();
        GetComponent<ParticleSystem>().Play();
        GetComponent<Animator>().enabled = true;
        sphereCollider.center = originalCenter;
        pickUpThrow.isLetGo = false;

        // MAKE OPPONENT PERMANENT RAGDOLL
        opponent.GetComponent<Animator>().enabled = false;
        opponent.GetComponent<PickUpThrow>().enabled = false;
        opponent.GetComponent<SphereCollider>().center = enlargedCenter;

        // UPDATE SCORE MANAGER DICTIONARY
    }
}
