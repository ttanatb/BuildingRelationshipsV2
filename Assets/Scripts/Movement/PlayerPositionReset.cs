using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionReset : MonoBehaviour
{
    [SerializeField]
    Terrain m_terrain;
    float m_threshold = 0.1f;
    PlayerMovement m_playerMovement;
    Rigidbody m_rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        m_playerMovement = GetComponent<PlayerMovement>();
        m_rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float currPosY = transform.position.y;
        float terrainY = m_terrain.SampleHeight(transform.position);

        if (currPosY - terrainY < -m_threshold)
        {
            Debug.Log("currPos: " + transform.position.y);
            Debug.Log("terrainY: " + terrainY);

            m_rigidBody.velocity = Vector3.zero;
            m_rigidBody.angularVelocity = Vector3.zero;
            //m_playerMovement.StopMovement();

            Vector3 newPos = transform.position;
            newPos.y = terrainY + m_threshold;
            transform.position = newPos;
        }
    }
}
