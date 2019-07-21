using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Purpose: This class manages the timing and execution of a phase
///             other classes populate the resolutions and animations lists and the phase managers iterates through each list and plays the 
///                animation or resolves an instance of combat
///                
///             Phase manager also has subphases that wait for player choices like port captures and target selection
/// </summary>
public static class PhaseManager 
{
    // a unique set of ships involved in ramming
    // this set is used to determine which action animations to skip
    public static HashSet<Ship> involvedInRamming = new HashSet<Ship>();

    // the basic action animations for ships, such as turning or forwards movement
    // each ship can only have one action animation perphase
    public static Dictionary<Ship,Animation> actionAnimations = new Dictionary<Ship,Animation>();

    // ship combat resolutions
    public static List<CombatResolution> rammingResolutions = new List<CombatResolution>();
    public static List<CombatResolution> catapultResolutions = new List<CombatResolution>();

    // misc animations
    public static List<SinkAnimation> sinkAnimations = new List<SinkAnimation>();    
    public static List<PortCaptureAnimation> captureAnimations = new List<PortCaptureAnimation>();

    // player input choices
    public static List<ShipTargetResolution> catapultTargetResolutions = new List<ShipTargetResolution>();
    public static List<ShipTargetResolution> rammingTargetResolutions = new List<ShipTargetResolution>();

    static int subPhaseIndex = 0;
  

    // when the player choses a target from a target resolution, the choice is stored here
    public static Ship chosenTarget = null;

    // subphases are executed as IEnumerator Coroutines for timing
    delegate IEnumerator subPhase();

    // this is the order in which subphases are executed 
    static subPhase[] subPhaseOrder = {
        resolveBasicActions ,
        sinkShips,
        resolveRedirects,
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
        //playingAnimation = true;
        subPhaseIndex = 0;
        yield return null;

        subPhaseProgress();
        updateText();

        foreach(subPhase s in subPhaseOrder) {
            yield return s();
            nextSubPhase();
            sinkAnimations.Clear();
        }
        
        actionAnimations.Clear();
        rammingResolutions.Clear();
        involvedInRamming.Clear();
        captureAnimations.Clear();
        catapultResolutions.Clear();

        GameManager.main.gameLogic.postAnimation();
        
    }


    /// <summary>
    /// This function caluclates the world size and the world position of the viewable world area from the game UI
    /// </summary>
    /// <returns>2 Vector2s, the first Vector2 is the upper left point of the viewable area, the second is the dimensions</returns>
    public static Vector2[] getBoardView() {
        float camHeight = Camera.main.orthographicSize * 2f;
        float camWidth = camHeight * Camera.main.aspect;
        Vector2 camPos = Camera.main.transform.position;
        float canvasWidth = GameObject.Find("OverlayCanvas").GetComponent<RectTransform>().rect.width;
        float canvasHeight = GameObject.Find("OverlayCanvas").GetComponent<RectTransform>().rect.height;
        float sideUIWidth = GameObject.Find("UISidePanel").GetComponent<RectTransform>().rect.width;
        float bottomUIHeight = GameObject.Find("UIBottomPanel").GetComponent<RectTransform>().rect.height;
        float widthRatio = (canvasWidth - sideUIWidth) / canvasWidth;
        float heightRatio = (canvasHeight - bottomUIHeight) / canvasHeight;
        Vector2[] r = new Vector2[2];
        r[0] = camPos - new Vector2(camWidth / 2,camHeight / -2);
        r[1] = new Vector2(camWidth * widthRatio,camHeight * -heightRatio);
        return r;
    }

    /// <summary>
    /// Used to create a list of vector2s to draw a cross on the given position
    /// </summary>
    /// <param name="v">the position to center the cross on</param>
    /// <param name="f">the size of the cross</param>
    /// <returns>an array of Vector2s</returns>
    public static Vector2[] crossOnPoint(Vector2 v,float f) {
        Vector2[] r = new Vector2[4];
        r[0] = new Vector2(v.x,v.y + f);
        r[1] = new Vector2(v.x,v.y - f);
        r[2] = new Vector2(v.x + f,v.y);
        r[3] = new Vector2(v.x - f,v.y);
        return r;
    }

    /// <summary>
    /// Used to create a list of vector2s to draw an x on the given position
    /// </summary>
    /// <param name="v">the position to center the x on</param>
    /// <param name="f">the size of the x</param>
    /// <returns>an array of Vector2s</returns>
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

    /// <summary>
    /// Used to create a list of vector2s to draw an 8 pointed start on the given position
    /// </summary>
    /// <param name="v">the position to center the star on</param>
    /// <param name="f">the size of the star</param>
    /// <returns>an array of Vector2s</returns>
    public static List<Vector2> starOnPoint(Vector2 v,float f) {
        List<Vector2> r = new List<Vector2>();
        r.AddRange(crossOnPoint(v,f));
        r.AddRange(xOnPoint(v,f));
        return r;
    }

