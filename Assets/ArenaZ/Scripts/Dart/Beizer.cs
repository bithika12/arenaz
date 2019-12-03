using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beizer : MonoBehaviour
{
    Vector3 ownPos;
    public GameObject cube;
    public Transform firstPos;
    public Transform midPos;
    public Transform lastPos;
    Vector3 lerp;
    private void Start()
    {
        cube.transform.position=firstPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lerp1 = Vector3.MoveTowards(firstPos.position, midPos.position, Time.deltaTime * 1f);
        firstPos.position = lerp1;
        Vector3 lerp2= Vector3.MoveTowards(midPos.position, lastPos.position, Time.deltaTime * 1f);
        midPos.position = lerp2;
        cube.transform.position = Vector3.MoveTowards(lerp1, lerp2, Time.deltaTime * 1f);
    }
}
