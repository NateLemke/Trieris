using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Animation 
{
    public Ship ship;
    protected float startTime;
    public bool complete = false;
    public Vector2 focusPoint;

    public abstract IEnumerator playAnimation();
    
}
