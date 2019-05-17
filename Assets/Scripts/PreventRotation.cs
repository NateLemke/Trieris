using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Purpose:    Used to prevent a gameobject from rotating with its parent
/// </summary>
public class PreventRotation : MonoBehaviour
{
    // the rotation to maintain for the attached gameobject
    Quaternion vertRotation;

    /// <summary>
    /// sets the gameobject's rotation to up
    /// </summary>
    void Awake()
    {
        transform.rotation = Quaternion.Euler(Vector3.up);
        vertRotation = transform.rotation;
    }

    /// <summary>
    /// Sets the gameobject this is attached to to always be facing "up"
    /// </summary>
    void LateUpdate()
    {
        transform.rotation = vertRotation;
    }
}
