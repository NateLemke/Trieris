using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadOnRammingResolution : RammingResolution {

    public HeadOnRammingResolution(Ship a,Ship b,int dmgB,int dmgA = 0) : base(a,b,dmgB,dmgA) {

    }

    public override IEnumerator resolve() {

        Vector3 midPoint = (shipA.Position + shipB.Position) / 2;

        yield return PhaseManager.focus((shipA.Position + shipB.Position) / 2);

        shipA.setIcon(Sprites.main.AttackIcon);
        shipB.setIcon(Sprites.main.AttackIcon);

        Vector3 startPosA = shipA.Position;
        Vector3 startPosB = shipB.Position;

        Vector3 endPosA = ((startPosB - startPosA) / 2.5f) + startPosA;
        Vector3 endPosB = ((startPosA - startPosB) / 2.5f) + startPosB;

        float startTime = Time.time;

        while(Time.time < startTime + 0.4f) {
            shipA.Position = Vector3.Lerp(startPosA,endPosA, (Time.time - startTime) / 0.4f);
            shipB.Position = Vector3.Lerp(startPosB,endPosB, (Time.time - startTime) / 0.4f);
            yield return null;
        }

        InitHeadOnAnimation();
        Sounds.main.playRandomCrunch();
        yield return new WaitForSeconds(0.3f);
        Sounds.main.playRandomCrunch();

        shipB.life -= damageToB;
        shipA.life -= damageToA;

        shipB.disableIcon();
        shipA.disableIcon();

        if (shipA.life == 0 || shipB.life == 0) {
            yield return new WaitForSeconds(SpeedManager.CombatPostDelay * 2);
        } else {
            yield return new WaitForSeconds(SpeedManager.CombatPostDelay);
        }

        startTime = Time.time;
        while (Time.time < startTime + 0.4f) {
            shipA.Position = Vector3.Lerp(endPosA,startPosA,(Time.time - startTime) / 0.4f);
            shipB.Position = Vector3.Lerp(endPosB,startPosB,(Time.time - startTime) / 0.4f);
            yield return null;

        }

    }

    public void InitHeadOnAnimation()
    {
        shipA.GetComponent<Animator>().SetTrigger("Collision");
        shipB.GetComponent<Animator>().SetTrigger("Collision");
    }


}
