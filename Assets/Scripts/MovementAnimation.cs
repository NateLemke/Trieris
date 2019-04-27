using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimation : Animation {

    public Node startPosition;
    public Node endPosition;

    public MovementAnimation(Node start, Node end, Ship s) {
        startPosition = start;
        endPosition = end;
        ship = s;
    }

    public override IEnumerator playAnimation(float speed) {
        if (!complete) {
            startTime = Time.time;

            while (Time.time - startTime < speed) {
                ship.transform.position = Vector3.Lerp(startPosition.getRealPos(),endPosition.getRealPos(),(Time.time - startTime) / speed);
                yield return null;
            }
            complete = true;
        }        
    }
}
