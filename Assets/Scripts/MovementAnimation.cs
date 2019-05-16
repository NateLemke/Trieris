using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Purpose:    This class manages the execution of a movement animation for a single ship (forwards or backwards)
/// </summary>
public class MovementAnimation : Animation {

    public Node startNode;
    public Node endNode;
    Vector3 startPos;
    Vector2 endPos;
    bool reverse;
    int momentum;

    /// <summary>
    /// Constructor. Sets values for startNode, endNode, ship, momentum, and reverse
    /// </summary>
    /// <param name="start">the node the ship starts at</param>
    /// <param name="end">the node the ship ends at</param>
    /// <param name="s">the ship that's moveing</param>
    /// <param name="m">the ship's momentum at the phase that they moved</param>
    /// <param name="r">whether or not the ship is moving forwards or backwards</param>
    public MovementAnimation(Node start, Node end, Ship s,int m, bool r = false) {
        startNode = start;
        endNode = end;
        ship = s;
        momentum = m;
        endPos = PhaseManager.shipNodePos(ship,endNode);
        reverse = r;
        focusPoint = startNode.getRealPos() + (endNode.getRealPos() - startNode.getRealPos()) / 2;
    }

    /// <summary>
    /// Plays the animation to move the ship. Updates the multi-ship per node position of both the start and end node.
    ///     Instantiates an arrow indicating direction and text indicating momentum (if momentum is greater than 1)
    ///     Also checks if it needs update any ports transparency.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator playAnimation() {
        if (complete) {
            yield break;
        }        

        yield return PhaseManager.focus(focusPoint);
        GameObject prefab = Resources.Load<GameObject>("prefabs/MovementArrow");
        GameObject arrow = GameObject.Instantiate(prefab,focusPoint,ship.transform.rotation);
        if (reverse) {
            arrow.transform.localScale = new Vector3(0.158f,-0.158f,0.158f);
        }
        arrow.GetComponentInChildren<SpriteRenderer>().color = ship.team.getColorLight();
        arrow.GetComponentInChildren<Text>().text = (momentum > 1) ? momentum.ToString() : "";
        arrow.GetComponentInChildren<Text>().text = (momentum > 1) ? momentum.ToString() : "";
        arrow.GetComponentInChildren<Text>().color = ship.team.getColorLight();
        yield return new WaitForSeconds(SpeedManager.ActionDelay);
        if (!complete) {
            startTime = Time.time;
            updatePositionOnNode(startNode);
            if(startNode.getPort() != null) {
                startNode.getPort().setTransparency();
            }
            while (Time.time - startTime < SpeedManager.ActionSpeed) {
                ship.transform.position = Vector3.Lerp(startNode.getRealPos(),endPos,(Time.time - startTime) / SpeedManager.ActionSpeed);
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

    /// <summary>
    /// Updates all ship positions on nodes, if they have completed any movement animations they have.
    /// </summary>
    /// <param name="n"></param>
    void updatePositionOnNode(Node n) {
        foreach (Ship s in n.getShips()) {
            if (PhaseManager.actionAnimations.ContainsKey(s) && (PhaseManager.actionAnimations[s] is MovementAnimation && !PhaseManager.actionAnimations[s].complete)) {
                continue;
            }
            s.transform.position = PhaseManager.shipNodePos(s,n);
        }
    }
}
