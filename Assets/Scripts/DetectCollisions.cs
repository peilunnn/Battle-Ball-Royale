using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DetectCollisions : NetworkBehaviour
{
    PickUpThrow pickUpThrow;
    SphereCollider sphereCollider;
    Vector3 originalCenter = new Vector3(0, 0.5f, -0.03f);
    Vector3 shrunkCenter = new Vector3(-0.03f, 0.9f, -0.03f);
    Vector3 enlargedCenter = new Vector3(0, 0.45f, -0.03f);
    AudioSource impactSound;

    ParticleSystem smoke;
    Animator animator;

    ScoreManager scoreManager;


    // Start is called before the first frame update
    void Awake()
    {
        impactSound = GameObject.Find("Impact").GetComponent<AudioSource>();
        pickUpThrow = gameObject.GetComponent<PickUpThrow>();
        sphereCollider = gameObject.GetComponent<SphereCollider>();
        sphereCollider.center = originalCenter;
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        smoke = GetComponent<ParticleSystem>();
        animator = GetComponent<Animator>();
    }

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
                CmdOnCollisionWithOpponent(opponent);
                pickUpThrow.CmdDeactivateOwnRagdoll();
            }

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
        // MAKE TEAMMATE FALL TO THE FLOOR FIRST, THEN PLAY SMOKE EFFECT AND MAKE HIM GET UP
        sphereCollider.center = shrunkCenter;

        yield return new WaitForSeconds(1);

        smoke.Play();

        yield return new WaitForSeconds(2);

        Debug.Log(pickUpThrow.isDead);

        animator.enabled = true;

        if (pickUpThrow.isDead)
            animator.enabled = false;

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
        smoke.Play();
        animator.enabled = true;

        if (pickUpThrow.isDead)
            animator.enabled = false;

        sphereCollider.center = originalCenter;
        pickUpThrow.isLetGo = false;

        // MAKE OPPONENT PERMANENT RAGDOLL
        opponent.GetComponent<PickUpThrow>().isDead = true;
        opponent.GetComponent<Animator>().enabled = false;
        // opponent.GetComponent<PickUpThrow>().enabled = false;
        opponent.GetComponent<SphereCollider>().center = enlargedCenter;

        // UPDATE SCORE MANAGER DICTIONARY
        scoreManager.UpdateDict(opponent);
    }
}
