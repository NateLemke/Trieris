﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Purpose: The catapult sub-phase during combat resolution. Focuses the camera where a catapult animation
/// is playing and creates combat icons to display. Animation will play out and ships involved will lose health
/// if hit.
/// </summary>
public class CatapultResolution : CombatResolution
{
    public Node missedNode;

    public bool interrupted;

    public CatapultResolution(Ship a,Ship b,int dmgToB, Node missed = null) : base(a,b,dmgToB) {
        missedNode = missed;
     }

    public override IEnumerator resolve() {

        if(shipA == null || (shipB == null && missedNode == null)) {
            yield break;
        }

        Vector2 focusPos;
        shipA.setIcon(Sprites.main.AttackIcon);
        if(missedNode == null) {
            shipB.setIcon(Sprites.main.TargetIcon);
            focusPos = (shipA.Position + shipB.Position) / 2;
        } else {
            focusPos = (shipA.Position + (Vector3)missedNode.getRealPos()) / 2;
        }
        
        yield return PhaseManager.focus(focusPos);

        yield return new WaitForSeconds(SpeedManager.CombatDelay);
        Sounds.main.playClip(Sounds.main.Launch);
        yield return new WaitForSeconds(SpeedManager.CatapultLaunchDelay);

        GameObject go = Resources.Load<GameObject>("prefabs/CatapultBullet");
        CatapultBullet bullet = GameObject.Instantiate(go,shipA.transform.position,Quaternion.identity).GetComponent<CatapultBullet>();

        

        if (missedNode == null) {
            bullet.endPos = shipB.Position;
            if(shipA.getNode() == shipB.getNode()) {
                bullet.sameNode = true;
            }
        } else {
            if (missedNode == shipA.getNode()) {
                bullet.endPos = shipA.Position + new Vector3(Random.Range(-0.4f,0.4f),Random.Range(-0.4f,0.4f));
            } else {
                bullet.endPos = missedNode.getRealPos();
            }
            bullet.missed = true;
        }
        
        bullet.startPos = shipA.transform.position;
        
           
        

        while (!bullet.impacted) {

            yield return null;
        }


        if (missedNode == null) {
            shipB.life -= damageToB;
        }
        
        if (missedNode == null && shipB.life == 0) {
            yield return new WaitForSeconds(SpeedManager.CombatPostDelay * 2);

        } else {
            yield return new WaitForSeconds(SpeedManager.CombatPostDelay);

        }

        shipA.disableIcon();

        if (missedNode == null) {
            shipB.disableIcon();
        }
        

        resolved = true;
        yield return null;
    }

}
