using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkAnimation : Animation {

    public SinkAnimation(Ship s) {
        ship = s;
    }

    public override IEnumerator playAnimation(float speed,float delay) {
        if(ship == null) {
            yield break;
        }

        PhaseManager.focus(ship.Position,0f,0.5f);

        ship.setIcon(Sprites.main.SinkIcon);
        yield return new WaitForSeconds(1f);
        ship.disableIcon();
        yield return new WaitForSeconds(0.3f);
        yield return null;
    }
}
