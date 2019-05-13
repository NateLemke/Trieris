using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public static Sounds main;
    AudioSource source;

    [SerializeField]
    private AudioClip crunch;
    public AudioClip Crunch { get { return crunch; } }

    [SerializeField]
    private AudioClip hiCrack;
    public AudioClip HiCrack { get { return hiCrack; } }



    [SerializeField]
    private List<AudioClip> impacts;
    public List<AudioClip> Impacts { get { return impacts; } }

    [SerializeField]
    private AudioClip sizzle;
    public AudioClip Sizzle { get { return sizzle; } }

    [SerializeField]
    private AudioClip fireball;
    public AudioClip Fireball { get { return fireball; } }

    [SerializeField]
    private AudioClip launch;
    public AudioClip Launch { get { return launch; } }

    [SerializeField]
    private AudioClip blub;
    public AudioClip Blub { get { return blub; } }

    [SerializeField]
    private AudioClip whistle;
    public AudioClip Whistle { get { return whistle; } }

    [SerializeField]
    private AudioClip rip;
    public AudioClip Rip { get { return rip; } }

    [SerializeField]
    private AudioClip longRip;
    public AudioClip LongRip { get { return longRip; } }

    [SerializeField]
    private AudioClip splash;
    public AudioClip Splash { get { return splash; } }

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        source = GetComponent<AudioSource>();
    }

    public void playClip(AudioClip c,float volume = 1f) {
        source.PlayOneShot(c,volume);
    }

    public void playRandomCrunch(float volume = 1f) {
        AudioClip c = Impacts[Random.Range(0,Impacts.Count)];
        source.PlayOneShot(c,volume);
    }
    
}
