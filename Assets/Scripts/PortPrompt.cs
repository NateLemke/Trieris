using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortPrompt : MonoBehaviour
{

    GameObject portPromptPanel;
    GameObject portNotification;

    void Start()
    {
        portPromptPanel = transform.Find("CapturePrompt").gameObject;
        portNotification = transform.Find("CaptureNotification").gameObject;
        //portNotification.SetActive(false);
        //portPromptPanel.SetActive(false);

        portNotification.SetActive(true);
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
            portNotification.SetActive(true);
            portPromptPanel.SetActive(false);
        }
    }
    void Update()
    {
        CheckUnfocus();
    }
}
