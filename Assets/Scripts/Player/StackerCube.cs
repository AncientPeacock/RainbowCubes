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
    [SerializeField] GameObject ground;

    Stack<GameObject> stack; //LIFO

    NavMeshAgent navMeshAgent;
    GameObject popedCube;
    GameObject stackedCube;
    AudioSource audioSource;
    Tween punchScaleTween;
    Color cubeColor;
    Renderer rend;

    Vector3 obstacleSize;
    Vector3 punchScale = new Vector3(.3f, .3f, .3f);
    Vector3 addPosEmoji = new Vector3(-.5f, 4f, 0f);
    Vector3 addPosText = new Vector3(-.5f, 3f, .5f);

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

        stack = new Stack<GameObject>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
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
            else if (other.gameObject.tag == "Stair")
            {
                if (stack.Count == 0)
                {
                    GetComponent<Movement>().enabled = false;
                    successCanvas.enabled = true;
                    //FindObjectOfType<SceneLoader>().LoadNextLevel();
                }
            }
        }
    }

    void StackedCube(Collider other)
    {
        audioSource.Play();

        stackedCube = other.gameObject;
        stack.Push(stackedCube);

        stackedCube.transform.parent = gameObject.transform; //SetParent is slightly slower
        yPos -= stack.Count;
        stackedCube.transform.position = new Vector3(xPos, yPos, zPos);
        stackedCube.tag = "Untagged"; //otherwise the stuck mechanic (while stuck.Count > 3) breaks cause triggers interact eachother

        GetRandomEmoji();
        PunchScaleCube(other);
        SetGroundColor();
    }

    void SetGroundColor()
    {
        cubeColor = stackedCube.GetComponent<Renderer>().material.color;
        rend = ground.GetComponent<Renderer>();
        rend.material.color = cubeColor;
    }

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
        popedCube = stack.Pop();
        popedCube.transform.SetParent(null, true);

        if (other.gameObject.tag == "ObstacleCube")
        {
            Invoke("DelayPopedCube", delayInSeconds);    //the movement got worse with IEnumerator
        }
        else if (other.gameObject.tag == "Stair")
        {
            currentNumb++;
            ShowStairFloatingText("X" + (currentNumb.ToString()));
            return;
        }

        other.gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    void DelayPopedCube()
    {
        navMeshAgent.baseOffset--;
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
}