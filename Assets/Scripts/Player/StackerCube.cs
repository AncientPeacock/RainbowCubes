using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class StackerCube : MonoBehaviour
{
    [SerializeField] Canvas gameOverCanvas;
    [SerializeField] Canvas successCanvas;
    [SerializeField] Transform target;
    [SerializeField] GameObject[] emojis = new GameObject[4];
    [SerializeField] GameObject floatingTextPrefab;
    //[SerializeField] GameObject ground;

    Stack<GameObject> stack1; //LIFO
    Stack<GameObject> stack2;

    NavMeshAgent leftNavMesh;
    NavMeshAgent rightNavMesh;
    NavMeshAgent navMesh;
    GameObject popedCube;
    GameObject stackedCube;
    AudioSource audioSource;
    Tween punchScaleTween;
    Color cubeColor;
    Renderer rend;
    TrailRenderer trail;
    Transform leftChild;
    Transform rightChild;

    Vector3 obstacleSize;
    Vector3 punchScale = new Vector3(.3f, .3f, .3f);
    Vector3 addPosEmoji = new Vector3(-1f, 6f, 0f);
    Vector3 addPosText = new Vector3(-1f, 5f, .5f);
    Vector3 trailPos = new Vector3(0f, -.5f,0f);

    float xPos, yPos, zPos;
    float delayInSeconds = .5f;
    int vibrato = 10;
    float duration = 1f;
    float elasticity = 1f;
    float strenght = 90f;
    float randomness = 90f;
    int currentNumb = 0;

    void Start()
    {
        DOTween.Init();

        gameOverCanvas.enabled = false;
        successCanvas.enabled = false;

        stack1 = new Stack<GameObject>(); //for left cube
        stack2 = new Stack<GameObject>(); //for right cube

        leftChild = GameObject.FindWithTag("LeftChild").GetComponent<Transform>();
        rightChild = GameObject.FindWithTag("RightChild").GetComponent<Transform>();

        //navMeshAgent = GetComponent<NavMeshAgent>();
        leftNavMesh = leftChild.GetComponent<NavMeshAgent>();
        rightNavMesh = rightChild.GetComponent<NavMeshAgent>();
        navMesh = GetComponent<NavMeshAgent>();

        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();

        trail = FindObjectOfType<TrailRenderer>();
        trail.enabled = false;

        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "LeftStackableCube" || other.gameObject.tag == "RightStackableCube")
        {
            StackedCube(other);
            // if (leftNavMesh.baseOffset > rightNavMesh.baseOffset)
            // {
            //     navMesh.baseOffset = leftNavMesh.baseOffset;
            //     rightNavMesh.baseOffset = navMesh.baseOffset; //otherwise the right cubes stay on the ground
            // }
            // else if (rightNavMesh.baseOffset > leftNavMesh.baseOffset)
            // {
            //     navMesh.baseOffset = rightNavMesh.baseOffset;
            //     leftNavMesh.baseOffset = navMesh.baseOffset;
            // }
            // else if (leftNavMesh.baseOffset == rightNavMesh.baseOffset)
            // {
            //     navMesh.baseOffset = leftNavMesh.baseOffset;
            // }
            //navMeshAgent.baseOffset++;
            // if (navMeshAgent.baseOffset == ((leftChild.position.y * 2) + .5f))
            // {
            //     navMeshAgent.baseOffset++;
            // }
            //if ()
            //navMeshAgent.baseOffset++;
            //if (leftChild.position.y )

            // if (other.gameObject.tag == "LeftStackableCube")
            // {
            //     if (stack1.Count >= stack2.Count)
            //     {
            //         navMeshAgent.baseOffset++;
            //     }
            // }
            // else if (other.gameObject.tag == "RightStackableCube")
            // {
            //     if (stack2.Count >= stack1.Count)
            //     {
            //         navMeshAgent.baseOffset++;
            //     }
            // }
            // //else { return; }
        }

        else if (other.gameObject.tag == "ObstacleCube" || other.gameObject.tag == "Stair")
        {
            obstacleSize.y = other.gameObject.GetComponent<BoxCollider>().size.y;

            if (obstacleSize.y <= stack1.Count || obstacleSize.y <= stack2.Count)
            {
                for (var i = 0; i < obstacleSize.y; i++)
                {
                    PopedCube(other);
                }
            }
            else if (other.gameObject.tag == "ObstacleCube" && (obstacleSize.y > stack1.Count || obstacleSize.y > stack2.Count))
            {
                GetComponent<Movement>().enabled = false;
                gameOverCanvas.enabled = true;
                //FindObjectOfType<SceneLoader>().ReloadLevel(); it s not necessary, you already use button
            }
            else if (other.gameObject.tag == "Stair")
            {
                if (stack1.Count == 0 || stack2.Count == 0)
                {
                    GetComponent<Movement>().enabled = false;
                    successCanvas.enabled = true;
                    //FindObjectOfType<SceneLoader>().LoadNextLevel();
                }
            }
        }

        else if (stack1.Count - stack2.Count >= 3 || stack2.Count - stack1.Count >= 3)
        {
            GetComponent<Movement>().enabled = false;
            gameOverCanvas.enabled = true;
            //TODO: ADD DEAD OR FALL ANIMATION
        }
    }

    void StackedCube(Collider other)
    {
        audioSource.Play();

        stackedCube = other.gameObject;

        

        if (other.gameObject.tag == "LeftStackableCube")
        {
            stack1.Push(stackedCube);
            SetPositionLeft();
            //if (stack1.Count >= stack2.Count) { navMeshAgent.baseOffset++; }
            stackedCube.transform.position = new Vector3(xPos, yPos, zPos);
            stackedCube.transform.parent = leftChild;
            stackedCube.tag = "Untagged"; //otherwise the stuck mechanic (while stuck.Count > 3) breaks cause triggers interact eachother
            GetRandomEmoji();
            PunchScaleCube(other);
            SetTrailColor();
            if (leftNavMesh.baseOffset <= stack1.Count + .5f)
            {
                leftNavMesh.baseOffset++;
            }
            else if (leftNavMesh.baseOffset > stack1.Count + .5f)
            {
                return;
            }
            //leftNavMesh.baseOffset++;
            // if (leftNavMesh.baseOffset == stack1.Count + .5f)
            // {
            //     leftNavMesh.baseOffset++;
            // }
            // else if (leftNavMesh.baseOffset > stack1.Count + .5f)
            // {
            //     return;
            // }
        }
        else if (other.gameObject.tag == "RightStackableCube")
        {
            stack2.Push(stackedCube);
            SetPositionRight();
            //if (stack2.Count >= stack1.Count) { navMeshAgent.baseOffset++; }
            stackedCube.transform.position = new Vector3(xPos, yPos, zPos);
            stackedCube.transform.parent = rightChild;
            stackedCube.tag = "Untagged"; //otherwise the stuck mechanic (while stuck.Count > 3) breaks cause triggers interact eachother
            GetRandomEmoji();
            PunchScaleCube(other);
            SetTrailColor();
            
            if (rightNavMesh.baseOffset <= (float)(stack2.Count + .5f))
            {
                rightNavMesh.baseOffset++;
            }
            else if (rightNavMesh.baseOffset > (float)(stack2.Count + .5f))
            {
                return;
            }
            // rightNavMesh.baseOffset++;
            // if (rightNavMesh.baseOffset == stack2.Count + .5f)
            // {
            //     rightNavMesh.baseOffset++;
            // }
            // else if (rightNavMesh.baseOffset > stack2.Count + .5f)
            // {
            //     return;
            // }
        }

        SetNavMeshBaseOffset();

        //SetPosition();
        //stackedCube.transform.position = new Vector3(xPos, yPos, zPos);
        //stackedCube.transform.parent = gameObject.transform; //SetParent is slightly slower

        //stackedCube.tag = "Untagged"; //otherwise the stuck mechanic (while stuck.Count > 3) breaks cause triggers interact eachother

        // GetRandomEmoji();
        // PunchScaleCube(other);
        // SetTrailColor();
        //SetGroundColor();
    }

    void SetNavMeshBaseOffset()
    {
        if (leftNavMesh.baseOffset > rightNavMesh.baseOffset)
        {
            navMesh.baseOffset = leftNavMesh.baseOffset;
            rightNavMesh.baseOffset = navMesh.baseOffset; //otherwise the right cubes stay on the ground
            // if (leftNavMesh.baseOffset > stack1.Count + .5f)
            // {
            //     return;
            // }
        }
        else if (rightNavMesh.baseOffset > leftNavMesh.baseOffset)
        {
            navMesh.baseOffset = rightNavMesh.baseOffset;
            leftNavMesh.baseOffset = navMesh.baseOffset; //cs:242 and cs:251 are ridiculous when they work together !!!
            // if (rightNavMesh.baseOffset > stack2.Count + .5f)
            // {
            //     return;
            // }
        }
        else if (leftNavMesh.baseOffset == rightNavMesh.baseOffset)
        {
            navMesh.baseOffset = rightNavMesh.baseOffset;
        }
    }

    void SetPositionRight()
    {
        xPos = rightChild.transform.position.x;
        yPos = rightChild.transform.position.y;
        zPos = rightChild.transform.position.z;
        yPos -= stack2.Count;
    }

    void SetPositionLeft()
    {
        xPos = leftChild.transform.position.x;
        yPos = leftChild.transform.position.y;
        zPos = leftChild.transform.position.z;
        yPos -= stack1.Count;
    }

    void SetTrailColor()
    {
        trail.enabled = true;
        rend = trail.GetComponent<Renderer>();
        cubeColor = stackedCube.GetComponent<Renderer>().material.color;
        rend.material.color = cubeColor;
    }

    // void SetPosition()
    // {
    //     xPos = transform.position.x;
    //     yPos = transform.position.y;
    //     zPos = transform.position.z;
    //     yPos -= stack.Count;

    // }

    void GetRandomEmoji()
    {
        int randomIndex = Random.Range(0, emojis.Length);
        GameObject instantiatedEmoji = Instantiate(emojis[randomIndex], (transform.position + addPosEmoji), Quaternion.identity) as GameObject;
        instantiatedEmoji.transform.LookAt(target);     //emoi must be look at the camera
        instantiatedEmoji.transform.DOShakeRotation(duration, strenght, vibrato, randomness, true).OnComplete( () => { Destroy(instantiatedEmoji); } );
    }

    void PunchScaleCube(Collider other)
    {
        punchScaleTween.Complete();
        punchScaleTween = other.gameObject.transform.DOPunchScale(punchScale, duration, vibrato, elasticity);
    }

    void PopedCube(Collider other)
    {
        if (other.gameObject.tag == "LeftStackableCube")
        {
            popedCube = stack1.Pop();
            popedCube.transform.SetParent(null, true);
        }

        else if (other.gameObject.tag == "RightStackableCube")
        {
            popedCube = stack2.Pop();
            popedCube.transform.SetParent(null, true);
        }

        popedCube.transform.SetParent(null, true);

        if (other.gameObject.tag == "ObstacleCube")
        {
            Invoke("DelayPopedCube", delayInSeconds);    //the movement got worse with IEnumerator

            if (stack1.Count == 0 || stack2.Count == 0)
            {
            trail.enabled = false;
            }
        }
        else if (other.gameObject.tag == "Stair")
        {
            currentNumb++;
            ShowStairFloatingText("X" + (currentNumb.ToString()));
            //return;
        }

        other.gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    void DelayPopedCube()
    {
        //navMeshAgent.baseOffset--;
        popedCube.GetComponent<BoxCollider>().enabled = false;
    }

    void ShowStairFloatingText(string text)
    {
        if (floatingTextPrefab)
        {
            GameObject prefab = Instantiate(floatingTextPrefab, transform.position + addPosText, Quaternion.identity);
            prefab.GetComponent<TextMesh>().text = text;
            prefab.transform.DOShakeRotation(duration, strenght, vibrato, randomness, true).OnComplete( () => { Destroy(prefab); } );
            //prefab.transform.DOMove(transform.position + addPosText, duration, true).OnComplete( () => { Destroy(prefab); } );
        }
    }

    // void SetGroundColor()
    // {
    //     //When the cube is stacked, the color of the ground is changed to the color of the cube.
    //     cubeColor = stackedCube.GetComponent<Renderer>().material.color;
    //     rend = ground.GetComponent<Renderer>();
    //     rend.material.color = cubeColor;
    // }
}