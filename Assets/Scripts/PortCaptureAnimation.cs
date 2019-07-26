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
            PhotonView.Get(GameManager.main).RPC("RunPortCaptureAnimation",RpcTarget.Others,(int)ship.team.TeamFaction,ship.Id);
        }

        yield return PhaseManager.focus(focusPoint);
        GameObject prefab = Resources.Load<GameObject>("Prefabs/PortCaptureAnimation");
        GameObject animObj = GameObject.Instantiate(prefab,ship.getNode().getRealPos(),Quaternion.identity);
        animObj.GetComponent<Canvas>().sortingLayerName = "UILayer";
        animObj.GetComponent<Canvas>().sortingOrder = 10;
        Image lowerImg = animObj.transform.Find("LowerImage").GetComponent<Image>();
        Image upperImg = animObj.transform.Find("LowerImage").transform.Find("UpperImage").GetComponent<Image>();

        upperImg.sprite = ship.getNode().Port.Team.getPortSprite();
        lowerImg.sprite = ship.team.getPortSprite();

        yield return new WaitForSeconds(SpeedManager.CaptureDelay);

        float timeStamp = Time.time + SpeedManager.CaptureSpeed;

        float fill = 1f;

        while(Time.time < timeStamp) {
            fill = (timeStamp - Time.time) / SpeedManager.CaptureSpeed;
            upperImg.fillAmount = fill;
            yield return null;
        }
        upperImg.fillAmount = 0;
        ship.getNode().Port.setTeam(ship.team);
        
        if (!GameManager.main.getPlayerShips().Contains(ship)) {
            int direction = ship.Ai.setNewShipDirection(ship);
            ship.setFront(direction);
            ship.setSpriteRotation();
        }

        yield return new WaitForSeconds(SpeedManager.CaptureDelay);
        GameObject.Destroy(animObj);
        GameManager.main.uiControl.updatePortsUI();
        yield return new WaitForSeconds(SpeedManager.CaptureDelay / 2);
    }
}
