using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatResolution {

    public Ship shipA;
    public Ship shipB;

    public int damageToB;

    protected bool resolved;

    public CombatResolution(Ship a, Ship b, int dmgB) {
        shipA = a;
        shipB = b;
        damageToB = dmgB;
    }

    public abstract IEnumerator resolve();

}
