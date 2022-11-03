using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class PlayerBaseOffset : MonoBehaviour
{
    [SerializeField] Canvas gameOverCanvas;
    [SerializeField] Canvas successCanvas;
    [SerializeField] GameObject leftChild;
    [SerializeField] GameObject rightChild;
    [SerializeField] GameObject floatingTextPrefab;

    Stack<GameObject> leftStack;
    Stack<GameObject> rightStack;

    NavMeshAgent navMesh;
    NavMeshAgent leftNavMesh;
    NavMeshAgent rightNavMesh;
    GameObject popedCube;
    GameObject stackedCube;
    GameObject trail;
    Renderer rend;
    TrailRenderer trailRend;
    GameObject leftPopedCube;
    GameObject rightPopedCube;
    Rigidbody rbLeft;
    Rigidbody rbRight;

    Vector3 obstacleSize;
    Vector3 addPosText = new Vector3(-1f, 5f, .5f);

    int currentNumb = 0;
    float delayInSeconds = .5f;
    int vibrato = 10;
    float duration = 1f;
    float strenght = 90f;
    float randomness = 90f;

    void Start() 
    {
        gameOverCanvas.enabled = false;
        successCanvas.enabled = false;

        trail = GameObject.FindWithTag("TrailLeft");
        trailRend = trail.GetComponent<TrailRenderer>();
        trailRend.enabled = false;

        rbLeft = leftChild.GetComponent<Rigidbody>();
        rbRight = rightChild.GetComponent<Rigidbody>();
    }

    void Update() 
    {
        navMesh = GetComponent<NavMeshAgent>();
        leftNavMesh = leftChild.GetComponent<NavMeshAgent>();
        rightNavMesh = rightChild.GetComponent<NavMeshAgent>();

        SetNavMeshBaseOffset();

        leftStack = leftChild.GetComponent<LeftStacker>().stack;
        rightStack = rightChild.GetComponent<RightStacker>().stack;

        if (leftStack.Count - rightStack.Count >= 3 || rightStack.Count - leftStack.Count >= 3)
        {
            GetComponent<Movement>().enabled = false;
            gameOverCanvas.enabled = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "ObstacleCube" || other.gameObject.tag == "Stair")
        {
            obstacleSize.y = other.gameObject.GetComponent<BoxCollider>().size.y;

            if (obstacleSize.y <= leftStack.Count && obstacleSize.y <= rightStack.Count)
            {
                if (leftStack.Count > 0 && rightStack.Count > 0)
                {
                    for (var i = 0; i < obstacleSize.y; i++)
                    {
                        PopedCube(other);
                        rbLeft.detectCollisions = true;
                        rbRight.detectCollisions = true;
                    }
                }
            }
            else if (other.gameObject.tag == "ObstacleCube")
            {
                if (obstacleSize.y > leftStack.Count || obstacleSize.y > rightStack.Count)
                {
                    GetComponent<Movement>().enabled = false;
                    gameOverCanvas.enabled = true;
                }
            }
            else if (other.gameObject.tag == "Stair")
            {
                if (leftStack.Count == 0 || rightStack.Count == 0)
                {
                    GetComponentInParent<Movement>().enabled = false;
                    successCanvas.enabled = true;
                }
            }
        }
    }

    void PopedCube(Collider other)
    {
        PopedChildCubes();

        if (other.gameObject.tag == "ObstacleCube")
        {
            Invoke("DelayPopedCube", delayInSeconds);
        }
        else if (other.gameObject.tag == "Stair") //PLAYER
        {
            currentNumb += 2;
            ShowStairFloatingText("X" + (currentNumb.ToString()));
        }

        other.gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    void PopedChildCubes()
    {
        leftPopedCube = leftStack.Pop();
        leftPopedCube.transform.SetParent(null, true);
        rightPopedCube = rightStack.Pop();
        rightPopedCube.transform.SetParent(null, true);
    }

    void DelayPopedCube()
    {
        DecreaseNavMeshBaseOffset();
        SetDisabledCollider();
    }

    void SetDisabledCollider()
    {
        leftPopedCube.GetComponent<BoxCollider>().enabled = false;
        rightPopedCube.GetComponent<BoxCollider>().enabled = false;
    }

    void DecreaseNavMeshBaseOffset()
    {
        navMesh.baseOffset--;
        leftNavMesh.baseOffset--;
        rightNavMesh.baseOffset--;
    }

    void ShowStairFloatingText(string text)
    {
        if (floatingTextPrefab)
        {
            GameObject prefab = Instantiate(floatingTextPrefab, transform.position + addPosText, Quaternion.identity);
            prefab.GetComponent<TextMesh>().text = text;
            prefab.transform.DOShakeRotation(duration, strenght, vibrato, randomness, true).OnComplete( () => { Destroy(prefab); } );
        }
    }

    void SetNavMeshBaseOffset()
    {
        if (leftNavMesh.baseOffset > rightNavMesh.baseOffset)
        {
            navMesh.baseOffset = leftNavMesh.baseOffset;
        }
        else if (rightNavMesh.baseOffset > leftNavMesh.baseOffset)
        {
            navMesh.baseOffset = rightNavMesh.baseOffset;
        }
        else if (leftNavMesh.baseOffset == rightNavMesh.baseOffset)
        {
            navMesh.baseOffset = rightNavMesh.baseOffset;
        }
    }
}