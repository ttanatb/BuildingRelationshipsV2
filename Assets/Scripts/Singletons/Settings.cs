using UnityEngine;
using System.Collections;

public class Settings : Singleton<Settings>
{
    public float ReticleSpeed { get; set; }

    // Use this for initialization
    void Start()
    {
        ReticleSpeed = 10.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
