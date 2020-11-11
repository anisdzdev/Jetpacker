using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window_Confetti : MonoBehaviour {

    [SerializeField] private Transform pfConfetti;
    [SerializeField] private Color[] colorArray;

    private List<Confetti> confettiList;
    private float spawnTimer;
    private const float SPAWN_TIMER_MAX = 0.15f;

    private void Awake() {
        confettiList = new List<Confetti>();
        SpawnConfetti();
    }

    private void Update() {
        foreach (Confetti confetti in new List<Confetti>(confettiList)) {
            if (confetti.Update()) {
                confettiList.Remove(confetti);
            }
        }

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f) {
            spawnTimer += SPAWN_TIMER_MAX;
            int spawnAmount = Random.Range(1, 4);
            for (int i = 0; i < spawnAmount; i++) {
                SpawnConfetti();
            }
        }
    }

    private void SpawnConfetti() {
        float width = transform.GetComponent<RectTransform>().rect.width;
        float height = transform.GetComponent<RectTransform>().rect.height;
        Vector2 anchoredPosition = new Vector2(Random.Range(-width / 2f, width / 2f), height / 2f);
        Color color = colorArray[Random.Range(0, colorArray.Length)];
        Confetti confetti = new Confetti(pfConfetti, transform, anchoredPosition, color, -height / 2f);
        confettiList.Add(confetti);
    }

    private class Confetti {

        private Transform transform;
        private RectTransform rectTransform;
        private Vector2 anchoredPosition;
        private Vector3 euler;
        private float eulerSpeed;
        private Vector2 moveAmount;
        private float minimumY;

        public Confetti(Transform prefab, Transform container, Vector2 anchoredPosition, Color color, float minimumY) {
            this.anchoredPosition = anchoredPosition;
            this.minimumY = minimumY;
            transform = Instantiate(prefab, container);
            rectTransform = transform.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;

            rectTransform.sizeDelta *= Random.Range(3f, 4f);

            euler = new Vector3(0, 0, Random.Range(0, 360f));
            eulerSpeed = Random.Range(50f, 60f);
            eulerSpeed *= Random.Range(0, 2) == 0 ? 1f : -1f;
            transform.localEulerAngles = euler;

            moveAmount = new Vector2(0, Random.Range(-100f, -300f));
            //transform.gameObject.AddComponent<Rigidbody2D>();
            transform.GetComponent<Image>().color = color;
        }

        public bool Update() {
            anchoredPosition += moveAmount * Time.deltaTime;
            rectTransform.anchoredPosition = anchoredPosition;

            euler.z += eulerSpeed * Time.deltaTime;
            transform.localEulerAngles = euler;

            if (anchoredPosition.y < minimumY) {
                Destroy(transform.gameObject);
                return true;
            } else {
                return false;
            }
        }


    }

}
