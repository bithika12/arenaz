using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAnimation : MonoBehaviour
{
    public float speed;

    void Update()
    {
        this.transform.Rotate(0f,0f,-(speed*Time.deltaTime));
    }
}
