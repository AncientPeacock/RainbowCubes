using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StackerCube : MonoBehaviour
{
    Stack<GameObject> stack;

    NavMeshAgent navMeshAgent;
    //ObstacleCubes obstacleCubes;
    GameObject popedCube;
    Vector3 obstacleSize;

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
            obstacleSize.y = other.gameObject.GetComponent<BoxCollider>().size.y;

            if (obstacleSize.y <= stack.Count)
            {
                for (var i = 0; i < obstacleSize.y; i++)
                {
                    PopedCube(other);
                    
                    navMeshAgent.baseOffset--;
                }
            }
            else if (obstacleSize.y > stack.Count)
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

        yPos = yPos - stack.Count;
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
