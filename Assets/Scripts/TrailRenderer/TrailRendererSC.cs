using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendererSC : MonoBehaviour
{
    [SerializeField] GameObject player;
    StackerCube stackerCube;
    TrailRenderer trail;

    float xPos, yPos, zPos;

    void Start() 
    {
        stackerCube = player.GetComponent<StackerCube>();
        trail = GetComponent<TrailRenderer>();
    }

    void Update() 
    {
        xPos = stackerCube.transform.position.x;
        yPos = -.4f;
        zPos = stackerCube.transform.position.z;
        trail.transform.position = new Vector3(xPos, yPos, zPos);
    }
}
