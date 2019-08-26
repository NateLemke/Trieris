using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class represents an instance of ramming combat between two ships
/// It is used to play an animation and resolve the combat damage
/// </summary>
public class RammingResolution : CombatResolution
{
    // both shipA and shipB may take damage, so an extra damage variable is added to this class
    public int damageToA;

    public RammingResolution(Ship a,Ship b,int dmgB, int dmgA=0) : base(a,b,dmgB) {
        damageToA += dmgA;
    }

    /// <summary>
    /// Plays the action animations and deals damage to the ships involved
    /// Also enables combat icons and focus the camera onto the animation
    /// </summary>
    /// <returns></returns>
    public override IEnumerator resolve() {

        if(shipA == null || shipA.gameObject == null) {
            yield break;
        }

        if (shipB == null || shipB.gameObject == null) {
            yield break;
        }

        yield return PhaseManager.SyncFocus(shipA.getNode().getRealPos());

        shipA.SetIconAttack();
        shipB.SetIconAttack();

        Animation A = null;
        try {
            A = PhaseManager.actionAnimations[shipA];
        } catch (Exception e) {
            Debug.LogWarning("no action animation for attacker");
            shipA.DisableIcon();
            shipB.DisableIcon();

            yield break;
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
            Debug.LogWarning("attacker, " + shipA + " has no animation?");
            yield break;
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
        Sounds.main.playRandomCrunch();
        //GameManager.main.PlayRandomCrunch();
        if (damageToA > 0) {
            yield return new WaitForSeconds(0.3f);
            Sounds.main.playRandomCrunch();
            //GameManager.main.PlayRandomCrunch();
        }

        shipA.TakeDamage(damageToA);
        shipB.TakeDamage(damageToB);

        //shipB.life -= damageToB;
        //shipA.life -= damageToA;

        shipB.CanFire = false;
        shipA.CanFire = false;

        shipB.updateFrontAfterCollision();
        shipA.updateFrontAfterCollision();

        shipB.setSpriteRotation();
        shipA.setSpriteRotation();

        shipB.DisableIcon();
        shipA.DisableIcon();
        if(shipA.life == 0 || shipB.life == 0) {
            yield return new WaitForSeconds(SpeedManager.CombatPostDelay * 2);

        } else {
            yield return new WaitForSeconds(SpeedManager.CombatPostDelay);
        }

        resolved = true;
        yield return null;
    }

    public void InitRammingAnimation()
    {
        shipA.GetComponent<Animator>().SetTrigger("Collision");
        shipB.GetComponent<Animator>().SetTrigger("Collision");
    }

}
