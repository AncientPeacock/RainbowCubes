using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StackerCube : MonoBehaviour
{
    Stack<GameObject> stack;
    //BoxCollider boxCollider;
    NavMeshAgent navMeshAgent;
    ObstacleCubes obstacleCubes;
    GameObject popedCube;

    float xPos, yPos, zPos;
    //float yColliderSize = 1f;

    void Start()
    {
        stack = new Stack<GameObject>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        //boxCollider = GetComponent<BoxCollider>();
    }

    void FixedUpdate() //cube position is more stable with FixedUpdate()
    {
        xPos = transform.position.x;
        yPos = transform.position.y;
        zPos = transform.position.z;

        //boxCollider.size = new Vector3(boxCollider.size.x, yColliderSize, boxCollider.size.z);
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "StackableCube")
        {
            stack.Push(other.gameObject);

            other.gameObject.transform.parent = gameObject.transform; //SetParent is slightly slower
            other.gameObject.GetComponent<BoxCollider>().isTrigger = true;

            if (stack.Count >= 1)
            {
                Debug.Log("stackC0: " + stack.Count);
                Debug.Log("yPos0: " + yPos);
                yPos = yPos - stack.Count;
                Debug.Log("yPos1: " + yPos);
                other.gameObject.transform.position = new Vector3(xPos, yPos, zPos);
            }

            //yColliderSize++;
            //Debug.Log("y: " + yColliderSize);
            navMeshAgent.baseOffset++;
        }

        if (other.gameObject.tag == "ObstacleCube")
        {
            if (stack.Count >= 1)
            {
                popedCube = stack.Pop();
                popedCube.GetComponent<BoxCollider>().enabled = false;
                popedCube.transform.SetParent(null, true);

                navMeshAgent.baseOffset--;

                other.gameObject.GetComponent<BoxCollider>().enabled = false;
                Debug.Log("stackC1: " + stack.Count);

                //child.transform.SetParent(null);
                //other.gameObject.transform.SetParent(null, true);
                //other.transform.parent.gameObject.SetActive(false);
                //if (obstacleCubes.GetObstacleCubeColliderSize().)

                //yColliderSize--;
                
            }
            else { return; }
        }
    }
}
