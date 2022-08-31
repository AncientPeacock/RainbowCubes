using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendererRightSC : MonoBehaviour
{
    [SerializeField] GameObject player; //rightCube
    StackerCube stackerCube;

    void Start() 
    {
        stackerCube = player.GetComponentInParent<StackerCube>();
    }

    void Update() 
    {
        stackerCube.SetPositionRight();
    }
}
