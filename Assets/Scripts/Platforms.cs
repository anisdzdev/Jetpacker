using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Platforms : MonoBehaviour {

    public float minPos;
    public float maxPos;
    Vector3[] waypoints = new Vector3[2];
    int currentpoint = 0;
    int pipeScore;
    public bool scored, isInLevelMode;
    public float speed = 1;
    bool activeCoin;
    public GameObject coin;

    
    public float timeFromZeroToMax = 2;

    // Start is called before the first frame update
    void Start() {
        waypoints[0] = new Vector3(minPos, transform.position.y, transform.position.z);
        waypoints[1] = new Vector3(maxPos, transform.position.y, transform.position.z);
        isInLevelMode = (FindObjectOfType<LevelManager>() != null);
        if(Random.Range(0, 4) == 0) {
            coin.SetActive(true);
            activeCoin = true;
        }else{
            coin.SetActive(false);
        }
    }
    private void Awake() {
        pipeScore = FindObjectOfType<GameManager>().score + 3;
    }

    // Update is called once per frame
    void Update() {
        float changeRatePerSecond = Vector3.Distance(transform.position, waypoints[currentpoint]) * Time.deltaTime;
        if (Vector3.Distance(transform.position, waypoints[currentpoint]) > 0.08) {
            if (!isInLevelMode) {
                if (!FindObjectOfType<GameManager>().enhancedPlatformMove) {
                    if (pipeScore < 10) {

                    } else if (pipeScore >= 10 && pipeScore <= 40) {
                        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentpoint], (pipeScore / 40f) * speed);
                    } else {
                        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentpoint], speed);
                    }
                } else {
                    transform.DOMove(waypoints[currentpoint], speed);
                }
            } else {
                if (!FindObjectOfType<LevelManager>().enhancedPlatformMove) {
                    if (FindObjectOfType<LevelManager>().beamSpeed != 0) {
                        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentpoint], FindObjectOfType<LevelManager>().beamSpeed);
                    } else {
                        if (pipeScore < 10) {

                        } else if (pipeScore >= 10 && pipeScore <= 40) {
                            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentpoint], (pipeScore / 40f) * speed);
                        } else {
                            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentpoint], speed);
                        }
                    }
                } else {
                    transform.DOMove(waypoints[currentpoint], speed);
                }
            }
        } else {

            currentpoint = (currentpoint == 1) ? currentpoint = 0 : currentpoint = 1;
        }

    }


    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !scored) {
            if (!isInLevelMode) {
                FindObjectOfType<GameManager>().score++;
                FindObjectOfType<GameManager>().SpawnPlatform();

                if (FindObjectOfType<PlayerMovement>().timer >= 0) {
                    FindObjectOfType<PlayerMovement>().obstacles++;
                } else {
                    FindObjectOfType<PlayerMovement>().obstacles = 0;
                }
                FindObjectOfType<PlayerMovement>().timer = 1.25f;
                if (activeCoin) {
                    FindObjectOfType<GameManager>().GetCoin(coin.transform.position.x, coin.transform.position.y, 1);
                    coin.SetActive(false);
                }

                FindObjectOfType<AudioManager>().ScoreSound();
                scored = true;
            } else {
                FindObjectOfType<LevelManager>().score++;
                FindObjectOfType<LevelManager>().SpawnPlatform();
               
                if (FindObjectOfType<PlayerMovement>().timer >= 0) {
                    FindObjectOfType<PlayerMovement>().obstacles++;
                } else {
                    FindObjectOfType<PlayerMovement>().obstacles = 0;
                }
                FindObjectOfType<PlayerMovement>().timer = 1.25f;
                if (activeCoin) {
                    FindObjectOfType<LevelManager>().GetCoin(coin.transform.position.x, coin.transform.position.y, 1);
                    coin.SetActive(false);
                }
                
                FindObjectOfType<AudioManager>().ScoreSound();
                scored = true;
            }
        }
    }
}
