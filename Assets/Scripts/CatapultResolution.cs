using Photon.Pun;
using System;
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
    // if the ship has a missed shot, then this will be the node its firing on
    // if the ship does not miss, then this will be null
    public Node missedNode;

    public CatapultResolution(Ship a,Ship b,int dmgToB, Node missed = null) : base(a,b,dmgToB) {
        missedNode = missed;
    }

    public override IEnumerator resolve() {

        if(shipA == null || (shipB == null && missedNode == null) || !shipA.CanFire) {
            yield break;
        }

        Vector2 focusPos;
        shipA.SetIconAttack();
        if(missedNode == null) {
            shipB.SetIconTarget();
            focusPos = (shipA.Position + shipB.Position) / 2;
        } else {
            focusPos = (shipA.Position + (Vector3)missedNode.getRealPos()) / 2;
        }
        
        yield return PhaseManager.SyncFocus(focusPos);

        yield return new WaitForSeconds(SpeedManager.CombatDelay);
        GameManager.main.LaunchSound();

        //Sounds.main.playClip(Sounds.main.Launch);
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

        if (PhotonNetwork.IsMasterClient) {
            PhotonView.Get(GameManager.main).RPC("SpawnFireball",RpcTarget.Others,bullet.startPos.x,bullet.startPos.y,bullet.endPos.x,bullet.endPos.y);
        }
        

        while (!bullet.impacted) {

            yield return null;
        }


        if (missedNode == null) {
            //shipB.life -= damageToB;
            shipB.TakeDamage(damageToB);
        }
        
        if (missedNode == null && shipB.life == 0) {
            yield return new WaitForSeconds(SpeedManager.CombatPostDelay * 2);

        } else {
            yield return new WaitForSeconds(SpeedManager.CombatPostDelay);

        }

        shipA.DisableIcon();
        shipA.CanFire = false;


        if (missedNode == null) {
            shipB.DisableIcon();
        }
        

        resolved = true;
        yield return null;
    }

}
