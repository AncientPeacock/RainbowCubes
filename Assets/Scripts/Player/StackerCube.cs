using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StackerCube : MonoBehaviour
{
    Stack<GameObject> stack;
    BoxCollider boxCollider;
    NavMeshAgent navMeshAgent;

    float xSize, ySize, zSize;
    // float xPos, yPos, zPos;

    void Start()
    {
        xSize = 1f;
        ySize = 1f;
        zSize = 1f;

        // xPos = transform.position.x;
        // yPos = transform.position.y;
        // zPos = transform.position.z;

        stack = new Stack<GameObject>();
        boxCollider = GetComponent<BoxCollider>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        boxCollider.size = new Vector3(xSize, ySize, zSize);
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "StackableCube")
        {
            //other.gameObject.transform.parent = gameObject.transform;
            stack.Push(other.gameObject);
            Debug.Log("stack: " + stack.Count);

            other.gameObject.transform.SetParent(gameObject.transform);
            // yPos --;
            // other.gameObject.transform.position = transform.position;
            // Debug.Log("ypos: " + yPos);

            ySize++;
            Debug.Log("y: " + ySize);
            
            navMeshAgent.baseOffset++;
            Debug.Log("navMesh" + navMeshAgent);

            //other.gameObject.tag = "Untagged";
        }

        else if (other.gameObject.tag == "ObstacleCube")
        {
            if (stack.Count >= 1)
            {
                stack.Pop();
                Debug.Log("stackpop: " + stack.Count);

                //child.transform.SetParent(null);

                ySize--;
                Debug.Log("y-: " + ySize);
            
                navMeshAgent.baseOffset--;
                Debug.Log("navMesh-: " + navMeshAgent);
            }
            else { return; }
        }
    }
}
