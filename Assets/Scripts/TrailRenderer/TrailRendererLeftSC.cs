using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendererLeftSC : MonoBehaviour
{
    [SerializeField] GameObject player; //leftCube
    StackerCube stackerCube;

    void Start() 
    {
        stackerCube = player.GetComponentInParent<StackerCube>();
    }

    void Update() 
    {
        stackerCube.SetPositionLeft();
    }
}
