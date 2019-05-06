using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatResolution {

    public Ship attacker;
    public Ship target;

    protected int damageToTarget;

    protected bool resolved;

    public CombatResolution(Ship a, Ship t, int dmgT) {
        attacker = a;
        target = t;
        damageToTarget = dmgT;
    }

    public abstract IEnumerator resolve();

}
