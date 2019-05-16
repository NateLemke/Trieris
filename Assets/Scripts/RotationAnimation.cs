using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the execution of a rotation animation for a single ship
/// </summary>
public class RotationAnimation : Animation {

    Quaternion startRotation;
    Quaternion endRotation;

    // portTurn bool is used to decide on which rotation arrow to use for the animation
    bool portTurn;


    /// <summary>
    /// Constructor, sets the inital and ending rotations and the focus position
    /// </summary>
    /// <param name="start">the starting rotation</param>
    /// <param name="end">the end rotation</param>
    /// <param name="s">the ship being animated</param>
    /// <param name="port">whether or not this ship is turning to port</param>
    public RotationAnimation(Quaternion start, Quaternion end, Ship s, bool port) {
        startRotation = start;
        endRotation = end;
        ship = s;
        portTurn = port;
        focusPoint = ship.Position;
    }

    /// <summary>
    /// Plays the rotation animation. Instantiates a sprite indicating rotation.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator playAnimation() {
        if (complete) {
            yield break;
        }

        yield return PhaseManager.focus(focusPoint);
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
