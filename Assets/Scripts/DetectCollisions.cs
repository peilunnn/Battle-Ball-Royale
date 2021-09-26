using System.Collections;
using UnityEngine;
using Mirror;
using UnityEditor;

public class DetectCollisions : NetworkBehaviour
{
    PickUpThrow pickUpThrow;

    ParticleSystem smoke;
    Animator animator;

    SphereCollider sphereCollider;
    Vector3 originalCenter = new Vector3(0, 0.5f, -0.03f);
    Vector3 shrunkCenter = new Vector3(-0.03f, 0.9f, -0.03f);
    Vector3 enlargedCenter = new Vector3(0, 0.45f, -0.03f);


    ScoreManager scoreManager;
    MyGameManager gameManager;
    AudioManager audioManager;
    UIManager UIManager;


    void Awake()
    {
        pickUpThrow = gameObject.GetComponent<PickUpThrow>();

        smoke = GetComponent<ParticleSystem>();
        animator = GetComponent<Animator>();

        sphereCollider = gameObject.GetComponent<SphereCollider>();
        sphereCollider.center = originalCenter;

        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        gameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }


    void OnCollisionEnter(Collision other)
    {
        if (!isLocalPlayer)
            return;

        if (pickUpThrow.isLetGo)
        {
            // enlarge collider to make it easier to hit an opponent
            sphereCollider.center = enlargedCenter;

            // managed to hit opponent
            if ((gameObject.tag == "TeamA" && other.gameObject.tag == "TeamB") || (gameObject.tag == "TeamB" && other.gameObject.tag == "TeamA"))
            {
                GameObject opponent = other.gameObject;
                CmdOnCollisionWithOpponent(opponent);
            }

            else
                CmdOnCollisionWithGround();
        }
    }


    [Command]
    void CmdOnCollisionWithGround() => RpcOnCollisionWithGround();


    [ClientRpc]
    void RpcOnCollisionWithGround() => StartCoroutine(OnCollisionWithGround());


    IEnumerator OnCollisionWithGround()
    {
        pickUpThrow.isLetGo = false;
        sphereCollider.center = shrunkCenter;

        yield return new WaitForSeconds(1);

        smoke.Play();

        yield return new WaitForSeconds(2);

        animator.enabled = true;

        if (pickUpThrow.isDead)
            animator.enabled = false;

        sphereCollider.center = originalCenter;
    }


    [Command]
    void CmdOnCollisionWithOpponent(GameObject opponent) => RpcOnCollisionWithOpponent(opponent);


    [ClientRpc]
    void RpcOnCollisionWithOpponent(GameObject opponent)
    {
        if (opponent.GetComponent<PickUpThrow>().isPickedUp)
            return;

        audioManager.impactSound.Play();
        smoke.Play();
        animator.enabled = true;

        if (pickUpThrow.isDead)
            animator.enabled = false;

        sphereCollider.center = originalCenter;
        pickUpThrow.isLetGo = false;

        opponent.GetComponent<PickUpThrow>().isDead = true;
        opponent.GetComponent<Light>().range = 1;
        opponent.GetComponent<Animator>().enabled = false;
        opponent.GetComponent<SphereCollider>().center = enlargedCenter;

        // add opponent to his teams list
        scoreManager.UpdateDict(opponent);

        if (!isServer)
            return;

        UIManager.RpcUpdatePlayerCountTexts();
        gameManager.CheckIfTeamWon();
    }
}