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

        yield return PhaseManager.focus(ship.Position);
        yield return new WaitForSeconds(SpeedManager.CombatDelay);        

        ship.setIcon(Sprites.main.SinkIcon);
        InitSinkAnimation();
        
        ship.disableIcon();
        yield return new WaitForSeconds(SpeedManager.CombatDelay);

        if (ship.getNode().getShips().Contains(ship)) {
            ship.getNode().getShips().Remove(ship);
        }

        if(ship.getNode().getPort() != null) {
            ship.getNode().getPort().setTransparency();
        }

        foreach (Ship s in ship.getNode().getShips()) { 
            s.transform.position = PhaseManager.shipNodePos(s,ship.getNode());
        }


        yield return null;
    }

    public void InitSinkAnimation()
    {
        ship.GetComponent<Animator>().SetTrigger("Sinking");
    }

}