    /// <summary>
    /// Draws a star in debug mode on the given point
    /// Used to show a coordinate when debugging
    /// </summary>
    /// <param name="v">the position to draw the star</param>
    /// <param name="f">the size of the star</param>
    /// <param name="c">the color of the star</param>
    /// <param name="duration">the duration to draw the star</param>
    public static void debugStarPoint(Vector2 v,float f,Color c,float duration = 0) {
        List<Vector2> points = starOnPoint(v,f);
        for (int i = 0; i < 8; i += 2) {
            Debug.DrawLine(points[i],points[i + 1],c,duration);
        }
    }

    /// <summary>
    /// Returns the world position for the center of the board view
    /// </summary>
    /// <returns></returns>
    public static Vector2 boardviewCenter() {
        Vector2[] bv = getBoardView();
        return (bv[0] + bv[1] / 2);
    }

    /// <summary>
    /// Draws the camera focus area in debug modes
    /// </summary>
    public static void drawFocusMargin() {
        Vector2[] bv = getBoardView();
        Debug.DrawLine(bv[0],bv[0] + bv[1],Color.white);

        float focusMargin = 0.08f * ((10f - Camera.main.orthographicSize) / 10f);

        float xMargin = bv[1].x * focusMargin;
        float yMargin = bv[1].y * focusMargin;

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

    /// <summary>
    /// Checks to see if the given coordinate is "in focus" or not
    /// A coordinate is considered in focus if it is within the focus rect of the viewable board area as seen with drawFocusMargin()
    /// </summary>
    /// <param name="v">the coordinate to text</param>
    /// <returns>true if the coordinate is out of focus</returns>
    public static bool outOfFocus(Vector2 v) {

        Vector2[] bv = getBoardView();
        float focusMargin = 0.08f * ( (10f -  Camera.main.orthographicSize) / 10f) ;
        float xMargin = bv[1].x * focusMargin;
        float yMargin = bv[1].y * focusMargin;

        float xMin = bv[0].x + xMargin;
        float xMax = bv[0].x + bv[1].x - xMargin;
        float yMin = bv[0].y + bv[1].y - yMargin;
        float yMax = bv[0].y + yMargin * 3;

        return (v.x < xMin || v.x > xMax || v.y < yMin || v.y > yMax);
    }

    /// <summary>
    /// Checks if the coordinate is out of focus, and moves the camera to that position if it is
    /// </summary>
    /// <param name="v">the position to focus on</param>
    /// <returns></returns>
    [PunRPC]
    public static IEnumerator focus(Vector2 v) {

        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
            PhotonView.Get(GameManager.main).RPC("focus",RpcTarget.Others,v.x,v.y);
        }

        if (outOfFocus(v)) {

            Vector2[] bv = getBoardView();
            Vector2 offset = (Vector2)Camera.main.transform.position - boardviewCenter();
            Vector3 pos = v + offset;
            yield return moveCameraTo(pos,SpeedManager.CameraFocusSpeed);
            pos.z = Camera.main.transform.position.z;
            Camera.main.transform.position = pos;
        }
    }


    /// <summary>
    /// Moves the camera to the given position over over a given time of seconds
    /// </summary>
    /// <param name="pos">the position to move the camera to</param>
    /// <param name="duration">the time in seconds the movement should take</param>
    /// <returns></returns>
    public static IEnumerator moveCameraTo(Vector3 pos, float duration) {
        Vector3 startPos = Camera.main.transform.position;
        pos.z = startPos.z;
        //movingCamera = true;
        float startTime = Time.time;
        while((Time.time - startTime) / duration < 1f) {
            Camera.main.transform.position = Vector3.Lerp(startPos,pos,(Time.time - startTime) / duration);
            yield return null;
        }
        //movingCamera = false;
    }

    /// <summary>
    /// Iterates through the list of action animations and plays them sequentially, skipping over ships involved in ramming
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Iterates through the list of pending ramming target choices and waits until each is resolved before continuing
    /// </summary>
    /// <returns></returns>
    public static IEnumerator rammingChoices() {
        
        if (rammingTargetResolutions.Count == 0) {
            yield break;
        }
        setSubphaseText("choose ramming targets");
        foreach (ShipTargetResolution tr in rammingTargetResolutions) {
            if (!tr.needsResolving()) {
                continue;
            }
            yield return focus(tr.attacker.Position);
            yield return tr.resolve();
            if (chosenTarget == null) {
                continue;
            }
            tr.attacker.ram(chosenTarget);
            yield return rammingResolutions[rammingResolutions.Count-1].resolve();
            tr.attacker.needRammingChoice = false;
        }
        while (GameManager.main.needRammingChoice()) {
            if (!GameManager.playerTeam.needRammingChoice()) {
                // waiting for another player to be ready
            }
            yield return null;
        }
    }

