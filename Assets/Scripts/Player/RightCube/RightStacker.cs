using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class RightStacker : MonoBehaviour
{
    [SerializeField] Canvas gameOverCanvas;
    [SerializeField] Transform target;
    [SerializeField] GameObject[] emojis = new GameObject[4];

    public Stack<GameObject> stack; //LIFO

    NavMeshAgent navMesh;
    GameObject stackedCube;
    AudioSource audioSource;
    Tween punchScaleTween;
    Color cubeColor;
    Renderer rend;
    GameObject trail;
    TrailRenderer trailRend;
    Rigidbody rb;
    PlayerBaseOffset OnT;

    Vector3 obstacleSize;
    Vector3 punchScale = new Vector3(.3f, .3f, .3f);
    Vector3 addPosEmoji = new Vector3(-1f, 6f, 0f);
    Vector3 trailPos = new Vector3(0f, -.5f,0f);

    float xPos, yPos, zPos;
    float delayInSeconds = .5f;
    int vibrato = 10;
    float duration = 1f;
    float elasticity = 1f;
    float strenght = 90f;
    float randomness = 90f;
    int currentNumb = 0;
    float yTrail = -.4f;

    void Start()
    {
        DOTween.Init();

        gameOverCanvas.enabled = false;

        stack = new Stack<GameObject>();

        navMesh = GetComponent<NavMeshAgent>();

        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();

        trail = GameObject.FindWithTag("TrailRight");
        trailRend = trail.GetComponent<TrailRenderer>();
        trailRend.enabled = false;

        rb = GetComponent<Rigidbody>();

        OnT = GetComponentInParent<PlayerBaseOffset>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "StackableCube")
        {
            StackedCube(other);
            navMesh.baseOffset++;
        }

        else if (other.gameObject.tag == "ObstacleCube" || other.gameObject.tag == "Stair")
        {
            OnT.OnTriggerEnter(other);
        }
    }

    public void StackedCube(Collider other)
    {
        audioSource.Play();

        stackedCube = other.gameObject;
        stack.Push(stackedCube);
        
        SetPosition();
        stackedCube.transform.position = new Vector3(xPos, yPos, zPos);
        stackedCube.transform.parent = gameObject.transform;
        stackedCube.tag = "Untagged";
        
        GetRandomEmoji();
        PunchScaleCube(other);
        SetTrailColor();
    }

    public void SetPosition()
    {
        xPos = transform.position.x;
        yPos = transform.position.y;
        zPos = transform.position.z;
        yPos -= stack.Count;

        trail.transform.position = new Vector3(xPos, yTrail, zPos);
    }

    void SetTrailColor()
    {
        trailRend.enabled = true;
        rend = trailRend.GetComponent<Renderer>();
        cubeColor = stackedCube.GetComponent<Renderer>().material.color;
        rend.material.color = cubeColor;
    }

    void GetRandomEmoji()
    {
        int randomIndex = Random.Range(0, emojis.Length);
        GameObject instantiatedEmoji = Instantiate(emojis[randomIndex], (transform.position + addPosEmoji), Quaternion.identity) as GameObject;
        instantiatedEmoji.transform.LookAt(target);
        instantiatedEmoji.transform.DOShakeRotation(duration, strenght, vibrato, randomness, true).OnComplete( () => { Destroy(instantiatedEmoji); } );
    }

    void PunchScaleCube(Collider other)
    {
        punchScaleTween.Complete();
        punchScaleTween = other.gameObject.transform.DOPunchScale(punchScale, duration, vibrato, elasticity);
    }
}