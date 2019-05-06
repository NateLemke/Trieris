using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortCaptureAnimation : Animation {

    public PortCaptureAnimation(Ship s) {
        ship = s;
    }

    public override IEnumerator playAnimation(float speed,float delay) {
        yield return PhaseManager.focus(ship.Position,0f,0.3f);
        GameObject prefab = Resources.Load<GameObject>("Prefabs/PortCaptureAnimation");
        GameObject animObj = GameObject.Instantiate(prefab,ship.getNode().getRealPos(),Quaternion.identity);
        animObj.GetComponent<Canvas>().sortingLayerName = "UILayer";
        Image lowerImg = animObj.transform.Find("LowerImage").GetComponent<Image>();
        Image upperImg = animObj.transform.Find("LowerImage").transform.Find("UpperImage").GetComponent<Image>();

        upperImg.sprite = ship.getNode().getPort().getTeam().getPortSprite();
        lowerImg.sprite = ship.team.getPortSprite();

        yield return new WaitForSeconds(delay);

        float timeStamp = Time.time + speed;

        float fill = 1f;

        while(Time.time < timeStamp) {
            fill = (timeStamp - Time.time) / speed;
            upperImg.fillAmount = fill;
            yield return null;
        }
        upperImg.fillAmount = 0;
        ship.getNode().getPort().setTeam(ship.team);
        yield return new WaitForSeconds(delay);
        GameObject.Destroy(animObj);
        GameManager.main.uiControl.updatePortsUI();
        yield return new WaitForSeconds(delay/2);
    }
}