    /// <summary>
    /// Iterates through all ramming animations and plays each sequentially
    /// </summary>
    /// <returns></returns>
    static IEnumerator resolveRamming() {
        subPhaseProgress();
        if (rammingResolutions.Count == 0) {
            yield break;
        }
        //rammingResolutions.Sort(new RammingSorter());
        setSubphaseText("resolving ramming");

        for (int i = 0; i < rammingResolutions.Count; i++) {
            yield return rammingResolutions[i].resolve();
        }

        yield return null;
    }



    /// <summary>
    /// Iterates through the list of pending catapult target choices and waits until each is resolved before continuing
    /// </summary>
    /// <returns></returns>
    public static IEnumerator catapultChoices() {
        subPhaseProgress();
        if (catapultTargetResolutions.Count == 0) {
            yield break;
        }
        setSubphaseText("chose catapult targets");

        foreach(ShipTargetResolution tr in catapultTargetResolutions) {
            if (!tr.attacker.CanFire || !tr.needsResolving()) {
                tr.attacker.needCatapultChoice = false;
                continue;
            }
            yield return focus(tr.attacker.Position);
            yield return tr.resolve();
            if(chosenTarget == null) {
                continue;
            }


            yield return new CatapultResolution(tr.attacker,chosenTarget,1).resolve();
            tr.attacker.needCatapultChoice = false;
        }

        while (GameManager.main.needCatapultChoice()) {
            if (!GameManager.playerTeam.needCatapultChoice()) {
                // waiting for another player to be ready
            }
            yield return null;
        }
    }

    /// <summary>
    /// Plays all catapult animations sequentially
    /// </summary>
    /// <returns></returns>
    public static IEnumerator resolveCatapults() {
        if(catapultResolutions.Count > 0) {
            setSubphaseText("resolving catapults");
            foreach(CatapultResolution cr in catapultResolutions) {

                yield return cr.resolve();
                cr.shipA.CanFire = false;
            }
        }        
    }    

    /// <summary>
    /// Checks for ships with zero life and plays a sinking animation for each sequentially
    /// Destroys ship gameobjects when done
    /// </summary>
    /// <returns></returns>
    public static IEnumerator sinkShips() {
        populateSinkAnimations();

        if (sinkAnimations.Count > 0) {
            setSubphaseText("sinking ships");
            for (int i = 0; i < sinkAnimations.Count; i++)
            {
                yield return sinkAnimations[i].playAnimation();
            }
            sinkAnimations.Clear();
        }
    }

    /// <summary>
    /// Checks for ships with zero life and populates the list of SinkAnimations
    /// </summary>
    public static void populateSinkAnimations() {
        foreach(Ship s in GameManager.main.getAllShips()) {
            if(s.life <= 0) {
                sinkAnimations.Add(new SinkAnimation(s));
            }
        }
    }

    /// <summary>
    /// Iterates through the list of port capture animations and plays each sequentially
    /// </summary>
    /// <returns></returns>
    public static IEnumerator resolvePortCapture() {
        
        if (captureAnimations.Count == 0) {
            yield break;
        }
        setSubphaseText("port capture");
        foreach (PortCaptureAnimation ca in captureAnimations) {
            
            yield return ca.playAnimation();
        }
    }

    /// <summary>
    /// Iterates through the list of port capture choices and waits until each is resolved before continuing
    /// </summary>
    /// <returns></returns>
    public static IEnumerator portCaptureChoice() {
        subPhaseProgress();
        if (!GameManager.main.needCaptureChoice()) {
            yield break;
        }
        setSubphaseText("choose port capture");

        Ship focusTarget = null;
        foreach(Ship s in GameManager.main.getPlayerShips()) {
            
            if (s.needCaptureChoice) {
                focusTarget = s;
                s.getNode().Port.activatePrompt(s);
                yield return focus(focusTarget.Position);
                while (s.needCaptureChoice || s.needRedirect)
                    yield return null;
            }
        }
        while (GameManager.main.needCaptureChoice()) {
            if (!GameManager.playerTeam.needCaptureChoice()) {
                // waiting for another player to be ready
            }
            yield return null;
        }
    }

