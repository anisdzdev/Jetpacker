using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinRotate : MonoBehaviour
{
    public Sprite[] sprites;
    public float animationSpeed = 0.1f;
    public bool isUI;
    public bool isCapableOfCollect;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Loop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Loop() {
        //destroy all game objects
        for (int i = 0; i < sprites.Length; i++) {
            if (isUI) {
                GetComponent<Image>().sprite = sprites[i];
            } else {
                GetComponent<SpriteRenderer>().sprite = sprites[i];
            }
            yield return new WaitForSeconds(animationSpeed);
        }
        StartCoroutine(Loop());
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (isCapableOfCollect) {
            if(other.gameObject.tag == "Player") {
                if (FindObjectOfType<LevelManager>() != null) {
                    FindObjectOfType<LevelManager>().GetCoin(transform.position.x, transform.position.y, 1);
                } else {
                    FindObjectOfType<GameManager>().GetCoin(transform.position.x, transform.position.y, 1);
                }
                
                gameObject.SetActive(false);
            }
        }
    }
}
