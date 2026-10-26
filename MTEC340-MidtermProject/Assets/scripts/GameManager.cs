using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    private frogger frogger;
    private home[] homes;
    private int score;
    private int lives;
    private int time;
    public GameObject gameOverUI;
    public GameObject startGameUI;
    public GameObject gameInfoUI;
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text timeText;
    AudioSource _source;
    [SerializeField] private AudioClip imHomeSFX;
    [SerializeField] private AudioClip levelCompleteSFX;
    [SerializeField] private AudioClip gameOverSFX;
    [SerializeField] private AudioClip BGM;
	[SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;
    [SerializeField, Min(0.1f)] private float spawnDelay = 8f;
    [SerializeField, Range(0f, 1f)] private float spawnRate = 0.25f;
	//[SerializeField] private PromptBehavior promptBehavior;
	public int levelNumber;


    private void Awake()
    {
		_source = GetComponent<AudioSource>();
        homes = Object.FindObjectsByType<home>();
        frogger = Object.FindAnyObjectByType<frogger>();
    }

    private void Start()
    {
        NewGame();
    }
    private void NewGame()
    {
		_source.Stop();
		frogger.gameObject.SetActive(false);
        gameOverUI.SetActive(false);
		gameInfoUI.SetActive(false);
		startGameUI.SetActive(true);
		//promptBehavior.StartBlink();
		_source.clip = BGM;
		_source.loop = true;
		_source.volume = 0.3f;
        _source.Play();
        SetScore(0);
        SetLives(3);
		levelNumber = 1;
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        startGameUI.SetActive(true);
        yield return null;

        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        startGameUI.SetActive(false);
		gameInfoUI.SetActive(true);
        NewLevel();
    }

    private void NewLevel()
    {
        for (int i = 0; i < homes.Length; i++)
        {
            homes[i].enabled = false;
        }
        Respawn();
    }
    
    private void Respawn()
    {
        frogger.Respawn();
        StopAllCoroutines();
        spawnBehavior = null;
        StartCoroutine(Timer(30));
        spawn();
    }

    private IEnumerator Timer(int duration)
    {
        time = duration;
        timeText.text = time.ToString(); 
        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time--;
            timeText.text = time.ToString();
        }

        frogger.death();
    }

    public void newRow()
    {
        SetScore(score + 10);
    }

	public void powerUpPickup()
    {
		SetScore(score + 500);
	}

    public void Died()
    {
        SetLives(lives - 1);
        if (lives > 0)
        {
            Invoke(nameof(Respawn), 1f);
        }
        else
        { 
            Invoke(nameof(GameOver), 1f);
        }
    }

    private void GameOver()
    {
        frogger.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
		_source.Stop();
		_source.PlayOneShot(gameOverSFX);
        StopAllCoroutines();
        spawnBehavior = null;
        StartCoroutine(PlayAgain());
    }

    private IEnumerator PlayAgain()
    {
        bool playAgain = false;
        while (!playAgain)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                playAgain = true;
            }
            yield return null;
        }

        NewGame();
    }

    public void HomeOccupied()
    {
        frogger.gameObject.SetActive(false);
        int bonusPoints = time * 20;
        SetScore(score + 50);
        if (Cleared())
        {
            SetScore(score + 1000);
            SetLives(lives + 1);
			levelNumber++;
            StopAllCoroutines();
            Invoke(nameof(NewLevel), 1f);
        }
        else
        {   
            Invoke(nameof(Respawn), 1f);
        }
		_source.PlayOneShot(imHomeSFX);
    }

    private bool Cleared()
    {
        for (int i = 0; i < homes.Length; i++)
            if (!homes[i].enabled)
            {
                return false;
            }
        _source.PlayOneShot(levelCompleteSFX);
        return true;
    }
    
    public void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();
    }
    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = lives.ToString();
    }
    
    private Coroutine spawnBehavior;
    private GameObject activePowerUp;

    public void spawn()
    {
        if (spawnBehavior == null)
        {
            if (powerUpPrefab == null)
            {
                return;
            }

            spawnBehavior = StartCoroutine(SpawnBehaviorRoutine());
        }
    }

    public void stopSpawn()
    {
        if (spawnBehavior != null)
        {
            StopCoroutine(spawnBehavior);
            spawnBehavior = null;
        }
    }

    private IEnumerator SpawnBehaviorRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);
            if (activePowerUp == null && Random.value < spawnRate)
            {
                Vector3 position = new Vector3(Random.Range(minBounds.x, maxBounds.x),
                    Random.Range(minBounds.y, maxBounds.y), 0f);

                activePowerUp = Instantiate(powerUpPrefab, position, Quaternion.identity);
            }
        }
    }
}
