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
        if (Input.GetKey(KeyCode.Space)) {
            speed = 0.12f;
            delay = 0.12f;
        }
        Vector3 pos = ship.Position;
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
            GameObject.Destroy(arrow);
        }
    }
}
