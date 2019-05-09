using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimation : Animation {

    public Node startNode;
    public Node endNode;
    Vector3 startPos;
    Vector2 endPos;
    bool reverse;

    public MovementAnimation(Node start, Node end, Ship s,bool r = false) {
        startNode = start;
        endNode = end;
        ship = s;

        endPos = PhaseManager.shipNodePos(ship,endNode);
        reverse = r;
    }

    public override IEnumerator playAnimation(float speed,float delay = 0.3f) {
        if (complete) {
            yield break;
        }        

        Vector3 arrowPos = startNode.getRealPos() + (endNode.getRealPos() - startNode.getRealPos())/2;
        yield return PhaseManager.focus(arrowPos,0.7f,0.3f);
        GameObject prefab = Resources.Load<GameObject>("prefabs/MovementArrow");
        GameObject arrow = GameObject.Instantiate(prefab,arrowPos,ship.transform.rotation);
        if (reverse) {
            arrow.transform.localScale = new Vector3(0.158f,-0.158f,0.158f);
        }
        arrow.GetComponent<SpriteRenderer>().color = ship.team.getColorLight();
        yield return new WaitForSeconds(delay);
        if (!complete) {
            startTime = Time.time;

            while (Time.time - startTime < speed) {
                ship.transform.position = Vector3.Lerp(startNode.getRealPos(),endPos,(Time.time - startTime) / speed);
                yield return null;
            }
            complete = true;
            ship.transform.position = endPos;
            GameObject.Destroy(arrow);
            foreach(Ship s in endNode.getShips()) {
                if(PhaseManager.actionAnimations.ContainsKey(s) && (PhaseManager.actionAnimations[s] is MovementAnimation && !PhaseManager.actionAnimations[s].complete)) {
                    continue;
                }
                s.transform.position = PhaseManager.shipNodePos(s, endNode);
            }
        }        
    }
}
