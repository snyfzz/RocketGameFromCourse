using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Camera : MonoBehaviour
{
    Vector3 rocketPos;
    GameObject rocket;

    
    // Start is called before the first frame update
    void Start()
    {
        rocket = GameObject.Find("Rocket");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rocketPos = rocket.transform.position;
        transform.position = new Vector3(rocketPos.x, rocketPos.y, transform.position.z);
    }
}
