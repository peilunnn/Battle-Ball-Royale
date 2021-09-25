using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource footstep;
    public AudioSource startGameSound;
    public AudioSource impactSound;

    // Start is called before the first frame update
    void Start()
    {
        startGameSound = transform.Find("StartGameSound").GetComponent<AudioSource>();
        footstep = transform.Find("Footstep").GetComponent<AudioSource>();
        impactSound = transform.Find("Impact").GetComponent<AudioSource>();
    }
}
