using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose: Abstract class that other animation classes inherit from. Defines what ship
/// is being used, animation start time, when it finishes, and where the camera should focus
/// </summary>
public abstract class Animation 
{
    public Ship ship;
    protected float startTime;
    public bool complete = false;
    public Vector2 focusPoint;

    public abstract IEnumerator playAnimation();
    
}
