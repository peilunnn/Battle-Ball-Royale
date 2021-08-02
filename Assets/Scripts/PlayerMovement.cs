using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    float moveSpeed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        float xPos = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + xPos, transform.position.y, transform.position.z), moveSpeed);

        float zPos = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z + zPos), moveSpeed);
    }
}