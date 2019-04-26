using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatResolution : MonoBehaviour{

    Ship shipA;
    Ship shipB;

    int damageToA;
    int damageToB;

    public IEnumerator resolve() {

        Animation A = AnimationManager.actionAnimations[shipA];
        Animation B = AnimationManager.actionAnimations[shipB];

        StartCoroutine(A.playAnimation());
        StartCoroutine(B.playAnimation());

        while(!A.complete && !B.complete) {
            yield return null;
        }

        // deal damage or something ...

        yield return null;
    }

}
