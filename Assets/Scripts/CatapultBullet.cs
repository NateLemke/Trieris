using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultBullet : MonoBehaviour
{
    public Vector3 startPos;
    public Ship target;
    float lifetime = 0.75f;
    float animationStart;

    // Start is called before the first frame update
    void Start()
    {
        animationStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(startPos,target.Position,(Time.time - animationStart) / lifetime);
        if(Time.time > animationStart + lifetime) {
            Destroy(this.gameObject);
        }
    }
}
