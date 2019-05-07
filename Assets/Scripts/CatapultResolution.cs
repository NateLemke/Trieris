using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultResolution : CombatResolution
{
    public CatapultResolution(Ship a,Ship t,int dmgT) : base(a,t,dmgT) {

    }

    public override IEnumerator resolve() {

        if(target == null || attacker == null) {
            yield break;
        }

        attacker.setIcon(Sprites.main.AttackIcon);
        target.setIcon(Sprites.main.TargetIcon);

        yield return new WaitForSeconds(SpeedManager.CombatDelay);

        GameObject go = Resources.Load<GameObject>("prefabs/CatapultBullet");
        CatapultBullet bullet = GameObject.Instantiate(go,attacker.transform.position,Quaternion.identity).GetComponent<CatapultBullet>();
        bullet.target = target;
        bullet.startPos = attacker.transform.position;

        Vector2 focusPos = attacker.Position + (attacker.Position - target.Position) / 2;
        yield return PhaseManager.focus(focusPos,0f,0.3f);

        while (bullet != null) {

            yield return null;
        }

        target.life -= damageToTarget;

        yield return new WaitForSeconds(SpeedManager.CombatDelay);

        attacker.disableIcon();
        target.disableIcon();

        resolved = true;
        yield return null;
    }
}
