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

    public override IEnumerator playAnimation() {
        if (!complete) {
            startTime = Time.time;

            while (Time.time - startTime < ANIMATION_SPEED) {
                ship.transform.rotation = Quaternion.Lerp(startRotation,endRotation,(Time.time - startTime) / ANIMATION_SPEED);
                yield return null;
            }
            complete = true;
        }      
    }
}
