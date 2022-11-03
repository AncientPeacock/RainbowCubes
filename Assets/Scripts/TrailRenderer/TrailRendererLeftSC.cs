using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendererLeftSC : MonoBehaviour
{
    [SerializeField] GameObject player; //Left cube
    LeftStacker stackerCube;

    void Start() 
    {
        stackerCube = player.GetComponent<LeftStacker>();
    }

    void Update() 
    {
        stackerCube.SetPosition();
    }
}
