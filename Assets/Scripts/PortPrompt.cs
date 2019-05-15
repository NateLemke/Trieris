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
        foreach (Ship s in currentShip.getNode().getShips())
            s.needCaptureChoice = false;
        GameManager.PortsCaptured++;
        currentShip.playerCapture();
        portPromptPanel.SetActive(false);
        GameManager.main.StartCoroutine(acceptAnimation(currentShip));
    }

    public void decline()
    {
        
        portPromptPanel.SetActive(false);
        GameManager.main.StartCoroutine(declineAnimation());
    }

    void Update()
    {
        CheckUnfocus();
    }

    static IEnumerator acceptAnimation(Ship s) {
        yield return new PortCaptureAnimation(s).playAnimation();
        foreach (Ship ps in GameManager.main.getPlayerShips()) {
            if (ps.needCaptureChoice) {
                yield return PhaseManager.focus(ps.Position);
                break;
            }
        }
        s.needCaptureChoice = false;
    }

    static IEnumerator declineAnimation() {
        yield return new WaitForSeconds(0.5f);
        foreach (Ship ps in GameManager.main.getPlayerShips()) {
            if (ps.needCaptureChoice) {
                yield return PhaseManager.focus(ps.Position);
                break;
            }
        }
    }
}
