using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used to run the Controls menu opened from the pause menu and at launch
/// </summary>
public class HelpMenu : MonoBehaviour
{
    GameObject background;
    GameObject image;
    GameObject mask;
    Vector3 childLocation;

    WindowData minimap = new WindowData(155f, 78f, 126f, 87f, -155f, -78f);
    WindowData shipTab = new WindowData(155f, 25f, 130f, 30f, -155f, -25f);
    WindowData combatPhase = new WindowData(155f, -31f, 125f, 27f, -155f, 31f);
    WindowData combatDirection = new WindowData(185.5f, -76f, 65f, 72f, -185.5f, 76f);
    WindowData movementPhase = new WindowData(158f, -6.3f, 127f, 37f, -158f, 6.3f);
    WindowData movementAction = new WindowData(126f, -75f, 70f, 70f, -126, 75f);

    WindowData cameraControls = new WindowData(-67f, 10f, 310f, 231f, 67f, -10f);
    WindowData direction = new WindowData(-62f, -37f, 25f, 25f, 62f, 37f);

    WindowData infoMain = new WindowData(50f, 60f, 180f, 125f, 0, 0);
    WindowData infoMap = new WindowData(205f, 50f, 125f, 150f, 0, 0);

    Text infoText;
    GameObject infoPanel;

    private List<Action> instructionList = new List<Action>();
    private int position;

    /// <summary>
    /// Struct used to store the window sizes of the mask and image for the help menu.
    /// X and Y are used for the mask locations and X2 and Y2 are used for the image locations
    /// </summary>
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

    /// <summary>
    /// Sets the gameobject references at the first frame the object is loaded and then calls the first function for displaying controls.
    /// </summary>
    void Start()
    {
        background = transform.Find("Controls/Content/Background").gameObject;
        mask = transform.Find("Controls/Content/Mask").gameObject;
        image = transform.Find("Controls/Content/Mask/Image").gameObject;
        infoPanel = transform.Find("Controls/InfoPanel").gameObject;
        infoText = transform.Find("Controls/InfoPanel/Text").GetComponent<Text>();



        position = 0;
        moveToMinimap();

        instructionList.Add(moveToMinimap);
        instructionList.Add(moveToShipTabs);
        instructionList.Add(moveToCombatPhase);
        instructionList.Add(moveToCombatDirection);
        instructionList.Add(moveToMovementPhase);
        instructionList.Add(moveToMovementAction);
        instructionList.Add(moveToCameraControls);
        instructionList.Add(moveToDirections);
    }

    public void next()
    {
        position++;
        if (position >= instructionList.Count)
            position = instructionList.Count - 1;
        instructionList[position]();
    }

    public void prev()
    {
        position--;
        if (position < 0)
            position = 0;
        instructionList[position]();
    }

    public void updatePosition(int input)
    {
        position = input;
    }

    /// <summary>
    /// Moves the cursor to the Minimap sections and updates the text of the infoPanel
    /// </summary>
    public void moveToMinimap()
    {
        mask.transform.localPosition = new Vector3(minimap.X, minimap.Y, 0);
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(minimap.Width, minimap.Height);
        image.transform.localPosition = new Vector3(minimap.X2, minimap.Y2, 0);
        infoPanel.transform.localPosition = new Vector3(infoMain.X, infoMain.Y, 0);
        infoPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(infoMain.Width, infoMain.Height);
        infoText.text = "This is the minimap. It will display the locations of all ships as well as the ports and their respective colors";
    }


    /// <summary>
    /// Moves the cursor to the Ship Tab sections and updates the text of the infoPanel
    /// </summary>
    public void moveToShipTabs()
    {
        mask.transform.localPosition = new Vector3(shipTab.X, shipTab.Y, 0);
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(shipTab.Width, shipTab.Height);
        image.transform.localPosition = new Vector3(shipTab.X2, shipTab.Y2, 0);
        infoPanel.transform.localPosition = new Vector3(infoMain.X, infoMain.Y, 0);
        infoPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(infoMain.Width, infoMain.Height);
        infoText.text = "Here you can select the ship you would like to set the actions for.";
    }
    
