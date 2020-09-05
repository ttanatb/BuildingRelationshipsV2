using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Quaternion rot = Quaternion.AngleAxis(Random.value * 360.0f, Vector3.up);
        transform.rotation = rot;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
