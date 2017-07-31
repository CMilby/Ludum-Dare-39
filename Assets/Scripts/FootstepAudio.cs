using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepAudio : MonoBehaviour {

    private AudioSource sfx;

    private void Awake()
    {
        sfx = GetComponent<AudioSource>();
    }
    public void step()
    {
        sfx.Play();
        sfx.pitch = Random.Range(0.9f, 1.1f);
    }
}
