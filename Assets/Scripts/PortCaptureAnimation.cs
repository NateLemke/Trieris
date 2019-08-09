using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Purpose:
///             This class plays manages the execution of a port capture animation.
/// </summary>
public class PortCaptureAnimation : Animation {

    /// <summary>
    /// Constructor, also sets the focus point for this animation
    /// </summary>
    /// <param name="s">the ship that's doing the capture</param>
    public PortCaptureAnimation(Ship s) {
        ship = s;
        focusPoint = ship.Position;
    }

    /// <summary>
    /// Plays the Animation. Instantiates the required prefabs and destroys them. Also sets the ship rotation if the ship is AI controlled.
    /// Transfers ownership of the port.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator playAnimation() {

        if(ship == null) {
            yield break;
        }

        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
            PhotonView.Get(GameManager.main).RPC("RunPortCaptureAnimation",RpcTarget.Others,(int)ship.team.TeamFaction,ship.Id, ship.Position.x,ship.Position.y);
        }
        Debug.LogFormat("Playing animation for team {0} which is {1} team, port number {2}, ship id {3}",(int)ship.team.TeamFaction,(int)ship.getNode().Port.Team.TeamFaction,ship.getNode().Port.id,ship.Id);

        yield return PhaseManager.SyncFocus(focusPoint);
        GameObject prefab = Resources.Load<GameObject>("Prefabs/PortCaptureAnimation");

        PortCaptureAnimationObject animObj;


        GameObject go = GameObject.Instantiate(prefab,ship.getNode().getRealPos(),Quaternion.identity);

        animObj = go.GetComponent<PortCaptureAnimationObject>();

        animObj.SetUpperImg( ship.getNode().Port.Team.getPortSprite());
        animObj.SetLowerImg(ship.team.getPortSprite());
            

        yield return new WaitForSeconds(SpeedManager.CaptureDelay + SpeedManager.CaptureSpeed);

        ship.getNode().Port.Team = ship.team;

        if (!GameManager.main.getPlayerShips().Contains(ship) && (!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient)) {
            int direction = ship.Ai.setNewShipDirection(ship);
            ship.setFront(direction);
            ship.setSpriteRotation();
        }

        yield return new WaitForSeconds(SpeedManager.CaptureDelay);       
        GameManager.main.uiControl.updatePortsUI();
        yield return new WaitForSeconds(SpeedManager.CaptureDelay / 2);
    }
}
