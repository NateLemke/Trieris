using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose:    This class is used to resolve a special ramming case where two ships in adjacent nodes try to pass by each other and ram head on 
///                 in the middle.
///                 The ships will stay in their nodes and take damage from the head-on ramming.
/// </summary>
public class HeadOnRammingResolution : RammingResolution {

    /// <summary>
    /// Constructor, takes info for the two ships involved, and the damage to each.
    /// </summary>
    /// <param name="a">reference to shipA</param>
    /// <param name="b">reference to shipB</param>
    /// <param name="dmgB">damage to shipB</param>
    /// <param name="dmgA">damage to shipA</param>
    public HeadOnRammingResolution(Ship a,Ship b,int dmgB,int dmgA = 0) : base(a,b,dmgB,dmgA) {

    }

    /// <summary>
    /// Plays the combat animation and resolved the pending damage.
    /// Animates the two ships to the midpoints between the nodes, applies the damage, then returns them to their original positions.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator resolve() {

        if(shipA == null || shipB == null){
            yield break;
        }

        Vector3 midPoint = (shipA.Position + shipB.Position) / 2;

        yield return PhaseManager.focus((shipA.Position + shipB.Position) / 2);

        shipA.setIcon(Sprites.main.AttackIcon);
        shipB.setIcon(Sprites.main.AttackIcon);

        Vector3 startPosA = shipA.Position;
        Vector3 startPosB = shipB.Position;

        Vector3 endPosA = ((startPosB - startPosA) / 2.5f) + startPosA;
        Vector3 endPosB = ((startPosA - startPosB) / 2.5f) + startPosB;

        float startTime = Time.time;

        while(Time.time < startTime + SpeedManager.HeadOnSpeed) {
            shipA.Position = Vector3.Lerp(startPosA,endPosA, (Time.time - startTime) / SpeedManager.HeadOnSpeed);
            shipB.Position = Vector3.Lerp(startPosB,endPosB, (Time.time - startTime) / SpeedManager.HeadOnSpeed);
            yield return null;
        }

        InitHeadOnAnimation();
        Sounds.main.playRandomCrunch();
        if (shipA.life == 0 || shipB.life == 0) {
            yield return new WaitForSeconds(SpeedManager.CombatPostDelay * 2);
        } else {
            yield return new WaitForSeconds(SpeedManager.CombatPostDelay);
        }
        Sounds.main.playRandomCrunch();

        shipB.life -= damageToB;
        shipA.life -= damageToA;

        shipB.disableIcon();
        shipA.disableIcon();


        startTime = Time.time;
        while (Time.time < startTime + 0.1f) {
            shipA.Position = Vector3.Lerp(endPosA,startPosA,(Time.time - startTime) / 0.1f);
            shipB.Position = Vector3.Lerp(endPosB,startPosB,(Time.time - startTime) / 0.1f);
            yield return null;
        }

    }

    /// <summary>
    /// Used to initiate animator components
    /// </summary>
    public void InitHeadOnAnimation()
    {
        shipA.GetComponent<Animator>().SetTrigger("Collision");
        shipB.GetComponent<Animator>().SetTrigger("Collision");
    }


}
