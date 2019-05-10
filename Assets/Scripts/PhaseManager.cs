using System.Collections;
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
    public static List<PortCaptureAnimation> captureAnimations = new List<PortCaptureAnimation>();

    static int subPhaseIndex = 0;

    public static bool movingCamera = false;

    public static bool playingAnimation = false;

    public static float nodeMultiShipScale = 0.34f;

    delegate IEnumerator subPhase();

    static subPhase[] subPhaseOrder = {
        playBasicActions ,
        sinkShips,        
        resolveRamming,
        rammingChoices,
        sinkShips,
        catapultChoices,
        resolveCatapults,
        sinkShips,
        portCaptureChoice,
        resolvePortCapture,        
        resolveRedirects,
    };


    public static IEnumerator playAnimations() {
        playingAnimation = true;
        subPhaseIndex = 0;
        yield return null;

        subPhaseProgress();
        updateText();

        foreach(subPhase s in subPhaseOrder) {
            yield return s();
            nextSubPhase();
            sinkAnimations.Clear();
        }

        //yield return playBasicActions();
        //yield return sinkShips();
        //yield return rammingChoices();
        //yield return playRammingActions();
        //yield return sinkShips();
        //yield return catapultChoices();
        //yield return resolveCatapults();        
        //yield return sinkShips();
        //yield return resolvePortCapture();
        //yield return portCaptureChoice();
        //yield return resolveRedirects();
        
        actionAnimations.Clear();
        rammingResolutions.Clear();
        involvedInRamming.Clear();
        captureAnimations.Clear();
        catapultResolutions.Clear();

        playingAnimation = false;
        GameManager.main.gameLogic.postAnimation();
        
    }
        
    //public static void addRamming(Ship attacker, Ship target, int damageToTarget, int damageToAttacker = 0) {
    //    rammingResolutions.Add(new RammingResolution(attacker,target,damageToTarget,damageToAttacker));
    //}

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
                //Debug.DrawLine(pos + Vector2.up * 0.1f,pos + Vector2.down * 0.1f,c);
                //Debug.DrawLine(pos + Vector2.left * 0.1f,pos + Vector2.right * 0.1f,c);
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

    //public static void focusCamera() {
    //    Vector2[] bv = getBoardView();
    //    Debug.DrawLine(bv[0],bv[0] + bv[1],Color.red);
    //}

    
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
    
    static IEnumerator playBasicActions() {
        setSubphaseText("resolving actions");
        List<Animation> anims = actionAnimations.Values.ToList();

        foreach (Animation a in anims) {
            if (involvedInRamming.Contains(a.ship)) {
                continue;
            }

            yield return a.playAnimation(SpeedManager.ActionDelay,SpeedManager.ActionSpeed);
        }

        yield return null;
    }

    public static IEnumerator rammingChoices() {
        subPhaseProgress();
        if (!GameManager.main.needRammingChoice) {
            yield break;
        }
        setSubphaseText("choose ramming targets");
        while (GameManager.main.needRammingChoice) {
            yield return null;
        }
    }

    static IEnumerator resolveRamming() {
        if(rammingResolutions.Count == 0) {
            yield break;
        }
        rammingResolutions.Sort(new RammingSorter());
        setSubphaseText("resolving ramming");

        for (int i = 0; i < rammingResolutions.Count; i++) {
            yield return rammingResolutions[i].resolve();
        }

        yield return null;
    }

    public static IEnumerator catapultChoices() {
        subPhaseProgress();
        if (!GameManager.main.needCatapultChoice) {
            yield break;
        }
        setSubphaseText("chose catapult targets");
        while (GameManager.main.needCatapultChoice) {
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

    public static IEnumerator sinkShips() {
        tempPopulateSinkAnimation();

        if (sinkAnimations.Count > 0) {
            setSubphaseText("sinking ships");
            foreach (SinkAnimation a in sinkAnimations) {
                yield return a.playAnimation(0.1f,0.1f);
            }
            sinkAnimations.Clear();
        }

    }

    public static void tempPopulateSinkAnimation() {
        foreach(Ship s in GameManager.main.getAllShips()) {
            if(s.life == 0) {
                sinkAnimations.Add(new SinkAnimation(s));
            }
        }
    }   

    public static IEnumerator resolvePortCapture() {
        
        if (captureAnimations.Count == 0) {
            yield break;
        }
        setSubphaseText("port capture");
        foreach (PortCaptureAnimation ca in captureAnimations) {
            
            yield return ca.playAnimation(SpeedManager.CaptureDelay,SpeedManager.CaptureSpeed);
        }
    }

    public static IEnumerator portCaptureChoice() {
        subPhaseProgress();
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

    public static void EnablePhaseUI() {
        UIControl.main.phaseAnnouncer.SetActive(true);
        //UIControl.main.subPhase.SetActive(true);
        //UIControl.main.subPhaseProgress.SetActive(true);
    }

    public static void DisablePhaseUI() {
        UIControl.main.phaseAnnouncer.SetActive(false);
        //UIControl.main.subPhase.SetActive(false);
        //UIControl.main.subPhaseProgress.SetActive(false);
    }

    public static void setSubphaseText(string s) {
        
        UIControl.main.subPhase.GetComponentInChildren<Text>().text = s;
    }

    public static void updateText() {
        int phase = GameManager.main.gameLogic.phaseIndex;
        
        GameObject phaseObj = UIControl.main.phase;
        phaseObj.SetActive(true);
        phaseObj.GetComponentInChildren<Text>().text = "Phase " + (phase + 1);
        GameManager.main.uiControl.GoText.text = "PHASE " + (phase + 1);

    }

    static void clearAnimationLists() {
        sinkAnimations.Clear();
    }

    static void removeShipFromCatapultAnimations(Ship s) {
        foreach(CombatResolution cr in catapultResolutions) {
            if( cr.shipA == s) {
                catapultResolutions.Remove(cr);
            }
        }
    }

    public static void addCatapultAnimation(Ship attacker, Ship target) {
        catapultResolutions.Add(new CatapultResolution(attacker,target,1));
    }

    public static void addCaptureAnimation(Ship s) {
        captureAnimations.Add(new PortCaptureAnimation(s));
    }

    public static void nextSubPhase() {
        SpeedManager.endSubPhaseSkip();
    }

    public static void subPhaseProgress() {

        GameObject outline = GameObject.Find("subphaseoutline");
        GameObject subPhaseIcon = null;
        
        switch (subPhaseIndex) {
            case 0:
            subPhaseIcon = GameObject.Find("actionphase"); break;
            case 1:
            subPhaseIcon = GameObject.Find("rammingphase"); break;
            case 2:
            subPhaseIcon = GameObject.Find("catapultphase"); break;
            case 3:
            subPhaseIcon = GameObject.Find("capturephase"); break;
            default:
            subPhaseIcon = null;break;
        }
        if(subPhaseIcon == null) {
            Debug.LogError("no valid subphase icon found");
        }


        outline.transform.position = subPhaseIcon.transform.position;
        subPhaseIndex++;
    }
    
    public static void addRammingResolution(Ship attacker, Ship target, int damage) {
        bool foundPair = false;
        foreach (RammingResolution rr in rammingResolutions) {
            if(rr.shipA == target && rr.shipB == attacker) {
                rr.damageToA = damage;
                foundPair = true;
                break;
            }
        }
        if (!foundPair) {
            involvedInRamming.Add(attacker);
            involvedInRamming.Add(target);
            rammingResolutions.Add(new RammingResolution(attacker,target,damage));
        }
    }

}

class RammingSorter : IComparer<CombatResolution> {
    public int Compare(CombatResolution x,CombatResolution y) {
        if (x.shipA.Position.x < y.shipA.Position.x) {
            return -1;
        } else {
            return (x.shipA.Position.y < y.shipA.Position.y) ? 1 : -1;
        }
    }
}
