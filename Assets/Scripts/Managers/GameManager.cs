using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float timer = 0;

    public static GameManager Instance;

    [Header("Player Life")]
    public PlayerLife playerLife;
    public float currentPlayerLife;
    public float lifeBeforeSceneChanged;

    [Header("Enemies Life")]
    //public EnemyLife enemyLife;
    public float currentEnemyLife;

    [Header("Hunger System")]
    public float hungryDecayAmount = 1.0f;
    public float hungryDecayRate = 1.0f;

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void Awake()
    {
        if (Instance is not null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        LifeTimer();
    }

    private void LifeTimer()
    {
        if (playerLife is not null && playerLife.currentLife > 0)
        {
            timer += Time.deltaTime;

            if (timer > hungryDecayRate)
            {
                playerLife.ChangeCurrentLife(-hungryDecayAmount);
                timer = 0f;
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {   
        // Reasign the Life script to the GameManager
        playerLife = FindObjectOfType<PlayerLife>();

        if (playerLife is not null)
        {
            currentPlayerLife = playerLife.currentLife;
        }
    }
}
