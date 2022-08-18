using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StackerCube : MonoBehaviour
{
    Stack<GameObject> stack;
    NavMeshAgent navMeshAgent;
    ObstacleCubes obstacleCubes;
    GameObject popedCube;

    float xPos, yPos, zPos;

    void Start()
    {
        stack = new Stack<GameObject>();

        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void FixedUpdate() //cube position is more stable with FixedUpdate()
    {
        xPos = transform.position.x;
        yPos = transform.position.y;
        Debug.Log("fixY: " + yPos);
        zPos = transform.position.z;
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "StackableCube")
        {
            StackedCube(other);
            navMeshAgent.baseOffset++;
        }

        else if (other.gameObject.tag == "ObstacleCube")
        {
            if (stack.Count >= 1)
            {
                PopedCube(other);
                navMeshAgent.baseOffset--;
            }
            else 
            {
                GetComponent<Movement>().enabled = false;
            }
            //TODO: GAME OVER (OBSTACLE.LENGHT > STACK.COUNT)
        }
    }

    void StackedCube(Collider other)
    {
        stack.Push(other.gameObject);

        other.gameObject.transform.parent = gameObject.transform; //SetParent is slightly slower

        Debug.Log("ypos0: " + yPos);
        yPos = yPos - stack.Count;
        Debug.Log("ypos: " + yPos + " " + stack.Count);
        other.gameObject.transform.position = new Vector3(xPos, yPos, zPos);

        other.gameObject.GetComponent<BoxCollider>().isTrigger = true; //otherwise the cube is stuck in the obstacle.
        other.gameObject.tag = "Untagged"; //otherwise the stuck mechanic (while stuck.Count > 3) breaks cause triggers interact eachother
    }

    void PopedCube(Collider other)
    {
        popedCube = stack.Pop();
        popedCube.GetComponent<BoxCollider>().enabled = false;
        popedCube.transform.SetParent(null, true);

        other.gameObject.GetComponent<BoxCollider>().enabled = false;
    }
}
