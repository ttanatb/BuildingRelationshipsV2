using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMoveLights : MonoBehaviour
{
    Vector3 m_baseLoc;

    // Start is called before the first frame update
    void Start()
    {
        m_baseLoc = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.Sin(Time.time) * 360.0f;
        transform.rotation = Quaternion.AngleAxis(t, Vector3.up);
    }
}
