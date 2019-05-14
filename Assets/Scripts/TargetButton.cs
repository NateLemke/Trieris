using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetButton : MonoBehaviour
{
    public ShipTargetResolution parent;
    public Ship target;

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



    private void OnMouseExit() {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.grey;
    }

    private void OnMouseUp() {
        
    }

    private void OnMouseUpAsButton() {
        


    }

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
