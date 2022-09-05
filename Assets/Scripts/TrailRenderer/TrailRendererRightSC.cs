using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendererRightSC : MonoBehaviour
{
    [SerializeField] GameObject player; //Right cube
    // StackerCube stackerCube;
    RightStacker stackerCube;

    void Start() 
    {
        // stackerCube = player.GetComponentInParent<StackerCube>();
        stackerCube = player.GetComponent<RightStacker>();
    }

    void Update() 
    {
        // stackerCube.SetPositionRight();
        stackerCube.SetPosition();
    }
}
