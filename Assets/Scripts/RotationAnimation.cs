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

    public override IEnumerator playAnimation(float speed, float delay = 0f) {
        if (complete) {
            yield break;
        }
        
        Vector3 pos = ship.Position;
        yield return PhaseManager.focus(pos,0.7f,0.3f);
        GameObject prefab = Resources.Load<GameObject>("prefabs/RotationArrow");
        GameObject arrow = GameObject.Instantiate(prefab,pos,ship.transform.rotation);
        if (portTurn) {
            arrow.transform.localScale = new Vector3(-1,1,1);
        }
        arrow.GetComponentInChildren<SpriteRenderer>().color = ship.team.getColorLight();
        yield return new WaitForSeconds(delay);
        if (!complete) {
            startTime = Time.time;

            while (Time.time - startTime < speed) {
                ship.transform.rotation = Quaternion.Lerp(startRotation,endRotation,(Time.time - startTime) / speed);
                yield return null;
            }
            complete = true;
            ship.transform.rotation = endRotation;
            GameObject.Destroy(arrow);
        }
    }
}
