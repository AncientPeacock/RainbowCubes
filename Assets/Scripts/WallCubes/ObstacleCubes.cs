using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCubes : MonoBehaviour
{
    BoxCollider boxCollider;

    void Start() 
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public Vector3 GetObstacleCubeColliderSize()
    {
        boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y, boxCollider.size.z);
        return boxCollider.size;
    }
}
