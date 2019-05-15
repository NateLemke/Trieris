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
    public static List<ShipTargetResolution> catapultTargetResolutions = new List<ShipTargetResolution>();
    public static List<ShipTargetResolution> rammingTargetResolutions = new List<ShipTargetResolution>();

    static int subPhaseIndex = 0;

    public static bool movingCamera = false;

    public static bool playingAnimation = false;

    public const float nodeMultiShipScale = 0.34f;

    public static Ship chosenTarget = null;

    delegate IEnumerator subPhase();

    static subPhase[] subPhaseOrder = {
        resolveBasicActions ,
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

    /// <summary>
    /// runs through the subphases for the current phase
    /// executes ramming, movement, port capture, etc
    /// </summary>
    /// <returns></returns>
    public static IEnumerator playPhaseAnimations() {
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

    public static Vector2 shipNodePos(Ship s,Node n, float xSpace = nodeMultiShipScale,float ySpace = 0.3f) {
        
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

        float offset = (rounded - 1) * xSpace / 2f;
        Vector2 pos = new Vector2(x * xSpace - offset,-y * ySpace);

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
        float canvasHeight = GameObject.Find("OverlayCanvas").GetComponent<RectTransform>().rect.height;
        float sideUIWidth = GameObject.Find("UISidePanel").GetComponent<RectTransform>().rect.width;
        float bottomUIHeight = GameObject.Find("UIBottomPanel").GetComponent<RectTransform>().rect.height;
        float widthRatio = (canvasWidth - sideUIWidth) / canvasWidth;
        float heightRatio = (canvasHeight - bottomUIHeight) / canvasHeight;
        //Debug.DrawLine(camPos + new Vector2(-camWidth / 2,2),camPos + new Vector2(camWidth * ratio / 2,2),Color.green);
        Vector2[] r = new Vector2[2];
        r[0] = camPos - new Vector2(camWidth / 2,camHeight / -2);
        r[1] = new Vector2(camWidth * widthRatio,camHeight * -heightRatio);
        return r;
    }



    public static Vector2[] crossOnPoint(Vector2 v,float f) {
        Vector2[] r = new Vector2[4];
        r[0] = new Vector2(v.x,v.y + f);
        r[1] = new Vector2(v.x,v.y - f);
        r[2] = new Vector2(v.x + f,v.y);
        r[3] = new Vector2(v.x - f,v.y);
        return r;
    }

    public static Vector2[] xOnPoint(Vector2 v,float f) {
        float sqr = Mathf.Sqrt(2);
        float val = Mathf.Sqrt(Mathf.Pow(f,2) / 2);
        Vector2[] r = new Vector2[4];
        r[0] = new Vector2(v.x + val,v.y + val);
        r[1] = new Vector2(v.x - val,v.y - val);
        r[2] = new Vector2(v.x + val,v.y - val);
        r[3] = new Vector2(v.x - val,v.y + val);

        return r;
    }

    public static List<Vector2> starOnPoint(Vector2 v,float f) {
        List<Vector2> r = new List<Vector2>();
        r.AddRange(crossOnPoint(v,f));
        r.AddRange(xOnPoint(v,f));
        return r;
    }

    public static void debugStarPoint(Vector2 v,float f,Color c,float duration = 0) {
        List<Vector2> points = starOnPoint(v,f);
        for (int i = 0; i < 8; i += 2) {
            Debug.DrawLine(points[i],points[i + 1],c,duration);
        }
    }

    public static Vector2 boardviewCenter() {
        Vector2[] bv = getBoardView();
        return (bv[0] + bv[1] / 2);
    }

    public static void drawFocusMargin() {
        Vector2[] bv = getBoardView();
        Debug.DrawLine(bv[0],bv[0] + bv[1],Color.red);

        float xMargin = bv[1].x * 0.08f;
        float yMargin = bv[1].y * 0.08f;

        float xMin = bv[0].x + xMargin;
        float xMax = bv[0].x + bv[1].x - xMargin;
        float yMin = bv[0].y + bv[1].y - yMargin;
        float yMax = bv[0].y + yMargin * 3;        

        Vector2 v1 = new Vector2(xMin,yMax);
        Vector2 v2 = new Vector2(xMin,yMin);
        Vector2 v3 = new Vector2(xMax,yMax);
        Vector2 v4 = new Vector2(xMax,yMin);

        Debug.DrawLine(v1,v2,Color.green);
        Debug.DrawLine(v1,v3,Color.red);
        Debug.DrawLine(v2,v4,Color.yellow);
        Debug.DrawLine(v3,v4,Color.blue);

        debugStarPoint(Camera.main.transform.position,0.2f,Color.red);
        debugStarPoint(bv[0] + bv[1] / 2,0.2f,Color.green);
    }

    public static bool outOfFocus(Vector2 v) {

        Vector2[] bv = getBoardView();
        float focusMargin = 0.08f;
        float xMargin = bv[1].x * focusMargin;
        float yMargin = bv[1].y * focusMargin;

        float xMin = bv[0].x + xMargin;
        float xMax = bv[0].x + bv[1].x - xMargin;
        float yMin = bv[0].y + bv[1].y - yMargin;
        float yMax = bv[0].y + yMargin * 3;

        return (v.x < xMin || v.x > xMax || v.y < yMin || v.y > yMax);
    }

    public static IEnumerator focus(Vector2 v) {

        if (outOfFocus(v)) {

            //debugStarPoint(Camera.main.transform.position,0.2f,Color.red);
            //debugStarPoint(bv[0] + bv[1] / 2,0.2f,Color.green);
            Vector2[] bv = getBoardView();
            Vector2 offset = (Vector2)Camera.main.transform.position - boardviewCenter();
            Vector3 pos = v + offset;
            yield return moveCameraTo(pos,SpeedManager.CameraFocusSpeed);
            pos.z = Camera.main.transform.position.z;
            Camera.main.transform.position = pos;
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

    static IEnumerator resolveBasicActions() {
        setSubphaseText("resolving actions");
        List<Animation> anims = actionAnimations.Values.ToList();

        bool pendingAnimation = true;
        while (pendingAnimation) {
            pendingAnimation = false;
            Animation closestToCamera = null;
            float closestDistance = Mathf.Infinity;
            bool foundFocus = false;

            List<Animation> inFocus = new List<Animation>();

            foreach (Animation a in anims) {

                if(!involvedInRamming.Contains(a.ship) && !a.complete) {
                    pendingAnimation = true;
                    if (!outOfFocus(a.focusPoint)) {
                        inFocus.Add(a);
                    } else {
                        if(Vector2.Distance(a.focusPoint,boardviewCenter()) < closestDistance) {
                            closestDistance = Vector2.Distance(a.focusPoint,boardviewCenter());
                            closestToCamera = a;
                            foundFocus = true;
                        }
                    }
                }                
            }

            foreach(Animation a in inFocus) {
                yield return a.playAnimation();
            }

            if (foundFocus) {
                yield return focus(closestToCamera.focusPoint);            
            }
        }

        

        yield return null;
    }

    public static IEnumerator rammingChoices() {
        subPhaseProgress();
        if (rammingTargetResolutions.Count == 0) {
            yield break;
        }
        setSubphaseText("choose ramming targets");
        foreach (ShipTargetResolution tr in rammingTargetResolutions) {

            yield return focus(tr.attacker.Position);
            yield return tr.resolve();
            tr.attacker.ram(chosenTarget);
            yield return rammingResolutions[rammingResolutions.Count-1].resolve();
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
        if (catapultTargetResolutions.Count == 0) {
            yield break;
        }
        setSubphaseText("chose catapult targets");

        foreach(ShipTargetResolution tr in catapultTargetResolutions) {
            yield return focus(tr.attacker.Position);
            yield return tr.resolve();
            yield return new CatapultResolution(tr.attacker,chosenTarget,1).resolve();
        }


    }

    public static IEnumerator resolveCatapults() {
        if(catapultResolutions.Count > 0) {
            setSubphaseText("resolving catapults");
            foreach(CatapultResolution cr in catapultResolutions) {
                if (!cr.interrupted)
                    yield return cr.resolve();
            }
        }        
    }    

    public static IEnumerator sinkShips() {
        tempPopulateSinkAnimation();

        if (sinkAnimations.Count > 0) {
            setSubphaseText("sinking ships");
            foreach (SinkAnimation a in sinkAnimations) {
                yield return a.playAnimation();
            }
            sinkAnimations.Clear();
        }

    }

    public static void tempPopulateSinkAnimation() {
        foreach(Ship s in GameManager.main.getAllShips()) {
            if(s.life <= 0) {
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
            
            yield return ca.playAnimation();
        }
    }

    public static IEnumerator portCaptureChoice() {
        subPhaseProgress();
        if (!GameManager.main.needCaptureChoice) {
            yield break;
        }
        setSubphaseText("choose port capture");

        Ship focusTarget = null;
        foreach(Ship s in GameManager.main.getPlayerShips()) {
            
            if (s.needCaptureChoice) {
                focusTarget = s;
                s.getNode().getPort().activatePrompt(s);
                yield return focus(focusTarget.Position);
                while (s.needCaptureChoice || s.needRedirect)
                    yield return null;
            }
        }
        while (GameManager.main.needCaptureChoice) {
            yield return null;
        }
    }

    public static IEnumerator resolveRedirects() {
        if (!GameManager.main.needRedirect) {
            yield break;
        }
        setSubphaseText("choose redirects");
        Ship focusTarget = null;
        foreach (Ship s in GameManager.main.getPlayerShips()) {
            if (s.needRedirect) {
                focusTarget = s;
            }
        }
        yield return focus(focusTarget.Position);
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
        if (attacker.CanFire)
        {
            catapultResolutions.Add(new CatapultResolution(attacker, target, 1));
            attacker.CanFire = false;
        }
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
    
    public static void addRammingResolution(Ship attacker, Ship target, int damage,int damageToSelf=0) {
//RammingResolution r = null;
        bool foundPair = false;
        foreach (RammingResolution rr in rammingResolutions) {
            if(rr.shipA == target && rr.shipB == attacker) {
                rr.damageToA += damage;
                rr.damageToB += damageToSelf;
                //r = rr;
                foundPair = true;
                break;
            }
        }
        if (!foundPair) {
            involvedInRamming.Add(attacker);
            involvedInRamming.Add(target);
            //r = 
            rammingResolutions.Add(new RammingResolution(attacker,target,damage,damageToSelf));
        }
        //return r;
    }

    public static void addMissedShot(Ship s, Node n) {
        if (s.CanFire)
        {
            CatapultResolution cr = new CatapultResolution(s, null, 0, n);

            catapultResolutions.Add(cr);

            s.CanFire = false;
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
