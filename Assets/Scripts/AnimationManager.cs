using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AnimationManager 
{
    public static HashSet<Ship> involvedInRamming = new HashSet<Ship>();
    public static Dictionary<Ship,Animation> actionAnimations = new Dictionary<Ship,Animation>();
    //public static List<Animation> shipAnimations = new List<Animation>();
    public static List<CombatResolution> rammingResolutions = new List<CombatResolution>();
    public static List<CombatResolution> catapultResolutions = new List<CombatResolution>();
    //public static AnimationManager main;

    //public static void Awake() {
    //    main = this;
    //}

    public static bool movingCamera = false;

    public static bool playingAnimation = false;

    public static float nodeMultiShipScale = 0.3f;


    public static IEnumerator playAnimations() {
        playingAnimation = true;
        rammingResolutions.Sort(new RammingSorter());
        
        yield return playBasicActions();
        yield return playRammingActions();
        // resolve ramming choices
        // play catapult animations

        playingAnimation = false;
        actionAnimations.Clear();
        rammingResolutions.Clear();
        involvedInRamming.Clear();
        GameManager.main.gameLogic.postAnimation();
        yield return null;
    }

    static IEnumerator playRammingActions() {

        // sort list

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

    private static void drawMultipleShips(Node node,Graphics g) {

        List<Ship> shipsInNode = new List<Ship>();
        //BufferedImage image;
        int scale = 15;
        int percent = 100;
        //double fontSize = FONT_SIZE;
        for (int i = 1; i < node.getShips().Count; i++) {
            percent = percent * (100 - scale) / 100;
        }
        //fontSize = fontSize * ((double)percent / 100);
        for (int i = 0; i < node.getShips().Count; i++) {
            //AffineTransform transform = new AffineTransform();
            Ship ship = node.getShips()[i];
            //image = resizeImage(getShipImage(ship),percent);
            //transform.rotate(ship.getFront() * QUARTER_RADIANS,image.getWidth() / 2,image.getHeight() / 2);
            //AffineTransformOp op = new AffineTransformOp(transform,AffineTransformOp.TYPE_BILINEAR);
            //image = op.filter(image,null);
            int x = ship.getNode().getY() * 60 + ((scale * percent / 100) * i);
            int y = ship.getNode().getX() * 60 + 10;
            //g.drawImage(image,x,y,null);
           // shipsInNode.add(new ShipImage(x,y,ship.getID(),ship.getLife()));
        }
        //addMultipleShipDetails(g,shipsInNode,(int)fontSize);
    }

    static void sortRammingOrder() {

    }

    public static Vector2 shipNodePos(Ship s,Node n) {

        //float scale = 0.3f;
        

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
        margin = bv[1].y * margin;
        if(v.x < bv[0].x + margin || v.x > bv[0].x + bv[1].x - margin || v.y < bv[0].y + margin || v.y > bv[0].y + +bv[1].y - margin) {
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
