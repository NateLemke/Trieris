using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleHealthBar : MonoBehaviour
{
    public Image bar;
    public Ship thisShip;

    // Update is called once per frame
    void Update()
    {
        HealthChange(thisShip.life * 25);
    }

    void HealthChange(float healthValue)
    {
        float amount = (healthValue / 100.0f);
        bar.fillAmount = amount;
    }
}
