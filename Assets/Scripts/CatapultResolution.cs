using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultResolution : CombatResolution
{
    public CatapultResolution(Ship a,Ship t,int dmgT) : base(a,t,dmgT) {

    }

    public override IEnumerator resolve() {

        if(shipB == null || shipA == null) {
            yield break;
        }

        shipA.setIcon(Sprites.main.AttackIcon);
        shipB.setIcon(Sprites.main.TargetIcon);

        yield return new WaitForSeconds(SpeedManager.CombatDelay);

        GameObject go = Resources.Load<GameObject>("prefabs/CatapultBullet");
        CatapultBullet bullet = GameObject.Instantiate(go,shipA.transform.position,Quaternion.identity).GetComponent<CatapultBullet>();
        bullet.target = shipB;
        bullet.startPos = shipA.transform.position;

        Vector2 focusPos = shipA.Position + (shipA.Position - shipB.Position) / 2;
        yield return PhaseManager.focus(focusPos,0f,0.3f);

        while (bullet != null) {

            yield return null;
        }

        shipB.life -= damageToB;

        yield return new WaitForSeconds(SpeedManager.CombatDelay);

        shipA.disableIcon();
        shipB.disableIcon();

        resolved = true;
        yield return null;
    }
}
