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

        ship.SetIconSink();



        InitSinkAnimation();
        
        yield return new WaitForSeconds(SpeedManager.CombatSinking);

        GameManager.main.uiControl.setDead((int)ship.team.TeamFaction, ship.Id);

        ship.DisableIcon();
        ship.sink();

        yield return null;
    }

    public void InitSinkAnimation()
    {
        Sounds.main.playClip(Sounds.main.Blub);
        ship.GetComponent<Animator>().SetTrigger("Sinking");
    }

}
