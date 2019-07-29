using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortCaptureAnimationObject : MonoBehaviour
{
    
    public float timeStamp;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Canvas>().sortingLayerName = "UILayer";
        GetComponent<Canvas>().sortingOrder = 10;
        timeStamp = Time.time;
    }

    // Update is called once per frame
    void Update() {
        if (timeStamp + SpeedManager.CaptureDelay > Time.time) {
            return;
        }
        else if (Time.time < timeStamp + SpeedManager.CaptureDelay + SpeedManager.CaptureSpeed) {
            getUpperImg().fillAmount = (timeStamp + SpeedManager.CaptureDelay + SpeedManager.CaptureSpeed - Time.time) / SpeedManager.CaptureSpeed;
        } else if (Time.time > timeStamp + SpeedManager.CaptureDelay * 2 + SpeedManager.CaptureSpeed) {
            Destroy(gameObject);
        } else {
            getUpperImg().fillAmount = 0f;
        }
    }

    public void SetLowerImg(Sprite sprite) {
        transform.GetChild(0).GetComponent<Image>().sprite = sprite;
    }

    public void SetUpperImg(Sprite sprite) {
        transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = sprite;
    }

    Image getUpperImg() {
        return transform.GetChild(0).GetChild(0).GetComponent<Image>();
    }

    
}
