using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class StackerCube : MonoBehaviour
{
    [SerializeField] Canvas gameOverCanvas;
    [SerializeField] Canvas successCanvas;
    [SerializeField] GameObject[] emojis = new GameObject[4];

    Stack<GameObject> stack; //LIFO

    NavMeshAgent navMeshAgent;
    GameObject popedCube;
    AudioSource audioSource;
    Tween punchScaleTween;

    Vector3 obstacleSize;
    Vector3 punchScale = new Vector3(.3f, .3f, .3f);
    Vector3 emojiPos = new Vector3(-.5f, 4f, 0f);

    float xPos, yPos, zPos;
    float delayInSeconds = .5f;
    int vibrato = 10;
    float duration = 1f;
    float elasticity = 1f;
    float strenght = 90f;
    float randomness = 90f;
    float destroyEmoji = .8f;

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

        GetRandomEmoji();
        PunchScaleCube(other);
    }

    void GetRandomEmoji()
    {
        int randomIndex = Random.Range(0, emojis.Length);
        GameObject instantiatedEmoji = Instantiate(emojis[randomIndex], (transform.position + emojiPos), Quaternion.identity) as GameObject;
        instantiatedEmoji.transform.DOShakeRotation(duration, strenght, vibrato, randomness, true);
        Destroy(instantiatedEmoji, destroyEmoji);
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