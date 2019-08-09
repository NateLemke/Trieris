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
            GetComponentInChildren<Text>().color = t.getColorLight();
            GetComponentInChildren<Text>().text = momentum.ToString();
        }      
    }


}
