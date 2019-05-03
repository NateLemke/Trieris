using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultResolution : CombatResolution
{
    public CatapultResolution(Ship a,Ship t,int dmgT) : base(a,t,dmgT) {

    }

    public override IEnumerator resolve() {

        GameObject go = Resources.Load<GameObject>("prefabs/CatapultBullet");
        CatapultBullet bullet = GameObject.Instantiate(go,attacker.transform.position,Quaternion.identity).GetComponent<CatapultBullet>();
        bullet.target = target;
        bullet.startPos = attacker.transform.position;

        Vector2 focusPos = attacker.Position + (attacker.Position - target.Position) / 2;
        yield return PhaseManager.focus(focusPos,0f,0.3f);

        while (bullet.gameObject != null) {

            yield return null;
        }

        target.life -= damageToTarget;

        resolved = true;
        yield return null;
    }
}
