using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RammingResolution : CombatResolution
{

    public int damageToA;

    public RammingResolution(Ship a,Ship b,int dmgB, int dmgA=0) : base(a,b,dmgB) {
        damageToA += dmgA;
    }

    public override IEnumerator resolve() {

        shipA.setIcon(Sprites.main.AttackIcon);
        shipB.setIcon(Sprites.main.AttackIcon);

        Animation A = null;
        try {
            A = PhaseManager.actionAnimations[shipA];
        } catch (Exception e) {
            Debug.LogError("no action animation for attacker");
        }

        Animation B = null;
        if (PhaseManager.actionAnimations.ContainsKey(shipB)) {
            B = PhaseManager.actionAnimations[shipB];
        }

        if(B != null && B.GetType() == typeof(RotationAnimation)) {
            yield return B.playAnimation();
        }

        try {
            if (A != null) {
                //StartCoroutine(A.playAnimation());
                GameManager.main.StartCoroutine(A.playAnimation());
            }            
        } catch (Exception e) {
            Debug.LogError("Rammed without moving?");
        }

        if (B != null) {
            //StartCoroutine(B.playAnimation());
            GameManager.main.StartCoroutine(B.playAnimation());
        }

        if (A == null) {
            Debug.LogError("attacker, " + shipA + " has no animation?");
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

        shipB.life -= damageToB;
        shipA.life -= damageToA;

        shipB.updateFrontAfterCollision();
        shipA.updateFrontAfterCollision();

        shipB.setSpriteRotation();
        shipA.setSpriteRotation();

        shipB.disableIcon();
        shipA.disableIcon();

        yield return new WaitForSeconds(SpeedManager.CombatDelay);

        resolved = true;
        yield return null;
    }

    public void InitRammingAnimation()
    {
        shipA.GetComponent<Animator>().SetTrigger("Collision");
        shipB.GetComponent<Animator>().SetTrigger("Collision");
    }

}