    /// <summary>
    /// Moves the cursor to the Combat Phase sections and updates the text of the infoPanel
    /// </summary>
    public void moveToCombatPhase()
    {
        mask.transform.localPosition = new Vector3(combatPhase.X, combatPhase.Y, 0);
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(combatPhase.Width, combatPhase.Height);
        image.transform.localPosition = new Vector3(combatPhase.X2, combatPhase.Y2, 0);
        infoPanel.transform.localPosition = new Vector3(infoMain.X, infoMain.Y, 0);
        infoPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(infoMain.Width, infoMain.Height);
        infoText.text = "Click on one of these targeting indicators to select the turn you would like to use your catapult.";
    }

    /// <summary>
    /// Moves the cursor to the Catapult Direction sections and updates the text of the infoPanel
    /// </summary>
    public void moveToCombatDirection()
    {
        mask.transform.localPosition = new Vector3(combatDirection.X, combatDirection.Y, 0);
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(combatDirection.Width, combatDirection.Height);
        image.transform.localPosition = new Vector3(combatDirection.X2, combatDirection.Y2, 0);
        infoPanel.transform.localPosition = new Vector3(infoMain.X, infoMain.Y, 0);
        infoPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(infoMain.Width, infoMain.Height);
        infoText.text = "Select the direction the catapult is fired.\n\nMake note of the direction the image of the ship on the UI is facing! The UI image is not always facing the same direction that your ship is.";
    }

    /// <summary>
    /// Moves the cursor to the Moement Phase sections and updates the text of the infoPanel
    /// </summary>
    public void moveToMovementPhase()
    {
        mask.transform.localPosition = new Vector3(movementPhase.X, movementPhase.Y, 0);
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(movementPhase.Width, movementPhase.Height);
        image.transform.localPosition = new Vector3(movementPhase.X2, movementPhase.Y2, 0);
        infoPanel.transform.localPosition = new Vector3(infoMain.X, infoMain.Y, 0);
        infoPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(infoMain.Width, infoMain.Height);
        infoText.text = "Click one of these buttons to choose the phase that you wish to enter in the current action. Phase 1 is the leftmost button while phase 4 is the rightmost one.\nYour ship can only act as many times as that ship has health.";
    }

    /// <summary>
    /// Moves the cursor to the Movement Action sections and updates the text of the infoPanel
    /// </summary>
    public void moveToMovementAction()
    {
        mask.transform.localPosition = new Vector3(movementAction.X, movementAction.Y, 0);
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(movementAction.Width, movementAction.Height);
        image.transform.localPosition = new Vector3(movementAction.X2, movementAction.Y2, 0);
        infoPanel.transform.localPosition = new Vector3(infoMain.X, infoMain.Y, 0);
        infoPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(infoMain.Width, infoMain.Height);
        infoText.text = "Here you can select a movement action to perform. Selecting an action will automatically move the cursor to the next phase.\n\nThe reverse action can \bONLY be performed after a hold action";
    }

    /// <summary>
    /// Moves the cursor to the Camera Control sections and updates the text of the infoPanel
    /// </summary>
    public void moveToCameraControls()
    {
        mask.transform.localPosition = new Vector3(cameraControls.X, cameraControls.Y, 0);
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(cameraControls.Width, cameraControls.Height);
        image.transform.localPosition = new Vector3(cameraControls.X2, cameraControls.Y2, 0);
        infoPanel.transform.localPosition = new Vector3(infoMap.X, infoMap.Y, 0);
        infoPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(infoMap.Width, infoMap.Height);
        infoText.text = "Use the WASD or arrow keys to move the camera. You can zoom in or out using the scrollwheel on your mouse";
    }

    /// <summary>
    /// Moves the cursor to the Direction sections and updates the text of the infoPanel
    /// </summary>
    public void moveToDirections()
    {
        mask.transform.localPosition = new Vector3(direction.X, direction.Y, 0);
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(direction.Width, direction.Height);
        image.transform.localPosition = new Vector3(direction.X2, direction.Y2, 0);
        infoPanel.transform.localPosition = new Vector3(infoMap.X, infoMap.Y, 0);
        infoPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(infoMap.Width, infoMap.Height);
        infoText.text = "Here you can see directional buttons. Click the direction you wish the ship to face.\nOnce set, your direction can only be changed like this by capturing a port or crashing into land.";
    }

    /// <summary>
    /// Closes the Controls instructions menu
    /// </summary>
    public void exitHelp()
    {
        if (GameObject.Find("OverlayCanvas/Objective") != null && GameObject.Find("OverlayCanvas/Objective").gameObject.active)
            GameManager.main.gameObject.GetComponent<UIControl>().startObjectiveFade();
        gameObject.SetActive(false);
    }
}
