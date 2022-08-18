using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StackerCube : MonoBehaviour
{
    Stack<GameObject> stack;
    BoxCollider boxCollider;
    NavMeshAgent navMeshAgent;

    float xColliderSize, yColliderSize, zColliderSize;
    float xPos, yPos, zPos;

    void Start()
    {
        stack = new Stack<GameObject>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        boxCollider = GetComponent<BoxCollider>();

        xColliderSize = 1f;
        yColliderSize = 1f;
        zColliderSize = 1f;

        boxCollider.size = new Vector3(xColliderSize, yColliderSize, zColliderSize);
    }

    void FixedUpdate() //cube position is more stable with FixedUpdate()
    {
        xPos = transform.position.x;
        yPos = transform.position.y;
        zPos = transform.position.z;
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "StackableCube")
        {
            stack.Push(other.gameObject);
            other.gameObject.transform.parent = gameObject.transform; //SetParent is slightly slower

            if (stack.Count >= 1)
            {
                yPos = yPos - stack.Count;
                other.gameObject.transform.position = new Vector3(xPos, yPos, zPos);
            }

            yColliderSize++;
            navMeshAgent.baseOffset++;
        }

        else if (other.gameObject.tag == "ObstacleCube")
        {
            if (stack.Count >= 1)
            {
                stack.Pop();

                //child.transform.SetParent(null);
                //other.gameObject.transform.SetParent(null, true);
                other.transform.parent.gameObject.SetActive(false);

                yColliderSize--;
                navMeshAgent.baseOffset--;
            }
            else { return; }
        }
    }
}
