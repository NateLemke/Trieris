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

        //if(GameManager.main.gameLogic.phaseIndex == 3 && attacker.team.getTeamType() == Team.Type.black) {
        //    ;
        //}
        Animation A = null;
        try {
            A = PhaseManager.actionAnimations[attacker];
        } catch (Exception e) {
            ;
        }
        
        Animation B = null;
        if (PhaseManager.actionAnimations.ContainsKey(target)) {
            B = PhaseManager.actionAnimations[target];
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
            Debug.LogWarning("Rammed without moving?");
        }
               
        if(A == null) {
            Debug.LogError("attacker, "+attacker+" has no animation?");
        }
        
        if(B == null) {
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

        if (target.life == 0) {
            //target.sink();
        } else {
            target.updateFrontAfterCollision();
        }

        if(attacker.life == 0) {
            //attacker.sink();
        } else {
            attacker.updateFrontAfterCollision();
        }

        
        

        resolved = true;
        yield return null;
    }

}
