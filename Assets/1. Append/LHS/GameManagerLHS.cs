using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManagerLHS : MonoBehaviour
{
    [SerializeField] GameObject GameEndPanel;

    [SerializeField] GameObject SquarePrefab;
    [SerializeField] int Capacity = 32;
    [SerializeField] List<Sprite> Sprites;

    public Queue<GameObject> ShelterQueue = new Queue<GameObject>();
    private GameObject front;

    private float queueGapY = 0.3f;
    private float queueGapZ = 0.5f;
    private Vector3 destinationPosition;

    [SerializeField] Button SortInfectedButton;
    [SerializeField] Button SortHumanButton;
    [SerializeField] GameObject KillZombiePanel;
    private float leftTime = 3.0f;
    private int shotCount = 0;

    private bool isGameEnded = false;

    [SerializeField] TMP_Text ScoreText;
    private int score = 0;

    [SerializeField] Image TimeGaugeImage;
    [SerializeField] TMP_Text TimeGaugeText;
    private float currentTimeGaugeWidth;
    private float gaugeDecreaseScale = 10.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int spritesCount = Sprites.Count;

        for (int i = 0; i < Capacity; i++)
        {
            GameObject go = Instantiate(SquarePrefab);
            go.gameObject.transform.position = new Vector3(0, i * queueGapY, i * queueGapZ);

            int randomIndex = Random.Range(0, spritesCount);

            switch (randomIndex)
            {
                case 0:
                    go.tag = "Human";
                    break;
                case 1:
                    go.tag = "Infected";
                    break;
                case 2:
                    go.tag = "Zombie";
                    break;
            }

            go.GetComponent<SpriteRenderer>().sprite = Sprites[randomIndex];


            ShelterQueue.Enqueue(go);
        }

        destinationPosition = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        SetTimeGaugeWidth(-Time.deltaTime * gaugeDecreaseScale);

        if (ShelterQueue.Count == 0)
        {
            isGameEnded = true;
        }

        if (isGameEnded == true)
        {
            GameEnd();
        }

        WhoIsNext();
    }

    IEnumerator MoveCamera()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = Camera.main.transform.position;
        destinationPosition = destinationPosition + new Vector3(0, queueGapY, queueGapZ);

        while (elapsedTime < 1f)
        {
            Camera.main.transform.position = Vector3.Lerp(startPosition, destinationPosition, elapsedTime);
            elapsedTime += Time.deltaTime * 5.0f;
            yield return null;
        }

        Camera.main.transform.position = destinationPosition;

        yield break;
    }

    IEnumerator SwipeAndDestroy(bool swipeLeft)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = front.transform.position;
        Vector3 destinationPosition = startPosition + new Vector3(10f, 0, 0);

        if (swipeLeft == true)
        {
            destinationPosition.x *= -1f;
        }

        GameObject frontCopy = front;
        ShelterQueue.Dequeue();

        while (elapsedTime < 1f)
        {
            frontCopy.transform.position = Vector3.Lerp(startPosition, destinationPosition, elapsedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(frontCopy.gameObject);

        yield break;
    }

    void WhoIsNext()
    {
        front = ShelterQueue.First();

        if (front.tag == "Zombie")
        {
            KillZombiePanel.gameObject.SetActive(true);
            StartCoroutine("KillZombie");
        }
    }

    public void SortHuman()
    {
        if (front.tag == "Human")
        {
            SetScore(100);
            SetTimeGaugeWidth(20f);
        }
        else
        {
            SetScore(-50);
            SetTimeGaugeWidth(-10f);
        }

        StopCoroutine("MoveCamera");
        StartCoroutine("MoveCamera");
        StartCoroutine("SwipeAndDestroy", true);
    }


    public void SortInfected()
    {
        if (front.tag == "Infected")
        {
            SetScore(100);
            SetTimeGaugeWidth(20f);
        }
        else
        {
            SetScore(-50);
            SetTimeGaugeWidth(-10f);
        }

        StopCoroutine("MoveCamera");
        StartCoroutine("MoveCamera");
        StartCoroutine("SwipeAndDestroy", false);
    }

    IEnumerator KillZombie()
    {
        while (shotCount < 1)
        {
            leftTime -= Time.deltaTime;
            yield return null;
        }

        if (shotCount >= 1)
        {
            SetScore(500);
            SetTimeGaugeWidth(100f);
        }
        else
        {
            SetScore(-500);
            SetTimeGaugeWidth(-100f);
        }

        leftTime = 3f;
        shotCount = 0;
        KillZombiePanel.gameObject.SetActive(false);

        StopCoroutine("MoveCamera");
        StartCoroutine("MoveCamera");

        ShelterQueue.Dequeue();
        Destroy(front.gameObject);

        yield break;
    }

    public void ShootZombie()
    {
        shotCount++;
    }

    void SetScore(int amount)
    {
        score = Mathf.Max(0, score + amount);
        ScoreText.text = score.ToString();
    }

    void SetTimeGaugeWidth(float amount)
    {
        float currentTimeGaugeWidth = TimeGaugeImage.rectTransform.rect.width;

        if (currentTimeGaugeWidth <= 0)
        {
            isGameEnded = true;
            return;
        }

        float nextTimeGaugeWidth = Mathf.Clamp(currentTimeGaugeWidth + amount, 0f, 500f);
        TimeGaugeImage.rectTransform.
            SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nextTimeGaugeWidth);
        TimeGaugeText.text = nextTimeGaugeWidth.ToString();
    }

    void GameEnd()
    {
        Time.timeScale = 0.0f;
        GameEndPanel.SetActive(true);
    }

    public void GameRestart()
    {
        isGameEnded = false;
        Time.timeScale = 1.0f;
        GameEndPanel.SetActive(false);
        SceneManager.LoadScene("SampleScene");
    }
}
