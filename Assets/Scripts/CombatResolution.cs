using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatResolution {

    public Ship attacker;
    public Ship target;

    int damageToTarget;
    int damageToAttacker;

    bool resolved = false;

    public CombatResolution(Ship a, Ship t, int dmgT, int dmgA) {
        attacker = a;
        target = t;
        damageToTarget = dmgT;
        damageToAttacker = dmgA;
    }

    public IEnumerator resolve() {

        Animation A = AnimationManager.actionAnimations[attacker];
        Animation B = null;
        if (AnimationManager.actionAnimations.ContainsKey(target)) {
            B = AnimationManager.actionAnimations[target];
        }

        try {
            if (A != null) {
                //StartCoroutine(A.playAnimation());
                GameManager.main.StartCoroutine(A.playAnimation(1f));
            }
            if (B != null) {
                //StartCoroutine(B.playAnimation());
                GameManager.main.StartCoroutine(B.playAnimation(1f));

            }
        } catch (Exception e) {
            ;
        }
        
        if(B == null) {
            while (!A.complete) {
                yield return null;
            }
        } else {
            while (!A.complete && !B.complete) {
                yield return null;
            }
        }



        
        target.life -= damageToTarget;
        attacker.life -= damageToAttacker;
        
        // deal damage or something ...

        resolved = true;
        yield return null;
    }

}
