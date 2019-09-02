using Photon.Pun;
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

        yield return PhaseManager.SyncFocus(ship.Position);
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

        if (PhotonNetwork.IsConnected) {
            PhotonView.Get(GameManager.main).RPC("InitSinkAnimation",RpcTarget.Others,ship.Id,(int)ship.team.TeamFaction);
        }
    }

}
