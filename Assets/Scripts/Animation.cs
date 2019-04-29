using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Animation 
{
    protected Ship ship;
    //public static float ANIMATION_SPEED = 0.7f;
    protected float startTime;
    public bool complete = false;

    public abstract IEnumerator playAnimation(float speed,float delay);
}
