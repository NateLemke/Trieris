using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortPrompt : MonoBehaviour
{

    private GameObject portPromptPanel;
    private GameObject portNotification;
    private Port thisPort;
    private Ship currentShip;

    void Start()
    {
        portPromptPanel = transform.Find("CapturePrompt").gameObject;
        portNotification = transform.Find("CaptureNotification").gameObject;
        //portNotification.SetActive(false);
        //portPromptPanel.SetActive(false);
    }

    public void activatePrompt()
    {
        portPromptPanel.SetActive(true);
        portNotification.SetActive(false);
    }

    private void CheckUnfocus()
    {
        if(Input.GetMouseButton(0) && portPromptPanel.activeSelf && !RectTransformUtility.RectangleContainsScreenPoint(portPromptPanel.GetComponent<RectTransform>(), Input.mousePosition, Camera.main))
        {
            notify();
        }
    }

    public void activateNotification(Port inPort, Ship inShip)
    {
        thisPort = inPort;
        currentShip = inShip;

        notify();
    }

    public void notify()
    {
        portNotification.SetActive(true);
        portPromptPanel.SetActive(false);
    }

    public void accept()
    {
        thisPort.setTeam(currentShip.team);
        currentShip.needCaptureChoice = false;
        portPromptPanel.SetActive(false);
    }

    public void decline()
    {
        currentShip.needCaptureChoice = false;
        portPromptPanel.SetActive(false);
    }

    void Update()
    {
        CheckUnfocus();
    }
}
