using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAnimation : Animation {

    Quaternion startRotation;
    Quaternion endRotation;
    bool portTurn;

    public RotationAnimation(Quaternion start, Quaternion end, Ship s, bool port) {
        startRotation = start;
        endRotation = end;
        ship = s;
        portTurn = port;
    }

    public override IEnumerator playAnimation() {
        if (complete) {
            yield break;
        }
        
        Vector3 pos = ship.Position;
        yield return PhaseManager.focus(pos,0.7f,0.3f);
        GameObject prefab = Resources.Load<GameObject>("prefabs/RotationArrow");
        GameObject arrow = GameObject.Instantiate(prefab,ship.transform);
        if (portTurn) {
            arrow.transform.localScale = new Vector3(-1,1,1);
        }
        arrow.GetComponentInChildren<SpriteRenderer>().color = ship.team.getColorLight();
        yield return new WaitForSeconds(SpeedManager.ActionDelay);
        if (!complete) {
            startTime = Time.time;

            while (Time.time - startTime < SpeedManager.ActionSpeed) {
                ship.transform.rotation = Quaternion.Lerp(startRotation,endRotation,(Time.time - startTime) / SpeedManager.ActionSpeed);
                yield return null;
            }
            complete = true;
            ship.transform.rotation = endRotation;
            GameObject.Destroy(arrow);
        }
    }
}
