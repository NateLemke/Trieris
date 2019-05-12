using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkAnimation : Animation {

    public SinkAnimation(Ship s) {
        ship = s;
    }

    public override IEnumerator playAnimation() {
        if(ship == null) {
            yield break;
        }

        PhaseManager.focus(ship.Position,0f,0.5f);

        ship.setIcon(Sprites.main.SinkIcon);
        InitSinkAnimation();
        yield return new WaitForSeconds(SpeedManager.CombatDelay);
        ship.disableIcon();
        yield return new WaitForSeconds(SpeedManager.CombatDelay);
        yield return null;
    }

    void InitSinkAnimation()
    {
        ship.GetComponent<Animator>().SetTrigger("Sinking");
    }
}
