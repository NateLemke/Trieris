using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose: Shows a catapult bullet being shot from a ship using vector points and particle emissions.
/// Plays sound clips while the animation is playing at the same time.
/// </summary>
public class CatapultBullet : MonoBehaviour
{
    public Vector3 startPos;
    
    public Vector3 endPos;
    float animationStart;
    Sounds sounds;
    public bool impacted = false;
    public bool missed = false;
    public bool sameNode;
    public Vector3 midPoint;

    // Start is called before the first frame update
    void Start()
    {
        sounds = Sounds.main;
        animationStart = Time.time;
        sounds.playClip(sounds.Fireball);
        sounds.playClip(sounds.Whistle,0.4f);
        midPoint = ((endPos + startPos) / 2) +new Vector3(0,1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        float lerpVal = (Time.time - animationStart) / SpeedManager.CatapultTravelTime;

        Vector3 curve1 = Vector3.Lerp(startPos,midPoint,lerpVal);
        Vector3 curve2 = Vector3.Lerp(midPoint,endPos,lerpVal);
        transform.position = Vector3.Lerp(curve1,curve2,lerpVal);

        if (Time.time > animationStart + SpeedManager.CatapultTravelTime && !impacted) {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<ParticleSystem>().enableEmission = false;

            if (!missed) {
                //sounds.playClip(sounds.Sizzle,0.5f);
                sounds.playRandomCrunch();
            } else {
                sounds.playClip(sounds.Splash);
            }
            sounds.playClip(sounds.Fireball,0.4f);
            
            impacted = true;
        }
        if(Time.time > animationStart + SpeedManager.CatapultTravelTime + SpeedManager.CombatPostDelay) {
            Destroy(this.gameObject);
        }
    }
}
