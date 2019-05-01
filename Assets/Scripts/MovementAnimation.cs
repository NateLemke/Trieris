using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimation : Animation {

    public Node startNode;
    public Node endNode;
    Vector3 startPos;
    Vector2 endPos;

    public MovementAnimation(Node start, Node end, Ship s) {
        startNode = start;
        endNode = end;
        ship = s;

        endPos = AnimationManager.shipNodePos(ship,endNode); 
    }

    public override IEnumerator playAnimation(float speed,float delay = 0.3f) {
        if (complete) {
            yield break;
        }
        
        if (Input.GetKey(KeyCode.Space) || InputControl.fastAnimation) {
            speed = 0.03f;
            delay = 0.03f;
        }
        Vector3 arrowPos = startNode.getRealPos() + (endNode.getRealPos() - startNode.getRealPos())/2;
        yield return AnimationManager.focus(arrowPos,0.7f,0.3f);
        GameObject prefab = Resources.Load<GameObject>("prefabs/MovementArrow");
        GameObject arrow = GameObject.Instantiate(prefab,arrowPos,ship.transform.rotation);
        arrow.GetComponent<SpriteRenderer>().color = ship.team.getColor();
        yield return new WaitForSeconds(delay);
        if (!complete) {
            startTime = Time.time;

            while (Time.time - startTime < speed) {
                ship.transform.position = Vector3.Lerp(startNode.getRealPos(),endPos,(Time.time - startTime) / speed);
                yield return null;
            }
            complete = true;
            GameObject.Destroy(arrow);
            foreach(Ship s in endNode.getShips()) {
                if(AnimationManager.actionAnimations.ContainsKey(s) && !(AnimationManager.actionAnimations[s] is MovementAnimation && !AnimationManager.actionAnimations[s].complete)) {
                    s.transform.position = AnimationManager.shipNodePos(s,endNode);
                }
                
            }
        }        
    }
}
