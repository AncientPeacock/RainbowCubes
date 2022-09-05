using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendererLeftSC : MonoBehaviour
{
    [SerializeField] GameObject player; //Left cube
    // StackerCube stackerCube;
    LeftStacker stackerCube;

    void Start() 
    {
        // stackerCube = player.GetComponentInParent<StackerCube>();
        stackerCube = player.GetComponent<LeftStacker>();
    }

    void Update() 
    {
        // stackerCube.SetPositionLeft();
        stackerCube.SetPosition();
    }
}
