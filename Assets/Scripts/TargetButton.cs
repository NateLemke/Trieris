using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to prompt the player to choose a target for ramming/catapult
/// </summary>
public class TargetButton : MonoBehaviour
{
    public ShipTargetResolution parent;
    public Ship target;

    /// <summary>
    /// Sets the button colour to red if moused over to give it a more "button-like" appearance.
    /// </summary>
    private void OnMouseOver() {
        //Debug.Log("Mouse over");
        if (closest()) {
            if (Input.GetMouseButtonDown(0)) {
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
                parent.chosenTarget = target;
            } else {
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;

            }
        }
    }

    /// <summary>
    /// Resets the button to grey when the mouse leaves it.
    /// </summary>
    private void OnMouseExit() {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.grey;
    }

    /// <summary>
    /// Determines which button is closest to the mouse (which one is being clicked.)
    /// </summary>
    /// <returns>Returns true if this is the button closest to the mouse, false otherwise.</returns>
    bool closest() {
        bool closest = true;
        foreach (TargetButton tb in parent.buttons) {
            if(tb == this) {
                continue;
            }
            float distance1 = Vector2.Distance(transform.position,InputControl.mouseWorldPos());
            float distance2 = Vector2.Distance(tb.transform.position,InputControl.mouseWorldPos());
            if (distance2 < distance1) {
                closest = false;
                break;
            }
        }
        return closest;
    }



    
}
