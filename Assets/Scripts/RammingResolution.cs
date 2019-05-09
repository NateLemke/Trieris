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
            yield return B.playAnimation(SpeedManager.CombatDelay,SpeedManager.ActionSpeed);
        }

        try {
            if (A != null) {
                //StartCoroutine(A.playAnimation());
                GameManager.main.StartCoroutine(A.playAnimation(SpeedManager.CombatDelay,SpeedManager.ActionSpeed));
            }            
        } catch (Exception e) {
            Debug.LogError("Rammed without moving?");
        }

        if (B != null) {
            //StartCoroutine(B.playAnimation());
            GameManager.main.StartCoroutine(B.playAnimation(SpeedManager.CombatDelay,SpeedManager.ActionSpeed));
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

        InitRammingAnimation();

        target.life -= damageToTarget;
        attacker.life -= damageToAttacker;

        target.updateFrontAfterCollision();
        attacker.updateFrontAfterCollision();

        target.setSpriteRotation();
        attacker.setSpriteRotation();

        target.disableIcon();
        attacker.disableIcon();

        yield return new WaitForSeconds(SpeedManager.CombatDelay);

        resolved = true;
        yield return null;
    }

    public void InitRammingAnimation()
    {
        attacker.GetComponent<Animator>().SetTrigger("Collision");
    }

}
