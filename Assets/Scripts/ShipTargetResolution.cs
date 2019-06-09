using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This class is used to manage the moment when a player must choose a target to ram or to shoot with a catapult
/// </summary>
public class ShipTargetResolution 
{
    public Ship attacker;
    public Ship chosenTarget;
    public List<Ship> targets;
    public List<TargetButton> buttons = new List<TargetButton>();
    string text;

    /// <summary>
    /// Constructor. Gets the references for the attacker, the list of targets, and the text to display when this choice is activated
    /// </summary>
    /// <param name="a"></param>
    /// <param name="t"></param>
    /// <param name="txt"></param>
    public ShipTargetResolution(Ship a, List<Ship> t, string txt="Choose Target") {
        attacker = a;
        targets = t;
        text = txt;
    }

    /// <summary>
    /// Activates this targeting choice, creates the UI objects and checks for input
    /// </summary>
    /// <returns></returns>
    public IEnumerator resolve() {
        if(attacker == null) {
            yield break;
        }

        

        GameObject prefab = Resources.Load<GameObject>("Prefabs/TargetButton");
        float xAverage = 0f;
        float yHighest = Mathf.NegativeInfinity;
        //float yHighest = targets[0].Position.y;
        foreach(Ship t in targets) {
            if(t == null) {
                continue;
            }


            if( t.Position.y > yHighest) {
                yHighest = t.Position.y;
            }
            xAverage += t.Position.x;
            GameObject go = GameObject.Instantiate(prefab,t.transform);
            TargetButton tb = go.GetComponent<TargetButton>();
            tb.parent = this;
            tb.target = t;
            buttons.Add(tb);
        }
        xAverage = xAverage / targets.Count;
        prefab = Resources.Load<GameObject>("Prefabs/ChooseText");
        GameObject textObj = GameObject.Instantiate(prefab,new Vector2(xAverage,yHighest + 0.6f),Quaternion.identity);
        textObj.GetComponentInChildren<Text>().text = text;
        textObj.GetComponent<Canvas>().sortingOrder = 10;

        Node targetNode = targets[0].getNode();
        foreach (Ship s in targetNode.Ships) {
            s.updateNodePos(0.6f,0.6f);
        }

        while (chosenTarget == null) {
            yield return null;
        }
        foreach(TargetButton tb in buttons) {
            GameObject.Destroy(tb.gameObject);
        }
        buttons.Clear();
        GameObject.Destroy(textObj);
        PhaseManager.chosenTarget = chosenTarget;
        foreach (Ship s in targetNode.Ships) {
            s.updateNodePos();
        }
    }
}
