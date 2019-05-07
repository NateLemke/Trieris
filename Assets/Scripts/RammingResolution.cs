using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RammingResolution : CombatResolution
{

    protected int damageToAttacker;

    public RammingResolution(Ship a,Ship t,int dmgT,int dmgA) : base(a,t,dmgT) {
        damageToAttacker = dmgA;
    }

    public override IEnumerator resolve() {

        attacker.setIcon(Sprites.main.AttackIcon);
        target.setIcon(Sprites.main.AttackIcon);

        Animation A = null;
        try {
            A = PhaseManager.actionAnimations[attacker];
        } catch (Exception e) {
            Debug.LogError("no action animation for attacker");
        }

        Animation B = null;
        if (PhaseManager.actionAnimations.ContainsKey(target)) {
            B = PhaseManager.actionAnimations[target];
        }

        if(B != null && B.GetType() == typeof(RotationAnimation)) {
            yield return B.playAnimation(0.3f,2f);
        }

        try {
            if (A != null) {
                //StartCoroutine(A.playAnimation());
                GameManager.main.StartCoroutine(A.playAnimation(0.3f,2f));
            }            
        } catch (Exception e) {
            Debug.LogError("Rammed without moving?");
        }

        if (B != null) {
            //StartCoroutine(B.playAnimation());
            GameManager.main.StartCoroutine(B.playAnimation(0.3f,2f));
        }

        if (A == null) {
            Debug.LogError("attacker, " + attacker + " has no animation?");
        }

        if (B == null) {
            while (!A.complete) {
                yield return null;
            }
        } else {
            while (!A.complete || !B.complete) {
                yield return null;
            }
        }

        target.life -= damageToTarget;
        attacker.life -= damageToAttacker;

        target.updateFrontAfterCollision();
        attacker.updateFrontAfterCollision();

        target.setSpriteRotation();
        attacker.setSpriteRotation();

        target.disableIcon();
        attacker.disableIcon();

        yield return new WaitForSeconds(0.4f);

        resolved = true;
        yield return null;
    }

}
