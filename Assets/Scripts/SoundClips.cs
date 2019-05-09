using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sound class to attach sound objects and play them
/// </summary>
public class SoundClips : MonoBehaviour
{
    public AudioClip soundClip;

    public AudioSource soundSource;

    // Start is called before the first frame update
    void Start()
    {
        soundSource.clip = soundClip;
        soundSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
