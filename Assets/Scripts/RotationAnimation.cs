using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAnimation : Animation {

    Quaternion startRotation;
    Quaternion endRotation;

    public RotationAnimation(Quaternion start, Quaternion end, Ship s) {
        startRotation = start;
        endRotation = end;
        ship = s;
    }

    public override IEnumerator playAnimation(float speed, float delay = 0f) {
        if (complete) {
            yield break;
        }
        if (Input.GetKey(KeyCode.Space) || InputControl.fastAnimation) {
            speed = 0.03f;
            delay = 0.03f;
        }
        Vector3 pos = ship.Position;
        yield return PhaseManager.focus(pos,0.7f,0.3f);
        GameObject prefab = Resources.Load<GameObject>("prefabs/RotationArrow");
        GameObject arrow = GameObject.Instantiate(prefab,pos,ship.transform.rotation);
        arrow.GetComponent<SpriteRenderer>().color = ship.team.getColor();
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
