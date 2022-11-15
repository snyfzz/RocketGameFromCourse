using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [SerializeField] Vector3 movePosition;
    [SerializeField] Vector3 moveRotation;
    [SerializeField][Range(0, 1)] float moveSpeed;

    [Range(0, 1)] float moveProgress;

    Vector3 startPosition;
    Vector3 movePos;

    float normalize; 


    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        normalize = Time.deltaTime * moveSpeed;
        ChangePos();
        ChangeRot();
    }

    void ChangePos()
    {
        moveProgress = Mathf.PingPong(Time.time * moveSpeed, 1);
        movePos = movePosition * moveProgress;
        transform.position = startPosition + movePos;
    }

    void ChangeRot()
    {
        transform.Rotate(moveRotation * normalize * 3);
    }
}
