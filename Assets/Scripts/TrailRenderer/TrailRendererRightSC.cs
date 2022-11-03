using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendererRightSC : MonoBehaviour
{
    [SerializeField] GameObject player; //Right cube
    RightStacker stackerCube;

    void Start() 
    {
        stackerCube = player.GetComponent<RightStacker>();
    }

    void Update() 
    {
        stackerCube.SetPosition();
    }
}
