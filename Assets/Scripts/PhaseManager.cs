﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class PhaseManager 
{
    public static HashSet<Ship> involvedInRamming = new HashSet<Ship>();
    public static Dictionary<Ship,Animation> actionAnimations = new Dictionary<Ship,Animation>();
    public static List<SinkAnimation> sinkAnimations = new List<SinkAnimation>();
    public static List<CombatResolution> rammingResolutions = new List<CombatResolution>();
    public static List<CombatResolution> catapultResolutions = new List<CombatResolution>();

    

    public static bool movingCamera = false;

    public static bool playingAnimation = false;

    public static float nodeMultiShipScale = 0.3f;


    public static IEnumerator playAnimations() {
        playingAnimation = true;

        updateText();
        yield return playBasicActions();
        yield return playRammingActions();
        yield return rammingChoices();
        yield return sinkShips();
        yield return resolveCatapults();
        yield return catapultChoices();
        yield return sinkShips();
        yield return portCapture();
        yield return resolveRedirects();
        
        actionAnimations.Clear();
        rammingResolutions.Clear();
        involvedInRamming.Clear();
        playingAnimation = false;
        GameManager.main.gameLogic.postAnimation();
        
    }

    static IEnumerator playRammingActions() {
        rammingResolutions.Sort(new RammingSorter());
        setSubphaseText("resolving ramming");

        for (int i = 0; i < rammingResolutions.Count;i++) {
            yield return rammingResolutions[i].resolve();
            Ship target = rammingResolutions[i].target;
            Ship attacker = rammingResolutions[i].attacker;
            for (int j = 0; j < rammingResolutions.Count; j++) {
                if(rammingResolutions[j].target == attacker && rammingResolutions[j].attacker == target) {
                    rammingResolutions[j].resolve();
                    break;
                }
            }
        }

        yield return null;
    }

    static IEnumerator playBasicActions() {
        setSubphaseText("resolving actions");
        List<Animation> anims = actionAnimations.Values.ToList();
        
        foreach(Animation a in anims) {
            if (involvedInRamming.Contains(a.ship)) {
                continue;
            }
            yield return a.playAnimation(0.3f,0.5f);
        }

        yield return null;
    }

    public static void addRamming(Ship attacker, Ship target, int damageToTarget, int damageToAttacker = 0) {
        rammingResolutions.Add(new CombatResolution(attacker,target,damageToTarget,damageToAttacker));
    }

    public static void addCatapult() {

    }

    //public static 

    public static Vector2 shipNodePos(Ship s,Node n) {
        
        List<Ship> ships = n.getShips();

        if(ships.Count == 0) {
            Debug.LogError("No ships in node!");
        }

        if(ships.Count == 1) {
            return n.getRealPos();
        }

        float sqr = Mathf.Sqrt(ships.Count);
        float rounded = Mathf.Ceil(sqr);


        int i = 0;
        for (; i < ships.Count; i++) {
            if(ships[i] == s) {
                break;
            }
        }

        int x = (i) % (int)rounded  ;
        int y = i / (int)rounded;

        float offset = (rounded - 1) * nodeMultiShipScale / 2f;
        Vector2 pos = new Vector2(x * nodeMultiShipScale - offset,-y * 0.3f);

        return pos + n.getRealPos();
    }

    public static void NodePositions(int f, Node n, Color c) {
        //float scale = 0.3f;
        float sqr = Mathf.Sqrt(f);
        float rounded = Mathf.Ceil(sqr);
        int counter = 0;
        for(int i = 0; i < rounded; i++) {
            for(int j = 0; j < rounded; j++) {
                float offset = (rounded -1)* nodeMultiShipScale / 2f;
                Vector2 pos = new Vector2(j * nodeMultiShipScale - offset,-i * 0.3f);
                pos += n.getRealPos();
                Debug.DrawLine(pos + Vector2.up * 0.1f,pos + Vector2.down * 0.1f,c);
                Debug.DrawLine(pos + Vector2.left * 0.1f,pos + Vector2.right * 0.1f,c);
                counter++;
                if(counter >= f) {
                    return;
                }
            }
        }
    }

    public static Vector2[] getBoardView() {
        float camHeight = Camera.main.orthographicSize * 2f;
        float camWidth = camHeight * Camera.main.aspect;
        Vector2 camPos = Camera.main.transform.position;
        //Debug.DrawLine(camPos + new Vector2(0,camHeight / 2),camPos + new Vector2(0,-camHeight / 2),Color.red);
        //Debug.DrawLine(camPos + new Vector2(camWidth / 2,0),camPos + new Vector2(-camWidth / 2,0),Color.blue);
        float canvasWidth = GameObject.Find("OverlayCanvas").GetComponent<RectTransform>().rect.width;
        float sideUIWidth = GameObject.Find("UISidePanel").GetComponent<RectTransform>().rect.width;
        float ratio = (canvasWidth - sideUIWidth) / canvasWidth;
        //Debug.DrawLine(camPos + new Vector2(-camWidth / 2,2),camPos + new Vector2(camWidth * ratio / 2,2),Color.green);
        Vector2[] r = new Vector2[2];
        r[0] = camPos - new Vector2(camWidth / 2,camHeight / 2);
        r[1] = new Vector2(camWidth * ratio,camHeight);
        return r;
    }

    public static void focusCamera() {
        Vector2[] bv = getBoardView();
        Debug.DrawLine(bv[0],bv[0] + bv[1],Color.red);
    }

    
    public static IEnumerator focus(Vector2 v, float margin, float speed) {
        Vector2[] bv = getBoardView();
        //margin = bv[1].y * margin;
        margin = 0;
        if(v.x < bv[0].x + margin || v.x > bv[0].x + bv[1].x - margin || v.y < bv[0].y + margin || v.y > bv[0].y + bv[1].y - margin) {
            yield return moveCameraTo(v,speed);
        }
    }

    public static IEnumerator moveCameraTo(Vector3 pos, float duration) {
        Vector3 startPos = Camera.main.transform.position;
        pos.z = startPos.z;
        movingCamera = true;
        float startTime = Time.time;
        while((Time.time - startTime) / duration < 1f) {
            Camera.main.transform.position = Vector3.Lerp(startPos,pos,(Time.time - startTime) / duration);
            yield return null;
        }
        movingCamera = false;
    }

    public static IEnumerator rammingChoices() {
        if (!GameManager.main.needRammingChoice) {
            yield break;
        }
        setSubphaseText("choose ramming targets");
        while (GameManager.main.needRammingChoice) {
            yield return null;
        }
    }

    public static IEnumerator resolveCatapults() {
        if(catapultResolutions.Count > 0) {
            setSubphaseText("resolving catapults");
            foreach(CombatResolution cr in catapultResolutions) {
                yield return cr.resolve();
            }
        }        
    }

    public static IEnumerator catapultChoices() {
        if (!GameManager.main.needCatapultChoice) {
            yield break;
        }
        setSubphaseText("chose catapult targets");
        while (GameManager.main.needCatapultChoice) {
            yield return null;
        }
    }

    public static IEnumerator sinkShips() {
        if(sinkAnimations.Count > 0) {
            setSubphaseText("sinking ships");
            foreach (SinkAnimation a in sinkAnimations) {
                yield return a.playAnimation(0.1f,0.1f);
            }
            sinkAnimations.Clear();
        }       
    }

    public static IEnumerator portCapture() {
        if (!GameManager.main.needCaptureChoice) {
            yield break;
        }
        setSubphaseText("choose port capture");
        while (GameManager.main.needCaptureChoice) {
            yield return null;
        }
    }

    public static IEnumerator resolveRedirects() {
        if (!GameManager.main.needRedirect) {
            yield break;
        }
        setSubphaseText("choose redirects");
        while (GameManager.main.needRedirect) {
            yield return null;
        }        
    }

    public static void setSubphaseText(string s) {
        UIControl.main.subPhase.SetActive(true);
        UIControl.main.subPhase.GetComponentInChildren<Text>().text = s;
    }

    public static void updateText() {
        int phase = GameManager.main.gameLogic.phaseIndex;
        
        GameObject phaseObj = UIControl.main.phase;
        if (phase == 4) {
            phaseObj.SetActive(false);
            UIControl.main.subPhase.SetActive(false);
        } else {
            phaseObj.SetActive(true);
            phaseObj.GetComponentInChildren<Text>().text = "Phase " + (phase+1);
        }

        
    }

    static void clearAnimationLists() {
        sinkAnimations.Clear();
    }

    static void removeShipFromCatapultAnimations(Ship s) {
        foreach(CombatResolution cr in catapultResolutions) {
            if( cr.attacker == s) {
                catapultResolutions.Remove(cr);
            }
        }
    }

}

class RammingSorter : IComparer<CombatResolution> {
    public int Compare(CombatResolution x,CombatResolution y) {
        if (x.attacker.Position.x < y.attacker.Position.x) {
            return -1;
        } else {
            return (x.attacker.Position.y < y.attacker.Position.y) ? 1 : -1;
        }
    }
}