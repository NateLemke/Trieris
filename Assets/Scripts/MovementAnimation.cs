using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementAnimation : Animation {

    public Node startNode;
    public Node endNode;
    Vector3 startPos;
    Vector2 endPos;
    bool reverse;
    int momentum;

    public MovementAnimation(Node start, Node end, Ship s,int m, bool r = false) {
        startNode = start;
        endNode = end;
        ship = s;
        momentum = m;
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
        arrow.GetComponentInChildren<SpriteRenderer>().color = ship.team.getColorLight();
        arrow.GetComponentInChildren<Text>().text = (momentum > 1) ? momentum.ToString() : "";
        arrow.GetComponentInChildren<Text>().text = (momentum > 1) ? momentum.ToString() : "";
        arrow.GetComponentInChildren<Text>().color = ship.team.getColorLight();
        yield return new WaitForSeconds(delay);
        if (!complete) {
            startTime = Time.time;
            updatePositionOnNode(startNode);
            if(startNode.getPort() != null) {
                startNode.getPort().setTransparency();
            }
            while (Time.time - startTime < speed) {
                ship.transform.position = Vector3.Lerp(startNode.getRealPos(),endPos,(Time.time - startTime) / speed);
                yield return null;
            }
            if(endNode.getPort() != null) {
                endNode.getPort().setTransparency();
            }
            complete = true;
            ship.transform.position = endPos;
            GameObject.Destroy(arrow);
            updatePositionOnNode(endNode);
        }        
    }

    void updatePositionOnNode(Node n) {
        foreach (Ship s in n.getShips()) {
            if (PhaseManager.actionAnimations.ContainsKey(s) && (PhaseManager.actionAnimations[s] is MovementAnimation && !PhaseManager.actionAnimations[s].complete)) {
                continue;
            }
            s.transform.position = PhaseManager.shipNodePos(s,n);
        }
    }
}
