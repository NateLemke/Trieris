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

        //if(GameManager.main.gameLogic.phaseIndex == 3 && attacker.team.getTeamType() == Team.Type.black) {
        //    ;
        //}
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
            if (B != null) {
                //StartCoroutine(B.playAnimation());
                GameManager.main.StartCoroutine(B.playAnimation(0.3f,2f));
            }
        } catch (Exception e) {
            Debug.LogError("Rammed without moving?");
        }

        if (A == null) {
            Debug.LogError("attacker, " + attacker + " has no animation?");
        }

        if (B == null) {
            while (!A.complete) {
                yield return null;
            }
        } else {
            while (!A.complete && !B.complete) {
                if (GameManager.main.gameLogic.phaseIndex == 3 && attacker.team.getTeamType() == Team.Type.black) {
                    ;
                }
                yield return null;
            }
        }

        target.life -= damageToTarget;
        attacker.life -= damageToAttacker;

        target.updateFrontAfterCollision();
        attacker.updateFrontAfterCollision();

        target.setSpriteRotation();
        attacker.setSpriteRotation();


        resolved = true;
        yield return null;
    }

}
