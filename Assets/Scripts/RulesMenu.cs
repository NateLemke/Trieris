using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulesMenu : MonoBehaviour
{
    GameObject image;

    WindowData minimap = new WindowData(205f, 93f, 126f, 87f, -155f, -78f);

    WindowData infoMain = new WindowData(230f, 0f, 125f, 220f, 0, 0);

    Text infoText;
    GameObject infoPanel;

    GameObject content;

    string position;

    List<string> catapultList;

    struct WindowData
    {
        private float x;
        private float y;
        private float width;
        private float height;

        private float x2;
        private float y2;

        public float X { get { return x; } }
        public float Y { get { return y; } }
        public float Width { get { return width; } }
        public float Height { get { return height; } }

        public float X2 { get { return x2; } }
        public float Y2 { get { return y2; } }

        public WindowData(float x, float y, float width, float height, float x2, float y2)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            this.x2 = x2;
            this.y2 = y2;
        }
    }

    void Start()
    {
        content = transform.Find("Content").gameObject;

        infoPanel = transform.Find("InfoPanel").gameObject;
        infoText = transform.Find("InfoPanel/Text").GetComponent<Text>();
        
        moveToActions();
    }

    public void moveToActions()
    {
        setImagesInactive();
        content.transform.Find("Actions").gameObject.SetActive(true);
        
        infoText.text = "Every turn, there are 4 phases in which actions can be set. You can only set actions for as much life as your ship has.\nExample: your ship has 3 life remaining, you can only set 3 actions for that ship";
    }

    public void moveToCatapults()
    {
        setImagesInactive();
        content.transform.Find("CatapultTarget").gameObject.SetActive(true);
        content.transform.Find("Catapult").gameObject.SetActive(true);

        infoText.text = "On the righthand side of the screen, there is a catapult targeting interface. On any turn, you may set a phase to aim and shoot a catapult for 1 damage.\nIf your ship is rammed, neither ship may shoot for the rest of the turn";
    }

    public void moveToRamming()
    {
        setImagesInactive();
        content.transform.Find("Broadside").gameObject.SetActive(true);
        content.transform.Find("Glancing1").gameObject.SetActive(true);
        content.transform.Find("HeadOn").gameObject.SetActive(true);
        content.transform.Find("Momentum").gameObject.SetActive(true);

        infoText.text = "When a ship moves into a node that contains another ship, it will ram that ship. If two ships move into the same space, they will ram each other! Broadside:\nShip rams another ship that is perpendicular to itself.\nGlancing:\n Ship rams another ship at an angle.\nHead On:\n Two ships ram each other while facing each other. If they are adjacent to each other, they will not enter each other's nodes";
    }

    public void moveToMultipleTargets()
    {
        setImagesInactive();
        content.transform.Find("MultipleTarget").gameObject.SetActive(true);

        infoText.text = "When a catapult or ram targets a node that contains multiple ships, you will get to choose the ship that damage is dealt to.";
    }

    public void moveToRedirection()
    {
        setImagesInactive();
        content.transform.Find("Redirection1").gameObject.SetActive(true);
        content.transform.Find("Redirection2").gameObject.SetActive(true);

        infoText.text = "Whenever a ship runs into land or captures a port/capital, that ship loses the rest of its actions this turn and can immediately choose the direction that it faces.";
    }

    public void moveToPorts()
    {
        setImagesInactive();
        content.transform.Find("Port").gameObject.SetActive(true);

        infoText.text = "When a ship lands on a port or capital, the player may choose to capture it. Doing so will make that ship lose its actions for the rest of the turn.\n If you hold on a port or capital that you own, your ship will begin to repair. On a port, you repair once a turn and on a capital, you repair every phase.";
    }

    public void moveToPortsCapitals()
    {
        setImagesInactive();
        content.transform.Find("Capital").gameObject.SetActive(true);

        infoText.text = "If a team controls 12 ports at any given time, that team wins the game. However, losing your team's capital will immediately cause you to lose.";
    }
    
    public void moveToShipsAlive()
    {
        setImagesInactive();
        content.transform.Find("Score").gameObject.SetActive(true);

        infoText.text = "If your team has no ships remaining, your team also loses. Even if you own more ports than your opponents. The number at the top is the number of ports you own and the ship icons are how many ships you own alive.";
    }
    
    public void setImagesInactive()
    {
        foreach(Transform child in transform.Find("Content"))
        {
            child.gameObject.SetActive(false);
        }
    }

    public void exitHelp()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
