using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortCaptureAnimation : Animation {

    public PortCaptureAnimation(Ship s) {
        ship = s;
    }

    public override IEnumerator playAnimation() {

        if(ship == null) {
            yield break;
        }

        yield return PhaseManager.focus(ship.Position,0f,0.3f);
        GameObject prefab = Resources.Load<GameObject>("Prefabs/PortCaptureAnimation");
        GameObject animObj = GameObject.Instantiate(prefab,ship.getNode().getRealPos(),Quaternion.identity);
        animObj.GetComponent<Canvas>().sortingLayerName = "UILayer";
        Image lowerImg = animObj.transform.Find("LowerImage").GetComponent<Image>();
        Image upperImg = animObj.transform.Find("LowerImage").transform.Find("UpperImage").GetComponent<Image>();

        upperImg.sprite = ship.getNode().getPort().getTeam().getPortSprite();
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
        ship.getNode().getPort().setTeam(ship.team);
        
        if (!GameManager.main.getPlayerShips().Contains(ship)) {
            int direction = ship.getAI().setNewShipDirection(ship);
            ship.setFront(direction);
            ship.setSpriteRotation();
        }


        yield return new WaitForSeconds(SpeedManager.CaptureDelay);
        GameObject.Destroy(animObj);
        GameManager.main.uiControl.updatePortsUI();
        yield return new WaitForSeconds(SpeedManager.CaptureDelay / 2);
    }
}
