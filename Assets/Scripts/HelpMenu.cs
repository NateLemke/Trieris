using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMenu : MonoBehaviour
{
    GameObject background;
    GameObject image;
    GameObject mask;
    Vector3 childLocation;

    WindowData minimap = new WindowData(207f, 95f, 130f, 90f, -157f, -80f);
    WindowData shipTab = new WindowData(208f, 40f, 130f, 30f, -157f, -25f);
    WindowData combatPhase = new WindowData(208f, -17f, 125f, 22.6f, -157f, 32f);
    WindowData combatDirection = new WindowData(236.5f, -60f, 70f, 70f, -186.5f, 75f);
    WindowData movementPhase = new WindowData(208f, -8.76f, 127f, 37f, -158f, 6.3f);
    WindowData movementAction = new WindowData(179f, -60f, 70f, 70f, -129, 75f);


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
        background = transform.Find("Panel/Background").gameObject;
        mask = transform.Find("Panel/Mask").gameObject;
        image = transform.Find("Panel/Mask/Image").gameObject;
        Debug.Log("Location: " + childLocation.x +", " + childLocation.y +", " + childLocation.z);
    }



    public void moveToMinimap()
    {
        mask.transform.localPosition = new Vector3(minimap.X, minimap.Y, 0);
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(minimap.Width, minimap.Height);
        image.transform.localPosition = new Vector3(minimap.X2, minimap.Y2, 0);
    }

    public void moveToShipTabs()
    {
        mask.transform.localPosition = new Vector3(shipTab.X, shipTab.Y, 0);
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(shipTab.Width, shipTab.Height);
        image.transform.localPosition = new Vector3(shipTab.X2, shipTab.Y2, 0); ;
    }

    public void moveToCombatPhase()
    {
        mask.transform.localPosition = new Vector3(combatPhase.X, combatPhase.Y, 0);
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(combatPhase.Width, combatPhase.Height);
        image.transform.localPosition = new Vector3(combatPhase.X2, combatPhase.Y2, 0); ;
    }

    public void moveToCombatDirection()
    {
        mask.transform.localPosition = new Vector3(combatDirection.X, combatDirection.Y, 0);
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(combatDirection.Width, combatDirection.Height);
        image.transform.localPosition = new Vector3(combatDirection.X2, combatDirection.Y2, 0); ;
    }

    public void moveToMovementPhase()
    {
        mask.transform.localPosition = new Vector3(movementPhase.X, movementPhase.Y, 0);
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(movementPhase.Width, movementPhase.Height);
        image.transform.localPosition = new Vector3(movementPhase.X2, movementPhase.Y2, 0); ;
    }
}
