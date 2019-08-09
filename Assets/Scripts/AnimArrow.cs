using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimArrow : MonoBehaviour
{
    private float timestamp;

    // Start is called before the first frame update
    void Start()
    {
        timestamp = Time.time + SpeedManager.ActionDelay + SpeedManager.ActionSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > timestamp) {
            Destroy(this.gameObject);
        }
    }

    public void Initialize(Team t, int momentum = 0) {
        GetComponentInChildren<SpriteRenderer>().color = t.getColor();     
        
        if(momentum > 1) {
            Text text = GetComponentInChildren<Text>();
            text.color = t.getColorLight();
            text.text = momentum.ToString();
        } else {
            if(GetComponentInChildren<Text>() != null) {
                GetComponentInChildren<Text>().enabled = false;
            }
        }      
    }


}
