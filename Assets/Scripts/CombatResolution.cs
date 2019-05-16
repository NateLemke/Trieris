using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose: Abstract class that other combat resolutions inherit from. Defines the ships involved in the
/// combat as well as the location it is occuring and damage dealt.
/// </summary>
public abstract class CombatResolution {

    public Ship shipA;
    public Ship shipB;

    public int damageToB;

    protected bool resolved;

    Vector2 focusPoint;

    public CombatResolution(Ship a, Ship b, int dmgB) {
        shipA = a;
        shipB = b;
        damageToB = dmgB;
    }

    public abstract IEnumerator resolve();

}
