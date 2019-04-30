
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempText : MonoBehaviour
{
    float timestap;
    public float lifetime;

    // Start is called before the first frame update
    void Start()
    {
        timestap = Time.time + lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > timestap) {
            Destroy(this.gameObject);
        }
        Color c = GetComponent<Text>().color;
        c.a = ((timestap - Time.time + lifetime) / (lifetime * 2)) ;
        GetComponent<Text>().color = c;
    }
}
