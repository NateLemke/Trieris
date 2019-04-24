using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventRotation : MonoBehaviour
{
    Quaternion vertRotation;

    void Awake()
    {
        //Vector3 eulerAngle = transform.rotation.eulerAngles;
        //transform.rotation = Quaternion.Euler(new Vector3(eulerAngle.x, eulerAngle.y, eulerAngle.z + 180));
        //vertRotation = transform.rotation;

        transform.rotation = Quaternion.Euler(Vector3.up);
        vertRotation = transform.rotation;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        transform.rotation = vertRotation;
    }
}
