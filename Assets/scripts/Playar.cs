using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using TMPro;
public class Playar : MonoBehaviour
{
    public Rigidbody rigid;
    public float forceMove;
    public float forceJump;
    public bool onFloor = false;

    public GameObject coinPrefab;
    public GameObject[] coinArray;
    public int coinArraySize = 3;
    public int score;

    public TMPro.TextMeshProUGUI tmp;
    public TMPro.TextMeshProUGUI timer;

    public GameObject spawnEffect;

    AudioSource audioSource;
    public AudioClip spawnAudio;


    public float coinRespawnPosY = 3f;
    public float coinRespawnPosYRNG = 1f;
    public float coinRespawnPosXRNG = 3f;
    public float timeLeft;

    public bool gameEnd;
    public int goal = 13;
    public TMPro.TextMeshProUGUI endGameText;
    public RectTransform endgamePanel;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

    }

    private void Awake()
    {
        coinArray = new GameObject[coinArraySize];
        for (int i = 0; i < coinArraySize; i++) {
            
            coinArray[i] = Instantiate(coinPrefab, RandomizePos(), coinPrefab.transform.rotation );
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        tmp.text = "Score: " + score;
        timer.text = "Timer: " + timeLeft;

        if (timeLeft <= 0)
        {
            timer.text = "Game Over";
            rigid.isKinematic = true;
            StartCoroutine(GameOver());
            
        }
        if (Input.GetKey(KeyCode.A))
        {
            rigid.AddForce(new Vector3(-1f * forceMove, 0f, 0f));
        }
        if (Input.GetKey(KeyCode.D))
        {
            rigid.AddForce(new Vector3(1f * forceMove, 0f, 0f));
        }
        if (Input.GetKey(KeyCode.Space) && onFloor)
        {
            rigid.AddForce(new Vector3(0f, 1f * forceJump, 0f), ForceMode.Impulse);
        }
    }

    private Vector3 RandomizePos()
    {
        Vector3 tempPos = new Vector3(Random.Range(-coinRespawnPosXRNG, coinRespawnPosXRNG), coinRespawnPosY + Random.Range(-coinRespawnPosYRNG, coinRespawnPosYRNG), 0f);
        return tempPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floorr")
        {
            onFloor = true;

        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Floorr")
        {
            onFloor = false;

        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coin"){
            //Debug.Log("Coin Collected");
            StartCoroutine(RespawnCoin(other.gameObject));
            score++;
        }
    }

   

    IEnumerator GameOver()
    {
        
        animator.SetBool("Gameover", true);
        OnGameover();
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("SampleScene");
    }

    IEnumerator RespawnCoin(GameObject coin)
    {
        coin.SetActive(false);
        yield return new WaitForSeconds(3f);
        Vector3 pos = RandomizePos();
        Debug.Log(pos);
        coin.transform.position = pos;
        audioSource.PlayOneShot(spawnAudio);
        Instantiate(spawnEffect, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity);
        coin.SetActive(true);

    }

    public bool Iswon
    {
        get
        {
            if (score < goal)
                return false;
            return true;
        }
    }
    private void OnGameover()
    {
        endgamePanel.gameObject.SetActive(true);
        endGameText.text = Iswon ? "Win" : "Lose";
    }


}