    /// <summary>
    /// Waits for all redirect choices to be made before continuing
    /// </summary>
    /// <returns></returns>
    public static IEnumerator resolveRedirects() {
        if (!GameManager.main.needRedirect()) {
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
        while (GameManager.main.needRedirect()) {
            if (!GameManager.playerTeam.needRedirectChoice()) {
                // waiting for another player
            }
            yield return null;
        }        
    }

    /// <summary>
    /// Enables the phase UI
    /// </summary>
    public static void EnablePhaseUI() {
        UIControl.main.phaseAnnouncer.SetActive(true);
    }

    /// <summary>
    /// Disables the phase UI
    /// </summary>
    public static void DisablePhaseUI() {
        UIControl.main.phaseAnnouncer.SetActive(false);
    }

    /// <summary>
    /// Sets the subphase text in the phase UI
    /// </summary>
    /// <param name="s"></param>
    public static void setSubphaseText(string s) {        
        UIControl.main.subPhase.GetComponentInChildren<Text>().text = s;
    }

    /// <summary>
    /// Updates the UI to display the proper phase index
    /// </summary>
    public static void updateText() {
        int phase = GameLogic.phaseIndex;
        
        GameObject phaseObj = UIControl.main.phase;
        phaseObj.SetActive(true);
        phaseObj.GetComponentInChildren<Text>().text = "Phase " + (phase + 1);
        GameManager.main.uiControl.GoText.text = "PHASE " + (phase + 1);

    }

    /// <summary>
    /// Iterates through all catapults animations and clears it from the list if it contains the given ship
    /// </summary>
    /// <param name="s">the ship to check for</param>
    static void removeShipFromCatapultAnimations(Ship s) {
        foreach(CombatResolution cr in catapultResolutions) {
            if( cr.shipA == s) {
                catapultResolutions.Remove(cr);
            }
        }
    }

    /// <summary>
    /// Adds a new catapult animation to the list
    /// </summary>
    /// <param name="attacker">the attacking ship</param>
    /// <param name="target">the ship targetting by the attacking ship</param>
    public static void addCatapultAnimation(Ship attacker, Ship target) {
        if (attacker.CanFire)
        {
            catapultResolutions.Add(new CatapultResolution(attacker, target, 1));
            
        }
    }

    /// <summary>
    /// Adds a capture animation to the list
    /// </summary>
    /// <param name="s">the ship that's going to capture</param>
    public static void addCaptureAnimation(Ship s) {
        captureAnimations.Add(new PortCaptureAnimation(s));
    }

    /// <summary>
    /// Used to trigger an end to a subphase skip in the speed manager
    /// </summary>
    public static void nextSubPhase() {
        SpeedManager.endSubPhaseSkip();
    }

    /// <summary>
    /// Used to display the progress in the subphase icons
    /// </summary>
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
    
    /// <summary>
    /// Either creates a new ramming resolution OR adds the damage to an existing ramming resolution that includes the attacker/target pair of ships
    /// </summary>
    /// <param name="attacker">the attacking ship</param>
    /// <param name="target">the target of the ram</param>
    /// <param name="damage">damage to the target ship</param>
    /// <param name="damageToSelf">damage to the defending ship</param>
    public static void addRammingResolution(Ship attacker, Ship target, int damage,int damageToSelf=0) {

        bool foundPair = false;
        foreach (RammingResolution rr in rammingResolutions) {
            if(rr.shipA == target && rr.shipB == attacker) {
                rr.damageToA += damage;
                rr.damageToB += damageToSelf;

                foundPair = true;
                break;
            }
        }
        if (!foundPair) {
            involvedInRamming.Add(attacker);
            involvedInRamming.Add(target);

            rammingResolutions.Add(new RammingResolution(attacker,target,damage,damageToSelf));
        }

    }

    /// <summary>
    /// Adds a catapult animation to the list for a missed shot
    /// </summary>
    /// <param name="s">the ship thats shooting</param>
    /// <param name="n">the node they're aiming at</param>
    public static void addMissedShot(Ship s, Node n) {
        if (s.CanFire)
        {
            CatapultResolution cr = new CatapultResolution(s, null, 0, n);

            catapultResolutions.Add(cr);

            
        }
    }

    /// <summary>
    /// Adds an animation for a adjacent headon ram
    /// Either creates a new rammingResolution OR adds the damage to an existing resolution with the matching AB pair
    /// </summary>
    /// <param name="a">shipA</param>
    /// <param name="b">shipB</param>
    public static void addAdjHeadOnRamming(Ship a, Ship b) {
        bool foundPair = false;
        int dmg = (a.getMomentum() == 0) ? 1 : a.getMomentum();
        foreach(RammingResolution rr in rammingResolutions) {
            if(rr.shipA == b && rr.shipB == a) {
                rr.damageToA = dmg;
                foundPair = true;
                break;
            }
        }

        if (!foundPair) {
            rammingResolutions.Add(new HeadOnRammingResolution(a,b,dmg));
        }
    }

}