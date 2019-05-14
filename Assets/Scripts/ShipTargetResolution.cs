using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipTargetResolution 
{
    public Ship attacker;
    public Ship chosenTarget;
    List<Ship> targets;
    public List<TargetButton> buttons = new List<TargetButton>();
    string text;

    public ShipTargetResolution(Ship a, List<Ship> t, string txt="Choose Target") {
        attacker = a;
        targets = t;
        text = txt;
    }

    public IEnumerator resolve() {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/TargetButton");
        float xAverage = 0f;
        float yHighest = targets[0].Position.y;
        foreach(Ship t in targets) {
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

        Node targetNode = targets[0].getNode();
        foreach (Ship s in targetNode.getShips()) {
            s.Position = PhaseManager.shipNodePos(s,targetNode,0.6f,0.6f);
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
        foreach (Ship s in targetNode.getShips()) {
            s.Position = PhaseManager.shipNodePos(s,targetNode);
        }
    }
}
