using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StackerCube : MonoBehaviour
{
    [SerializeField] Canvas gameOverCanvas;
    [SerializeField] Canvas successCanvas;

    Stack<GameObject> stack; //LIFO

    NavMeshAgent navMeshAgent;
    GameObject popedCube;
    Vector3 obstacleSize;
    AudioSource audioSource;

    float xPos, yPos, zPos;
    float delayInSeconds = .5f;

    void Start()
    {
        gameOverCanvas.enabled = false;
        successCanvas.enabled = false;

        stack = new Stack<GameObject>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
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

        else if (other.gameObject.tag == "ObstacleCube" || other.gameObject.tag == "Stair")
        {
            obstacleSize.y = other.gameObject.GetComponent<BoxCollider>().size.y;

            if (obstacleSize.y <= stack.Count)
            {
                for (var i = 0; i < obstacleSize.y; i++)
                {
                    PopedCube(other);
                }
            }
            else if (other.gameObject.tag == "ObstacleCube" && obstacleSize.y > stack.Count)
            {
                GetComponent<Movement>().enabled = false;
                gameOverCanvas.enabled = true;
                //FindObjectOfType<SceneLoader>().ReloadLevel(); it s not necessary, you already use button
            }
            else if (other.gameObject.tag == "Stair" && stack.Count == 0)
            {
                GetComponent<Movement>().enabled = false;
                successCanvas.enabled = true;
                //FindObjectOfType<SceneLoader>().LoadNextLevel();
            }
        }
    }

    void StackedCube(Collider other)
    {
        audioSource.Play();
        stack.Push(other.gameObject);

        other.gameObject.transform.parent = gameObject.transform; //SetParent is slightly slower

        yPos -= stack.Count;
        other.gameObject.transform.position = new Vector3(xPos, yPos, zPos);

        other.gameObject.tag = "Untagged"; //otherwise the stuck mechanic (while stuck.Count > 3) breaks cause triggers interact eachother
    }

    void PopedCube(Collider other)
    {
        popedCube = stack.Pop();
        popedCube.transform.SetParent(null, true);

        if (other.gameObject.tag == "ObstacleCube")
        {
            Invoke("DelayPopedCube", delayInSeconds);    //the movement got worse with IEnumerator
        }
        else if (other.gameObject.tag == "Stair")
        {
            return;
        }

        other.gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    void DelayPopedCube()
    {
        navMeshAgent.baseOffset--;
        popedCube.GetComponent<BoxCollider>().enabled = false;
    }
}