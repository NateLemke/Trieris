using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float actionDelay;
    public float actionSpeed;
    public float catapultDelay;

    public float cameraFocusSpeed;

    public float fastFactor;
    public bool fastAnimations;
    public bool skipSubPhase;

    public static TimeManager main;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
